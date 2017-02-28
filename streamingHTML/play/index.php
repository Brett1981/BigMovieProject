<?php 
session_start();
//server communicator
include_once '../server/serverClass.php';

//root of project
$dir_root = dirname(dirname(__FILE__ ));

//navigation dir
$dir_nav = $dir_root.'\website\navigation_left.php';

//Server client init
$client = Server::Client();

//Init variables
$api;
$guid;
$session;
$watching = array();
if(isset($_SESSION['user_data']['unique_id']) && $_SESSION['user_data']['unique_id'] != null){
    //play movie to registered user
    if(isset($_GET['id']) && $_GET['id'] != null){
        $mGuid = $_GET['id'];
        try {
            $api = getMovie($_SESSION['user_data']['unique_id'], $mGuid);
            $api_session = getSessionRegistered($_SESSION['user_data']['unique_id'], $mGuid);
        } catch (Exception $e) {
            echo 'Caught exception: ',  $e->getMessage(), "\n";
        }
        $data = json_decode(json_decode(json_encode($api)));
        $session = json_decode($api_session, true);
        if(isset($data) && $data != null){
            $watching['movie_name'] = $data->movieData->Movie_Info->title;
            $watching['homepage'] = $data->movieData->Movie_Info->homepage;
            $watching['vote_average'] = $data->movieData->Movie_Info->vote_average;
            $watching['watched'] = $data->movieData->views;
        }
        else{ header('location: ../index.php'); }
    }
    else{
        header('location: ../index.php');
    }
    
}
else{ 
    //play movie to guest
    if(isset($_GET['id']) && $_GET['id'] != null){
        $mGuid = $_GET['id'];
        $guest_id = uniqid();
        try {
            $api = getMovie($guest_id, $mGuid);
        } catch (Exception $e) {
            echo 'Caught exception: ',  $e->getMessage(), "\n";
        }
        $data = json_decode(json_decode(json_encode($api)));
        $session = $data->sessionGuest->session_id;
        if(isset($data) && $data != null){
            $watching['movie_name'] = $data->movieData->Movie_Info->title;
            $watching['homepage'] = $data->movieData->Movie_Info->homepage;
            $watching['vote_average'] = $data->movieData->Movie_Info->vote_average;
            $watching['watched'] = $data->movieData->views;
        }
        else{ header('location: ../index.php'); }
    }
    else{ header('location: ../movies'); }
    
}
function getSessionRegistered($user_id, $movie_id){
    if(isset($user_id) && isset($movie_id) && $movie_id != null){
        $u = array('user_id' => $user_id, 'movie_id' => $movie_id);
        $result = Server::getSession($u);
        if($result == null){
            header('location: ../movies/');
            $_SESSION['play_error'] = "Error retrieving data";
            exit();
        }
        return $result;
    }
    else{ header('location: ../movies/'); exit(); }
}

function getMovie($user_id, $movie_id, $username = null, $password = null)
{
    if(isset($user_id) && isset($movie_id) && $movie_id != null){
        $data = array('user_id' => $user_id, 'movie_id' => $movie_id);
        $result = Server::getMovie($data);
        if($result === null){
            header('location: ../movies/');
            $_SESSION['play_error'] = "No movie was found ";
            exit();
        }
        return $result;
    }
    else{ header('location: ../movies/'); exit(); }
}


?>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
    <head>
        <meta http-equiv="Content-Type" content="text/html;charset=UTF-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1">
        <title><?php  if(isset($data) && $data != null){ 
            if($data->movieData->Movie_Info->title != ""){ 
                echo $data->movieData->Movie_Info->title; 
            }elseif($data->movieData->name != ""){ 
                echo $data->movieData->name; 
            }else{ echo "Unknown movie";}  
        }?></title>
        <!-- VideoJs plugin and stylesheet -->
        <script src="../assets/videojs/video.min.js"></script>
        <link href="../assets/videojs/video-js.min.css" rel="stylesheet"> 
        <!-- If you'd like to support IE8 -->
        <script src="../assets/videojs/ie8/videojs-ie8.min.js"></script>
        <link rel="stylesheet" type="text/css" href="../css/style.css"/>
        <link href="https://fonts.googleapis.com/icon?family=Material+Icons"
      rel="stylesheet">
        <script
  src="https://code.jquery.com/jquery-3.1.1.min.js"
  integrity="sha256-hVVnYaiADRTO2PzUGmuLJr8BLUSjGIZsDYGmIJLv2b8="
  crossorigin="anonymous"></script>
        <script type="text/javascript">

            function onLoad() {
                var sec = parseInt(document.location.search.substr(1));
                
                if (!isNaN(sec))
                    mainPlayer.currentTime = sec;
            }
        </script>
    </head>
    <body>
        <!-- Sidebar -->
        <?php include $dir_nav; ?>
        <!-- /#sidebar-wrapper -->
        <!-- Page Content -->
        <div class="main">
                <div>
                    <?php if(isset($data)){ ?>
                    <video id="my-video" class="video-js"  poster="<?php if(isset($data)){ echo 'https://image.tmdb.org/t/p/w600'.$data->movieData->Movie_Info->backdrop_path; } ?>" data-setup='{"controls": true, "autoplay": true, "preload": "auto"}'>
                    <?php  $guid = $data->movieData->guid;
                            if($data->movieData->ext == "mp4"){ 
                                echo "<source src='http://31.15.224.24:53851/api/video/play/".$session."' type='video/mp4'/>"; 
                            }elseif($data->ext == "webm"){
                                echo "<source src='http://31.15.224.24:53851/api/video/play/".$session."'  type='video/webm'>"; 
                            } 
                            /*echo "<track kind='captions' src='http://31.15.224.24:8080/assets/subtitles/Angry.Birds.2016.720p.BluRay.x264-[YTS.AG].vtt' srclang='en' label='English' />";*/
                         ?>
                        <p class="vjs-no-js">
                          To view this video please enable JavaScript, and consider upgrading to a web browser that
                          <a href="http://videojs.com/html5-video-support/" target="_blank">supports HTML5 video</a>
                        </p>
                    </video>
                    <?php } ?>
                </div>
            </div>
            <!-- /#page-content-wrapper -->
        <script type="application/javascript">
            function toggleSidenav() {
              document.body.classList.toggle('sidenav-active');
            }
        </script>
    </body>
</html>
