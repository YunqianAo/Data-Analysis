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
$playerId         = isset($_POST['playerId']) ? intval($_POST['playerId']) : null;
$sessionStartTime = isset($_POST['sessionStartTime']) ? trim($_POST['sessionStartTime']) : null;

if ($playerId === null || $sessionStartTime === null) {
    http_response_code(400);
    echo json_encode(["error" => "缺少参数 / Missing parameters"], JSON_UNESCAPED_UNICODE);
    exit;
}

try {
    // 可选：先校验 player 是否存在 / Optional: verify player exists
    $chk = $pdo->prepare("SELECT id FROM NewPlayers WHERE id = :id");
    $chk->execute([":id" => $playerId]);
    if ($chk->rowCount() === 0) {
        http_response_code(404);
        echo json_encode(["error" => "玩家不存在 / Player not found"], JSON_UNESCAPED_UNICODE);
        exit;
    }

    // 插入 Sessions，返回 sessionId / Insert session and return sessionId
    $sql = "INSERT INTO Sessions (player_id, start_time) VALUES (:player_id, :start_time)";
    $stmt = $pdo->prepare($sql);
    $stmt->execute([
        ":player_id" => $playerId,
        ":start_time"=> $sessionStartTime
    ]);

    $sessionId = (int)$pdo->lastInsertId();
    echo json_encode(["sessionId" => $sessionId], JSON_UNESCAPED_UNICODE);
    exit;
} catch (Throwable $e) {
    http_response_code(500);
    echo json_encode(["error" => "创建会话失败 / Create session failed", "detail" => $e->getMessage()], JSON_UNESCAPED_UNICODE);
    exit;
}
