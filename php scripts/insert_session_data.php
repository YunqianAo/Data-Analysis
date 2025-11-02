<?php
header('Content-Type: application/json; charset=utf-8');

$servername = "localhost";
$username = "yunqiana";
$password = "ScDYLmF29m4r";
$dbname = "yunqiana";

$conn = new mysqli($servername, $username, $password, $dbname);
if ($conn->connect_error) {
  echo json_encode(["status"=>"error","message"=>"DB连接失败: ".$conn->connect_error]); exit;
}

if ($_SERVER["REQUEST_METHOD"] !== "POST") {
  echo json_encode(["status"=>"error","message"=>"只接受 POST 请求"]); exit;
}

$playerId = $_POST['playerId'] ?? '';
$sessionStartTime = $_POST['sessionStartTime'] ?? '';

if ($playerId === '' || $sessionStartTime === '') {
  echo json_encode(["status"=>"error","message"=>"缺少字段 playerId 或 sessionStartTime"]); exit;
}

$sql = "INSERT INTO Sessions (player_id, session_start_time) VALUES (?, ?)";
$stmt = $conn->prepare($sql);
$stmt->bind_param("is", $playerId, $sessionStartTime);

if ($stmt->execute()) {
  echo json_encode(["status"=>"success","sessionId"=>$conn->insert_id]);
} else {
  echo json_encode(["status"=>"error","message"=>"插入失败: ".$conn->error]);
}

$stmt->close();
$conn->close();
