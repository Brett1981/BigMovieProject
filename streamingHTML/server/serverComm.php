<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Description of serverComm
 *
 * @author pangypc
 */
class Server {
    //put your code here
    public static $serverIp = '31.15.224.24';
    public static $serverPort = '53851';
    public static $httpType = 'http';
    private static  $client = null;
    
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
    
    //GET: all movies
    public static function getAllMovies(){
        return Server::getData("/api/video/allmovies");
    }
    //GET: movies by genre
    public static function getByGenre($data){
        return Server::getData("/api/video/genre/",$data);
    }
   //GET: top 10
    public static function getTop10(){
        return Server::getData("/api/video/top10");
    }
    //GET: last 10
    public static function getLast10(){
        return Server::getData("/api/video/last10");
    }
    //GET: user data
    public static function getUser($data){
        return Server::getData("/api/user/getuser/",$data);
    }
    //GET: user profile picture
    public static function getUserProfilePicture($data){
        return Server::getData("/api/user/getprofilepicture/",$data);
    }
    //GET: user history
    public static function getUserHistory($data){
        return Server::getData("/api/user/getuserhistory/",$data);
    }
    //GET: check register form data
    public static function checkFormUser($data){
        return Server::getData("/api/user/check/",$data);
    }
    //POST: Login
    public static function login($data){
        return Server::postData($data, "/api/user/login");
    }
    //POST: Register
    public static function register($data){
        return Server::postData($data, "/api/user/create");
    }
    //POST: retrieve temp session
    public static function getSession($data){
        return Server::postData($data,"/api/video/getsession");
    }
    //POST: retrieve movie data
    public static function getMovie($data){
        return Server::postData($data, "/api/video/getmovie");
    }
    //POST: change user profile picture
    public static function setUserProfilePicture($data){
        return Server::postData($data, "/api/user/changeprofilepicture");
    }   
}
