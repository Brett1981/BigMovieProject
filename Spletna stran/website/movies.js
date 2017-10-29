$(document).ready(function(){
    $('#movieModal .close').on('click',function(event){
        Movie.close();
    });
    $('#movieModal').on('click',function(e){
      if($('#movieModal').is(':visible') && e.target == this){
        Movie.close();
      }
    });
});
function View(x){
    Movie.view($(x).children(".movie_data").children(".id")[0].innerHTML);
}
    
function watch(){
    Movie.watch(Movie.id);
}

function modalView(id){
    Movie.modalView(id);
}

function closeMovieModal(){
    Movie.close();
} 

function getMovieInfo(id){
    Movie.modalView(id);
}





