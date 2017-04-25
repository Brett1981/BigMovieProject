<?php
session_start();
//server communicator
include_once '/server/serverClass.php';
$client = Server::Client();


//root of project
$data['dirRoot']        = dirname(dirname(__FILE__ ));

//Website url
$data['serverPath']     = 'http://'.$_SERVER['HTTP_HOST'];
$data['serverRoot']     = $data['serverPath'].'/'.basename(dirname(__FILE__));
$data['serverDir']	= $data['serverRoot'].'/uploads/';

//target
$data['targetDir'] 	= $_SERVER['DOCUMENT_ROOT'].'/'.basename(dirname(__FILE__))."/uploads/";
$data['targetFile'] 	= $data['targetDir'].basename($_FILES["avatar"]["name"]);
$_SESSION['upload'] = $data;

//$api = 'http://31.15.224.24:53851/api/user/changeprofilepicture';
;
$isUploaded = 0;

$uploadOk = 1;
$imageFileType = pathinfo($data['targetFile'],PATHINFO_EXTENSION);
// Check if image file is a actual image or fake image
if(isset($_POST["submit"]) 
        && $_GET['avatar'] === 'upload'
        && !empty($_FILES['avatar']['tmp_name'])) {
    $check = getimagesize($_FILES["avatar"]["tmp_name"]);
    if($check !== false) {
        echo "File is an image - " . $check["mime"] . ".";
        $uploadOk = 1;
		// Check file size
		if ($_FILES["avatar"]["size"] > 500000) {
			$_SESSION['post_message'] = "Sorry, your file is too large.";
			$uploadOk = 0;
		}
		list($width, $height) = getimagesize($_FILES["avatar"]["tmp_name"]);
		if($width > 500 ||$height > 500){
			$_SESSION['post_message'] = "Image is too big, choose a smaler image.";
			$uploadOk = 0;
		}
		// Allow certain file formats
		if($imageFileType != "jpg" && $imageFileType != "png" && $imageFileType != "jpeg"
		&& $imageFileType != "gif" ) {
			$_SESSION['post_message'] = "Sorry, only JPG, JPEG, PNG & GIF files are allowed.";
			$uploadOk = 0;
		}
		if ($uploadOk == 0) {
			$_SESSION['post_message'] = "Sorry, your file was not uploaded.";
		// if everything is ok, try to upload file
		} else {
			if (move_uploaded_file($_FILES["avatar"]["tmp_name"], $data['targetFile'])) {
				echo "The file ". basename( $_FILES["avatar"]["name"]). " has been uploaded.";
				$isUploaded = 1;
			} else {
				$_SESSION['post_message'] = "Sorry, there was an error uploading your file.";
			}
		}
    } else {
        $_SESSION['post_message'] = "File is not an image.";
        $uploadOk = 0;
    }
}
else{
	header('location: '.$data['serverRoot'].'/profile/');
}


if($isUploaded == 1){
    $data = array('unique_id' => $_SESSION['user']['unique_id'], 'image_url' => $data['serverDir'].basename($_FILES["avatar"]["name"]));
    Server::EditUserProfilePicture($data);
    //var_dump(file_post_contents($api,$data,$target_file));
    $_SESSION['post_message'] = "Profile updated.";
    //delete file on successfull upload
    unlink($_SESSION['upload']['targetFileUpload']); 
    //redirect to profile page of user
    $_SESSION['user']['user_image'] = Server::GetUserProfilePicture($_SESSION['user']['unique_id']);
}
if(isset($_SESSION['user']['unique_id']))
{ 
    header('location: '.$_SESSION['upload']['serverRoot']); 
}
else
{ 
    header('location: '.$_SESSION['upload']['serverRoot'].'/profile/'); 
}
exit();


function file_post_contents($url, $data, $file, $username = null, $password = null)
{
    $ch = curl_init( $url );
    # Setup request to send json via POST
    curl_setopt( $ch, CURLOPT_POSTFIELDS, $data );
    curl_setopt( $ch, CURLOPT_HTTPHEADER, array('Content-Type:application/json'));
    # Return response instead of printing.
    curl_setopt( $ch, CURLOPT_RETURNTRANSFER, true );
    # Send request.
    $result = curl_exec($ch);
    curl_close($ch);
    
    return $result;
}
?>