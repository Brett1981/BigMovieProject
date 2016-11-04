<?php
session_start();
/*$_SESSION['guid'] = "3fbddcc4-a446-4e5b-9d27-a8c118009ced";*/
/*if(isset($_SESSION['guid']) && $_SESSION['guid'] != null){
    header('Location: ../movies/');
}*/
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
                            <input type="text" placeholder="Username" name="username">
                            <input type="password" placeholder="Password" name="password">
                            <input type="email" placeholder="Email" name="email">
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
        <script type="application/javascript">
            
            $("#switch").click(function(event){
                var parent = $('.container').find('div');
                var first = parent.first();
                //.addClass("hidden")
                var last = parent.last();
                //.removeClass("hidden")
                first.addClass("hidden");
                last.removeClass("hidden");
            });
            $("#login-button").click(function(event){
                event.preventDefault();
                var user = {username : event.currentTarget.form.username.value,password : event.currentTarget.form.password.value};
                postData(user,"login");
            });
            
            $("#switch2").click(function(event){
                var parent = $('.container').find('div');
                var first = parent.first();
                //.addClass("hidden")
                var last = parent.last();
                //.removeClass("hidden")
                first.removeClass("hidden");
                last.addClass("hidden");
            });
            $("#register-button").click(function(event){
                event.preventDefault();
                var register = {username : event.currentTarget.form.username.value,password : event.currentTarget.form.password.value, user_email: event.currentTarget.form.email.value};
                postData(register,"register");
                //error
            });
            
            function postData(post,form){
                var http = new XMLHttpRequest();
                var url = "";
                if(form == "login"){ url = "http://31.15.224.24:53851/api/users/login"; }else if(form == "register"){ url = "http://31.15.224.24:53851/api/users/create"; }
                var params = JSON.stringify(post);
                http.open("POST", url, true);
                
                //Send the proper header information along with the request
                http.setRequestHeader('Access-Control-Allow-Headers', '*');
                http.setRequestHeader('Access-Control-Allow-Origin', '*');
                http.setRequestHeader("Content-type", "application/json");

                http.onreadystatechange = function() {//Call a function when the state changes.
                    if(http.readyState == 4 && http.status == 200) {
                        var j = JSON.parse(http.responseText);
                        j.status = http.status;
                        if(form == "login"){ 
                            $('#login-form').fadeOut(500); 
                        }else if(form == "register"){ 
                            $('#register-form').fadeOut(500); 
                            
                        }
                        $('.wrapper').addClass('form-success');
                        if(form == "login"){ 
                            console.log(j);
                            sessionStorage.setItem('user_id', j.user_id);
                            window.location.href = "../movies/index.php?id="+j.user_id;
                        }else if(form == "register"){ 
                            $.redirect("../login/index.php"); 
                            
                        }
                        
                    }
                    
                    
                }
                http.send(params);
            }
        </script>
    </body>
</html>