<?php
include_once '../server/serverClass.php';
class Navigation{
    private $dirRoot;
    private $dirNav;
    
    private $serverPath;
    private $serverRoot;
    
    private $icons;
    private $userDefIcon;
    
    private $homePage;
    private $profilePage;
    
    public $user;
    
    
    public function __construct($data){
        $this->dirRoot      = $data['dirRoot'];
        $this->dirNav       = $data['dirNav'];
        $this->serverPath   = $data['serverPath'];
        $this->serverRoot   = $data['serverRoot'];
        $this->icons        = $data['icons'];
        $this->userDefIcon  = $data['userDefIcon'];
        $this->homePage     = $data['homePage'];
        $this->profilePage  = $data['profilePage'];
        if(isset($data['userGuid']) && !empty($data['userGuid'])){
            $this->user = $this->getUserData($data);
        }
        else{
            $this->user = $this->setGuestUserData();
        }
        $_SESSION['user'] = $this->user;
        return $this->user;
    }   

    function getUserData($data){
        $client = Server::Client();
        $this->user = Server::getUser($data['userGuid']);
        if(empty($this->user)){
            return $this->setGuestUserData();
        }
        else{
            if($this->user['profile_image'] === null){
                $this->user['profile_image'] = $this->userDefIcon;
            }
            else{
                $this->user['profile_image'] = Server::getUserProfilePicture($id);
                $_SESSION['user']['profileImage'] = $this->user->userImage;
            }
            return $this->user;
        }
    }
    
    function setGuestUserData(){
        $this->user['unique_id'] = null;
        $this->user['profile_image'] = $this->userDefIcon;
        $this->user['username'] = "Guest";
        return $this->user;
    }
    
    public static function getGuid($data){
        
    }
    
    public static function getUserImage($data){
        
    }
}
