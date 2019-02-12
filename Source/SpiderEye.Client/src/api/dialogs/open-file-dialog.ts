import {
    ApiCallback,
    FileResult,
    ValueCallback,
    ErrorCallback,
    FileFilter,
} from "../../index";
import { OpenFileDialogConfig } from "./open-file-dialog-config";

export class OpenFileDialog {

    public title: string;
    public initialDirectory: string;
    public fileName: string;
    public fileFilters: FileFilter[] = [];
    public multiselect: boolean = false;

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
        const config: OpenFileDialogConfig = {
            title: this.title,
            initialDirectory: this.initialDirectory,
            fileName: this.fileName,
            fileFilters: this.fileFilters,
            multiselect: this.multiselect,
        };

        window._spidereye.invokeApi<FileResult, OpenFileDialogConfig>("Dialog.showOpenFileDialog", config, callback);
    }
}
