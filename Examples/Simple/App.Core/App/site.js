
window.addEventListener("load", function () {
    var count = 0;
    setInterval(function () {
        count++;
        document.title = 'Title Changes: ' + count;
    }, 1000);
});
