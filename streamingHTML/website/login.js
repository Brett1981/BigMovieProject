/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
//objects containing data 
var title;
var backupTitle;

// Login & register object 
var login = new function(){
    this.url    = "../login/client/client.php?login";
    this.regUrl = "../login/client/client.php?register";
    this.to = "";
    this.modalTitle = function(){
        return $('.modal-content .modal-header').children("h2");
    } 
    this.userReg = function(u,p,e,b,d){
        return {
            username    :u,
            password    :p, 
            email       :e, 
            birthday    :b, 
            display_name:d
        };
    }
    this.userLogin = function(u,p){
        return  {
            username    :u,
            password    :p
        };
    }
    this.Status = {
        SUCCESS :   {value:1,   name:"Success", code:"S"},
        ERROR   :   {value:2,   name:"Error",   code:"E"}
    };
    this.userRegData;
    //submit data 
    this.submit = function(data,call){
        var req = $.ajax({
            type: "POST",
            url: call,
            data: data
        });

        req.done(function(info){
            //console.log(info);
            var json =  JSON.parse(info);
            var arr = {response : json, data : data};
            if(json['response'] == 'success'){ 
                if(call == this.url){ login.completed(arr,this.url);  }
                else { login.completed(arr,this.regUrl);}
            }
            else{ 
                if(call == this.url){ login.failed(arr,this.url);  }
                else { login.failed(arr,this.regUrl);}
            }
        });
    }
    
    //on failed call
    this.failed = function(data,type){
        console.log(data);
        console.log(type);
        login.modalDom(data,type,this.Status.ERROR);
    }
    
    //on completed call
    this.completed = function(data,type){
        console.log(data);
        console.log(type);
        login.modalDom(data,type,this.Status.SUCCESS);
    }
    
    //edit modal DOM 
    this.modalDom = function(data,type,status){
        if(status == this.Status.SUCCESS){
            if(type == this.regUrl){
            
            }
            else{
                sessionStorage.setItem('user_id', data.response.uid);
                var id = "index.php?uid=" + data.response.uid;
                login.to = this.rootUrl(id);
                login.redirect();
            } 
        }
        else{
            
        }
    }
    
    //Parent check function for all elements inside register form
    this.check = function(data,type){
        if(this.userRegData == null){
            this.userRegData = this.userReg();
        }
        if(type.name == "username"){
            if(this.validateUsername({username:data})){
                this.userRegData.username = data;
            }
        }
        else if(type.name.includes("password")){
            if(type.name.includes("v")){
                if(this.userRegData.password != data){
                    //display error
                    
                }
                else{
                    //reset error
                }
            }
            else{
                this.userRegData.password = data;
            }
            
        }
        else if(type.name == "email"){
            if(this.validateEmail(data)){
                this.userRegData.email = data;
            }
        }
        else if(type.name == "display_name"){
            console.log(data);
        }
    }
    
    //Check username validity
    this.validateUsername = function(value){
        console.log(title);
        var req = $.ajax({
                type: "POST",
                url: '../login/client/client.php?check',
                data:value
            });
        title.text("Checking username validity...");
        req.done(function(info){
            var json = JSON.parse(info);
            console.log(json);
            if(json['username_status'] == true){ 
                //username is available
                title.text("Username is available!");
                var displayName = $('.modal-content .modal-register #register-form').children("display_name");
                displayName.text(value.username);
                return true;
            }
            else{
                title.text("Username is not available!");
                return false;
            }
        });
        return false;
    }
    
    //check email
    this.validateEmail = function(data){
        var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        return re.test(data);
    }
    
    //Generate url to redirect on login or register
    this.rootUrl = function(path){
        if(!path){
            return window.location.protocol + "//" + window.location.host + window.location.pathname;
        }
        return window.location.protocol + "//"+window.location.host + window.location.pathname + path;
    }
    
    //Redirect
    this.redirect = function(){
        setTimeout(function(){ 
            window.location = login.to;
        }, 2000);     
    }
} 

/* Login / Register */
$('#login-form').submit(function(event){
    event.preventDefault();
    var f = event.currentTarget;
    var user = login.userLogin(
                f[0].value,
                btoa(f[1].value)
            );
    login.submit(user,login.url);      
});

$('#register-form').submit(function(event){
    var f = event.currentTarget.form;
    var register = login.userReg(
                    f.username.value,
                    f.password.value,
                    f.email.value,
                    f.birthday.value,
                    f.display_name.value
                    );
    console.log(register);
    login.submit(register,login.regUrl);       
});


var modal = new function(){
    this.loginClick = function(){
        $('#loginModal').css("display","block");
        title = login.modalTitle();
        backupTitle = title;
    }
    this.profileClick = function(){
        $('#loginModal').css("display","block");
    }
    
    this.exit = function(event){
        if (event.target == $('.close')[0] ||event.target == $('#loginModal')[0] ||event.target.className == 'bg-bubbles') {
            $('#loginModal').css("display","none");
            $('.hamburger-menu-top').css("display","none");
        }
    }
    
    this.close = function(){
         $('#loginModal').css("display","none");
    }
    
    this.domItems = function(){
        return {
            mfi :   $('.modal-footer').children(),
            mf  :   $('.modal-footer'),
            ml  :   $('.modal-login'),
            mr  :   $('.modal-register')
            };
    }
}

var mItems = modal.domItems();

$('#user-login').click(function(){
    modal.loginClick();
})

$('#login-pic').click(function(){
    modal.profileClick();
})

// When the user clicks anywhere outside of the modal, close it
window.onclick = function(event) {
    modal.exit(event);
}

// When the user clicks on <span> (x), close the modal
$('.close').click = function() {
    modal.close();
}

$('#modal-footer-login').click(function(){
    toggleModalFooterItems();
});

$('#modal-footer-register').click(function(){
    toggleModalFooterItems(); 
});

function toggleModalFooterItems(){
    mItems.ml.toggle();
    mItems.mr.toggle();
    mItems.mf.find(mItems.mfi[0]).toggle();
    mItems.mf.find(mItems.mfi[1]).toggle();
}

/* If user is loged in and browser is on a device or smaller monitor*/


