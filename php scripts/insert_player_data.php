<?php
// 连接数据库（中英注释）：创建 PDO 并设置错误模式、编码
// DB connect: create PDO, set error mode and charset
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
$name    = isset($_POST['name']) ? trim($_POST['name']) : null;
$country = isset($_POST['country']) ? trim($_POST['country']) : null;
$age     = isset($_POST['age']) ? intval($_POST['age']) : null;
$gender  = isset($_POST['gender']) ? trim($_POST['gender']) : null; // 以字符串接收再入库
$date    = isset($_POST['date']) ? trim($_POST['date']) : null;     // yyyy-MM-dd HH:mm:ss

if ($name === null || $country === null || $age === null || $gender === null || $date === null) {
    http_response_code(400);
    echo json_encode(["error" => "缺少参数 / Missing parameters"], JSON_UNESCAPED_UNICODE);
    exit;
}

try {
    // 插入玩家数据 / Insert player data
   // 插入玩家数据 / Insert player data
$sql = "INSERT INTO `NewPlayers` (`name`, `country`, `age`, `gender`, `date`)
        VALUES (:name, :country, :age, :gender, :created_at)";
$stmt = $pdo->prepare($sql);
$stmt->execute([
    ":name"       => $name,
    ":country"    => $country,
    ":age"        => $age,
    ":gender"     => $gender,
    ":created_at" => $date
]);


    // 获取自增ID并返回 / Get last insert id and return
    $playerId = (int)$pdo->lastInsertId();
    echo json_encode(["playerId" => $playerId], JSON_UNESCAPED_UNICODE);
    exit;
} catch (Throwable $e) {
    http_response_code(500);
    echo json_encode(["error" => "插入玩家失败 / Insert player failed", "detail" => $e->getMessage()], JSON_UNESCAPED_UNICODE);
    exit;
}
