/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
$('#user-login').click(function(){
    $('#loginModal').css("display","block");
})

// When the user clicks anywhere outside of the modal, close it
window.onclick = function(event) {
    if (event.target == $('.close')[0] ||event.target == $('#loginModal')[0]) {
        $('#loginModal').css("display","none");
    }
}



// When the user clicks on <span> (x), close the modal
$('.close').click = function() {
    console.log("escape clicked");
    $('#loginModal').css("display","none");
}

$('#switch').click(function(){
    if(this.innerHTML == 'Register'){
        $('.modal-login').css("display","none");
        $('.modal-register').css("display","block");
    }
    else{
        $('.modal-login').css("display","block");
        $('.modal-register').css("display","none");
    }
});

$('#login-form').submit(function(event){
    event.preventDefault();
    var f = event.currentTarget;
    var user = {username : f[0].value,
                password : f[1].value
               };
    console.log(user);
    postData(user,"login");      
});

$('#register-form').submit(function(event){
    var f = event.currentTarget.form;
    var register = { username : f.username.value,
                    password : f.password.value, 
                    email: f.email.value, 
                    display_name : f.display_name.value,
                    birthday : f.birthday.value, 
                    };
    console.log(register);
    //postData(register,"register");          
});


//user check register

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
    if(form == "login"){ url = "../login/client/client.php?login"; }
    else if(form == "register"){ url = "../login/client/client.php?register"; }
    var req = $.ajax({
        type: "POST",
        url: url,
        data: data
    });
    
    req.done(function(info){
        console.log(info);
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
        h1.text("You can now login as " + data.cred.username);
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
var f_password = null;
function check(value,type){
    if(type.name == "username"){
        checkUsername({username:value});
    }
    else if(type.name == "password"){
        f_password = value;
    }
    else if(type.name == "v_password"){
        if(value == f_password){
            
            console.log("Password 1 and 2 match!");
        }
        else{
            console.log("Password's do not match!");
        }
    }
    else if(type.name == "email"){
        console.log(value);
    }
    else if(type.name == "display_name"){
        console.log(value);
    }
}

function checkUsername(value){
    //var http = new XMLHttpRequest();
    h1 = $('.wrapper .container #register').children("h1");
    var req = $.ajax({
            type: "POST",
            url: '../login/client/client.php?check',
            data:value
        });
    h1.text("Checking username validity...");
    req.done(function(info){
        var json = JSON.parse(info);
        console.log(json);
        if(json['username_status'] == true){ 
            //username is available
            h1.text("Username is available!");
        }
        else{
            h1.text("Username is not available!");
        }
    });
}
function checkPass(){
    
}