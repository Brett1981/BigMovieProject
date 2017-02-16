<?php
session_start();
include_once '../server/serverComm.php';
$dir_nav =  ($_SERVER['DOCUMENT_ROOT'].'/streamingHTML/');
/*$_SESSION['guid'] = "3fbddcc4-a446-4e5b-9d27-a8c118009ced";*/
//var_dump($_POST);
$enableGenres = true;
$genreMovies;
$top10;
$last10;
$all;
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
<!DOCTYPE HTML>
<!--
	Multiverse by HTML5 UP
	html5up.net | @ajlkn
	Free for personal and commercial use under the CCA 3.0 license (html5up.net/license)
-->
<html>
	<head>
		<title>Multiverse by HTML5 UP</title>
		<meta charset="utf-8" />
		<meta name="viewport" content="width=device-width, initial-scale=1, user-scalable=no" />
		<!--[if lte IE 8]><script src="assets/js/ie/html5shiv.js"></script><![endif]-->
		<link rel="stylesheet" href="assets/css/main.css" />
		<!--[if lte IE 9]><link rel="stylesheet" href="assets/css/ie9.css" /><![endif]-->
		<!--[if lte IE 8]><link rel="stylesheet" href="assets/css/ie8.css" /><![endif]-->
	</head>
	<body>

		<!-- Wrapper -->
			<div id="wrapper">

				<!-- Header -->
					<header id="header">
						<h1><a href="index.html"><strong>Multiverse</strong> by HTML5 UP</a></h1>
						<nav>
							<ul>
								<li><a href="#footer" class="icon fa-info-circle">About</a></li>
							</ul>
						</nav>
					</header>

				<!-- Main -->
					<div id="main">
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
                                            echo "<article class='thumb'>";
                                                echo "<a href='https://image.tmdb.org/t/p/w300/{$data[$i]["MovieInfo"]["poster_path"]}' class='image'><img src='https://image.tmdb.org/t/p/w300/{$data[$i]["MovieInfo"]["poster_path"]}' alt='' /></a>";
                                                
                                                echo "<div><h2>{$data[$i]["MovieInfo"]["title"]}</h2></div>";
                                                /*$release = date_format(new DateTime($data[$i]["MovieInfo"]["release_date"]), 'Y');
                                                echo "<p>Release : {$release} </p>";*/
                                                echo "<p>{$data[$i]["MovieInfo"]["tagline"]}</p>";
                                            //<div class='id' style='display:none'>".$data[$i]["movie_guid"]."</div>
                                            /*$genres = array();
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
                                            echo $movie;*/
                                            echo "</article>";
                                        }
                                            ?>
					</div>

				<!-- Footer -->
					<footer id="footer" class="panel">
						<div class="inner split">
							<div>
								<section>
									<h2>Magna feugiat sed adipiscing</h2>
									<p>Nulla consequat, ex ut suscipit rutrum, mi dolor tincidunt erat, et scelerisque turpis ipsum eget quis orci mattis aliquet. Maecenas fringilla et ante at lorem et ipsum. Dolor nulla eu bibendum sapien. Donec non pharetra dui. Nulla consequat, ex ut suscipit rutrum, mi dolor tincidunt erat, et scelerisque turpis ipsum.</p>
								</section>
								<section>
									<h2>Follow me on ...</h2>
									<ul class="icons">
										<li><a href="#" class="icon fa-twitter"><span class="label">Twitter</span></a></li>
										<li><a href="#" class="icon fa-facebook"><span class="label">Facebook</span></a></li>
										<li><a href="#" class="icon fa-instagram"><span class="label">Instagram</span></a></li>
										<li><a href="#" class="icon fa-github"><span class="label">GitHub</span></a></li>
										<li><a href="#" class="icon fa-dribbble"><span class="label">Dribbble</span></a></li>
										<li><a href="#" class="icon fa-linkedin"><span class="label">LinkedIn</span></a></li>
									</ul>
								</section>
								<p class="copyright">
									&copy; Unttled. Design: <a href="http://html5up.net">HTML5 UP</a>.
								</p>
							</div>
							<div>
								<section>
									<h2>Get in touch</h2>
									<form method="post" action="#">
										<div class="field half first">
											<input type="text" name="name" id="name" placeholder="Name" />
										</div>
										<div class="field half">
											<input type="text" name="email" id="email" placeholder="Email" />
										</div>
										<div class="field">
											<textarea name="message" id="message" rows="4" placeholder="Message"></textarea>
										</div>
										<ul class="actions">
											<li><input type="submit" value="Send" class="special" /></li>
											<li><input type="reset" value="Reset" /></li>
										</ul>
									</form>
								</section>
							</div>
						</div>
					</footer>

			</div>

		<!-- Scripts -->
			<script src="assets/js/jquery.min.js"></script>
			<script src="assets/js/jquery.poptrox.min.js"></script>
			<script src="assets/js/skel.min.js"></script>
			<script src="assets/js/util.js"></script>
			<!--[if lte IE 8]><script src="assets/js/ie/respond.min.js"></script><![endif]-->
			<script src="assets/js/main.js"></script>

	</body>
</html>