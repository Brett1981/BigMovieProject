<?php 
session_start();
    try {
        $api = file_get_contents('http://localhost:53851/api/video/getmovie?id=83ba8796-57e0-4465-d462-5be019a3376c');
        
    } catch (Exception $e) {
        echo 'Caught exception: ',  $e->getMessage(), "\n";
    }
    $data = json_decode($api,true);
   
?>

<!DOCTYPE html>
<html>
    <head>
        <script type="text/javascript">

            function onLoad() {
                var sec = parseInt(document.location.search.substr(1));
                
                if (!isNaN(sec))
                    mainPlayer.currentTime = sec;
            }
        </script>
        <title>Partial Content Demonstration</title>
    </head>
    <body>
        <h3><?php  if($data["MovieInfo"]["title"] != ""){ echo $data["MovieInfo"]["title"]; }elseif($data["movie_name"] != ""){ echo $data["movie_name"]; }  ?> Content Demonstration</h3>
        <hr />
        <video id="mainPlayer" width="640" height="360" 
            autoplay="autoplay" controls="controls" onloadeddata="onLoad()">
            <source src="http://localhost:53851/api/video/play?id=<?php echo $data["movie_guid"];  ?>" />
        </video>
        <!-- 83ba8796-57e0-4465-d462-5be019a3376c -->
        <!-- fbb10db9-6932-b38d-b3e1-fc34c109c91f -->
    </body>
</html>