/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

// Login & register object 

$(document).ready(function(){
    Login.getModalTitle();
    /* Login / Register */
    $('#login-form').submit(function(event){
        event.preventDefault();
        var f = event.currentTarget;
        var user = Login.userLogin(
            f[0].value,
            btoa(f[1].value)
        );
        Login.submit(user,Login.url);
    });

    $('#register-form').submit(function(event){
        event.preventDefault();
        var f = event.currentTarget;
        console.log(f);
        var register = Login.userReg(
            f[0].value,
            f[1].value,
            f[3].value,
            f[4].value,
            f[5].value
        );
        console.log(register);
        Login.submit(register,Login.regUrl);
    });

    var mItems = Modal.domItems();

    $('#user-login').click(function(){
        Menu.disableBodyScroll(true);
        Modal.loginClick();
    });

    $('#login-pic').click(function(){
        Menu.disableBodyScroll(true);
        Modal.profileClick();
    });

// When the user clicks anywhere outside of the modal, close it
    window.onclick = function(event) {
        Modal.exit(event);
        Menu.disableBodyScroll(false);
    };

// When the user clicks on <span> (x), close the modal
    $('.close').click = function() {
        Modal.close();
        Menu.disableBodyScroll(false);
    };

    $('#modal-footer-login').click(function(){
        toggleModalFooterItems();
    });

    $('#modal-footer-register').click(function(){
        toggleModalFooterItems();
    });

    function toggleModalFooterItems(){
        mItems.ml.toggle();
        mItems.mr.toggle();
        mItems.mf.find(mItems.mfc[0]).toggle();
        mItems.mf.find(mItems.mfc[1]).toggle();
    }
});



/* If user is loged in and browser is on a device or smaller monitor*/


