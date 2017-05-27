/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */


var Movie = new function(){
    this.id = null;
    
    this.watch = function(id){
        if(this.id !== null){
            window.location.href = "../play/index.php?id="+id;
        }
    }
    
    this.view = function(data){
        if(typeof data !== 'string' ){
            data = data.toString();
        }
        Menu.disableBodyScroll(true);
        this.id = data;
        this.modalView(this.id);
    }
    
    this.show = function(){
        $('#movieModal').show();
        Menu.disableBodyScroll(true);
    }
    
    this.close = function(){
        $('#movieModal').hide();
        Menu.disableBodyScroll(false);
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
                    Movie.createModalView(d,mview);
                    Movie.show();
                }
                else{
                    alert("No movie was found! Contact the site's administrator!");
                    Movie.hide();
                }
            })
            .fail(function(data){
                alert(data);
            });
        }
    }
    
    this.search = function(data){
        Search.get(data);
    }
    
    
}

var Search = new function(){
    this.urlImages = "https://image.tmdb.org/t/p/w92";
    /*this.genres = {
        All               : 'index.php?showall',
        Action            : 'action',
        Adventure         : 'adventure',
        Animation         : 'animation',
        Comedy            : 'comedy',
        Drama             : 'drama',
        Family            : 'family',
        History           : 'history',
        Horror            : 'horror',
        Science Fiction   : 'scifi',
        Thriller          : 'thriller'
    };*/
    
    this.get = function(item){
        var m = $.get("../website/get.php?contains="+item,function(){});
        m.done(function(data){
            if(data.length > 0){
                Search.create(JSON.parse(data));
            }
        });
    }
    this.create = function(data){
        //TODO - Implement search list
        var sl = $('.search-list');
        var items = $('.search-list #items');
        items.empty();
        data.forEach(function(item){
            if(item.Movie_Info != null){
                items.append(Search.newLiElement(item));
            }
        });
        sl.css('display','block');
    }
    
    this.newLiElement = function(data){
        if(data !== null){
            var k = "'";
            console.log(data);
            var h =  "<li>"
                    + "<a href='#' ";
                        h += 'onclick="Movie.view(\''+data.guid+'\')">';
                        h += "<img alt='poster' class='thumb' src='"+Search.urlImages+data.Movie_Info.poster_path+"' width='92'/>"
                        + "<span class='info'>"
                            + "<p>"+data.Movie_Info.title+"</p>"
                            + "<p>"+Search.setGenres(data.Movie_Info.genres)+"</p>"
                        + "</span>"
                        + "<p style='display:none;' id='mid'>"+data.guid+"</p>"
                    + "</a>"
                 + "</li>"
            return h;
        }
    }
    
    this.setGenres = function(genres){
        var g = genres.split('|');
        var genre = new Array();
        for(var x = 0; x < g.length && x < 3; x++){
            genre[x] = g[x].split(':')[1] ;
        }
        return genre;
    }
    
    this.view = function(data){
        console.log($(data));
        Movie.view($(data).children("#mid")[0].innerHTML);
    }
    
    
}