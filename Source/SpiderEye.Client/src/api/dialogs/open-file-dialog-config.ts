import { FileFilter } from "../../index";

export interface OpenFileDialogConfig {
    title: string;
    initialDirectory: string;
    fileName: string;
    fileFilters: FileFilter[];
    multiselect: boolean;
}
