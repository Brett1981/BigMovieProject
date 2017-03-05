$(document).ready(function(){
    if($(window).has('sidenav-active')){
        var w = $(window).width();
        if(w < 1024){
            $('#hamburger').removeClass('open');
            $('.hamburger-menu-top').hide();
        }
        else{
            $('#hamburger').addClass('open');
        }
        
    }
    $('#hamburger').click(function(){
        $(this).toggleClass('open');
        var w =$( window ).width();
        if(w <= 1024){
            $('.hamburger-menu-top').toggle();
        }
        else{
            document.body.classList.toggle('sidenav-active');

        }
    });


    $('.hamburger-menu-top ul li ul li').click(function(event){
        var data = {
            w : $( window ).width(),
            t : $('div.navigacija').has(event.target),
            v : $('.hamburger-menu-top').is(':visible'),
            f : $('.hamburger-menu-top').find(event.target)
        };
        console.log(!data.t.length);
        if(data.w <= 1024 && data.v ){
            if(data.t.length){
                $('.hamburger-menu-top').toggle();
            }
        }
    });

    $(window).click(function(event){ 
        if(!$(event.target).closest('.navigacija').length) {
            if($('.hamburger-menu-top').is(":visible")) {
                if($(window).width() < 1024){
                    $('.hamburger-menu-top').hide();
                    $('#hamburger').toggleClass('open');
                }
            }
        }        
    });
    $(window).resize(function(event){
        var w =$( window ).width();
        var v = $('.hamburger-menu-top').is(':visible');
        if(w > 1024){
            if(v && $(window).has('sidenav-active')){
                $('#hamburger').addClass('open');
            }
            else{
                $('#hamburger').removeClass('open');
            }
            $('.hamburger-menu-top').css("display","block");
        }
        else{
            $('#hamburger').removeClass('open');
            $('.hamburger-menu-top').css("display","none");
        }
    });

});