<?php
$servername = "localhost";
$username = "guillemaa";
$password = "48251199m";
$dbname = "guillemaa";

$conn = new mysqli($servername, $username, $password, $dbname);

if ($conn->connect_error) {
    die("Conexión fallida: " . $conn->connect_error);
}

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    $playerId = $_POST['playerId'];
    $sessionStartTime = $_POST['sessionStartTime'];

    $sql = "INSERT INTO Sessions (player_id, session_start_time) VALUES ($playerId, '$sessionStartTime')";

    if ($conn->query($sql) === TRUE) {
        echo json_encode(["status" => "success", "message" => "Nueva sesión registrada exitosamente"]);
    } else {
        echo json_encode(["status" => "error", "message" => "Error al insertar datos: " . $conn->error]);
    }
}

$conn->close();
?>
