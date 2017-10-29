<?php
class Movies {
    private static $data_all;
    private static $html_all;

    private static $data_lastAdded;
    private static $html_lastAdded;
    private static $html_lastAddedTitle;

    private static $data_mostViewed;
    private static $html_mostViewed;
    private static $html_mostViewedTitle;

    private static $nRow;

    private static $data_partial;
    private static $html_partial;
    
    private static $UrlImages = "http://image.tmdb.org/t/p/w160";
    private static $ImageOnClickCall = "View(this);";

    public static function createMovieList($movies, $type){
        if ($type != 'all') {
           return Movies::Partial($movies);
        } else{
           return Movies::All($movies);
        }
    }

    public static function itemsInRow($row = 6){
        self::$nRow = $row;
    }
    //last 6 movies added
    private static function LastAdded($movies){
        $sort;
        foreach ($movies as $key => $part) {
            $sort[$key] = strtotime($part['added']);
        }
        array_multisort($sort,SORT_DESC,$movies);
        return array_slice($movies,0,self::$nRow);
    }

    //most viewed
    private static function MostViewed($movies){
        $sort;
        foreach ($movies as $key => $part) {

            $sort[$key] = (int)$part['views'];
        }
        array_multisort($sort,SORT_DESC,$movies);
        return array_slice($movies,0,self::$nRow);
    }

    //all
    private static function All($movies){
        
        self::$data_all = $movies;
        //Razdelimo seznam v dva dela
        //Zadnje dodani filmi
        self::$data_lastAdded = Movies::LastAdded($movies);
        //Najbolj ogledani filmi
        self::$data_mostViewed = Movies::MostViewed($movies);

        //kreiramo div element z razredom in naslovom za zadnje dodane filme
        self::$html_lastAddedTitle = "<div class='last-added title'><a>Latest movies</a><hr></div>";
        //kreiramo div element z razredom in naslovom za najbolj ogledane filme
        self::$html_mostViewedTitle = "<div class='most-viewed title'><a>Popular movies</a><hr></div>";
        //element div pripnemo starševskem elemenu
        self::$html_lastAdded = "<div class='seperator-movies'>".self::$html_lastAddedTitle."<div class='fp-movies'>";
        self::$html_mostViewed = "<div class='seperator-movies'>".self::$html_mostViewedTitle."<div class='fp-movies'>";

        //Vsakemu filmu kreiramo potrebne html elemente
        foreach(self::$data_lastAdded as $item){
            self::$html_lastAdded .= Movies::movieToHtmlTopView($item);
        }
        //Vsakemu filmu kreiramo potrebne html elemente
        foreach(self::$data_mostViewed as $item){
            self::$html_mostViewed .= Movies::movieToHtmlTopView($item);
        }
        //
        self::$html_lastAdded .= "</div></div>";
        self::$html_mostViewed .= "</div></div>";
        //vrnemo celoten html odsek in pripnemo v html dokument
        return self::$html_mostViewed.self::$html_lastAdded;
    }

    //partial
    private static function Partial($movie){
        self::$html_partial = "<div class='bg-movies'>";
        foreach($movie as $item){
            self::$html_partial .= Movies::movieToHtml($item);
        }
        self::$html_partial .= "</div>";
        return self::$html_partial;
    }

    //write movie html and return it
    private static function movieToHtml($movie){
        $movieHtml ="<div id='m' class='movie' onClick='".self::$ImageOnClickCall."'>
                    <div class='poster'>
                        <img alt='poster' src='".self::$UrlImages.$movie["Movie_Info"]["poster_path"]."' width='120'/>
                        <div class='gradient'></div>
                    </div>
                    <div class='movie_data'>
                        <div class='id' style='display:none'>".$movie["guid"]."</div>
                        <div class='title' style='min-width: 200px;'>
                            <p>".$movie["Movie_Info"]["title"]."</p>
                            <p style='font-style: italic;'>(".date_format(new DateTime($movie["Movie_Info"]["release_date"]), 'Y').")</p>
                            <p>".$movie["Movie_Info"]["tagline"]."</p>
                            <p>";
                        $genres = array();
                        if(strpos($movie["Movie_Info"]["genres"], '|') !== false){
                            $genres = explode("|",$movie["Movie_Info"]["genres"]);
                            $movieHtml .= Movies::getGenres($genres);
                        }else{
                            $genres = explode(":",$movie["Movie_Info"]["genres"]);

                            $movieHtml .= (!empty($genres[1]))?(string)$genres[1]:"";

                        }
                $movieHtml .=  "</p>
                        </div>
                    </div>
                </div>";
        return $movieHtml;
    }

    private static function movieToHtmlTopView($movie){
        $movieHtml ="<div id='m' class='movie' onClick='".self::$ImageOnClickCall."'>
                    <div class='poster'>
                        <img alt='poster' src='".self::$UrlImages.$movie["Movie_Info"]["poster_path"]."' width='120'/>
                        <div class='gradient'></div>
                    </div>
                    <div class='movie_data'>
                        <div class='id' style='display:none'>".$movie["guid"]."</div>
                        <div class='title'>
                            <p>".$movie["Movie_Info"]["title"]."</p>
                        </div>
                    </div>
                </div>";
        return $movieHtml;
    }
    private static function getGenres($data){
        $genre = "";
        for($i = 0; $i < count($data);$i++){
            $x = explode(":",$data[$i]);
            if(count($i) < 2){
                if($i == 0){
                    $genre .= (string)$x[1] ."/";
                }
                else{
                    $genre .= (string)$x[1];
                    break;
                }
            }
            else{
                $genre .= (string)$x[1];
                break;
            }
        }
        return $genre;

    }
}
