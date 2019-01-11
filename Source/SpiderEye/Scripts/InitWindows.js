window._spidereye = new SpiderEyeInterface(function (e) { window.external.notify(e); });
window._spidereye.updateTitle(document.title);

if (typeof MutationObserver !== 'undefined') {
    new MutationObserver(function (mutations) {
        window._spidereye.updateTitle(mutations[0].target.nodeValue);
    }).observe(document.querySelector('title'),
        { subtree: true, characterData: true, childList: true });
} else if (document.attachEvent) {
    document.attachEvent('onpropertychange', function (e) {
        if (e.propertyName === 'title') {
            window._spidereye.updateTitle(document.title);
        }
    });
}
