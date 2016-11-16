<?php
session_start();
$dir_nav =  ($_SERVER['DOCUMENT_ROOT'].'/streamingHTML/');
/*$_SESSION['guid'] = "3fbddcc4-a446-4e5b-9d27-a8c118009ced";*/
//var_dump($_POST);
$enableGenres = true;
if(isset($_GET['id']) && $_GET['id'] != null){
    $_SESSION['guid'] = $_GET['id'];
}
$genreMovies;
if(isset($_GET['genre']) && $_GET['genre'] != ""){
    $g = $_GET['genre'];
    $genreMovies = json_decode(file_get_contents('http://31.15.224.24:53851/api/video/genre/'.$g),true);
}
/*if($_SESSION['guid'] == null && (isset($_POST['user_id']) && $_POST['user_id'] != null)){
    $_SESSION['guid'] = $_POST['user_id'];
}
else{
    header('Location: ../login/');
}*/

//echo $_SESSION['guid'];
?>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
    <head>
        <meta http-equiv="Content-Type" content="text/html;charset=UTF-8" />
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
        <?php include $dir_nav.'website/navigation_left.php'; ?>
        <!-- /#sidebar-wrapper -->
        <!-- Page Content -->
        <div class="main">
            <div class="movies">
            <?php 
                $data;
                if(isset($genreMovies) && $genreMovies != null){
                    $data = $genreMovies;
                }
                else{
                    $data = json_decode(file_get_contents('http://31.15.224.24:53851/api/video/allmovies'),true);
                }
            
                for($i = 0; $i < count($data); $i++){
                $movie = "<div id='m' class='movie' onClick='movie(this);'>

                        <div class='poster'>
                            <img alt='poster' src='https://image.tmdb.org/t/p/w300/".$data[$i]["MovieInfo"]["poster_path"]."' width='120'/>
                            <div class='gradient'></div>
                        </div>
                        <div class='movie_data'>
                            <div class='id' style='display:none'>".$data[$i]["movie_guid"]."</div>
                            <div class='title'>
                                <p>".$data[$i]["MovieInfo"]["title"]."</p><p style='font-style: italic;'>(".date_format(new DateTime($data[$i]["MovieInfo"]["release_date"]), 'Y').")</p>
                                <p>".$data[$i]["MovieInfo"]["tagline"]."</p>
                            <p>";
                            $genres = array();
                            if(strpos($data[$i]["MovieInfo"]["genres"], '|') !== false){
                                $genres = explode("|",$data[$i]["MovieInfo"]["genres"]);
                                for($y = 0; $y < count($genres);$y++){
                                    $x = explode(":",$genres[$y]);
                                    if($y < 2){
                                        if($y == 0){
                                            $movie .= (string)$x[1] ."/";
                                        }
                                        else{
                                            $movie .= (string)$x[1];
                                            break;
                                        }
                                    }
                                }
                            }else{
                                $genres = explode(":",$data[$i]["MovieInfo"]["genres"]);
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
