function movie(x){
    var id = $(x).children(".movie_data").children(".id")[0].innerHTML;
    modalMovie(id);
    //window.location.href = "../play/index.php?id="+id;
}

function modalMovie(id){
  if(id !== null)
  {
      var mh = $('.modal-movie-header').children('h2');
      var mm = $('.modal-movie-body');
      mh[0].innerHTML = "test";
      mm[0].innerHTML = "<div>wat</div>";
      getMovieInfo(id);
      console.log();
      console.log();

      $('#movieModal').show();
  }
}
$('#movieModal .close').on('click',function(event){
  closeMovieModal();
});
$('#movieModal').on('click',function(){
  if($('#movieModal').is(':visible')){
    closeMovieModal();
  }
})

function closeMovieModal(){
  $('#movieModal').hide();
}

function getMovieInfo(id){

}
