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

$sessionId = isset($_POST['sessionId']) ? intval($_POST['sessionId']) : 0;
$end       = isset($_POST['sessionEndTime']) ? $_POST['sessionEndTime'] : null;

if ($sessionId <= 0 || empty($end)) {
    http_response_code(400);
    echo json_encode(["error"=>"参数缺失 / Missing params: sessionId or sessionEndTime"], JSON_UNESCAPED_UNICODE);
    exit;
}

try {
    $stmt = $pdo->prepare("UPDATE Sessions SET end_time = :et WHERE id = :sid");
    $stmt->execute([
        ":et"  => $end,
        ":sid" => $sessionId
    ]);

    echo json_encode(["status"=>"ok"], JSON_UNESCAPED_UNICODE);
    exit;
} catch (Throwable $e) {
    http_response_code(500);
    echo json_encode(["error"=>"结束会话更新失败 / Update session end failed","detail"=>$e->getMessage()], JSON_UNESCAPED_UNICODE);
    exit;
}
