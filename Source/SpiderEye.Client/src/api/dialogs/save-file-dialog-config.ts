import { FileFilter } from "@se";

export interface SaveFileDialogConfig {
    title: string;
    initialDirectory: string;
    fileName: string;
    fileFilters: FileFilter[];
    overwritePrompt: boolean;
}
