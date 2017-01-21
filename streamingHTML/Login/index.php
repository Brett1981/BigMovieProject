<?php
session_start();
/*$_SESSION['guid'] = "3fbddcc4-a446-4e5b-9d27-a8c118009ced";*/
if(isset($_SESSION['guid']) && $_SESSION['guid'] != null){
    header('Location: ../movies/');
}
?>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
    <head>
        <meta http-equiv="Content-Type" content="text/html;charset=UTF-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1">
        <title>Movies</title>
        <link rel="stylesheet" type="text/css" href="../css/style.css"/>
        <script
  src="https://code.jquery.com/jquery-3.1.1.min.js"
  integrity="sha256-hVVnYaiADRTO2PzUGmuLJr8BLUSjGIZsDYGmIJLv2b8="
  crossorigin="anonymous"></script>
        <script src="../assets/js/jquery.redirect.js" type="application/javascript"></script>
         
        <link rel="stylesheet" href="//maxcdn.bootstrapcdn.com/font-awesome/4.3.0/css/font-awesome.min.css"/>
    </head>
    <body>
        <!-- Page Content -->
        <div class="main">
            <div id="authentication" class="wrapper">
                <div class="container">
                    <div id="login" class="">
                        <h1>Welcome</h1>
                        <form id="login-form" class="form" method="post">
                            <input type="text" placeholder="Username" name="username">
                            <input type="password" placeholder="Password" name="password">
                            <button type="submit" id="login-button" class="preventSubmit">Login</button>
                            <a id="switch">Register</a>
                        </form>
                    </div>
                    <div id="register" class="hidden">
                        <h1>Register</h1>

                        <form id="register-form" class="form" method="post">
                            <input type="text" placeholder="Username" name="username" onblur="check(value,this)" required>
                            <input type="password" placeholder="Password" name="password" onblur="check(value,this)" required>
                            <input type="password" placeholder="Verify password" name="v_password" onblur="check(value,this)" required>
                            <input type="email" placeholder="Email" name="email" onblur="check(value,this)" required>
                            <input type="date" placeholder="Birthday" name="birthday">
                            <input type="text" placeholder="Display name" name="display_name" onblur="check(value,this)" required>
                            <button type="submit" id="register-button" class="preventSubmit">Register</button>
                            <a id="switch2">Login</a>
                        </form>

                    </div>
                </div>
                

                <ul class="bg-bubbles">
                    <li></li>
                    <li></li>
                    <li></li>
                    <li></li>
                    <li></li>
                    <li></li>
                    <li></li>
                    <li></li>
                    <li></li>
                    <li></li>
                </ul>
            </div>
        </div>
        <!-- /#page-content-wrapper -->
        <script src="login.js" type="application/javascript"></script>
    </body>
</html>