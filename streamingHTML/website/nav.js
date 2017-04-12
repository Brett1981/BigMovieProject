$(document).ready(function(){
    
    var menu = new function(){
        this.hamburgerToggler = false;
        this.hamburgerStatus;
        
        this.getHamStatus = function(){
            this.hamburgerStatus = {
                sidenavActive   : $('body').hasClass('sidenav-active'),
                toggler         : $('#hamburger'),
                top             : {
                    topVisible: $('.hamburger-menu-top').is(':visible'),
                    topHamburger: $('.hamburger-menu-top')
                }
            };
            return this.hamburgerStatus;
        }
        
        this.change = function(){
            var w = $(window).width();
            if(w < 1024){
                $('#hamburger').removeClass('open');
                $('.hamburger-menu-top').hide();
            }
            else{
                $('#hamburger').addClass('open');
            }
        }
        
        this.toggle = function(){
            $(this).toggleClass('open');
            var w =$( window ).width();
            if(w <= 1024){
                $('.hamburger-menu-top').toggle();
            }
            else{
                document.body.classList.toggle('sidenav-active');
            }
        }
        
        this.hamburgerMenuTopClick = function(){
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
        }
        
        this.windowClick = function(){
            if(!$(event.target).closest('.navigacija').length) {
                if($('.hamburger-menu-top').is(":visible")) {
                    if($(window).width() < 1024){
                        $('.hamburger-menu-top').hide();
                        $('#hamburger').toggleClass('open');
                    }
                }
            }      
        }
        
        this.windowResize = function(){
            var w =$( window ).width();
            var v = $('.hamburger-menu-top').is(':visible');
            if(w > 1024){
                if(v && $('body').has('sidenav-active')){
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
        }
    }
    console.log(menu.getHamStatus());
    if($(window).has('sidenav-active')){
        menu.change();
    }
    $('#hamburger').click(function(){
        menu.toggle();
    });

    $('.hamburger-menu-top ul li ul li').click(function(event){
        menu.hamburgerMenuTopClick();
    });

    $(window).click(function(event){ 
        menu.windowClick();
    });
    $(window).resize(function(event){
        menu.windowResize();
    });
    
    

});