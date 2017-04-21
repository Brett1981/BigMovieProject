<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Description of serverClass
 *
 * @author pangypc
 */
class Server {
    //put your code here
    public static $debug        = false;
    public static $serverIp     = '213.143.88.175';
    public static $serverPort   = '53851';
    public static $httpType     = 'http';
    private static $client      = null;
    private static $apiUrl      = ["user" => "/api/user/", "video" => "/api/video/"];

    //Client
    public static function Client(){
        self::$client = Server::Connection();
        return self::$client;
    }
    //Connection string
    private static function Connection(){
        return self::$httpType."://".self::$serverIp.":".self::$serverPort;
    }
    //GET
    private static function getData($api = null,$data = null){
        if($api != null){
            try{
               return json_decode(file_get_contents(Server::$client.$api.$data),true);
            }
            catch(Exception $e){
                echo 'there was an error ', $e->getMessage(),"\n";
            }
        }
        else{ header('locaton ../index.php'); }
    }
    //POST
    private static function postData($data,$api){
        $pData = json_encode($data);
        //cURL call to api
        $ch = curl_init(Server::$client.$api);
        # Setup request to send json via POST
        curl_setopt( $ch, CURLOPT_POSTFIELDS, $pData );
        curl_setopt( $ch, CURLOPT_HTTPHEADER, array('Content-Type:application/json'));
        # Return response instead of printing.
        curl_setopt( $ch, CURLOPT_RETURNTRANSFER, true );
        # Send request.
        $result = curl_exec($ch);
        curl_close($ch);
        return $result;
    }

    public static function getDataTest(){
      $movies = array();
      include 'MovieObjectClass.php';
      for($i = 0; $i < 10; $i++){
        $movieData = json_decode(json_encode(new MovieDataObject()),true);
        $movieInfo = json_decode(json_encode(new MovieInfoObject()),true);
        foreach($movieData as $key => $value){
          $movies[$i][$key] = $value;
        }
        $movies[$i]['Movie_Info'] = $movieInfo;
      }
      return $movies;
    }
    
    public static function getDataOneTest(){
         include 'MovieObjectClass.php';
        $movies = array();
        $movieData = json_decode(json_encode(new MovieDataObject()),true);
        $movieInfo = json_decode(json_encode(new MovieInfoObject()),true);
        foreach($movieData as $key => $value){
          $movies[0][$key] = $value;
        }
        $movies[0]['Movie_Info'] = $movieInfo;
        return $movies;
    }
    //GET: all movies
    public static function getAllMovies(){
        return Server::getData(self::$apiUrl['video']."all");
    }
    
    public static function getMovieById($data){
        return Server::getData(self::$apiUrl['video']."get/",$data);
    }
    //GET: movies by genre
    public static function getByGenre($data){
        return Server::getData(self::$apiUrl['video']."bygenre/",$data);
    }
   //GET: top 10
    public static function getTop10(){
        return Server::getData(self::$apiUrl['video']."top10");
    }
    //GET: last 10
    public static function getLast10(){
        return Server::getData(self::$apiUrl['video']."last10");
    }
    //GET: retrieve temp session
    public static function getBySession($data){
        return Server::getData(self::$apiUrl['video']."getbysession/",$data);
    }
    //POST: retrieve temp session
    public static function getSession($data){
        return Server::postData($data, self::$apiUrl['video']."getsession");
    }
    //POST: retrieve movie data
    public static function getMovie($data){
        return Server::postData($data, self::$apiUrl['video']."get");
    }
    
    //POST: change user profile picture
    public static function setUserProfilePicture($data){
        return Server::postData($data, self::$apiUrl['user']."profilepicture");
    }
    //POST: Login
    public static function login($data){
        return Server::postData($data, self::$apiUrl['user']."login");
    }
    //POST: Register
    public static function register($data){
        return Server::postData($data, self::$apiUrl['user']."create");
    }
    //GET: user data
    public static function getUser($data){
        return Server::getData(self::$apiUrl['user']."byid/",$data);
    }
    //GET: user profile picture
    public static function getUserProfilePicture($data){
        return Server::getData(self::$apiUrl['user']."profilepicture/",$data);
    }
    //GET: user history
    public static function getUserHistory($data){
        return Server::getData(self::$apiUrl['user']."history/",$data);
    }
    //GET: check register form data
    public static function checkFormUser($data){
        return Server::getData(self::$apiUrl['user']."check/",$data);
    }
}
