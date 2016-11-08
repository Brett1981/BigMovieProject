<?php 
$server_path = 'http://'.$_SERVER['HTTP_HOST'];
$icons = $server_path.'/streamingHTML/assets/icons/';

$home = $server_path.'/streamingHTML/movies/';

$user = json_decode(file_get_contents('http://31.15.224.24:53851/api/user/getuser/'.$_SESSION['guid']),true);
$guid_nav = $user["unique_id"];
$img = $user["profile_image"];
$profile = $server_path.'/streamingHTML/profile/index.php?user='.$guid_nav;


/*if(isset($username) && $username != null){ $user[0] = $username; }else{ $user[0] = "Username";}*/

$navigation = "<div class='hamburger' id='hamburger' onclick='toggleSidenav();'>
          <div></div>
          <div></div>
          <div></div>
        </div>
        <nav>
          <div class='user'>
            <a href='$profile'>";
                if($img != null){
                    $navigation .= "<img class='user_img' src='data:image/jpeg;base64, $img' style='width:100px;'/>";
                }
                else{
                    $navigation .= "<img class='user_img' src='../assets/icons/user_default_icon.png' style='width:100px;'/>";
                }
                $navigation .= "<!-- For modern browsers. -->
                <i class='material-icons'>settings</i>
            </a>
            <span>
                <p>Welcome:</p>
                <p class='user_username'>".$user["username"]."</p>
            </span>
          </div>
          <div class='links'>
            <a class='active' href='".$server_path."/streamingHTML/movies/'>Home</a>
            <a href='#'>Search</a>
            <a href='#'>About</a>
            <a href='".$server_path."/streamingHTML/index.php?logout=".$guid_nav."'>Logout</a>
          </div>";
          if(isset($enableGenres) && $enableGenres == true){
              $navigation .= "<div class='search'>
                <br/>
                <br/>
                <a>Genres:</a>
                <ul>
                    <li><a href='index.php'>All</a></li>
                    <li><a href='index.php?genre=action'>Action</a></li>
                    <li><a href='index.php?genre=adventure'>Adventure</a></li>
                    <li><a href='index.php?genre=animation'>Animation</a></li>
                    <li><a href='index.php?genre=comedy'>Comedy</a></li>
                    <li><a href='index.php?genre=drama'>Drama</a></li>
                    <li><a href='index.php?genre=family'>Family</a></li>
                    <li><a href='index.php?genre=history'>History</a></li>
                    <li><a href='index.php?genre=horror'>Horror</a></li>
                    <li><a href='index.php?genre=scifi'>Science Fiction</a></li>
                    <li><a href='index.php?genre=thriller'>Thriller</a></li>           
                </ul>
            </div>";
          }

//src='".$def."user_default_icon.png'
if(isset($watching) && $watching != null){
    $navigation .= "<div class='movie_watching'>
                        <p>Watching: ".$watching['movie_name']."</p>
                    </div>
                    <div class='movie_information'>
                        <div class='homepage'>
                            <a href='".$watching['homepage']."'>
                                <i class='material-icons' style='font-size:32px;color:darkviolet'>launch</i>
                                <p>Homepage</p>
                            </a>
                        </div>
                        <div class='vote_average'>
                            <i class='material-icons' style='font-size:32px;color:darkviolet'>favorite</i>
                            <p>".$watching['vote_average']."/10</p>
                        </div>
                        <div class='watched'>
                            <i class='material-icons' style='font-size:32px;color:darkviolet'>local_movies</i>
                            <div class='tooltip'>".$watching['watched']."
                                <span class='tooltiptext'>Number of times this movie was viewed</span>
                            </div>
                        </div>
                   </div>";
}
 echo $navigation."</nav>";

?>