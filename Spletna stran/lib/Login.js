/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
var div = "<div>"   , span  = "<span>"  , 
    img     = "<img>"   , 
    a       = "<a>"     , p     = "<p>"     , i = "<i>",
    li      = "<li>"    , ul    = "<ul>"    ;

var Login = new function(){
    this.url        = "../login/client/client.php?login";
    this.regUrl     = "../login/client/client.php?register";
    this.regCheck   = "../login/client/client.php?check";
    this.to = "";
    this.title = $();

    var bckpTitle = $('.modal-content .modal-header').children("h2");
    var isSetTitleOrig = false;
    
    this.statusDiv = $('#loginModal .modal-status');

    this.getModalTitle = function(){
        if(isSetTitleOrig) {
            this.title = $('.modal-content .modal-header').children("h2");
            isSetTitleOrig = true;
        }
    };
    this.setModalTitle = function(data){
        this.title = bckpTitle;
        if(!data){
            this.title = bckpTitle;
        }
        else{
            this.title.text(data);
        }
    };
    this.userReg = function(u,p,e,b,d){
        return {
            username    :u,
            password    :p, 
            email       :e, 
            birthday    :b, 
            display_name:d
        };
    };
    this.userLogin = function(u,p){
        return  {
            username    :u,
            password    :p
        };
    };
    this.Status = {
        SUCCESS :   {value:1,   name:"Success", code:"S"},
        ERROR   :   {value:2,   name:"Error",   code:"E"}
    };
    
    //post data to server
    this.post = function(value,call){
        return $.ajax({
                type: "POST",
                url: call,
                data:value
            });
    }
    
    //submit data 
    this.submit = function(data,call){
        Login.showForm(call,false);
        Login.showLoader(true);
        var req = Login.post(data,call);
        req.done(function(info){
            //console.log(info);
            if(Extensions.JSONCheck(info)){
                var json =  JSON.parse(info);
                var arr = {response : json, data : data};
                if(json['response'] == 'success'){ 
                    if(call == Login.url){ Login.completed(arr,Login.url);  }
                    else { Login.completed(arr,Login.regUrl);}
                }
            }
            else{ 
                if(call == Login.url){ Login.failed(arr,Login.url);  }
                else { Login.failed(arr,Login.regUrl);}
            }
            return false;
        });
        req.fail(function(xhr,status,error){
            if(call == Login.url){ Login.failed(arr,Login.url);  }
            else { Login.failed(arr,Login.regUrl);}
        })
    };
    
    //on failed call
    this.failed = function(data,type){
        this.modalDom(data,type,this.Status.ERROR);
        Login.showForm(type,true);
        Login.showLoader(false);
    };
    
    //on completed call
    this.completed = function(data,type){
        this.modalDom(data,type,this.Status.SUCCESS);
    };
    
    //edit modal DOM 
    this.modalDom = function(data,type,status){
        if(status === this.Status.SUCCESS){
            if(type === Login.regUrl){
                //set UI for new registered user
                Login.showLoader(false);
                $('#loginModal .modal-status').empty();
                $('#loginModal .modal-status').append(
                    $(a).text("Registration Successfull"),
                    $(a).text("You can now login")
                    );
                //poprav za login status
                
            }
            else{
                sessionStorage.setItem('user_id', data.response.uid);
                var id = "../movies/index.php?uid=" + data.response.uid;
                this.to = id;
                this.redirect();
            } 
        }
        else{
            Login.setModalTitle(data.response.response);
        }
    };
    
    this.showForm = function(link,visible = false){
        if(link !== null){
            var type;
            if(link === Login.url){
                type = $('#loginModal .modal-login');
            }
            else if(link === Login.regUrl){
                type = $('#loginModal .modal-register');
            }

            if(type !== null){
                if(visible){
                    type.css('display','block');
                }
                else{
                    type.css('display','none');
                }
            }
        }
    };
    
    this.showLoader = function(visible){
        var loader = $('#loginModal .modal-loader');
        
        if(visible){ loader.css('display','block'); }
        else{ loader.css('display','none'); }
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
                var text = "";
                if(this.userRegData.password != data){
                    text = "Password's do not match!";
                    this.userRegData.password = "";
                }
                else{
                    text = "Password's match";
                }
                Login.setModalTitle(text);
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
    };
    
    //Check username validity
    this.validateUsername = function(value){
        var req = Login.post(value,this.regCheck);
        this.setModalTitle("Checking username validity...");
        req.done(function(info){
            var json = JSON.parse(info);
            console.log(json);
            if(json['username_status'] == true){ 
                //username is available
                Login.setModalTitle("Username is available!");
                var displayName = $('.modal-content .modal-register #register-form').children("display_name");
                displayName.text(value.username);
                return true;
            }
            else{
                Login.setModalTitle("Username is not available!");
                return false;
            }
        });
        return false;
    };

    //check email
    this.validateEmail = function(data){
        var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        return re.test(data);
    };
    
    //Generate url to redirect on login or register
    /*this.rootUrl = function(path){
        console.log(location.pathname);
        if(!path){
            return location.protocol + "//" + location.host + location.pathname;
        }
        return location.protocol + "//" + location.host + location.pathname + path;
    };*/
    
    //Redirect
    this.redirect = function(){
        setTimeout(function(){ 
            window.location.replace(Login.to);
            //window.location = Login.to;
        }, 2000);  
    }
};
