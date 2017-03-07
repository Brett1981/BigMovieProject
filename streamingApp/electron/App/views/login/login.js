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
