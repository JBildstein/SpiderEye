import { ApiCallback, EventCallback } from "./index";

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

    public static invokeApi<T = any, U = any>(id: string, parameters: T, callback: ApiCallback<U>): void {
        SpiderEye.checkBridgeReady();
        window._spidereye.invokeApi<T, U>(id, parameters, callback);
    }

    public static addEventHandler<T = any, U = void>(name: string, callback: EventCallback<T, U>): void {
        SpiderEye.checkBridgeReady();
        window._spidereye.addEventHandler<T, U>(name, callback);
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
