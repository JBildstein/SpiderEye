import {
    ApiCallback,
    FileResult,
    ValueCallback,
    ErrorCallback,
    FileFilter,
} from "../../index";
import { SaveFileDialogConfig } from "./save-file-dialog-config";

export class SaveFileDialog {

    public title: string;
    public initialDirectory: string;
    public fileName: string;
    public fileFilters: FileFilter[] = [];
    public overwritePrompt: boolean = true;

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
        const config: SaveFileDialogConfig = {
            title: this.title,
            initialDirectory: this.initialDirectory,
            fileName: this.fileName,
            fileFilters: this.fileFilters,
            overwritePrompt: this.overwritePrompt,
        };

        window._spidereye.invokeApi<SaveFileDialogConfig, FileResult>("Dialog.showSaveFileDialog", config, callback);
    }
}
