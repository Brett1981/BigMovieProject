function movie(x){
    window.location.href = "../play/index.php?id="+$(x).children(".movie_data").children(".id")[0].innerHTML;
}