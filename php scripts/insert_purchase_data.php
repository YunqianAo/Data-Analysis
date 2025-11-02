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
$playerId     = isset($_POST['playerId']) ? intval($_POST['playerId']) : null;
$itemId       = isset($_POST['itemId']) ? intval($_POST['itemId']) : null;
$purchaseDate = isset($_POST['purchaseDate']) ? trim($_POST['purchaseDate']) : null;

if ($playerId === null || $itemId === null || $purchaseDate === null) {
    http_response_code(400);
    echo json_encode(["error" => "缺少参数 / Missing parameters"], JSON_UNESCAPED_UNICODE);
    exit;
}

try {
    // 可选：校验玩家存在 / Optional: verify player exists
    $chk = $pdo->prepare("SELECT id FROM NewPlayers WHERE id = :id");
    $chk->execute([":id" => $playerId]);
    if ($chk->rowCount() === 0) {
        http_response_code(404);
        echo json_encode(["error" => "玩家不存在 / Player not found"], JSON_UNESCAPED_UNICODE);
        exit;
    }

    // 插入购买记录 / Insert purchase
    $sql = "INSERT INTO Purchases (player_id, item_id, purchase_time) VALUES (:player_id, :item_id, :purchase_time)";
    $stmt = $pdo->prepare($sql);
    $stmt->execute([
        ":player_id"     => $playerId,
        ":item_id"       => $itemId,
        ":purchase_time" => $purchaseDate
    ]);

    $purchaseId = (int)$pdo->lastInsertId();
    echo json_encode(["ok" => true, "purchaseId" => $purchaseId], JSON_UNESCAPED_UNICODE);
    exit;
} catch (Throwable $e) {
    http_response_code(500);
    echo json_encode(["error" => "插入购买失败 / Insert purchase failed", "detail" => $e->getMessage()], JSON_UNESCAPED_UNICODE);
    exit;
}
