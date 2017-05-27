<?php
//server communicator
require '../server/serverClass.php';
//movie list class
require 'movieClass.php';

session_start();
//root of project
$dir_root = dirname(dirname(__FILE__ ));

//navigation dir
$dir_nav = $dir_root.'\website\nav.php';

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
    $genreMovies = Server::GetByGenre($g);
}
elseif(isset($_GET['top10'])){
    $top10 = Server::GetTop10();
}
elseif(isset($_GET['last10'])){
    $last10 = Server::GetLast10();
}
elseif(isset($_GET['showall'])){
    $showall = Server::GetAllMovies();
}
else{
    $all = Server::GetAllMovies();
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
        <link rel="stylesheet" type="text/css" href="../css/loader.css"/>
        <link href="https://fonts.googleapis.com/icon?family=Material+Icons"
      rel="stylesheet">
        <script src="../assets/js/jquery-3.1.1.min.js"></script>
    </head>
    <body class="sidenav-active">
        <!-- Sidebar -->
        <?php include $dir_nav; ?>
        <!-- /#sidebar-wrapper -->
        <!-- Page Content -->
        <div class="main">
            <!-- Movie list -->
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
        <!-- Modal Movie -->
        <div id='movieModal' class='modal'>
          <div class='modal-movie-content'>
            <div class='modal-movie-header'>
              <span class='close'>&times;</span>
              <h2></h2>
            </div>
            <div class='modal-movie-body'>
            </div>
          </div>
        </div>
        <!-- /#page-content-wrapper -->
    </body>
</html>
