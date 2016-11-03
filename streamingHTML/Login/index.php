<?php
session_start();
$_SESSION['guid'] = "3fbddcc4-a446-4e5b-9d27-a8c118009ced";
?>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
    <head>
        <meta http-equiv="Content-Type" content="text/html;charset=UTF-8" />
        <title>Movies</title>
        <link rel="stylesheet" type="text/css" href="../css/style.css"/>
        <script
  src="https://code.jquery.com/jquery-3.1.1.min.js"
  integrity="sha256-hVVnYaiADRTO2PzUGmuLJr8BLUSjGIZsDYGmIJLv2b8="
  crossorigin="anonymous"></script>
    </head>
    <body>
        <!-- Page Content -->
        <div class="main">
            <form method="post" action="../index.php?login=user">
                <input type="text" name="username" value=""></input>
                <input type="password" name="password" </input>
                <input type="submit" name="submit"></input>
            </form>
        </div>
        <!-- /#page-content-wrapper -->
        <script type="application/javascript">
        </script>
    </body>
</html>