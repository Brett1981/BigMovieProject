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
        console.log(register);
        postData(register,"register");
});
function loginSuccess(data,cred,form){
    console.log("Login successful");
    var user = {
        'client' : data,
        'cred' : cred,
        'form':form
    };
    changeHtmlDom(user);
}
function loginFailure(data){
    console.log("Login failed");
}
function registerSuccess(data,cred,form){
    console.log("Register successful");
    var user = {
        'client' : data,
        'cred' : cred,
        'form':form
    };
    changeHtmlDom(user);
}
function registerFailure(data){
    console.log("Register failed");
}
function postData(data,form){
    var url = "";
    if(form == "login"){ url = "client/client.php?login"; }
    else if(form == "register"){ url = "client/client.php?register"; }
    var req = $.ajax({
        type: "POST",
        url: url,
        data: data
    });
    
    req.done(function(info){
        var json =  JSON.parse(info);
        if(json['response'] == 'success'){ 
            if(form == "login"){ loginSuccess(json,data,form);  }
            else if(form == "register"){ registerSuccess(json,data,form);}
        }
        else{ 
            if(form == "login"){ loginFailure(json);  }
            else if(form == "register"){ registerFailure(json);}
        }
    });
}
function changeHtmlDom(data){
    var h1 = null;
    if(data.form == "login"){
        h1 = $('.wrapper .container #login').children("h1");
        //change welcome sign so that user see's changes
        $('#login-form').fadeOut(500); 
        $('.wrapper').addClass('form-success');
        h1.text("Welcome, " + data.cred.username);
        sessionStorage.setItem('user_id', data.client.uid);
        setTimeout(function(){ 
            redirectTo[0] = "../movies/index.php?id="+data.client.uid; 
            redirectTo.myMethod(); 
        }, 2000);
    }
    else if(data.form == "register"){ 
        h1 = $('.wrapper .container #register').children("h1");
        //change welcome sign so that user see's changes}
        $('#register-form').fadeOut(500); 
        $('.wrapper').addClass('form-success');
        h1.text("You can now login as " + data.data.username);
        setTimeout(function(){ 
            redirectTo[0] = "../login/index.php"; 
            redirectTo.myMethod(); 
        }, 2000);
    }
}

function registerUser(){
    
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