function SpiderEyeInterface(exfn) {
    this.updateTitle = function (title) {
        exfn(JSON.stringify({
            type: "title",
            parameters: title
        }));
    };

    var callbackIds = 0;
    var callbacks = {};
    this.invokeApi = function (id, parameters, callback) {
        if (typeof id !== "string") {
            throw new Error("id must be a string");
        }

        if (typeof callback !== "function") {
            throw new Error("callback must be a function");
        }

        var callbackId = callbackIds++;
        callbacks[callbackId] = callback;
        exfn(JSON.stringify({
            type: "api",
            id: id,
            parameters: parameters,
            callbackId: callbackId
        }));
    };

    this._endApiCall = function (callbackId, result) {
        var callback = callbacks[callbackId];
        if (callback) {
            callback(result);
            delete callbacks[callbackId];
        }
    };

    var events = {};
    this.addEventHandler = function (name, callback) {
        if (typeof name !== "string") {
            throw new Error("name must be a string");
        }

        if (typeof callback !== "function") {
            throw new Error("callback must be a function");
        }

        events[name] = callback;
    };

    this.removeEventHandler = function (name) {
        if (typeof name === "string") {
            delete event[name];
        }
    };

    this._sendEvent = function (name, value) {
        var result = undefined;
        var error = undefined;
        var callback = events[name];
        if (callback) {
            try { result = callback(value); }
            catch (e) { error = e; }
        } else {
            return "{ \"noSubscriber\": true }";
        }

        return Json.stringify({
            result: result,
            hasResult: typeof result !== "undefined",
            error: error,
            success: typeof error === "undefined"
        });
    };
}
