<?php
header('Content-Type: application/json; charset=utf-8');

$servername = "localhost";
$username = "yunqiana";
$password = "ScDYLmF29m4r";
$dbname = "yunqiana";

$conn = new mysqli($servername, $username, $password, $dbname);
if ($conn->connect_error) { echo json_encode(["status"=>"error","message"=>"DB连接失败: ".$conn->connect_error]); exit; }

if ($_SERVER["REQUEST_METHOD"] !== "POST") { echo json_encode(["status"=>"error","message"=>"只接受 POST 请求"]); exit; }

$playerId = $_POST['playerId'] ?? '';
$itemId = $_POST['itemId'] ?? '';
$purchaseDate = $_POST['purchaseDate'] ?? '';

if ($playerId === '' || $itemId === '' || $purchaseDate === '') {
  echo json_encode(["status"=>"error","message"=>"缺少字段 playerId 或 itemId 或 purchaseDate"]); exit;
}

$sql = "INSERT INTO Purchases (player_id, item_id, purchase_date) VALUES (?, ?, ?)";
$stmt = $conn->prepare($sql);
$stmt->bind_param("iis", $playerId, $itemId, $purchaseDate);

if ($stmt->execute()) {
  echo json_encode(["status"=>"success"]);
} else {
  echo json_encode(["status"=>"error","message"=>"插入失败: ".$conn->error]);
}

$stmt->close();
$conn->close();
