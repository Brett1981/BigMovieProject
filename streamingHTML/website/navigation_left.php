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
if(isset($_GET['id']) && !empty($_GET['id'])){
    if(strlen($_GET['id']) == 36){
        $_SESSION['guid'] = $_GET['id'];
    }
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
    $guid_nav = $user['unique_id'];
}else if(isset($_SESSION['guid']) && !empty($_SESSION['guid'])){
    getUserData($_SESSION['guid']);
}
else{
    //user is guest
    $user['username'] = "Guest";
    $_SESSION['user_img'] = $user_def_icon;
    $_SESSION['user_img_backup'] = $user_def_icon;
    
}

function getUserData($id)
{
    $user = Server::getUser($id);
    $guid_nav = $user["unique_id"];
    $img = null;
    if($user['profile_image'] !== null){
        $_SESSION['guid'] = $user['unique_id'];
        $img = Server::getUserProfilePicture($id);
    }
    else{
        $img_def = $GLOBALS['user_def_icon'];
    }
    if(!empty($img) && strlen($img) > 60){
        $_SESSION['user_img'] = $img;
        $_SESSION['user_img_backup'] = $img;
    }
    else{
        $_SESSION['user_img'] = $img_def;
        $_SESSION['user_img_backup'] = $img_def;

    }
    $_SESSION['user_data'] = $user;
    var_dump($_SESSION);
   
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
$navigation .=     "<a href='{$server_root}/index.php?logout={$_SESSION['guid']}'>Logout</a>";
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
      <h2>Login / Register</h2>
    </div>
    <div class='modal-login' id='login' style='display:block;'>
        <div class='modal-body'>
            <form id='login-form' class='form' method='post' >
                <input type='text' placeholder='Username' name='username'>
                <input type='password' placeholder='Password' name='password'>
                <button type='submit' id='login-button' class='preventSubmit'>Login</button>
            </form>
            <button id='switch'>Register</button>
        </div>
    </div>
    <div class='modal-register' id='register' style='display:none;'>
        <div class='modal-body'>
            <form id='register-form' class='form' method='post'>
                <input type='text' placeholder='Username' name='username' onblur='check(value,this)' required>
                <input type='password' placeholder='Password' name='password' onblur='check(value,this)' required>
                <input type='password' placeholder='Verify password' name='v_password' onblur='check(value,this)' required>
                <input type='email' placeholder='Email' name='email' onblur='check(value,this)' required>
                <input type='date' placeholder='Birthday' name='birthday'>
                <input type='text' placeholder='Display name' name='display_name' onblur='check(value,this)' required>
                <button type='submit' id='register-button' class='preventSubmit'>Register</button>
            </form>
            <button id='switch'>Login</button>
        </div>
    </div>
    <div class='modal-footer'>
    </div>
  </div>
  <script type='text/javascript' src='../website/login.js'></script>
  <ul class='bg-bubbles'>
        <li></li>
        <li></li>
        <li></li>
        <li></li>
        <li></li>
        <li></li>
        <li></li>
        <li></li>
        <li></li>
        <li></li>
    </ul>
</div>

";

?>