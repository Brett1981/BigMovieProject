/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

var Menu = new function(){
        this.hamburgerToggler = false;
        this.hamburgerStatus;
        this.bodyScroll;
        
        this.setHamStatus = function(){
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
            var w = $(document).width();
            if(w < 1025){
                $('#hamburger').removeClass('open');
                $('.hamburger-menu-top').hide();
            }
            else{
                $('#hamburger').addClass('open');
            }
        }
        
        this.toggle = function(item){
            
            $(item).toggleClass('open');
            var w =$(document).width();
            if(w < 1025){
                $('.hamburger-menu-top').toggle();
            }
            else{
                document.body.classList.toggle('sidenav-active');
            }
        }
        
        this.hamburgerMenuTopClick = function(event){
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
        
        this.windowClick = function(event){
            if(!$(event.target).closest('.navigacija').length) {
                if($('.hamburger-menu-top').is(":visible")) {
                    if($(window).width() < 1025){
                        $('.hamburger-menu-top').hide();
                        $('.hamburger').toggleClass('open');
                    }
                }
            }      
        }
        
        this.windowResize = function(){
            var w = $( document ).width();
            var v = $('.hamburger-menu-top').is(':visible');
            if(w > 1024){
                if($('body').has('sidenav-active')){
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
        
        this.disableBodyScroll = function(data){
            var o = $('body');
            if(data == true){
                o.css("overflowY","hidden");
            }
            else{
                o.css("overflowY","auto");
            }
        }
    }
