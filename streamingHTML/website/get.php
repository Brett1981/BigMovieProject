<?php
$debug = true;

include '../server/serverClass.php';

$client = Server::Client();

if(isset($_GET['id']) && !empty($_GET['id'])){
    if(!$debug){
        $response = Server::GetMovieById(array('movie_id' => $id));    
    }
    else{
        $response = Server::getDataOneTest();
    }
    echo json_encode($response,true);
    exit();
}

?>