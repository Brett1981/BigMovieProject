<?php
session_start();
if(isset($_GET['logout']) && $_GET['logout'] !== null)
{
    $_SESSION = null;
    session_destroy();
    header('Location: index.php');
}
else
{
    header('Location: movies/');
}
exit();

?>