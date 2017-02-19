<?php
session_start();
if(isset($_GET['logout']) && $_GET['logout'] !== null)
{
    echo $_GET['logout'];
    $_SESSION['user_data'] = null;
    $_SESSION['user_img'] = null;
    session_destroy();
    header('Location: login/');
}
elseif(isset($_GET['login']) && $_GET['login'] == "user" ){
    echo $_POST['username'];
    echo $_POST['password'];
    
}
else
{
    header('Location: movies/');
}
exit();

?>