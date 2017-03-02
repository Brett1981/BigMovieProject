<?php
class MovieDataObject
{
      public $Id = 0;
      public $name = "test";
      public $ext = "mp4";
      public $guid = "2fc3b8c3-ab98-460c-b407-d955b0d399fa";
      public $folder = "test folder";
      public $dir = "c:/";
      public $views = 0;
      public $added = "15-02-2017 18:02:48";
      public $enabled = true;

      function __constructor(){
        return json_decode(json_encode($this));
      }

}
class MovieInfoObject
{
      public $id = 0;
      public $adult = false;
      public $backdrop_path = "/4PiiNGXj1KENTmCBHeN6Mskj2Fq.jpg";
      public $budget = "none";
      public $homepage = "www.google.com";
      public $imdb_id = "sdkasjdlas";
      public $original_title = "test";
      public $popularity = "test";
      public $overview = "test";
      public $poster_path = "/4PiiNGXj1KENTmCBHeN6Mskj2Fq.jpg";
      public $release_date = "15-02-2017 18:02:48";
      public $revenue = "00";
      public $status = "Released";
      public $tagline = "test";
      public $title  = "Test";
      public $vote_average = "7";
      public $vote_count = "1000";
      public $genres = "47:Action";
      public $production_countries = "";
      public $production_companies = "";
      public $spoken_languages  = "";

      function __constructor(){
        return json_decode(json_encode($this));
      }

}

?>
