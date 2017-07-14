/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

var div = "<div>"   , span  = "<span>"  , 
    img     = "<img>"   , 
    a       = "<a>"     , p     = "<p>"     , i = "<i>",
    li      = "<li>"    , ul    = "<ul>"    ;
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
    
    this.createModalViewDiv = function(data){
        var minfo = data.Movie_Info;
        var release = minfo.release_date.split('T');
        
        //header
        $(".modal-movie-header h2")
            .empty()
            .append(
                $(p)
                .text( minfo.original_title + " ("+ release[0].substring(0,4)+")")
                );

        //body
        var top = $();
        var bottom = $();
        $(".modal-movie-body").empty();
        top =  $(div).addClass("top-modal-data").append(
                    $(div).addClass("left-modal-data").append(
                        $(img).attr({
                            alt : 'poster', 
                            src : 'http://image.tmdb.org/t/p/w300' + minfo.poster_path
                        })
                    ),
                    $(div).addClass("right-modal-data").append(
                        $(div).append(
                            $(p).text(minfo.tagline)
                        ),
                        $(div).append(
                            $(div).append(
                                $(p).text("Release date: "+release[0])
                            )
                        ),
                        $(div).append(
                            $(a).attr("href",minfo.homepage).append(
                                $(i).addClass("material-icons").attr("style","font-size:32px;color:darkviolet").text("link"),
                                $(p).text("More information")
                            ),
                            $(a).attr("href",'http://www.imdb.com/title/'+minfo.imdb_id).append(
                                $(i).addClass("material-icons").attr("style","font-size:32px;color:darkviolet").text("movie"),
                                $(p).text("iMDB")
                            )
                        ),
                    ),
                );
        bottom = $(a).addClass("bottom-modal-data").attr({
                        href: '#',
                        onclick: 'watch(\"'+data.guid+'"\)'
                    }).append(
                        $(div).addClass("bottom-modal-data-div").append(
                            $(p).text("Play"),
                            $(i).addClass("material-icons").attr({
                                style:'font-size:32px; color:white;'
                            }).text("play_circle_outline")
                        )
                    );
        $(".modal-movie-body").append($(top));
        $(".modal-movie-body").append($(bottom));
    }
    
    this.createModalArray = function(header,body){
        return  {mh:header, mb:body};
    }
    
    this.modalView = function(data){
        if(data !== null)
        {
            var m = this.getMovieData(data);
            m.done(function(data){
                var d = JSON.parse(data);
                if(d != null){
                    Movie.createModalViewDiv(d);
                    Movie.show();
                    return;
                }
                alert("No movie was found! Contact the site's administrator!");
                Movie.close();
                return;
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
    this.urlImages = "http://image.tmdb.org/t/p/w92";
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
        //console.log("../website/get.php?contains="+encodeURIComponent(item.trim()));
        var m = $.get("../website/get.php?contains="+encodeURIComponent(item.trim()),function(){});
        m.done(function(data){
            //console.log(data);
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
            console.log(data);
            return $(li).append(
                $(a).attr({
                    href: '#',
                    onclick: 'Movie.view(\"'+data.guid+'"\)'
                }).append(
                    $(img).attr({
                        alt: 'poster',
                        src: Search.urlImages+data.Movie_Info.poster_path,
                        width: '92px'
                    }).addClass("thumb"),
                    $(span).addClass("info").append(
                        $(p).text(data.Movie_Info.title),
                        $(p).text(Search.setGenres(data.Movie_Info.genres))
                    )
                )
            );
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