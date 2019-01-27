import { FileFilter } from "@se";

export interface OpenFileDialogConfig {
    title: string;
    initialDirectory: string;
    fileName: string;
    fileFilters: FileFilter[];
    multiselect: boolean;
}
