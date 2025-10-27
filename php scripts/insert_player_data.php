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
    $playerName = $_POST['name'];
    $country = $_POST['country'];
    $age = $_POST['age'];
    $gender = $_POST['gender'];
    $date = $_POST['date'];

    $sql = "INSERT INTO NewPlayers (name, country, age, gender, date) VALUES ('$playerName', '$country', $age, $gender, '$date')";

    if ($conn->query($sql) === TRUE) {
        $last_id = $conn->insert_id;
        echo json_encode(["status" => "success", "message" => "Nuevo jugador registrado exitosamente", "playerId" => $last_id]);
    } else {
        echo json_encode(["status" => "error", "message" => "Error al insertar datos: " . $conn->error]);
    }
}

$conn->close();
?>
