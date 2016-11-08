<?php
session_start();
$api = 'http://31.15.224.24:53851/api/users/changeprofilepicture';
$server_port = "8080";
$isUploaded = 0;
$target_dir = $_SERVER['DOCUMENT_ROOT']."/streamingHTML/uploads/";
$target_file = $target_dir . basename($_FILES["avatar"]["name"]);
$server_dir = "http://".$_SERVER['SERVER_NAME'].":".$_SERVER['SERVER_PORT']."/streamingHTML/uploads/".basename($_FILES["avatar"]["name"]);
$uploadOk = 1;
$imageFileType = pathinfo($target_file,PATHINFO_EXTENSION);
// Check if image file is a actual image or fake image
if(isset($_POST["submit"])) {
    $check = getimagesize($_FILES["avatar"]["tmp_name"]);
    
    if($check !== false) {
        echo "File is an image - " . $check["mime"] . ".";
        $uploadOk = 1;
    } else {
        $_SESSION['post_message'] = "File is not an image.";
        $uploadOk = 0;
    }
}
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
    //$_SESSION['post_message'] = "Sorry, your file was not uploaded.";
// if everything is ok, try to upload file
} else {
    if (move_uploaded_file($_FILES["avatar"]["tmp_name"], $target_file)) {
        echo "The file ". basename( $_FILES["avatar"]["name"]). " has been uploaded.";
        $isUploaded = 1;
    } else {
        $_SESSION['post_message'] = "Sorry, there was an error uploading your file.";
    }
}

if($isUploaded == 1){
    $data = array('unique_id' => $_SESSION['guid'], 'image_url' => $server_dir);
    $data = json_encode($data);
    //cURL call to api
    var_dump(file_post_contents($api,$data,$target_file));
    $_SESSION['post_message'] = "Profile updated.";
    //delete file on successfull upload
    unlink($target_file); 
    //redirect to profile page of user
    header('location: ../streamingHTML/profile/index.php?user='.$_SESSION['guid']);
    exit();
}
else{
    header('location: ../streamingHTML/profile/index.php?user='.$_SESSION['guid']);
    exit();
}


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