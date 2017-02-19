/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
$('#user-login').click(function(){
    $('#loginModal').css("display","block");
})

// When the user clicks anywhere outside of the modal, close it
window.onclick = function(event) {
    if (event.target == $('.close')[0] ||event.target == $('#loginModal')[0]) {
        $('#loginModal').css("display","none");
    }
}



// When the user clicks on <span> (x), close the modal
$('.close').click = function() {
    console.log("escape clicked");
    $('#loginModal').css("display","none");
}
