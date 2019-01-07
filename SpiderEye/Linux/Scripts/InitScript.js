window.external = {
    invoke: function (x) {
        window.webkit.messageHandlers.external.postMessage(x);
    }
};
