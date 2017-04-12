var movie = new function(){
    this.id = null;
    
    this.watch = function(id){
        if(this.id !== null){
            window.location.href = "../play/index.php?id="+id;
        }
    }
    
    this.set = function(data){
        this.id = data;
        this.modalView(this.id);
    }
    
    this.show = function(){
        $('#movieModal').show();
    }
    
    this.hide = function(){
        $('#movieModal').hide();
    }
    
    this.getMovieData = function(data){
        return $.get("../website/get.php?id="+data, function(){})
    }
    
    this.createModalView = function(data,objects){
        objects.mh[0].innerHTML = "<p>"+data.Movie_Info.original_title+" ("+data.Movie_Info.release_date.substring(0,4)+")</p>";
        objects.mb[0].innerHTML = this.createModalViewDiv(data);
        
    }
    
    this.createModalViewDiv = function(data){
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
                            + "<p> Release date: "+release[0]+"</p>"
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
    
    this.createModalArray = function(header,body){
        return  {mh:header, mb:body};
    }
    
    this.modalView = function(data){
        if(data !== null)
        {
            var mview = this.createModalArray($('.modal-movie-header').children('h2'),$('.modal-movie-body'));
            var m = this.getMovieData(data);
            m.done(function(data){
                var d = JSON.parse(data);
                console.log(d);
                if(d != null){
                    movie.createModalView(d,mview);
                    movie.show();
                }
                else{
                    alert("No movie was found! Contact the site's administrator!");
                    movie.hide();
                }
            })
            .fail(function(data){
                alert(data);
            });
        }
    }
}

function set(x){
    movie.set($(x).children(".movie_data").children(".id")[0].innerHTML);
}

function watch(){
    movie.watch(movie.id);
}

function modalView(id){
  movie.modalView(id);
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
    movie.modalView(id);
}

