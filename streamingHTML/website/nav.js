$(document).ready(function(){
    
    function toggleSidenav() {
        document.body.classList.toggle('sidenav-active');
    }
    
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
    
    $("#hamburger").one('click',function(){ 
        Menu.windowClick(this);
        /*console.log($(event.target).closest("#hamburger"));
        if($(event.target).closest("#hamburger").length > 0){
            
        }*/
    });
    $(window).resize(function(event){
        Menu.windowResize();
    });
    
    function disableBodyScroll(data){
        Menu.disableBodyScroll(data);
    }

});