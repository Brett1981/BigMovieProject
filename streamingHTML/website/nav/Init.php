<?php
//require '../server/serverClass.php';
require 'User.php';
class Navigation{
    private $dirRoot;
    private $dirNav;
    
    private $serverPath;
    private $serverRoot;
    private $homePage;

    public $profile;
    
    public function __construct($data){
        $this->dirRoot      = $data['dirRoot'];
        $this->dirNav       = $data['dirNav'];
        $this->serverPath   = $data['serverPath'];
        $this->serverRoot   = $data['serverRoot'];
        $this->homePage     = $data['homePage'];
        $this->profile = new User($data);
    }   

    function UserLogin($data = null){
        return $this->profile->Get($data);
    }
    function GetUserData($data){
        
        
    }
    
    function SetGuestUserData(){
        
    }
    
    public static function GetGuid($data){
        
    }
    
    public static function GetUserImage($data){
        
    }
}
