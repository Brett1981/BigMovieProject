$(document).ready(function(){
    
    console.log(Menu.setHamStatus());
    if($(window).has('sidenav-active')){
        Menu.change();
    }
    $('#hamburger').click(function(){
        Menu.toggle(this);
    });

    $('.hamburger-menu-top ul li ul li').click(function(event){
        console.log(event);
        Menu.hamburgerMenuTopClick(event);
    });

    $(window).click(function(event){ 
        Menu.windowClick(event);
    });
    $(window).resize(function(event){
        Menu.windowResize();
    });
    
    function disableBodyScroll(data){
        Menu.disableBodyScroll(data);
    }
    

});