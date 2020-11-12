window._spidereye = new SpiderEyeBridge(function (e) { window.chrome.webview.postMessage(JSON.parse(e)); });
window._spidereye.updateTitle(document.title);

observeTitle();

function observeTitle() {
    if (document.querySelector('title') == null) {
        setTimeout(observeTitle, 10);
        return;
    }

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

}