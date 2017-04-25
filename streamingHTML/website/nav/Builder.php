<?php
class Builder{
    
    public $watching;
    public $logedIn;
    public $enableGenres;
    public $data;
    public $scripts;
    public $user;
            
    public function __construct($data){
        $this->watching     = $data['isWatching'];
        $this->logedIn      = $data['isLogedIn'];
        $this->user         = $data['user'];
        $this->enableGenres = $data['enableGenres'];
        $this->data         = $data['data'];
        $this->scripts      = $data['scripts'];
        $this->Create($this);
    }
    
    function Create($data){
        $s      = $this->LoadScripts($this->scripts);
        
        echo "<div class='navigacija'>";
        
        $n      = $this->Navigation("start");
        
        $u      = $this->User(
                $this->logedIn,
                $this->data,
                $this->user);
        
        $h      = $this->HamburgerMenu("start");
        
        $l      = $this->Links(
                $this->enableGenres,
                $this->logedIn,
                $this->data,
                $this->user
                );
        
        $he     = $this->HamburgerMenu("end");
        
        $w      = "";
        
        if(isset($this->watching) && $this->watching != null){
            $w  = $this->Watching($this->watching);
        }
        
        $m      = $this->Modal();
        
        $ne     = $this->Navigation("end");

        echo $n.$u.$h.$l.$w.$he.$ne.$m.$s;
        echo "</div>";
    }

    function Navigation($d = null){
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

    function User($logedIn,$data,$user){
        $u = "<div class='user'>";
            if($logedIn){
                if(isset($user['image']) && $user['image'] !== $data['userDefIcon']){
                    $u .= "<a href='{$data['profilePage']}'><img class='user_img' src='data:image/jpeg;base64, {$user['image']}' ";
                }
                else{
                    $u .= "<a href='{$data['profilePage']}'><img class='user_img' src='{$user['image']}' ";
                } 
            }
            else{
                    $u .= "<a href='#' id='login-pic'><img class='user_img' src='{$data['userDefIcon']}' ";
            }
                    $u .= " style='width:100px;'/> 
                    <!-- For modern browsers. -->
                    <i class='material-icons'>settings</i>
                    </a>
                    <span>
                        <p>Welcome:</p>
                        <p class='user_username'>{$user['username']}</p>
                    </span>
                </div>";
        return $u;
    }

    function HamburgerMenu($d){
        if($d !== "end"){
            $h = "<div class='hamburger-menu-top'>";
        }
        else{
            $h = "</div>";
        }
        return $h;

    }

    function Links($enableGenres,$logedIn,$data,$user){
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
                $u  .=         "<li><a href='{$data['serverRoot']}/index.php?logout={$user['unique_id']}'>Logout</a></li>";
            }
                $u  .=       "</ul>
                    </li>
                </ul>
            </div>";
            if(isset($enableGenres) && $enableGenres == true){
                $u  .=  "<div class='search'>
                            <ul>
                                <li><a>Genres</a>
                                    <ul>";
                                    foreach($data['genres'] as $key => $value){
                                        $u .= "<li><a href='";
                                        if(strpos($value, '.php') !== false){
                                            $u .= $value;
                                        }
                                        else{
                                            $u .= "index.php?genre={$value}";
                                        }
                                        $u .= "'>{$key}</a></li>";
                                    }
                                $u .= "</ul>
                                </li>
                            </ul>
                        </div>";
                    }
        return $u;
    }

    function Watching($d){

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

    function Modal(){
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

    function LoadScripts($lib){
        $scripts = "";
        foreach($lib as $key ){
            foreach($key['items'] as $items){
                $scripts .= "<script src='".$key['root'].$items."'></script>";
            }
        }
        return $scripts;

    }
}

?>