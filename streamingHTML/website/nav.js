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
    
    function disableBodyScroll(data){
        Menu.disableBodyScroll(data);
    }
        
    var timer, delay = 1000;
    $('.search #searchbox').bind('keydown', function(e) {
        clearTimeout(timer);
            timer = setTimeout(function() {
                Movie.search(e.target.value);
            },delay);
    });
    
    $(window).on('resize load',function(){
        //resize menu 
        Menu.windowResize();
        //search list for phones is different
        if($(window).width() < 420){
            $('.search-list').css('right','-60px');
        }
        else{
            $('.search-list').css('right','0');
        }
    });
    //hide search list
    $(document).mouseup(function (e)
    {
        var container = $('.search-list');

        if (!container.is(e.target) // if the target of the click isn't the container...
            && container.has(e.target).length === 0) // ... nor a descendant of the container
        {
            container.css('display','none');
        }
    });
    /*$('.search-list').on('clickout',function(){
        this.css('display','none');
    })*/
});