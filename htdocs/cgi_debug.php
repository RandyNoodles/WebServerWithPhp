<?php
session_start(); //Start session


//This script helps to debug our server.
//It displays the CGI environment variables, POST data, and session data in a nice set of tables.




//Define the CGI variables you want to display
$cgivars = [
    "DOCUMENT_ROOT",
    "SCRIPT_FILENAME",
    "SCRIPT_NAME",
    "REQUEST_METHOD",
    "QUERY_STRING",
    "CONTENT_TYPE",
    "CONTENT_LENGTH",
    "HTTPS",
    "HTTP_USER_AGENT",
    "REQUEST_URI",
    "SERVER_PROTOCOL",
    "SERVER_NAME",
    "SERVER_PORT",
    "REMOTE_ADDR",
    "REMOTE_PORT",
    "GATEWAY_INTERFACE"
];

//Function to generate HTML table
function renderTable($title, $data)
{
    echo "<h2>$title</h2>";
    echo "<table border='1' cellpadding='5' cellspacing='0' style='border-collapse: collapse;'>";
    echo "<tr><th>Key</th><th>Value</th></tr>";

    if (empty($data)) {
        echo "<tr><td colspan='2'><i>No data available</i></td></tr>";
    } else {
        foreach ($data as $key => $value) {
            echo "<tr><td><b>" . htmlspecialchars($key) . "</b></td><td>" . htmlspecialchars($value) . "</td></tr>";
        }
    }

    echo "</table><br>";
}

//Collect CGI variables
$cgiData = [];
foreach ($cgivars as $var) {
    if (isset($_SERVER[$var])) {
        $cgiData[$var] = $_SERVER[$var];
    }
}

//Collect POST data
$postData = !empty($_POST) ? $_POST : ["No POST data received."];

//ollect SESSION data
$sessionData = !empty($_SESSION) ? $_SESSION : ["No session data available."];

//HTML Output
echo "<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>PHP CGI Debug</title>
</head>
<body>
    <h1>PHP CGI Debug</h1>";

renderTable("CGI Environment Variables", $cgiData);
renderTable("POST Data", $postData);
renderTable("Session Data", $sessionData);

echo "</body></html>";
?>
