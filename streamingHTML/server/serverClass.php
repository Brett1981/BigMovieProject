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
    private static $client      = null;
    
    public static $debug        = false;
    public static $serverIp     = '213.143.88.175';
    public static $serverPort   = '53851';
    public static $httpType     = 'http';
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
    private static function Get($api = null,$data = null){
        if($api != null){
            try{
               if($d = file_get_contents(Server::$client.$api.$data)){
                   return json_decode($d,true);
               }
               else{
                   throw new Exception;
               }     

            }
            catch(Exception $e){
                echo 'There was an error. Try and refresh the page.', $e->getMessage(),"\n";
                exit();
            }
        }
        else{ header('locaton ../index.php'); }
    }
    //POST
    private static function Post($data,$api){
        try
        {
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
        } catch (Exception $ex) {
            echo 'There was an error', $e->getMessage(),"\n";
            exit();
        }
        return null;
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
    
    /*public static function Get($item,$call,$data){
        $get = "";
        if($call["url"] != null){
            if($item["hasData"]){
                $get = $call["url"].$item["action"]."/";
            }
            else{
                $get = $call["url"].$item["action"];
            }
        }
        
        if($get != ""){
            if($item["hasData"]){
                return Server::getData($get,$data);
            }
            return Server::getData($get); 
        }
    }
    
    public static function Post($item,$call,$data){
        if($call["url"] != null){
            if($item["action"] != null){
                if($data != null && $item["hasData"]){ 
                    return Server::postData($data, $call["url"].$item["action"]);
                }
                else{
                    return Server::postData($call["url"].$item["action"]);
                }
            }
        }
    }*/
    //GET: all movies
    public static function GetAllMovies(){
        return Server::Get(self::$apiUrl['video']."all");
    }
    
    public static function GetMovieById($data){
        return Server::Get(self::$apiUrl['video']."get/",$data);
    }
    //GET: movies by genre
    public static function GetByGenre($data){
        return Server::Get(self::$apiUrl['video']."bygenre/",$data);
    }
    //GET: return movies from search
    public static function MovieSearch($data){
        return Server::Get(self::$apiUrl['video']."search/",$data);
    }
    //GET: top 10
    public static function GetTop10(){
        return Server::Get(self::$apiUrl['video']."top10");
    }
    //GET: last 10
    public static function GetLast10(){
        return Server::Get(self::$apiUrl['video']."last10");
    }
    //GET: retrieve temp session
    public static function GetBySession($data){
        return Server::Get(self::$apiUrl['video']."getbysession/",$data);
    }
    //POST: retrieve temp session
    public static function GetSession($data){
        return Server::Post($data, self::$apiUrl['video']."getsession");
    }
    //POST: retrieve movie data
    public static function GetMovie($data){
        return Server::Post($data, self::$apiUrl['video']."get");
    }
    
    
    //POST: change user profile picture
    public static function EditUserProfilePicture($data){
        return Server::Post($data, self::$apiUrl['user']."profilepicture");
    }
    //POST: Login
    public static function Login($data){
        return Server::Post($data, self::$apiUrl['user']."login");
    }
    //POST: Register
    public static function Register($data){
        return Server::Post($data, self::$apiUrl['user']."create");
    }
    //GET: user data
    public static function GetUser($data){
        return Server::Get(self::$apiUrl['user']."byid/",$data);
    }
    //GET: user profile picture
    public static function GetUserProfilePicture($data){
        return Server::Get(self::$apiUrl['user']."profilepicture/",$data);
    }
    //GET: user history
    public static function GetUserHistory($data){
        return Server::Get(self::$apiUrl['user']."history/",$data);
    }
    //GET: check register form data
    public static function CheckFormUser($data){
        return Server::Get(self::$apiUrl['user']."check/",$data);
    }
}
