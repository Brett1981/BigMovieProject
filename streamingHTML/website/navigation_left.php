<?php 
$server_path = 'http://'.$_SERVER['HTTP_HOST'];
$icons = $server_path.'/streamingHTML/assets/icons/';

$home = $server_path.'/streamingHTML/movies/';

$user = json_decode(file_get_contents('http://31.15.224.24:53851/api/users/getuser?guid='.$_SESSION['guid']),true);
$img = $user["profile_image"];
$profile = $server_path.'/streamingHTML/profile/index.php?user='.$user["unique_id"];


if(isset($username) && $username != null){ $user[0] = $username; }else{ $user[0] = "Username";}

$navigation = "<nav>
          <div class='user'>
            <a href='$profile'>
                <img class='user_img' src='data:image/jpeg;base64, $img' style='width:100px;'/>
            </a>
            <span>
                <p>Welcome:</p>
                <p class='user_username'>".$user["username"]."</p>
            </span>
          </div>
          <div class='links'>
            <a class='active' href='".$server_path."/streamingHTML/'>Home</a>
            <a href='#'>Search</a>
            <a href='#'>About</a>
            <a href='#'>Logout</a>
          </div>";
//src='".$def."user_default_icon.png'
if(isset($watching) && $watching != null){
    $navigation .= "
                <div class='movie_watching'>
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
               </div>";
}
 echo $navigation."</nav>";

?>