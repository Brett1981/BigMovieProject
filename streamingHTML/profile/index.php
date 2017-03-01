<?php
session_start();

//server communicator
include_once '../server/serverClass.php';
$client = Server::Client();

//root of project
$dir_root = dirname(dirname(__FILE__ ));

//navigation dir
$dir_nav = $dir_root.'\website\navigation_left.php';

//init variables
$data = null;



if(isset($_GET['user'])  || isset($_SESSION['user']['unique_id'])){
    $user_id = null;
    $data = array('user' => null, 'history' => null);
    if(!empty($_SESSION['user']['unique_id'])){ $user_id = $_SESSION['user']['unique_id']; }
    elseif(!empty($_GET['user'])){ $user_id = $_GET['user']; }
    else{ header('location ../index.php'); }
    $data['user'] = Server::getUser($user_id);
    $data['history'] = Server::getUserHistory($user_id);
}
?>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
    <head>
        <meta http-equiv="Content-Type" content="text/html;charset=UTF-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1">
        <title><?php echo $data['user']['display_name']. " profile"?></title>
        <link rel="stylesheet" type="text/css" href="../css/style.css"/>
        <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">
        <script
  src="https://code.jquery.com/jquery-3.1.1.min.js"
  integrity="sha256-hVVnYaiADRTO2PzUGmuLJr8BLUSjGIZsDYGmIJLv2b8="
  crossorigin="anonymous"></script>
    </head>
    <body>
        <!-- Sidebar -->
        <?php include $dir_nav; ?>
        <!-- /#sidebar-wrapper -->
        <!-- Page Content -->
        <div class="main">
            <?php 
                if(isset($data['user']) && $data['user'] != null){
                    $profile = "<div class='user_profile'>
                            <div class='profile_picture'>";
                    
                    $profile .= "<form name='profile_pic_form' action='../upload.php?avatar=upload' method='post' enctype='multipart/form-data' class='profile_pic_form'>";
                    //echo strlen($_SESSION['user_img']);
                    if(strlen($_SESSION['user']['profile_image']) > 100){
                        $profile .= "<img alt='{$data['user']['display_name']}_picture' src='data:image/jpeg;base64, {$_SESSION['user']['profile_image']}' />";
                    }
                     else{
                        $profile .= "<img alt='profile_picture' src='{$_SESSION['user']['profile_image']}' style='width:100px;'/>";
                    }
                    $profile .= "   <div class='change_avatar'><a>Change profile picture</a><input type='file' name='avatar' class='avatar'/></div>
                                    <input type='submit' value='Upload Image' name='submit'>
                                </form></div>";
                    $profile .= "<div class='user_data'>
                                    <form  action='../upload.php?user=upload' method='post' class='user_data_form'>
                                        <div><label>Email: </label> <input type='email' name='user_email' value='{$data['user']['email']}' readonly/></div>
                                        <div><label>Profile created: </label><input type='text' name='profile_created' value='{$data['user']['profile_created']}' readonly/></div>
                                        <div><label>Last logon: </label><input type='text' name='last_logon' value='{$data['user']['last_logon']}' readonly/></div>
                                        <div><label>Birthday: </label><input type='date' name='user_birthday' value='{$data['user']['birthday']}' /></div>
                                    </form>
                                </div>";
                    echo $profile;
                                
                    if(isset($_SESSION['post_message'])){
                        echo "<div class='error'>{$_SESSION['post_message']}</div>";
                        $_SESSION['post_message'] = "";
                    }
                                    
                    echo "</div>";
                    $history = "<div class='user_log'><table><tbody><tr><th>Action</th><th>Type</th><th>Date</th></tr></tbody><tbody>";
                    foreach($data['history'] as $array){
                        $history .= "<tr>";
                        foreach($array as $key => $value){
                            switch($key){
                                case 'user_action':$history .= "<td>{$value}</td>"; ;break;
                                case 'user_type': $history .= "<td>{$value}</td>";;break;
                                case 'user_datetime': $history .= "<td>{$value}</td>";;break;
                            }
                        }
                        $history .= "</tr>";
                    }
                    echo $history."</tbody></table></div>";
                }
            ?>
        </div>
        <!-- /#page-content-wrapper -->
        <script type="application/javascript">
            function toggleSidenav() {
              document.body.classList.toggle('sidenav-active');
            }
        </script>
    </body>
</html>