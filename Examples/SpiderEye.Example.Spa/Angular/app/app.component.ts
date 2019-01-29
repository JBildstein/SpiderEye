import { Component, OnInit } from '@angular/core';
import {
    MessageBox,
    MessageBoxButtons,
    SaveFileDialog,
    OpenFileDialog,
    BrowserWindow,
    DialogResult,
    FileResult,
} from 'spidereye';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {

    messageBoxResult: DialogResult;
    saveFileResult: FileResult;
    openFileResult: FileResult;

    ngOnInit() {
        let count = 0;
        setInterval(function () {
            count++;
            document.title = 'Title Changes: ' + count;
        }, 1000);
    }

    openMessageBox() {
        this.messageBoxResult = null;

        var box = new MessageBox();
        box.title = "Hello World";
        box.message = "Hello World from the Webview";
        box.buttons = MessageBoxButtons.OkCancel;

        box.show(result => this.messageBoxResult = result);
    }

    openSaveFileDialog() {
        this.saveFileResult = null;

        var dialog = new SaveFileDialog();
        dialog.title = "Hello World";

        dialog.show(result => this.saveFileResult = result);
    }

    openOpenFileDialog() {
        this.openFileResult = null;

        var dialog = new OpenFileDialog();
        dialog.title = "Hello World";

        dialog.show(result => this.openFileResult = result);
    }

    openWindow() {
        var browserWindow = new BrowserWindow({
            title: "Hello World",
            backgroundColor: "#FFFFFF",
            canResize: true,
            width: 900,
            height: 600,
            useBrowserTitle: true,
            url: "/index.html"
        });

        browserWindow.show();
    }
}
