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
      var m = getMovieInfo(id);
      m.done(function(data){
            var d = JSON.parse(data);
            console.log(d);
            var minfo = d[0].Movie_Info;
            var mdata = d[0];
            mh[0].innerHTML = minfo.original_title;
            mm[0].innerHTML = "<div>"+mdata.guid+"</div>";
            $('#movieModal').show();
            
        })
        .fail(function(data){
            alert(data);
      });      
      
      
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
    return $.get("../website/get.php?id="+id, function(){})
}
