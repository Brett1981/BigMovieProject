<?php 
$client = Server::Client();

//server communicator
include_once '../server/serverClass.php';

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
/*if($_SESSION['guid'] == null){
    session_destroy();
    header('location: ../login/');
    
}*/
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
}else if(isset($_SESSION['guid']) && !empty($_SESSION['guid'])){
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
else{
    //user is guest
    $user['username'] = "Guest";
    $_SESSION['user_img'] = $user_def_icon;
    $_SESSION['user_img_backup'] = $user_def_icon;
    
}
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
                <div class='user'>";
                        if(isset($img) && strlen($img) > 100){
                            $navigation .= "<a href='$profile'><img class='user_img' src='data:image/jpeg;base64, $img' style='width:100px;'/>";
                        }
                        else{
                            $navigation .= "<a href='#'><img class='user_img' src='../assets/icons/user_default_icon.png' style='width:100px;'/>";
                        }
                        $navigation .= "<!-- For modern browsers. -->
                        <i class='material-icons'>settings</i>
                    </a>
                    <span>
                        <p>Welcome:</p>
                        <p class='user_username'>{$user["username"]}</p>
                    </span>
                </div>
                <div class='links'>";
                if(!isset($_SESSION['guid']) && $user["username"] == "Guest"){
                  $navigation .= "<a id='user-login' href='#' >Login / Register</a>";
                }
$navigation .=    "<a class='active' href='{$server_root}/movies/'>Home</a>
                  <a href='#'>Search</a>
                  <a href='#'>About</a>";

             if(isset($_SESSION['guid'])){
$navigation .=     "<a href='{$server_root}/index.php?logout={$guid_nav}'>Logout</a>";
             }     
$navigation .= "</div>";            
                
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
                   </div>
                </div>";
}
 echo $navigation."<script src='../website/login.js'></script></nav>";
 echo "<!-- The Modal -->
<div id='loginModal' class='modal'>

  <!-- Modal content -->
  <div class='modal-content'>
    <div class='modal-header'>
      <span class='close'>&times;</span>
      <h2>Modal Header</h2>
    </div>
    <div class='modal-body'>
      <p>Some text in the Modal Body</p>
      <p>Some other text...</p>
    </div>
    <div class='modal-footer'>
      <h3>Modal Footer</h3>
    </div>
  </div>
</div>
<style>
/* The Modal (background) */
.modal {
    display: none; /* Hidden by default */
    position: fixed; /* Stay in place */
    z-index: 1; /* Sit on top */
    padding-top: 100px; /* Location of the box */
    left: 0;
    top: 0;
    width: 100%; /* Full width */
    height: 100%; /* Full height */
    overflow: auto; /* Enable scroll if needed */
    background-color: rgb(0,0,0); /* Fallback color */
    background-color: rgba(0,0,0,0.4); /* Black w/ opacity */
}

/* Modal Content */
.modal-content {
    position: relative;
    background-color: #fefefe;
    margin: auto;
    padding: 0;
    border: 1px solid #888;
    width: 50%;
    box-shadow: 0 4px 8px 0 rgba(0,0,0,0.2),0 6px 20px 0 rgba(0,0,0,0.19);
    -webkit-animation-name: animatetop;
    -webkit-animation-duration: 0.4s;
    animation-name: animatetop;
    animation-duration: 0.4s
}

/* Add Animation */
@-webkit-keyframes animatetop {
    from {top:-300px; opacity:0} 
    to {top:0; opacity:1}
}

@keyframes animatetop {
    from {top:-300px; opacity:0}
    to {top:0; opacity:1}
}

/* The Close Button */
.close {
    color: white;
    float: right;
    font-size: 28px;
    font-weight: bold;
}

.close:hover,
.close:focus {
    color: #000;
    text-decoration: none;
    cursor: pointer;
}

.modal-header {
    padding: 2px 16px;
    background-color: #5cb85c;
    color: white;
}

.modal-body {padding: 2px 16px;}

.modal-footer {
    padding: 2px 16px;
    background-color: #5cb85c;
    color: white;
}
</style>
"

?>