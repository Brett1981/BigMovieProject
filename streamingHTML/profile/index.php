<?php
session_start();
$dir_nav =  ($_SERVER['DOCUMENT_ROOT'].'/streamingHTML/'); //default server path
$data = null;
if(isset($_GET['user']) && $_GET['user'] != null){
    $guid = $_GET['user'];
    $data = json_decode(file_get_contents('http://31.15.224.24:53851/api/users/getuser?guid='.$_SESSION['guid']),true);
    /*if($data != null){
        print_r(var_dump($data));
    }*/
}
?>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
    <head>
        <meta http-equiv="Content-Type" content="text/html;charset=UTF-8" />
        <title><?php ?></title>
        <link rel="stylesheet" type="text/css" href="../css/style.css"/>
        <script
  src="https://code.jquery.com/jquery-3.1.1.min.js"
  integrity="sha256-hVVnYaiADRTO2PzUGmuLJr8BLUSjGIZsDYGmIJLv2b8="
  crossorigin="anonymous"></script>
    </head>
    <body>
        <div id="wrapper">
            <!-- Sidebar -->
            <?php include $dir_nav.'website/navigation_left.php'; ?>
            <!-- /#sidebar-wrapper -->
            <!-- Page Content -->
            <div class="profile_wrapper">
                <?php 
                    if(isset($data) && $data != null){
                        $display_name = $data["user_display_name"];
                        $img = $data["profile_image"];
                        echo "<div class='user_profile'>
                                <div class='profile_picture'>
                                    <img alt='".$display_name."_picture' src='data:image/jpeg;base64, $img' />
                                </div>
                        
                        </div>";
                    }
                ?>
            </div>
            <!-- /#page-content-wrapper -->
        </div>
        <script type="application/javascript">
            function movie(x){
               var text = $(x).children(".movie_data").children(".id")[0].innerHTML;
                window.location.href = "../play/index.php?id="+text;
                console.log(text);
            }
        </script>
    </body>
</html>