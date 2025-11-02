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
    echo json_encode(["error"=>"数据库连接失败 / DB connect failed","detail"=>$e->getMessage()], JSON_UNESCAPED_UNICODE);
    exit;
}

// 读取POST参数 / Read POST params
$playerId = isset($_POST['playerId']) ? intval($_POST['playerId']) : 0;
$start    = isset($_POST['sessionStartTime']) ? $_POST['sessionStartTime'] : null;

if ($playerId <= 0 || empty($start)) {
    http_response_code(400);
    echo json_encode(["error"=>"参数缺失 / Missing params: playerId or sessionStartTime"], JSON_UNESCAPED_UNICODE);
    exit;
}

try {
    // 插入到 Sessions 表（字段按你现有结构）/ Insert into Sessions with your current columns
    $stmt = $pdo->prepare("INSERT INTO Sessions (player_id, start_time) VALUES (:pid, :st)");
    $stmt->execute([
        ":pid" => $playerId,
        ":st"  => $start
    ]);

    // 返回新会话ID / Return new session id
    $sessionId = (int)$pdo->lastInsertId();
    echo json_encode(["sessionId"=>$sessionId], JSON_UNESCAPED_UNICODE);
    exit;
} catch (Throwable $e) {
    http_response_code(500);
    echo json_encode(["error"=>"插入会话失败 / Insert session failed","detail"=>$e->getMessage()], JSON_UNESCAPED_UNICODE);
    exit;
}
