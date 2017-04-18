/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

var Login = new function(){
    this.url    = "../login/client/client.php?login";
    this.regUrl = "../login/client/client.php?register";
    this.to = "";
    this.title = $();

    var bckpTitle = $('.modal-content .modal-header').children("h2");
    var isSetTitleOrig = false;

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
                if(call == this.url){ Login.completed(arr,this.url);  }
                else { Login.completed(arr,this.regUrl);}
            }
            else{ 
                if(call == this.url){ Login.failed(arr,this.url);  }
                else { Login.failed(arr,this.regUrl);}
            }
        });
    };
    
    //on failed call
    this.failed = function(data,type){
        console.log(data);
        console.log(type);
        this.modalDom(data,type,this.Status.ERROR);
    };
    
    //on completed call
    this.completed = function(data,type){
        console.log(data);
        console.log(type);
        this.modalDom(data,type,this.Status.SUCCESS);
    };
    
    //edit modal DOM 
    this.modalDom = function(data,type,status){
        if(status == this.Status.SUCCESS){
            if(type == this.regUrl){
            
            }
            else{
                sessionStorage.setItem('user_id', data.response.uid);
                var id = "../movies/index.php?uid=" + data.response.uid;
                this.to = id;
                this.redirect();
            } 
        }
        else{
            
        }
    };
    
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
    };
    
    //Check username validity
    this.validateUsername = function(value){
        var req = $.ajax({
                type: "POST",
                url: '../login/client/client.php?check',
                data:value
            });
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
