<?php 
$def = 'http://'.$_SERVER['HTTP_HOST'].'/streamingHTML/assets/icons/';
$user[] = array();
if(isset($username) && $username != null){ $user[0] = $username; }else{ $user[0] = "Username";}

echo "<div class='navigation_left'>
                <div class='user'>
                    <img class='user_img' src='".$def."user_default_icon.png' style='width:100px;'/>
                    <span>
                        <p>Welcome:</p>
                        <p class='user_username'>".$user[0]."</p>
                    </span>
                </div>
                <div class='navigation_items'>
                    <div class='hvr-underline-from-center'>
                        <a href='*'>
                            <img alt='Home' src='".$def."home.png'/>
                            <p>Home</p>
                        </a>
                    </div>
                    <div class='hvr-underline-from-center'>
                        <a href='*'>
                            <img alt='Search' src='".$def."magnifying-glass.png'/>
                            <p>Search</p>
                        </a>
                    </div>
                    <div class='hvr-underline-from-center'>
                        <a href='*'>
                            <img alt='Logout' src='".$def."logout.png'/>
                            <p>Logout</p>
                        </a>
                    </div>
                </div>
            </div>"

?>