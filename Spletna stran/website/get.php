<?php
$debug = false;

include '../server/serverClass.php';

$client = Server::Client();

if(isset($_GET['id']) && !empty($_GET['id'])){
    if(!$debug){
        $response = Server::GetMovieById($_GET['id']);    
    }
    else{
        $response = Server::getDataOneTest();
    }
    echo json_encode($response);
    exit();
}
else if(isset($_GET['contains']) && !empty($_GET['contains'])){
    echo json_encode(Server::MovieSearch($_GET['contains']));
    exit();
}

?>