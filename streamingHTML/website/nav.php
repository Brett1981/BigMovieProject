<?php
//server communicator

require 'nav/Init.php';
require 'nav/Builder.php';
if(!class_exists("Server")){
    require '../server/serverClass.php';
}



$client = Server::Client();


//root of project
$data['dirRoot']        = dirname(dirname(__FILE__ ));
//Website url
$data['serverPath']     = 'http://'.$_SERVER['HTTP_HOST'];
$data['serverRoot']     = $data['serverPath'].'/'.basename(dirname(dirname(__FILE__)));
//navigation dir
$data['dirNav']         = $data['dirRoot'].'\website\navigation_left.php';
//content location
$data['icons']          = $data['serverRoot'].'/assets/icons/';
$data['userDefIcon']    = $data['icons'] .'user_default_icon.png';
$data['homePage']       = $data['serverRoot'].'/movies/';
//nov profile link!
$data['profilePage']    = $data['serverRoot'].'/profile/index.php';
//script list
$lib = [
    'lib'       => [
                    'root' => '../lib/',
                    'items' => ['Login.js','Menu.js','Modal.js','Movie.js']
                   ],
    'website'   => [
                    'root' => '../website/',
                    'items' => ['nav.js','login.js','movies.js']
                   ]
        ];

$data['genres'] = array(
    "All"               => 'index.php?showall',
    'Action'            => 'action',
    'Adventure'         => 'adventure',
    'Animation'         => 'animation',
    'Comedy'            => 'comedy',
    'Drama'             => 'drama',
    'Family'            => 'family',
    'History'           => 'history',
    'Horror'            => 'horror',
    'Science Fiction'   => 'scifi',
    'Thriller'          => 'thriller'
    );

$modalJsAction = "Login.check(value,this)";
$modal = array(
    "login" => array(
        "div" => array("class" => "modal-login","id" => "login","style" => array("display" => "block", "overflow-y" => "auto")),
        "form" => array("id" => "login-form","class" => "form","method" => "post","onsubmit" => ""),
        "inputs" => array(
            array("type" => "text","placeholder" => "Username","name"  => "username","onblur" => "","isRequired" => true),
            array("type" => "text","placeholder" => "Password","name"  => "password","onblur" => "","isRequired" => true),
        ),
        "submit" => array("type" => "submit","id" => "login-button","class" => "preventSubmit","text" => "Login")
    ),
    "register" => array(
        "div" => array( "class" => "modal-register", "id"    => "register", "style" => array("display" => "none", "overflow-y" => "auto")),
        "form" => array("id" => "register-form","class" => "form","method" => "post","onsubmit"  => "return false"),
        "inputs" => array(
            array("type" => "text","placeholder" => "Username","name"  => "username","onblur" => $modalJsAction,"isRequired" => true),
            array("type" => "password","placeholder" => "Password","name"  => "password","onblur" => $modalJsAction,"isRequired" => true),
            array("type" => "password","placeholder" => "Verify password","name"  => "v_password","onblur" => $modalJsAction,"isRequired" => true),
            array("type" => "email","placeholder" => "Email","name"  => "email","onblur" => $modalJsAction,"isRequired" => true),
            array("type" => "date","placeholder" => "Birthday","name"  => "birthday","onblur" => $modalJsAction,"isRequired" => true),
            array("type" => "text","placeholder" => "Display name","name"  => "display_name","onblur" => $modalJsAction,"isRequired" => true),
        ),
        "submit" => array( "type" => "submit","id" => "login-button","class" => "preventSubmit","text" => "Login")
    )
);


//user guid length
$data['guidLength']     = 36;

$navData = new Navigation($data);
$profile = null;

if(isset($_SESSION['user']) && !empty($_SESSION['user']) && !empty($_SESSION['user']['unique_id'])){
    $profile = $_SESSION['user'];
}
else if((isset($_GET['uid']) && !empty($_GET['uid']))
        && empty($_SESSION['user']['unique_id'])){

        if(strlen($_GET['uid']) == $data['guidLength'] ){
            //guid is from registered user
            // for test "7ee711fa-91f6-4902-9e26-fe739c4638a3"
            $navData->UserLogin($_GET['uid']);
        }
        else{
            $navData->UserLogin();
        }

        if(!empty($navData->profile->user)){
            $profile = $navData->profile->user;
            $_SESSION['user'] = $profile;
        }
}
else{
    $navData->UserLogin();
}
if(count($profile) > 4){
    //is reg
    $navData->profile->logedIn = true;
}
else{
    //is guest
    $navData->profile->logedIn = false;
}

$build = array(
    'isWatching'    =>  null,
    'isLogedIn'     =>  $navData->profile->logedIn,
    'user'          =>  null,
    'enableGenres'  =>  null,
    'data'          =>  $data,
    'scripts'       =>  $lib,
    'modal'         =>  $modal
);


if(isset($enableGenres)){
    $build['enableGenres'] = $enableGenres;
}
if(isset($watching) && !empty($watching)){
    $build['isWatching'] = $watching;
}
if(isset($_SESSION['user']) && !empty($_SESSION['user']['unique_id'])){
    $build['user'] = $_SESSION['user'];
    $build['isLogedIn'] = true;
}
else if(isset($navData->profile->user) && !empty($navData->profile->user)){
    $build['user'] = $navData->profile->user;

}

//builds the page index
$builder = new Builder($build);



?>
