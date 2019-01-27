import {
    ApiCallback,
    EmptyCallback,
    ErrorCallback,
    BrowserWindowConfig,
} from "../../index";

export class BrowserWindow {

    private config: BrowserWindowConfig;

    constructor(config: BrowserWindowConfig) {
        if (config == null) {
            throw new Error("No config provided");
        }

        this.config = config;
    }

    public show(result?: EmptyCallback, error?: ErrorCallback): void {
        this.showBase(apiResult => {
            if (apiResult.success) {
                if (result != null) {
                    result();
                }
            } else if (error != null) {
                error(apiResult.error);
            }
        });
    }

    public showAsync(): Promise<void> {
        return new Promise((resolve, reject) => {
            this.showBase(result => {
                if (result.success) {
                    resolve();
                } else {
                    reject(new Error(result.error));
                }
            });
        });
    }

    private showBase(callback: ApiCallback<void>): void {
        window._spidereye.invokeApi<BrowserWindowConfig, void>("BrowserWindow.show", this.config, callback);
    }
}
