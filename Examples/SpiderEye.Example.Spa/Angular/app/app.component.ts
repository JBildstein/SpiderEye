import { Component, OnInit } from '@angular/core';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
    ngOnInit() {
        let count = 0;
        setInterval(function () {
            count++;
            document.title = 'Title Changes: ' + count;
        }, 1000);
    }
}
