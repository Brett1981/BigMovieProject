$("#switch").click(function(event){
    h1 = $('.wrapper .container #register').children("h1");
    h1.text("Register");
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
    var f = event.currentTarget.form
    var user = {username : f.username.value,
                password : f.password.value
               };
    postData(user,"login");
});

$("#switch2").click(function(event){
   
    h1.text("Welcome");
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
    var f = event.currentTarget.form;

        var register = { username : f.username.value,
                    password : f.password.value, 
                    user_email: f.email.value, 
                    user_display_name : f.display_name.value,
                    user_birthday : f.birthday.value, 
                    };
        postData(register,"register");
});

function postData(post,form){
    var http = new XMLHttpRequest();
    var url = "";
    if (form == "login"){ url = "http://31.15.224.24:53851/api/user/login"; }else if(form == "register"){ url = "http://31.15.224.24:53851/api/user/create"; }
    var params = JSON.stringify(post);
    http.open("POST", url, true);

    //Send the proper header information along with the request
    http.setRequestHeader('Access-Control-Allow-Headers', '*');
    http.setRequestHeader('Access-Control-Allow-Origin', '*');
    http.setRequestHeader("Content-type", "application/json");
    var h1 = null;
    
    if(form == "login"){h1 = $('.wrapper .container #login').children("h1");
                        //change welcome sign so that user see's changes
                       }
    else if(form == "register"){ h1 = $('.wrapper .container #register').children("h1");
                                //change welcome sign so that user see's changes}
                               }
     

    http.onreadystatechange = function() {//Call a function when the state changes.
        if (http.readyState == 4 && http.status == 200) {
            var j = JSON.parse(http.responseText);
            j.status = http.status;
            if (form == "login"){ 
                $('#login-form').fadeOut(500); 
            }else if (form == "register"){ 
                $('#register-form').fadeOut(500); 

            }

            $('.wrapper').addClass('form-success');
            if(form == "login"){ 
                h1.text("Welcome, " + post.username);
                sessionStorage.setItem('user_id', j.user_id);
                setTimeout(function(){ 
                    redirectTo[0] = "../movies/index.php?id="+j.user_id; 
                    redirectTo.myMethod(); 
                }, 2000);
            }else if(form == "register"){ 
                h1.text("You can now login.");
                setTimeout(function(){ 
                    redirectTo[0] = "../login/index.php"; 
                    redirectTo.myMethod(); 
                }, 2000);
            }
        }
        else if (http.readyState == 4 && http.status == 404){
            h1.text("User is not found");
        }
        else if (http.readyState == 4 && http.status == 401){
            h1.text("You are not authorized");
        }
        else {
            h1.text("An error occured");
        }
    }
    http.send(params);
}
redirectTo = [""];
redirectTo.myMethod = function (sProperty) {
    window.location.href = redirectTo[0];
};

function check(value,type){
    if(type.name == "username"){
        checkUsername(value);
    }
    else if(type.name == "password"){
        console.log(value);
    }
    else if(type.name == "v_password"){
        console.log(value);
    }
    else if(type.name == "email"){
        console.log(value);
    }
    else if(type.name == "display_name"){
        console.log(value);
    }
}

function checkUsername(value){
    var http = new XMLHttpRequest();
    h1 = $('.wrapper .container #register').children("h1");
    var url = "http://31.15.224.24:53851/api/user/check/"+value;
    console.log(url);
    http.open("GET", url, true);
    http.setRequestHeader('Access-Control-Allow-Headers', '*');
    http.setRequestHeader('Access-Control-Allow-Origin', '*');
    http.setRequestHeader("Content-type", "plain/text");
    http.onreadystatechange = function(){
        if (http.readyState == 4 && http.status == 200){
            console.log(http.responseText);
            if(http.responseText == "OK"){
                
                h1.text("Username is valid");
            }
            else if(http.responseText == "NOK"){
                h1.text("Username is already in use");
            }
        }
        else if(http.readyState == 4 && http.status != 200){
            
        }
        
    }
    http.send();
}
function checkPass(){
    
}