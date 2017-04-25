<?php
//require '../server/serverClass.php';
class User{
    public $user;
    private $userDefIcon;
    private $profilePage;
    private $icons;
    function __construct($data){
        $this->userDefIcon = $data["userDefIcon"];
        $this->profilePage = $data["profilePage"];
        $this->icons       = $data["icons"];
    }
    function Get($guid){
        if(!empty($guid)){
            $this->user = Server::GetUser($guid);
            
            if(empty($this->user)){
                return $this->Guest();
            }
            else{
                if(empty($this->user['profile_image'])){
                    $this->user['profile_image'] = $this->userDefIcon;
                }
                else{
                    $result = Server::GetUserProfilePicture($guid);
                    if(isset($result['Result']) && !empty($result['Result'])){
                        if($result['IsCompleted']){
                            $this->user['image'] = $result['Result'];
                        }
                        else{
                            $this->user['image'] = $this->userDefIcon;
                        }
                        //return $this->user;
                    }
                }
                //return null;
            }
        }
        else{
            return $this->Guest();
        }
    }
    
    function Guest(){
        $this->user['unique_id'] = null;
        $this->user['profile_image'] = $this->userDefIcon;
        $this->user['username'] = "Guest";
        return $this->user;
    }
}
?>
