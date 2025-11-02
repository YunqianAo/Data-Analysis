<?php
header('Content-Type: application/json; charset=utf-8');
echo json_encode([
  'method' => $_SERVER['REQUEST_METHOD'],
  'POST' => $_POST,
  'GET' => $_GET,
], JSON_UNESCAPED_UNICODE | JSON_PRETTY_PRINT);
