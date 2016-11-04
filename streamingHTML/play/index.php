<?php 
session_start();
$dir_nav =  ($_SERVER['DOCUMENT_ROOT'].'/streamingHTML/');
$api;
$guid;
$watching = array();

if(isset($_SESSION['guid']) && $_SESSION['guid'] != null){
    if(isset($_GET['id']) && $_GET['id'] != null){
        $mGuid = $_GET['id'];
        try {
            
        /*$api = file_get_contents('http://31.15.224.24:53851/api/video/getmovie?id='.$mGuid);*/
            $api = get_movie($_SESSION['guid'], $mGuid);
        } catch (Exception $e) {
            echo 'Caught exception: ',  $e->getMessage(), "\n";
        }
        $data = json_decode($api,true);
        
        if(isset($data) && $data != null){
            $watching['movie_name'] = $data['MovieInfo']['title'];
            $watching['homepage'] = $data['MovieInfo']['homepage'];
            $watching['vote_average'] = $data['MovieInfo']['vote_average'];
            $watching['watched'] = $data['movie_views'];
        }
    }
    else{
        header('location: ../movies');
    }
    
}
else{
    header('location: ../login');
}

function get_movie($user_id, $movie_id, $username = null, $password = null)
{
    $api = "http://192.168.1.10:53851/api/video/getmovie";
    if((isset($user_id) && $user_id != null) && isset($movie_id) && $movie_id != null){
        $data = array('user_id' => $user_id, 'movie_id' => $movie_id);
        
        $data = json_encode($data);
        //cURL call to api
        $ch = curl_init( $api);
        # Setup request to send json via POST
        curl_setopt( $ch, CURLOPT_POSTFIELDS, $data );
        curl_setopt( $ch, CURLOPT_HTTPHEADER, array('Content-Type:application/json'));
        # Return response instead of printing.
        curl_setopt( $ch, CURLOPT_RETURNTRANSFER, true );
        # Send request.
        $result = curl_exec($ch);
        curl_close($ch);
        if($result == null){
            header('location: ../movies/');
            $_SESSION['play_error'] = "No movie was found ";
            exit();
        }
        
        return $result;
    }
    else{
        header('location: ../movies/');
        exit();
    }
    
}
?>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
    <head>
        <meta http-equiv="Content-Type" content="text/html;charset=UTF-8" />
        <title><?php  if(isset($data) && $data != null){ 
            if($data["MovieInfo"]["title"] != ""){ 
                echo $data["MovieInfo"]["title"]; 
            }elseif($data["movie_name"] != ""){ 
                echo $data["movie_name"]; 
            }else{ echo "Unknown movie";}  
        }?></title>
        
        <link href="http://vjs.zencdn.net/5.11.9/video-js.css" rel="stylesheet">
        <!-- If you'd like to support IE8 -->
        <script src="http://vjs.zencdn.net/ie8/1.1.2/videojs-ie8.min.js"></script>
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
        <?php include $dir_nav.'website/navigation_left.php'; ?>
        <!-- /#sidebar-wrapper -->
        <!-- Page Content -->
        <div class="main">
                <div>
                    <?php if(isset($data)){ ?>
                    <video id="my-video" class="video-js"  poster="<?php if(isset($data)){ echo 'https://image.tmdb.org/t/p/w600'.$data["MovieInfo"]["backdrop_path"]; } ?>" data-setup='{"controls": true, "autoplay": true, "preload": "auto"}'>
                        <?php if(isset($data)){
                            $guid = $data["movie_guid"];
                            if($data["movie_ext"] == "mp4"){ 
                                echo "<source src='http://31.15.224.24:53851/api/video/play?id=".$guid."' type='video/mp4'/>"; 
                            }elseif($data["movie_ext"] == "webm"){
                                echo "<source src='http://31.15.224.24:53851/api/video/play?id=".$guid."'  type='video/mp4'>"; 
                            } 
                            /*echo "<track kind='captions' src='http://31.15.224.24:8080/assets/subtitles/Angry.Birds.2016.720p.BluRay.x264-[YTS.AG].vtt' srclang='en' label='English' />";*/
                        } ?>
                        <p class="vjs-no-js">
                          To view this video please enable JavaScript, and consider upgrading to a web browser that
                          <a href="http://videojs.com/html5-video-support/" target="_blank">supports HTML5 video</a>
                        </p>
                    </video>

                    <script src="http://vjs.zencdn.net/5.11.9/video.js"></script>
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
