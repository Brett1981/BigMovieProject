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
//script list
$lib = [
    'lib'       => [
                    'root' => '../lib/',
                    'items' => ['Login.js','Menu.js','Modal.js','Movie.js']
                   ],
    'website'   => [
                    'root' => '../website/',
                    'items' => ['nav.js','login.js','movies.js']
                   ]
        ]; 
//init variables
$img;
$user;
$guid_nav;
$data['logedIn']        = false;
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
        $data['logedIn'] = true;
    }
    else{
        $data['logedIn'] = false;
    }
}

$eGenres = null;
if(isset($enableGenres)){
    $eGenres = $enableGenres;
}
if(isset($watching) && !empty($watching)){
    setNavigation($data['logedIn'],$eGenres,$data,$watching,$lib );
}
else{
    setNavigation($data['logedIn'],$eGenres,$data,null,$lib );
}

function setNavigation($logedIn,$enableGenres,$data,$watching = null,$lib){
    $s  = loadScripts($lib);
    echo "<div class='navigacija'>";
    $n  = navigation("start");
    $u  = user($logedIn,$data);
    $h  = hamburgerMenu("start");
    $l  = links($enableGenres,$logedIn,$data);
    $he  = hamburgerMenu("end");
    $w = "";
    if(isset($watching) && $watching != null){
        $w  = watching($watching);
    }
    $m  = modal();
    $ne  = navigation("end");
    
    echo $n.$u.$h.$l.$w.$he.$ne.$m.$s;
    echo "</div>";
}

function navigation($d = null){
    if($d !== "end"){
        $n      =  "<div class='hamburger' id='hamburger'>
                        <div></div>
                        <div></div>
                        <div></div>
                    </div>
                <nav>";
        $n      .= "<div class='nav-scroll'>";
    }
    else{
        $n      = "</div></nav>";
    }
    return $n;
    
}

function user($logedIn,$data){
    $u = "<div class='user'>";
        if($logedIn){
            if(isset($_SESSION['user']['profile_image']['Result']) && $_SESSION['user']['profile_image']['Result'] !== $data['userDefIcon']){
                $u .= "<a href='{$data['profilePage']}'><img class='user_img' src='data:image/jpeg;base64, {$_SESSION['user']['profile_image']['Result']}' ";
            }
            else{
                $u .= "<a href='{$data['profilePage']}'><img class='user_img' src='{$_SESSION['user']['profile_image']}' ";
            } 
        }
        else{
                $u .= "<a href='#' id='login-pic'><img class='user_img' src='{$_SESSION['user']['profile_image']}' ";
        }
                $u .= " style='width:100px;'/> 
                <!-- For modern browsers. -->
                <i class='material-icons'>settings</i>
                </a>
                <span>
                    <p>Welcome:</p>
                    <p class='user_username'>{$_SESSION['user']['username']}</p>
                </span>
            </div>";
    return $u;
}

function hamburgerMenu($d){
    if($d !== "end"){
        $h = "<div class='hamburger-menu-top'>";
    }
    else{
        $h = "</div>";
    }
    return $h;
    
}

function links($enableGenres,$logedIn,$data){
    $u = "<div class='links'>
            <ul>
                <li>
                    <a>Menu</a>
                        <ul>";
        if(!$logedIn){
            $u  .=         "<li id='user-login'><a id='login' href='#' >Login</a></li>";
        }
            $u  .=         "<li class='active'><a id='home' href='{$data['serverRoot']}/movies/' >Home</a></li>
                            <li><a id='search' href='#'>Search</a></li>
                            <li><a id='about' href='#'>About</a></li>";

        if($logedIn){
            $u  .=         "<li><a href='{$data['serverRoot']}/index.php?logout={$_SESSION['user']['unique_id']}'>Logout</a></li>";
        }
            $u  .=       "</ul>
                </li>
            </ul>
        </div>";

        if(isset($enableGenres) && $enableGenres == true){
            $u  .=  "<div class='search'>
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
                    </div>";
                }
    return $u;
}

function watching($d){
    
    return "<div class='movie_watching'>
                        <p>Watching: ".$d['movie_name']."</p>
                    </div>
                    <div class='movie_information'>
                        <div class='homepage'>
                            <a href='".$d['homepage']."'>
                                <i class='material-icons' style='font-size:32px;color:darkviolet'>launch</i>
                                <p>Homepage</p>
                            </a>
                        </div>
                        <div class='vote_average'>
                            <i class='material-icons' style='font-size:32px;color:darkviolet'>favorite</i>
                            <p>".$d['vote_average']."/10</p>
                        </div>
                        <div class='watched'>
                            <i class='material-icons' style='font-size:32px;color:darkviolet'>local_movies</i>
                            <div class='tooltip'>".$d['watched']."
                                <span class='tooltiptext'>Number of times this movie was viewed</span>
                            </div>
                        </div>
                   </div>
                </div>";

}

function modal(){
    return "<!-- The Modal -->
                <div id='loginModal' class='modal'>

                  <!-- Modal content -->
                  <div class='modal-content'>
                    <div class='modal-header'>
                      <span class='close'>&times;</span>
                      <h2>Login / Register</h2>
                    </div>
                    <div class='modal-body'>
                        <div class='modal-login' id='login' style='display:block;'>

                                <form id='login-form' class='form' method='post' >
                                    <input type='text' placeholder='Username' name='username' required>
                                    <input type='password' placeholder='Password' name='password' required>
                                    <button type='submit' id='login-button' class='preventSubmit'>Login</button>
                                </form>

                        </div>
                        <div class='modal-register' id='register' style='display:none;'>

                                <form id='register-form' class='form' method='post' onsubmit='return false'>
                                    <input type='text' placeholder='Username' name='username' onblur='Login.check(value,this)' required>
                                    <input type='password' placeholder='Password' name='password' onblur='Login.check(value,this)' required>
                                    <input type='password' placeholder='Verify password' name='v_password' onblur='Login.check(value,this)' required>
                                    <input type='email' placeholder='Email' name='email' onblur='Login.check(value,this)' required>
                                    <input type='date' placeholder='Birthday' name='birthday'>
                                    <input type='text' placeholder='Display name' name='display_name' onblur='Login.check(value,this)' required>
                                    <button type='submit' id='register-button' class='preventSubmit'>Register</button>
                                </form>

                        </div>
                        <div class='modal-loader' style='display:none;'>
                            <div class='cssload-container'>
                                    <ul class='cssload-flex-container'>
                                            <li>
                                                    <span class='cssload-loading'></span>
                                            </li>
                                    </ul>
                            </div>	
                        </div>
                    </div>
                    <div class='modal-footer'>
                        <p style=''><a id='modal-footer-register'>If you do not have an account register here</a></p>
                        <p style='display:none;'><a id='modal-footer-login'>Click here if you have an account</a></p>
                    </div>
                  </div>
                </div>
            </div>";
}

function loadScripts($lib){
    $scripts = "";
    foreach($lib as $key ){
        foreach($key['items'] as $items){
            $scripts .= "<script src='".$key['root'].$items."'></script>";
        }
    }
    return $scripts;
    
}


?>
