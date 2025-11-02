<?php
// 连接数据库 / DB connect
$DB_HOST = "localhost";
$DB_NAME = "yunqiana";
$DB_USER = "yunqiana";
$DB_PASS = "ScDYLmF29m4r";


header('Content-Type: application/json; charset=utf-8');

try {
    $pdo = new PDO("mysql:host=$DB_HOST;dbname=$DB_NAME;charset=utf8mb4", $DB_USER, $DB_PASS, [
        PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION
    ]);
} catch (Throwable $e) {
    http_response_code(500);
    echo json_encode(["error" => "数据库连接失败 / DB connection failed", "detail" => $e->getMessage()], JSON_UNESCAPED_UNICODE);
    exit;
}

// 校验 POST 参数 / Validate POST params
$sessionId     = isset($_POST['sessionId']) ? intval($_POST['sessionId']) : null;
$sessionEndTime= isset($_POST['sessionEndTime']) ? trim($_POST['sessionEndTime']) : null;

if ($sessionId === null || $sessionEndTime === null) {
    http_response_code(400);
    echo json_encode(["error" => "缺少参数 / Missing parameters"], JSON_UNESCAPED_UNICODE);
    exit;
}

try {
    // 更新会话的结束时间 / Update session end time
    $sql = "UPDATE Sessions SET end_time = :end_time WHERE id = :id";
    $stmt = $pdo->prepare($sql);
    $stmt->execute([
        ":end_time" => $sessionEndTime,
        ":id"       => $sessionId
    ]);

    if ($stmt->rowCount() === 0) {
        http_response_code(404);
        echo json_encode(["error" => "会话不存在或未修改 / Session not found or unchanged"], JSON_UNESCAPED_UNICODE);
        exit;
    }

    echo json_encode(["ok" => true, "sessionId" => $sessionId], JSON_UNESCAPED_UNICODE);
    exit;
} catch (Throwable $e) {
    http_response_code(500);
    echo json_encode(["error" => "结束会话失败 / End session failed", "detail" => $e->getMessage()], JSON_UNESCAPED_UNICODE);
    exit;
}
