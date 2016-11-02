<?php 
session_start();
$dir_nav =  ($_SERVER['DOCUMENT_ROOT'].'/streamingHTML/');
    $api;
    $guid;
    $watching = array();
    if(isset($_GET['id']) && $_GET['id'] != null){
        $mGuid = $_GET['id'];
        try {
        $api = file_get_contents('http://31.15.224.24:53851/api/video/getmovie?id='.$mGuid);
        
        } catch (Exception $e) {
            echo 'Caught exception: ',  $e->getMessage(), "\n";
        }
        $data = json_decode($api,true);
    }

    if(isset($data) && $data != null){
        $watching['movie_name'] = $data['MovieInfo']['title'];
        $watching['homepage'] = $data['MovieInfo']['homepage'];
        $watching['vote_average'] = $data['MovieInfo']['vote_average'];
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
        <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">
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
        <div class="hamburger" id="hamburger" onclick="toggleSidenav();">
          <div></div>
          <div></div>
          <div></div>
        </div>
        <!-- Sidebar -->
        <?php include $dir_nav.'website/navigation_left.php'; ?>
        <!-- /#sidebar-wrapper -->
        <!-- Page Content -->
        <div class="main">
                <div>
                    <?php if(isset($data)){
                        echo "<div class='play_movie_header'>
                                <img alt='movie_poster' src='https://image.tmdb.org/t/p/w300/".$data["MovieInfo"]["backdrop_path"]."'>
                            </div>
                            <div class='movie_information'>
                                <h1>".$data["MovieInfo"]["title"]."</h1>
                                <p>".$data["MovieInfo"]["tagline"]."</p>
                                <p>Release: ".$data["MovieInfo"]["release_date"]."</p>
                                <span>
                                    <p>".$data["MovieInfo"]["overview"]."</p>
                                </span>
                            </div>
                        ";
                    }
                    
                    ?>
                </div>
                <div>
                    <?php if(isset($data)){ ?>
                    <video id="my-video" class="video-js" controls preload="auto" width="640" height="264" poster="<?php if(isset($data)){ echo 'https://image.tmdb.org/t/p/w600'.$data["MovieInfo"]["backdrop_path"]; } ?>" data-setup="{}">
                        <?php if(isset($data)){
                            $guid = $data["movie_guid"];
                            if($data["movie_ext"] == "mp4"){ 
                                echo "<source src='http://31.15.224.24:53851/api/video/play?id=".$guid."' type='video/mp4'/>"; 
                            }elseif($data["movie_ext"] == "webm"){
                                echo "<source src='http://31.15.224.24:53851/api/video/play?id=".$guid."'  type='video/mp4'>"; 
                            } 
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
