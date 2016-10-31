<?php 
session_start();
    $api;
    $guid;

    if(isset($_GET['id']) && $_GET['id'] != null){
        $guid = $_GET['id'];
        try {
        $api = file_get_contents('http://31.15.224.24:53851/api/video/getmovie?id='.$guid);
        
        } catch (Exception $e) {
            echo 'Caught exception: ',  $e->getMessage(), "\n";
        }
        $data = json_decode($api,true);
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
        <link rel="stylesheet" type="text/css" href="../css/style.css"/>
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
        <div id="wrapper">
            <!-- Sidebar -->
            <div class="navigation_left">
                <div class="user">
                    <img class="user_img" href=""/>
                    <p class="user_username">Test</p>
                </div>
            </div>
            <!-- /#sidebar-wrapper -->
            <!-- Page Content -->
            <div class="content_wrapper">
                <div>
                
                </div>
                <div>
                    <video id="mainPlayer" width="800" style="position:absolute; left:300px; top:100px"
                    autoplay="autoplay" controls="controls" onloadeddata="onLoad()">
                        <source src="http://31.15.224.24:53851/api/video/play?id=<?php echo $data["movie_guid"];  ?>" />
                    </video>
                </div>
                
            
            </div>
            
            <!-- 83ba8796-57e0-4465-d462-5be019a3376c -->
            <!-- fbb10db9-6932-b38d-b3e1-fc34c109c91f -->
            <!-- /#page-content-wrapper -->
        </div>
        <script type="application/javascript">
        </script>
    </body>
</html>
