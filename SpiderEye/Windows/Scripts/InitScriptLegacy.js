window.external.ChangeTitle(document.title);

if (typeof MutationObserver !== 'undefined') {
    new MutationObserver(function (mutations) {
        window.external.ChangeTitle(mutations[0].target.nodeValue);
    }).observe(document.querySelector('title'),
        { subtree: true, characterData: true, childList: true });
} else if (document.attachEvent) {
    document.attachEvent('onpropertychange', function (e) {
        if (e.propertyName === 'title') {
            window.external.ChangeTitle(document.title);
        }
    });
}
