/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

var Modal = new function(){
    this.loginClick = function(){
        $('#loginModal').css("display","block");
        
    };
    this.profileClick = function(){
        $('#loginModal').css("display","block");
    };
    
    this.exit = function(event){
        var lm = $('#loginModal')[0];
        if (event.target == $('.close')[0] ||event.target ==  lm ||event.target.className == 'bg-bubbles') {
            this.close();
        }
    };
    
    this.close = function(){
         $('#loginModal').css("display","none");
    };
    
    this.domItems = function(){
        var mf = $('.modal-footer');
        return {
            mfc :   mf.children(),
            mf  :   mf,
            ml  :   $('.modal-login'),
            mr  :   $('.modal-register')
            };
    }
};
