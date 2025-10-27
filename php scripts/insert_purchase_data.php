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
    $itemId = $_POST['itemId'];
    $purchaseDate = $_POST['purchaseDate'];

    $sql = "INSERT INTO Purchases (player_id, item_id, purchase_date) VALUES ($playerId, $itemId, '$purchaseDate')";

    if ($conn->query($sql) === TRUE) {
        echo json_encode(["status" => "success", "message" => "Compra registrada exitosamente"]);
    } else {
        echo json_encode(["status" => "error", "message" => "Error al insertar datos: " . $conn->error]);
    }
}

$conn->close();
?>
