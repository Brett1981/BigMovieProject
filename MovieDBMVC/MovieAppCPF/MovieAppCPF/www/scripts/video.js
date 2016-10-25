window.onload = function () {
    var url = localStorage.movie_url;
    document.querySelector("#movie > source").src = "http://213.143.88.177:1515" + url;
    var video = document.getElementsByTagName('video')[0];
    var source = video.getElementsByTagName('source');
    source = "http://213.143.88.177:1515" + url;
    video.load();
}