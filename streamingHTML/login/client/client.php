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
            $result = json_decode(Server::login($cred));
            if(!empty($result)){
                $_SESSION['guid'] = $result->user_id;
                $responseArr['uid'] = $result->user_id;
                $responseArr['response'] = 'success';
                echo json_encode($responseArr);
            }
            else{
                $responseArr['uid'] = '';
            $responseArr['response'] = 'Wrong username or password!';
            echo json_encode($responseArr);
            }
        }
        else{
            //username or password was empty
            $responseArr['uid'] = '';
            $responseArr['response'] = 'Username or password was empty!';
            echo json_encode($responseArr);
        }
    }
    else{
        //not set username or password
        $responseArr['uid'] = '';
        $responseArr['response'] = 'Username or password was not set!';
        echo json_encode($responseArr);
    }
}
elseif(isset($_GET['register'])){
    $reg = filter_input_array(INPUT_POST,$_POST);
    if(!empty($reg['username']) && !empty($reg['password']) && !empty($reg['user_email']) && !empty($reg['user_display_name'])){
        $result = json_decode(Server::register($reg));
        if(!empty($result)){
            $responseArr['uid'] = $result->unique_id;
            $responseArr['username'] = $result->username;
            $responseArr['status'] = "success";
            echo json_encode($responseArr);
        }
        
    }
}
else{
    //empty post data
    $responseArr['uid'] = '';
    $responseArr['response'] = 'No data was sent! Contact the administrator.';
    echo json_encode($responseArr);
}

