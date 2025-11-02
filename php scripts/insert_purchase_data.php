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

$playerId = isset($_POST['playerId']) ? intval($_POST['playerId']) : 0;
$itemId   = isset($_POST['itemId']) ? intval($_POST['itemId']) : 0;
$ptime    = isset($_POST['purchaseDate']) ? $_POST['purchaseDate'] : null;

if ($playerId <= 0 || $itemId <= 0 || empty($ptime)) {
    http_response_code(400);
    echo json_encode(["error"=>"参数缺失 / Missing params: playerId/itemId/purchaseDate"], JSON_UNESCAPED_UNICODE);
    exit;
}

try {
    $stmt = $pdo->prepare("INSERT INTO Purchases (player_id, item_id, purchase_time) VALUES (:pid, :iid, :pt)");
    $stmt->execute([
        ":pid" => $playerId,
        ":iid" => $itemId,
        ":pt"  => $ptime
    ]);

    echo json_encode(["status"=>"ok"], JSON_UNESCAPED_UNICODE);
    exit;
} catch (Throwable $e) {
    http_response_code(500);
    echo json_encode(["error"=>"插入购买失败 / Insert purchase failed","detail"=>$e->getMessage()], JSON_UNESCAPED_UNICODE);
    exit;
}
