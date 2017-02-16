<?php 
include_once '../server/serverComm.php';
$client = Server::Client();

//server communicator
include_once '../server/serverComm.php';

//root of project
$dir_root = dirname(dirname(__FILE__ ));

//Website url
$server_path = 'http://'.$_SERVER['HTTP_HOST'];
$server_root = $server_path.'/'.basename(dirname(dirname(__FILE__)));
//navigation dir
$dir_nav = $dir_root.'\website\navigation_left.php';

//content location
$icons = $server_root.'/assets/icons/';
$user_def_icon = $icons.'user_default_icon.png';
$home = $server_root.'/movies/';

//init variables
$img;
$user;
$guid_nav;
if($_SESSION['guid'] == null){
    session_destroy();
    header('location: ../login/');
    
}
if(isset($_SESSION['user_img']) && $_SESSION['user_img'] != null && $_SESSION['user_img'] != $_SESSION['user_img_backup']){
    $img = $_SESSION['user_img'];
    $_SESSION['user_img_backup'] = $_SESSION['user_img'];
}

if(isset($_SESSION['user_data']) && $_SESSION['user_data'] != null){
    if(empty($_SESSION['user_img'])){
        $img = $user_def_icon;
        $_SESSION['user_img'] = $user_def_icon;
    }
    else{
        $img = $_SESSION['user_img'];
    }
    
    $user = $_SESSION['user_data'];
    $guid_nav = $user["unique_id"];
}else{
    $user = Server::getUser($_SESSION['guid']);
    $guid_nav = $user["unique_id"];
    $img = null;
    if($user['profile_image'] !== null){
        $img = Server::getUserProfilePicture($_SESSION['guid']);
    }
    else{
        $img = $user_def_icon;
    }
    if(!empty($img)){
        $_SESSION['user_img'] = $img;
        $_SESSION['user_img_backup'] = $img;
    }
    else{
        $_SESSION['user_img'] = $user_def_icon;
        $_SESSION['user_img_backup'] = $user_def_icon;
    }
    $_SESSION['user_data'] = $user;
}
//star profile link!
//$profile = $server_path.'/streamingHTML/profile/index.php?user='.$guid_nav; 
//nov profile link!
$profile = $server_root.'/profile/index.php';


/*if(isset($username) && $username != null){ $user[0] = $username; }else{ $user[0] = "Username";}*/

$navigation = "<div class='hamburger' id='hamburger' onclick='toggleSidenav();'>
          <div></div>
          <div></div>
          <div></div>
        </div>
        <nav>
            <div class='nav-scroll'>
                <div class='user'>
                    <a href='$profile'>";
                        if(strlen($img) > 100){
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
                        <p class='user_username'>{$user["username"]}</p>
                    </span>
                </div>
                <div class='links'>
                  <a class='active' href='{$server_root}/movies/'>Home</a>
                  <a href='#'>Search</a>
                  <a href='#'>About</a>
                  <a href='{$server_root}/index.php?logout={$guid_nav}'>Logout</a>
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
                </div>
            ";
            
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
 echo $navigation."</div></nav>";

?>