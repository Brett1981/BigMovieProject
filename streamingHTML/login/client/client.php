<?php
require_once '../../server/serverComm.php';
/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
$client = Server::Client();
$responseArr;
if(isset($_GET['login']) && isset($_POST)){
    $cred = $_POST;
    if(isset($cred['username']) && isset($cred['password'])){
        if(!empty($cred['username']) && !empty($cred['password'])){
            $result = Server::login($cred);
            $data = json_decode($result);
            if(($data->user_id) !== ''){
                $_SESSION['guid'] = $data->user_id;
                $responseArr['uid'] = $data->user_id;
                $responseArr['response'] = 'success';
                echo json_encode($responseArr);
                exit();
            }
            else{
                $responseArr['uid'] = '';
                $responseArr['response'] = 'Wrong username or password!';
                echo json_encode($responseArr);
                exit();
            }
        }
        else{
            //username or password was empty
            $responseArr['uid'] = '';
            $responseArr['response'] = 'Username or password was empty!';
            echo json_encode($responseArr);
            exit();
        }
    }
    else{
        //not set username or password
        $responseArr['uid'] = '';
        $responseArr['response'] = 'Username or password was not set!';
        echo json_encode($responseArr);
        exit();
    }
}
elseif(isset($_GET['register'])){
    $reg = $_POST;
    if(!empty($reg['username']) && !empty($reg['password']) && !empty($reg['user_email']) && !empty($reg['user_display_name'])){
        $result = Server::register($reg);
        $data = json_decode($result); // decode string json so that we get stdclass object
        $results = $data->Result; //export result part of stdclass to var
        if(($results->unique_id) !== '' ){
            $responseArr['uid'] = $results->unique_id;
            $responseArr['username'] = $results->username;
            $responseArr['response'] = "success";
            echo json_encode($responseArr);
            exit();
        }
        else{
            $responseArr['uid'] = null;
            $responseArr['username'] = null;
            $responseArr['response'] = "failed";
            echo json_encode($responseArr);
            exit();
        }
    }
}
elseif(isset($_GET['check']) && isset($_POST)){
    $data = $_POST;
    $responseArr['username_status'] = null;
    $responseArr['username'] = null;
    if(!empty($data['username'])){
        $responseArr['username'] = $data['username'];
        $result = Server::checkFormUser($data['username']);
        if(!empty($result)){ $responseArr['username_status'] = false; }
        else{ $responseArr['username_status'] = true; }
        echo json_encode($responseArr);
        exit();
    }
    else{ $responseArr['username_status'] = false; $responseArr['username'] = "Not specified";}
    echo json_encode($responseArr);
    exit();
}
else{
    //empty post data
    $responseArr['uid'] = '';
    $responseArr['response'] = 'No data was sent! Contact the administrator.';
    echo json_encode($responseArr);
    exit();
}

