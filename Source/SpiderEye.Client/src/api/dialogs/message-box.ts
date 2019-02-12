import {
    ApiCallback,
    MessageBoxButtons,
    DialogResult,
    ValueCallback,
    ErrorCallback,
    MessageBoxConfig,
} from "../../index";

export class MessageBox {

    public static show(config: MessageBoxConfig, result?: ValueCallback<DialogResult>, error?: ErrorCallback): void {
        MessageBox.showBase(config, apiResult => {
            if (apiResult.success) {
                if (result != null) {
                    result(apiResult.value);
                }
            } else if (error != null) {
                error(apiResult.error);
            }
        });
    }

    public static showAsync(config: MessageBoxConfig): Promise<DialogResult> {
        return new Promise((resolve, reject) => {
            MessageBox.showBase(config, result => {
                if (result.success) {
                    resolve(result.value);
                } else {
                    reject(new Error(result.error));
                }
            });
        });
    }

    private static showBase(config: MessageBoxConfig, callback: ApiCallback<DialogResult>): void {
        window._spidereye.invokeApi<DialogResult, MessageBoxConfig>("Dialog.showMessageBox", config, callback);
    }

    public title: string;
    public message: string;
    public buttons: MessageBoxButtons;

    public show(result?: ValueCallback<DialogResult>, error?: ErrorCallback): void {
        MessageBox.show({ title: this.title, message: this.message, buttons: this.buttons }, result, error);
    }

    public showAsync(): Promise<DialogResult> {
        return MessageBox.showAsync({ title: this.title, message: this.message, buttons: this.buttons });
    }
}
