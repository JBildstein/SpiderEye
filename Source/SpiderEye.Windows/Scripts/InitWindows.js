window._spidereye = new SpiderEyeBridge(function (e) { window.external.notify(e); });
window._spidereye.updateTitle(document.title);

if (typeof MutationObserver !== 'undefined') {
    new MutationObserver(function () {
        window._spidereye.updateTitle(document.title);
    }).observe(document.querySelector('title'),
        { subtree: true, characterData: true, childList: true });
} else if (document.attachEvent) {
    document.attachEvent('onpropertychange', function (e) {
        if (e.propertyName === 'title') {
            window._spidereye.updateTitle(document.title);
        }
    });
}
