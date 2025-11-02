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

$name    = isset($_POST['name']) ? $_POST['name'] : null;
$country = isset($_POST['country']) ? $_POST['country'] : null;
$age     = isset($_POST['age']) ? intval($_POST['age']) : null;
$gender  = isset($_POST['gender']) ? floatval($_POST['gender']) : null;
$date    = isset($_POST['date']) ? $_POST['date'] : null;

if (empty($name) || empty($country) || $age===null || $gender===null || empty($date)) {
    http_response_code(400);
    echo json_encode(["error"=>"参数缺失 / Missing params: name/country/age/gender/date"], JSON_UNESCAPED_UNICODE);
    exit;
}

try {
    // 表结构假设：NewPlayers(id PK AI, name, country, age, gender, created_at)
    $stmt = $pdo->prepare("
        INSERT INTO NewPlayers (name, country, age, gender, created_at)
        VALUES (:name, :country, :age, :gender, :created_at)
    ");
    $stmt->execute([
        ":name"       => $name,
        ":country"    => $country,
        ":age"        => $age,
        ":gender"     => $gender,
        ":created_at" => $date
    ]);

    $playerId = (int)$pdo->lastInsertId();
    echo json_encode(["playerId"=>$playerId], JSON_UNESCAPED_UNICODE);
    exit;
} catch (Throwable $e) {
    http_response_code(500);
    echo json_encode(["error"=>"插入玩家失败 / Insert player failed","detail"=>$e->getMessage()], JSON_UNESCAPED_UNICODE);
    exit;
}
