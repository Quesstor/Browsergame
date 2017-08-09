function nextSong() {
    var songs = ["1.mp3", "2.mp3", "3.mp3", "4.mp3", "5.mp3"];
    document.getElementById("BGsong").src = "/music/bg/" + (songs[Math.round(Math.random() * songs.length)] || songs[0]);
    document.getElementById("BGmusic").load();
    document.getElementById("BGmusic").play();
    document.getElementById('BGmusic').addEventListener('ended', function () {
        setTimeout("nextSong()", Math.random() * 3 * 60 * 1000 + 30 * 1000);
    });
};
//SOUNDS AND MUSIC
function playSound(name) {
    document.getElementById("soundSrc").src = "/music/sounds/" + name + ".mp3";
    document.getElementById("sounds").load();
    document.getElementById("sounds").play();
}