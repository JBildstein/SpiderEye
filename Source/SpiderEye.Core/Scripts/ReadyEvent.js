(function () {
    var e = document.createEvent('Event');
    e.initEvent('spidereye-ready', true, true);
    window.dispatchEvent(e);
})();
