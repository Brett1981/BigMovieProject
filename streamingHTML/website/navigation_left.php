<?php


//server communicator
include_once '../server/serverClass.php';
include_once 'navigationClass.php';
$client = Server::Client();
//root of project
$data['dirRoot']        = dirname(dirname(__FILE__ ));
//Website url
$data['serverPath']     = 'http://'.$_SERVER['HTTP_HOST'];
$data['serverRoot']     = $data['serverPath'].'/'.basename(dirname(dirname(__FILE__)));
//navigation dir
$data['dirNav']         = $data['dirRoot'].'\website\navigation_left.php';
//content location
$data['icons']          = $data['serverRoot'].'/assets/icons/';
$data['userDefIcon']    = $data['icons'] .'user_default_icon.png';
$data['homePage']       = $data['serverRoot'].'/movies/';
//nov profile link!
$data['profilePage']    = $data['serverRoot'].'/profile/index.php';

//init variables
$img;
$user;
$guid_nav;
$logedIn = false;
if(isset($_SESSION['user']) && !empty($_SESSION['user']) && !empty($_SESSION['user']['unique_id'])){

    $user = $_SESSION['user'];
}
elseif((isset($_GET['uid']) && !empty($_GET['uid']))
        && empty($_SESSION['user']['unique_id'])){
    if(strlen($_GET['uid']) == 36 ){
        $data['userGuid'] = $_GET['uid'];
        //init navigation class
        $navData = new Navigation($data);
        $user = $navData->user;
        $_SESSION['user'] = $user;

    }
}
else{
    $navData = new Navigation($data);
    $user = $navData->user;
    $_SESSION['user'] = $user;
}

if(isset($user) && !empty($user)){
    if(isset($user['unique_id']) && !empty($user['unique_id'])){
        $logedIn = true;
    }
    else{
        $logedIn = false;
    }
}

$navigation = "<div class='hamburger' id='hamburger' onclick='toggleSidenav();'>
          <div></div>
          <div></div>
          <div></div>
        </div>
        <nav>
            <div class='nav-scroll'>
                <div class='user'>";
                        if($logedIn){
                            if(strpos($_SESSION['user']['profile_image'], 'user_def_icon.png') !== false)
                                $navigation .= "<a href='{$data['profilePage']}'><img class='user_img' src='data:image/jpeg;base64, {$_SESSION['user']['profile_image']}' ";
                            else
                                $navigation .= "<a href='{$data['profilePage']}'><img class='user_img' src='{$_SESSION['user']['profile_image']}' ";
                        }
                        else{
                            $navigation .= "<a href='#'><img class='user_img' src='{$_SESSION['user']['profile_image']}' ";
                        }
                        $navigation .= " style='width:100px;'/> ";
                        $navigation .= "<!-- For modern browsers. -->
                        <i class='material-icons'>settings</i>
                    </a>
                    <span>
                        <p>Welcome:</p>
                        <p class='user_username'>{$_SESSION['user']['username']}</p>
                    </span>
                </div>
                <div class='links'><ul>
                    <li>
                        <a>Menu</a>
                            <ul>";
                if(!$logedIn){
                  $navigation .= "<li id='user-login'><a href='#' >Login</a></li>";
                }
$navigation .=    "<li class='active'><a href='{$data['serverRoot']}/movies/' >Home</a></li>
                  <li><a href='#'>Search</a></li>
                  <li><a href='#'>About</a></li>";

             if($logedIn){
$navigation .=     "<li><a href='{$data['serverRoot']}/index.php?logout={$_SESSION['user']['unique_id']}'>Logout</a></li>";
             }
    $navigation .= "</ul>
                    </li>
                    </ul>
                    </div>";

                if(isset($enableGenres) && $enableGenres == true){
                $navigation .= "<div class='search'>
                    <br/>
                    <br/>
                    <ul>
                        <li><a>Genres</a>
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
                        </li>
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
        </div>
    </div>
    <div class='modal-footer'>
        <p style=''><a id='modal-footer-register'>If you do not have an account register here</a></p>
        <p style='display:none;'><a id='modal-footer-login'>Click here if you have an account</a></p>
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
