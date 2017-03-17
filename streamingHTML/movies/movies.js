function movie(x){
    var id = $(x).children(".movie_data").children(".id")[0].innerHTML;
    modalMovie(id);

}

function watch(id){
    window.location.href = "../play/index.php?id="+id;
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
            if(d != null){
              var minfo = d.Movie_Info;
              var mdata = d;
              mh[0].innerHTML = "<p>"+minfo.original_title+"</p><p>("+minfo.release_date.substring(0,4)+")</p>";
              mm[0].innerHTML = createMovieModalDiv(mdata);
              $('#movieModal').show();
            }
            else{
              alert("No movie was found! Contact the site's administrator!");
              $('#movieModal').hide();
            }
        })
        .fail(function(data){
            alert(data);
      });
  }
}

$('#movieModal .close').on('click',function(event){
  closeMovieModal();
});
$('#movieModal').on('click',function(e){
  if($('#movieModal').is(':visible') && e.target == this){
    closeMovieModal();
  }
})

$('#movieModal')
  .on('show', function () {
    $('body').on('wheel.modal mousewheel.modal', function () {
      return false;
    });
  })
  .on('hidde', function () {
    $('body').off('wheel.modal mousewheel.modal');
  });
function closeMovieModal(){
  $('#movieModal').hide();
}

function getMovieInfo(id){
    return $.get("../website/get.php?id="+id, function(){})
}

function createMovieModalDiv(data){
    var minfo = data.Movie_Info;
    var release = minfo.release_date.split('T');
    var mDiv = "";
          mDiv += "<div class='top-modal-data'>";
            mDiv += "<div class='left-modal-data'><img alt='poster' src='https://image.tmdb.org/t/p/w300" + minfo.poster_path +"'/></div>";

            mDiv += "<div class='right-modal-data'>";
              mDiv += "<div>"
                      + "<p>"+minfo.tagline+"</p>"
                    + "</div>";
              mDiv += "<div>"
                      + "<a href='"+minfo.homepage+"'>"
                        + "<i class='material-icons' style='font-size:32px;color:darkviolet'>link</i>"
                        + "<p>More information</p>"
                      + "</a>"

                      + "<a href='http://www.imdb.com/title/"+minfo.imdb_id+"'>"
                        + "<i class='material-icons' style='font-size:32px;color:darkviolet'>movie</i>"
                        + "<p>IMDb</p>"
                      + "</a>"

                   +  "</div>";
              mDiv += "<div>"
                      + "<div>"
                        + "<p>"+release[0]+"</p>"
                      + "</div>"

                      + "<div>"
                        + "<p>"+"</p>"
                      + "</div>"

                      + "<div>"
                      + "</div>"
                   + "</div>";
            mDiv +="</div>";
          mDiv += "</div>";
            mDiv += "<a class='bottom-modal-data' href='#'"; mDiv += 'onclick="watch(\''+data.guid+'\')">';
              mDiv   += "<div class='bottom-modal-data-div'>"
                      + "<p>Play</p>"
                      + "<i class='material-icons' style='font-size:32px; color:white;'>play_circle_outline</i>"
                    + "</div>"
                  + "</a>";
        return mDiv;
}
