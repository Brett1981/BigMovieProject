<?php
class Builder{
    
    public $watching;
    public $logedIn;
    public $enableGenres;
    public $data;
    public $scripts;
    public $user;
    public $modal;
            
    public function __construct($data){
        $this->watching     = $data['isWatching'];
        $this->logedIn      = $data['isLogedIn'];
        $this->user         = $data['user'];
        $this->enableGenres = $data['enableGenres'];
        $this->data         = $data['data'];
        $this->scripts      = $data['scripts'];
        $this->modal        = $data['modal'];
        $this->Create($this);
    }
    
    function Create($data){
        echo $this->Start(true); 
        $items = array();
        $items[]      = $this->Navigation("start");
        
        $items[]      = $this->User(
                            $this->logedIn,
                            $this->data,
                            $this->user
                            );
        
        $items[]      = $this->Search();
        
        $items[]      = $this->HamburgerMenu("start");
        
        $items[]      = $this->Links(
                            $this->enableGenres,
                            $this->logedIn,
                            $this->data,
                            $this->user
                            );

        if(isset($this->watching) && $this->watching != null){
            $items[]  = $this->Watching($this->watching);
        }
        
        $items[]     = $this->HamburgerMenu("end");
        
        $items[]     = $this->Navigation("end");
        
        $items[]     = $this->Modal($this->modal);
        
        $items[]     = $this->LoadScripts($this->scripts);
        foreach($items as $i){
            echo $i;
        }
        echo $this->Start(false);
    }
    private function Start($isStart){
            if($isStart){
                return "<div class='navigacija'>";
            }
            else{
                return "</div>";
            }
    }
    private function Navigation($d = null){
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
    private function Search(){
        return $s = "<!--- Search form --->
            <div class='search'>
                <form>
                    <input type='text' name='search' placeholder='Search..'>
                </form>
            </div>";
    }
    private function User($logedIn,$data,$user){
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

    private function HamburgerMenu($d){
        if($d !== "end"){
            $h = "<div class='hamburger-menu-top'>";
        }
        else{
            $h = "</div>";
        }
        return $h;

    }

    private function Links($enableGenres,$logedIn,$data,$user){
        $u = "
            <!--- Menu --->
            <div class='links'>
                <ul>
                    <li>
                        <a>Menu</a>
                            <ul>";
            if(!$logedIn){
                $u  .=         "<li id='user-login'><a id='login' href='#' >Login</a></li>";
            }
                $u  .=         "<li class='active'><a id='home' href='{$data['serverRoot']}/movies/' >Home</a></li>
                                <li><a id='about' href='#'>About</a></li>";

            if($logedIn){
                $u  .=         "<li><a href='{$data['serverRoot']}/index.php?logout={$user['unique_id']}'>Logout</a></li>";
            }
                $u  .=       "</ul>
                    </li>
                </ul>
            </div>";
            if(isset($enableGenres) && $enableGenres == true){
                $u  .=  "
                    <!--- Genres --->
                    <div class='genres'>
                            <ul>
                                <li>
                                    <a>Genres</a>
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

    private function Watching($d){

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

    private function Modal($m){
        $form = "";
        foreach($m as $item){
            $form .= "<div class='{$item['div']['class']}' id='{$item['div']['id']}' style='display:{$item['div']['style']['display']};overflow-y:{$item['div']['style']['overflow-y']}'>"
            . "<form id='{$item['form']['id']}' class='{$item['form']['class']}' method='{$item['form']['method']}' onsubmit='{$item['form']['onsubmit']}'>";
            foreach($item['inputs'] as $inp){
                $form .= "<input type='{$inp['type']}' placeholder='{$inp['placeholder']}' name='{$inp['name']}' onblur='{$inp['onblur']}' ";
                if($inp['isRequired']){ $form .= "required"; }
                $form .= ">";
            }
            $form .= "<button type='{$item['submit']['type']}' id='{$item['submit']['id']}' class='{$item['submit']['class']}' >{$item['submit']['text']}</button>"
            . "</form>"
            . "</div>";
        }
        return "<!-- The Modal -->
                    <div id='loginModal' class='modal'>

                        <!-- Modal content -->
                        <div class='modal-content'>
                            <div class='modal-header'>
                                <span class='close'>&times;</span>
                                <h2>Login / Register</h2>
                            </div>
                        <div class='modal-body'>
                            <!--- Forms --->
                            {$form}
                            <!--- Loader --->
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
            return $modal;
    }

    private function LoadScripts($lib){
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