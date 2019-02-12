import { ApiCallback, EventCallback, ApiResult } from "./index";

export class SpiderEye {

    static get isReady() {
        return window._spidereye != null;
    }

    public static onReady(callback: () => void): void {
        if (callback == null) {
            throw new Error("No callback provided");
        }

        if (SpiderEye.isReady) {
            callback();
        } else {
            window.addEventListener("spidereye-ready", callback);
        }
    }

    public static onReadyAsync(): Promise<void> {
        return new Promise(resolve => {
            if (SpiderEye.isReady) {
                resolve();
            } else {
                window.addEventListener("spidereye-ready", () => resolve());
            }
        });
    }

    public static invokeApi<TResult = any, TParam = any>(id: string, parameters: TParam, callback: ApiCallback<TResult>): void {
        SpiderEye.checkBridgeReady();
        window._spidereye.invokeApi<TResult, TParam>(id, parameters, callback);
    }

    public static addEventHandler<TResult = any, TParam = any>(name: string, callback: EventCallback<TResult, TParam>): void {
        SpiderEye.checkBridgeReady();
        window._spidereye.addEventHandler<TResult, TParam>(name, callback);
    }

    public static removeEventHandler(name: string): void {
        SpiderEye.checkBridgeReady();
        window._spidereye.removeEventHandler(name);
    }

    private static checkBridgeReady() {
        if (!SpiderEye.isReady) {
            throw new Error("SpiderEye is not ready yet");
        }
    }
}
