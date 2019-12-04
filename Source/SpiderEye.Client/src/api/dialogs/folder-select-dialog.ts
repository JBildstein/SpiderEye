import {
    ApiCallback,
    FileResult,
    ValueCallback,
    ErrorCallback,
} from "../../index";
import { FolderSelectDialogConfig } from "./folder-select-dialog-config";

export class FolderSelectDialog {

    public title: string;
    public selectedPath: string;

    public show(result?: ValueCallback<FileResult>, error?: ErrorCallback): void {
        this.showBase(apiResult => {
            if (apiResult.success) {
                if (result != null) {
                    result(apiResult.value);
                }
            } else if (error != null) {
                error(apiResult.error);
            }
        });
    }

    public showAsync(): Promise<FileResult> {
        return new Promise((resolve, reject) => {
            this.showBase(result => {
                if (result.success) {
                    resolve(result.value);
                } else {
                    reject(new Error(result.error));
                }
            });
        });
    }

    private showBase(callback: ApiCallback<FileResult>): void {
        const config: FolderSelectDialogConfig = {
            title: this.title,
            selectedPath: this.selectedPath,
        };

        window._spidereye.invokeApi<FileResult, FolderSelectDialogConfig>("f0631cfea99a_Dialog.showFolderSelectDialog", config, callback);
    }
}
