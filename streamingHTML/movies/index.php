<?php
session_start();

//server communicator
include_once '../server/serverComm.php';

//root of project
$dir_root = dirname(dirname(__FILE__ ));

//navigation dir
$dir_nav = $dir_root.'\website\navigation_left.php';

//genres init
$enableGenres = true;
$genreMovies;
$top10;
$last10;
$all;

//new client init
$client = Server::Client();

if(isset($_GET['id']) && $_GET['id'] != null){
    $_SESSION['guid'] = $_GET['id'];
}

if(isset($_GET['genre']) && $_GET['genre'] != ""){
    $g = $_GET['genre'];
    $genreMovies = Server::getByGenre($g);
}
elseif(isset($_GET['top10'])){
    $top10 = Server::getTop10();
}
elseif(isset($_GET['last10'])){
    $last10 = Server::getLast10();
}
else{
    $all = Server::getAllMovies();
}

?>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
    <head>
        <meta http-equiv="Content-Type" content="text/html;charset=UTF-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1">
        <title>Movies</title>
        <link rel="stylesheet" type="text/css" href="../css/style.css"/>
        <link href="https://fonts.googleapis.com/icon?family=Material+Icons"
      rel="stylesheet">
        <script
  src="https://code.jquery.com/jquery-3.1.1.min.js"
  integrity="sha256-hVVnYaiADRTO2PzUGmuLJr8BLUSjGIZsDYGmIJLv2b8="
  crossorigin="anonymous"></script>
        
    </head>
    <body class="sidenav-active">
        <!-- Sidebar -->
        <?php include $dir_nav; ?>
        <!-- /#sidebar-wrapper -->
        <!-- Page Content -->
        <div class="main">
            <div class="movies">
            <?php 
                $data;
                if(isset($genreMovies) && $genreMovies != null){
                    $data = $genreMovies;
                }
                else if(isset($top10) && $top10 != null){
                    $data = $top10;
                }
                 else if(isset($last10) && $last10 != null){
                    $data = $last10;
                }
                else{
                    $data = $all;
                }
            
                for($i = 0; $i < count($data); $i++){
                $movie = "<div id='m' class='movie' onClick='movie(this);'>

                        <div class='poster'>
                            <img alt='poster' src='https://image.tmdb.org/t/p/w160".$data[$i]["Movie_Info"]["poster_path"]."' width='120'/>
                            <div class='gradient'></div>
                        </div>
                        <div class='movie_data'>
                            <div class='id' style='display:none'>".$data[$i]["guid"]."</div>
                            <div class='title' style='min-width: 200px;'>
                                <p>".$data[$i]["Movie_Info"]["title"]."</p><p style='font-style: italic;'>(".date_format(new DateTime($data[$i]["Movie_Info"]["release_date"]), 'Y').")</p>
                                <p>".$data[$i]["Movie_Info"]["tagline"]."</p>
                            <p>";
                            $genres = array();
                            if(strpos($data[$i]["Movie_Info"]["genres"], '|') !== false){
                                $genres = explode("|",$data[$i]["Movie_Info"]["genres"]);
                                for($y = 0; $y < count($genres);$y++){
                                    $x = explode(":",$genres[$y]);
                                    if(count($y) < 2){
                                        if($y == 0){
                                            $movie .= (string)$x[1] ."/";
                                        }
                                        else{
                                            $movie .= (string)$x[1];
                                            break;
                                        }
                                    }
									else{
										$movie .= (string)$x[1];
										break;
									}
                                }
                            }else{
                                $genres = explode(":",$data[$i]["Movie_Info"]["genres"]);
                                $movie .= (string)$genres[1];
                                
                            }
                    $movie .=  "</p></div>
                        </div>
                    </div>";
                    echo $movie;
                }
            ?>
            
            </div>
        </div>
        <!-- /#page-content-wrapper -->
        <script type="application/javascript">
            function toggleSidenav() {
              document.body.classList.toggle('sidenav-active');
            }
            function movie(x){
               var text = $(x).children(".movie_data").children(".id")[0].innerHTML;
                window.location.href = "../play/index.php?id="+text;
                console.log(text);
            }
        </script>
    </body>
</html>
