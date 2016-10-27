<?php
session_start();
?>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
    <head>
        <meta http-equiv="Content-Type" content="text/html;charset=UTF-8" />
        <title>Movies</title>
        <link rel="stylesheet" type="text/css" href="../css/style.css"/>
    </head>
    <body>
        <h3>Movie list</h3>
        <div class="movielist">
            <?php 
            $data = json_decode(file_get_contents('http://localhost:53851/api/video/allmovies'),true);
                for($i = 0; $i < count($data); $i++){
                echo "<div class='movie'>
                        <hr/>
                        <div class='poster'>
                            <img alt='poster' src='https://image.tmdb.org/t/p/w300/".$data[$i]["MovieInfo"]["poster_path"]."' width='120'/>
                        </div>
                        <div class='title'>
                            <a>".$data[$i]["MovieInfo"]["title"]."</a>
                            <a>".$data[$i]["MovieInfo"]["tagline"]."</a>
                        </div>
                        <div class='movieinfo'>
                            <span>
                                <a>".$data[$i]["MovieInfo"]["overview"]."</a>
                            </span>
                        </div>
                    
                    </div>";
                }
            ?>
        </div>
        
    </body>
</html>
