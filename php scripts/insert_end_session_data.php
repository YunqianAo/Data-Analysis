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
    $sessionId = $_POST['sessionId'];
    $sessionEndTime = $_POST['sessionEndTime'];

    $sql = "UPDATE Sessions SET session_end_time = '$sessionEndTime' WHERE id = $sessionId";

    if ($conn->query($sql) === TRUE) {
        echo json_encode(["status" => "success", "message" => "Sesión finalizada exitosamente"]);
    } else {
        echo json_encode(["status" => "error", "message" => "Error al actualizar datos: " . $conn->error]);
    }
}

$conn->close();
?>
