import { Component, OnInit } from '@angular/core';
import { finalize } from 'rxjs/operators';

import { ExampleService } from './services/example.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {

    exampleName = 'Hello World';

    loading = false;
    loadingSlow = false;

    loadingResult = 'press button to load';
    loadingSlowResult = 'press button to load';

    constructor(private exampleServices: ExampleService) {
    }

    ngOnInit() {
        let count = 0;
        setInterval(function () {
            count++;
            document.title = 'Title Changes: ' + count;
        }, 1000);
    }

    loadExamples() {
        this.loading = true;
        this.loadingResult = null;
        this.exampleServices.getExample(this.exampleName)
            .pipe(finalize(() => this.loading = false))
            .subscribe(result => this.loadingResult = result,
                error => this.loadingResult = 'load error');

        this.loadingSlow = true;
        this.loadingSlowResult = null;
        this.exampleServices.getSlowExample(this.exampleName)
            .pipe(finalize(() => this.loadingSlow = false))
            .subscribe(result => this.loadingSlowResult = result,
                error => this.loadingSlowResult = 'load error');
    }
}
