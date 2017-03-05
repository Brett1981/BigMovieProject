<?php
session_start();
//movie list class
include_once 'movieClass.php';
//server communicator
include_once '../server/serverClass.php';
//root of project
$dir_root = dirname(dirname(__FILE__ ));

//navigation dir
$dir_nav = $dir_root.'\website\navigation_left.php';

//genres init
$enableGenres = true;
$genreMovies;
$top10;
$last10;
$showall;
$all;


//new client init
$client = Server::Client();

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
elseif(isset($_GET['showall'])){
    $showall = Server::getAllMovies();
}
else{
    $all = Server::getAllMovies();
    //Unit test for movies//
    //$all = Server::getDataTest();
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
            <?php
                Movies::itemsInRow(5);
                if(isset($genreMovies) && $genreMovies != null){
                    echo Movies::createMovieList($genreMovies,'genre');
                }
                else if(isset($top10) && $top10 != null){
                    echo Movies::createMovieList($top10,'top10');
                }
                else if(isset($last10) && $last10 != null){
                    echo Movies::createMovieList($last10,'last10');
                }
                elseif(isset($showall) && $showall != null){
                    echo Movies::createMovieList($showall,'showall');
                }
                else{
                    echo Movies::createMovieList($all,'all');
                    echo "<div class='show-more'><a href='index.php?showall'>Show more</a></div>";
                }
            ?>
        </div>
        <!-- /#page-content-wrapper -->
        <script type="application/javascript" src="../website/nav.js"></script>
        <script type="application/javascript" src="movies.js"></script>



    </body>
</html>
