new MutationObserver(function (mutations) {
    window.external.notify(mutations[0].target.nodeValue);
}).observe(document.querySelector('title'),
    { subtree: true, characterData: true, childList: true });
