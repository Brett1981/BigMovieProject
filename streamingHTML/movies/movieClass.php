<?php
class Movies {
    private static $data_all; 
    private static $html_all;
    private static $data_lastAdded; 
    private static $html_lastAdded;
    private static $data_mostViewed; 
    private static $html_mostViewed;
    private static $nRow;

    public static function createMovieList($data, $type){
        if ($type != 'all') {
           return Movies::Partial($data);
        } else{
           return Movies::All($data);
        }
    }
    
    public static function itemsInRow($row){
        self::$nRow = $row;
    }
    //last 6 movies added 
    private static function LastAdded($data){
        $sort;
        foreach ($data as $key => $part) {
           $sort[$key] = strtotime($part['added']);
        }
        array_multisort($sort,SORT_DESC,$data);
        return array_slice($data,0,6);
    }

    //most viewed
    private static function MostViewed($data){
        $sort;
        foreach ($data as $key => $part) {
           $sort[$key] = (int)$part['views'];
        }
        array_multisort($sort,SORT_DESC,$data);
        return array_slice($data,0,6);
    }

    //all
    private static function All($data){

        self::$data_all = $data;
        self::$data_lastAdded = Movies::LastAdded($data);
        self::$data_mostViewed = Movies::MostViewed($data);

        self::$html_all = "<div class='movies'>";
        self::$html_lastAdded = "<div class='last-movies'>";
        self::$html_mostViewed = "<div class='top-movies'>";

        foreach(self::$data_lastAdded as $item){
            self::$html_lastAdded .= Movies::movieToHtml($item);
        }
        foreach(self::$data_mostViewed as $item){
            self::$html_mostViewed .= Movies::movieToHtml($item);
        }
        foreach(self::$data_all as $item){
            self::$html_all .= Movies::movieToHtml($item);
        }
        self::$html_all .= "</div>";
        self::$html_lastAdded .= "</div>";
        self::$html_mostViewed .= "</div>";
        return self::$html_lastAdded.self::$html_mostViewed.self::$html_all;
    }

    //partial
    private static function Partial($data){

    }

    //write movie html and return it
    private static function movieToHtml($data){
        $movie ="<div id='m' class='movie' onClick='movie(this);'>
                    <div class='poster'>
                        <img alt='poster' src='https://image.tmdb.org/t/p/w160".$data["Movie_Info"]["poster_path"]."' width='120'/>
                        <div class='gradient'></div>
                    </div>
                    <div class='movie_data'>
                        <div class='id' style='display:none'>".$data["guid"]."</div>
                        <div class='title' style='min-width: 200px;'>
                            <p>".$data["Movie_Info"]["title"]."</p><p style='font-style: italic;'>(".date_format(new DateTime($data["Movie_Info"]["release_date"]), 'Y').")</p>
                            <p>".$data["Movie_Info"]["tagline"]."</p>
                            <p>";
                        $genres = array();
                        if(strpos($data["Movie_Info"]["genres"], '|') !== false){
                            $genres = explode("|",$data["Movie_Info"]["genres"]);
                            $movie .= Movies::getGenres($genres);
                        }else{
                            $genres = explode(":",$data["Movie_Info"]["genres"]);
                            $movie .= (string)$genres[1];

                        }
                $movie .=  "</p>
                        </div>
                    </div>
                </div>";
        return $movie;
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



//backup 

/*
 *  <div class="movies">
            <?php 
                $data;
                if(isset($genreMovies) && $genreMovies != null){
                    $data = $genreMovies;
                }
                else if(isset($top10) && $top10 != null){
                    $data = $top10;
                }
                 else if(isset($last10) && $last10 != null){
                    $data = $last10;
                }
                else{
                    $data = $all;
                }
                
                for($i = 0; $i < count($data); $i++){
                $movie = "<div id='m' class='movie' onClick='movie(this);'>

                        <div class='poster'>
                            <img alt='poster' src='https://image.tmdb.org/t/p/w160".$data[$i]["Movie_Info"]["poster_path"]."' width='120'/>
                            <div class='gradient'></div>
                        </div>
                        <div class='movie_data'>
                            <div class='id' style='display:none'>".$data[$i]["guid"]."</div>
                            <div class='title' style='min-width: 200px;'>
                                <p>".$data[$i]["Movie_Info"]["title"]."</p><p style='font-style: italic;'>(".date_format(new DateTime($data[$i]["Movie_Info"]["release_date"]), 'Y').")</p>
                                <p>".$data[$i]["Movie_Info"]["tagline"]."</p>
                            <p>";
                            $genres = array();
                            if(strpos($data[$i]["Movie_Info"]["genres"], '|') !== false){
                                $genres = explode("|",$data[$i]["Movie_Info"]["genres"]);
                                for($y = 0; $y < count($genres);$y++){
                                    $x = explode(":",$genres[$y]);
                                    if(count($y) < 2){
                                        if($y == 0){
                                            $movie .= (string)$x[1] ."/";
                                        }
                                        else{
                                            $movie .= (string)$x[1];
                                            break;
                                        }
                                    }
									else{
										$movie .= (string)$x[1];
										break;
									}
                                }
                            }else{
                                $genres = explode(":",$data[$i]["Movie_Info"]["genres"]);
                                $movie .= (string)$genres[1];
                                
                            }
                    $movie .=  "</p></div>
                        </div>
                    </div>";
                    echo $movie;
                }
            ?>
            
            </div>
 */