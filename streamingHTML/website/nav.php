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
            $navData->UserLogin($_GET['uid']); //"7ee711fa-91f6-4902-9e26-fe739c4638a3"
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
//$_SESSION['userData'] = 

$build = array(
    'isWatching'    =>  null,
    'isLogedIn'     =>  $navData->profile->logedIn,
    'user'          =>  null,
    'enableGenres'  =>  null,
    'data'          =>  $data,
    'scripts'       =>  $lib
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
$builder = new Builder($build);



?>
