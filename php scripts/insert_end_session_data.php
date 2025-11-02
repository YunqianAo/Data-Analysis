<?php
header('Content-Type: application/json; charset=utf-8');

$servername = "localhost";
$username = "yunqiana";
$password = "ScDYLmF29m4r";
$dbname = "yunqiana";

$conn = new mysqli($servername, $username, $password, $dbname);
if ($conn->connect_error) { echo json_encode(["status"=>"error","message"=>"DB连接失败: ".$conn->connect_error]); exit; }

if ($_SERVER["REQUEST_METHOD"] !== "POST") { echo json_encode(["status"=>"error","message"=>"只接受 POST 请求"]); exit; }

$sessionId = $_POST['sessionId'] ?? '';
$sessionEndTime = $_POST['sessionEndTime'] ?? '';

if ($sessionId === '' || $sessionEndTime === '') { echo json_encode(["status"=>"error","message"=>"缺少字段 sessionId 或 sessionEndTime"]); exit; }

$sql = "UPDATE Sessions SET session_end_time = ? WHERE id = ?";
$stmt = $conn->prepare($sql);
$stmt->bind_param("si", $sessionEndTime, $sessionId);

if ($stmt->execute()) {
  echo json_encode(["status"=>"success","message"=>"Sesión finalizada"]);
} else {
  echo json_encode(["status"=>"error","message"=>"更新失败: ".$conn->error]);
}

$stmt->close();
$conn->close();
