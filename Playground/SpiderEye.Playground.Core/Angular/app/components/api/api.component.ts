import { Component } from '@angular/core';
import {
    MessageBox,
    MessageBoxButtons,
    SaveFileDialog,
    OpenFileDialog,
    DialogResult,
    BrowserWindow,
    BrowserWindowConfig,
} from 'spidereye';

@Component({
    selector: 'app-api',
    templateUrl: './api.component.html',
    styleUrls: ['./api.component.scss']
})
export class ApiComponent {
    MessageBoxButtons = MessageBoxButtons;
    messageBox: MessageBox;
    messageBoxResult: string;

    saveDialog: SaveFileDialog;
    saveResult: string;
    saveFile: string;

    openDialog: OpenFileDialog;
    openResult: string;
    openFile: string;

    windowConfig: BrowserWindowConfig;

    constructor() {
        this.initMessageBox();
        this.initSaveDialog();
        this.initOpenDialog();
        this.initWindow();
    }

    async showMessageBox() {
        const result = await this.messageBox.showAsync();
        this.messageBoxResult = DialogResult[result];
    }

    async showSaveDialog() {
        const result = await this.saveDialog.showAsync();
        this.saveResult = DialogResult[result.dialogResult];
        this.saveFile = result.file;
    }

    async showOpenDialog() {
        const result = await this.openDialog.showAsync();
        this.openResult = DialogResult[result.dialogResult];
        this.openFile = result.file;
    }

    showWindow() {
        const browserWindow = new BrowserWindow(this.windowConfig);
        browserWindow.show();
    }

    private initMessageBox() {
        this.messageBox = new MessageBox();
        this.messageBox.title = 'Hello World';
        this.messageBox.message = 'Hello World from the Webview';
        this.messageBox.buttons = MessageBoxButtons.Ok;
    }

    private initSaveDialog() {
        this.saveDialog = new SaveFileDialog();
        this.saveDialog.title = 'Hello World';
    }

    private initOpenDialog() {
        this.openDialog = new OpenFileDialog();
        this.openDialog.title = 'Hello World';
    }

    private initWindow() {
        this.windowConfig = {
            title: 'Hello World',
            width: 900,
            height: 600,
            minWidth: 0,
            minHeight: 0,
            maxWidth: 0,
            maxHeight: 0,
            backgroundColor: '#303030',
            canResize: true,
            useBrowserTitle: true,
            enableScriptInterface: true,
            enableDevTools: true,
            url: '/index.html'
        };
    }
}
