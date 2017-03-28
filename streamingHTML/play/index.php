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
if(isset($_SESSION['user']['unique_id']) && !empty($_SESSION['user']['unique_id']) ){
    //play movie to registered user
    if(isset($_GET['id']) && $_GET['id'] != null){
        $mGuid = $_GET['id'];
        try {
            $api = getMovie($_SESSION['user']['unique_id'], $mGuid);
            $api_session = getSessionRegistered($_SESSION['user']['unique_id'], $mGuid);
        } catch (Exception $e) {
            echo 'Caught exception: ',  $e->getMessage(), "\n";
        }
        $play = json_decode(json_decode(json_encode($api)));
        $session = json_decode($api_session, true);
        if(isset($play) && $play != null){
            $watching['movie_name'] = $play->movieData->Movie_Info->title;
            $watching['homepage'] = $play->movieData->Movie_Info->homepage;
            $watching['vote_average'] = $play->movieData->Movie_Info->vote_average;
            $watching['watched'] = $play->movieData->views;
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
        $play = json_decode(json_decode(json_encode($api)));
        $session = $play->sessionGuest->session_id;
        if(isset($play) && $play != null){
            $watching['movie_name'] = $play->movieData->Movie_Info->title;
            $watching['homepage'] = $play->movieData->Movie_Info->homepage;
            $watching['vote_average'] = $play->movieData->Movie_Info->vote_average;
            $watching['watched'] = $play->movieData->views;
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
        $play = array('user_id' => $user_id, 'movie_id' => $movie_id);
        $result = Server::getMovie($play);
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
        <title><?php  if(isset($play) && $play != null){
            if($play->movieData->Movie_Info->title != ""){
                echo $play->movieData->Movie_Info->title;
            }elseif($play->movieData->name != ""){
                echo $play->movieData->name;
            }else{ echo "Unknown movie";}
        }?></title>
        <script src="../assets/js/jquery-3.1.1.min.js"></script>
        <!-- VideoJs plugin and stylesheet -->
        <script src="../assets/videojs/video.min.js"></script>
        <link href="../assets/videojs/video-js.min.css" rel="stylesheet">
        <!-- support IE8 -->
        <script src="../assets/videojs/ie8/videojs-ie8.min.js"></script>
        <link rel="stylesheet" type="text/css" href="../css/style.css"/>
        <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">
        <script type="text/javascript">

            function onLoad() {
                var sec = parseInt(document.location.search.substr(1));

                if (!isNaN(sec))
                    mainPlayer.currentTime = sec;
            }
        </script>

    </head>
    <body class="sidenav-active">
        <!-- Sidebar -->
        <?php include $dir_nav; ?>
        <!-- /#sidebar-wrapper -->
        <!-- Page Content -->
        <div class="main">
                <div>
                    <?php if(isset($play)){ ?>
                    <video id="my-video" class="video-js"  poster="<?php if(isset($play)){ echo 'https://image.tmdb.org/t/p/w600'.$play->movieData->Movie_Info->backdrop_path; } ?>" data-setup='{"controls": true, "autoplay": false, "preload": "auto"}'>
                    <?php  $guid = $play->movieData->guid;
                            if($play->movieData->ext == "mp4"){
                                echo "<source src='{$client}/api/video/play/".$session."' type='video/mp4'/>";
                            }elseif($play->ext == "webm"){
                                echo "<source src='{$client}/api/video/play/".$session."'  type='video/webm'>";
                            }
                         ?>
                        <p class="vjs-no-js">
                          To view this video please enable JavaScript, and consider upgrading to a web browser that
                          <a href="http://videojs.com/html5-video-support/" target="_blank">supports HTML5 video</a>
                        </p>
                    </video>
                    <script src="../assets/videojs/videojs.hotkeys.min.js"></script>
                    <script>
                      // initialize the plugin
                      videojs('my-video').ready(function() {
                        this.hotkeys({
                          volumeStep: 0.1,
                          seekStep: 5,
                          enableMute: true,
                          enableFullscreen: true,
                          enableNumbers: false,
                          enableVolumeScroll: true,
                          // Enhance existing simple hotkey with a complex hotkey
                          fullscreenKey: function(e) {
                            // fullscreen with the F key or Ctrl+Enter
                            return ((e.which === 70) || (e.ctrlKey && e.which === 13));
                          },
                          // Custom Keys
                          customKeys: {
                            // Add new simple hotkey
                            simpleKey: {
                              key: function(e) {
                                // Toggle something with S Key
                                return (e.which === 83);
                              },
                              handler: function(player, options, e) {
                                // Example
                                if (player.paused()) {
                                  player.play();
                                } else {
                                  player.pause();
                                }
                              }
                            },
                            // Add new complex hotkey
                            complexKey: {
                              key: function(e) {
                                // Toggle something with CTRL + D Key
                                return (e.ctrlKey && e.which === 68);
                              },
                              handler: function(player, options, event) {
                                // Example
                                if (options.enableMute) {
                                  player.muted(!player.muted());
                                }
                              }
                            },
                            // Override number keys example from https://github.com/ctd1500/videojs-hotkeys/pull/36
                            numbersKey: {
                              key: function(event) {
                                // Override number keys
                                return ((event.which > 47 && event.which < 59) || (event.which > 95 && event.which < 106));
                              },
                              handler: function(player, options, event) {
                                // Do not handle if enableModifiersForNumbers set to false and keys are Ctrl, Cmd or Alt
                                if (options.enableModifiersForNumbers || !(event.metaKey || event.ctrlKey || event.altKey)) {
                                  var sub = 48;
                                  if (event.which > 95) {
                                    sub = 96;
                                  }
                                  var number = event.which - sub;
                                  player.currentTime(player.duration() * number * 0.1);
                                }
                              }
                            },
                            emptyHotkey: {
                              // Empty
                            },
                            withoutKey: {
                              handler: function(player, options, event) {
                                  console.log('withoutKey handler');
                              }
                            },
                            withoutHandler: {
                              key: function(e) {
                                  return true;
                              }
                            },
                            malformedKey: {
                              key: function() {
                                console.log('I have a malformed customKey. The Key function must return a boolean.');
                              },
                              handler: function(player, options, event) {
                                //Empty
                              }
                            }
                          }
                        });
                      });
                    </script>
                    <?php } ?>
                </div>
            </div>
            <!-- /#page-content-wrapper -->
        <script type="application/javascript">
            function toggleSidenav() {
              document.body.classList.toggle('sidenav-active');
            }
        </script>
        <script type="text/javascript" src="../website/nav.js"></script>
        <script type="text/javascript" src="play.js"></script>
    </body>
</html>
