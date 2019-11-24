import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';

import { SpiderEyeService } from './services/spidereye.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit, OnDestroy {
    showUpdate = false;
    currentDate: Date;

    private dateUpdated: Subscription;

    constructor(private spidereye: SpiderEyeService) {
    }

    ngOnInit() {
        this.dateUpdated = this.spidereye
            .registerHandler<string>('dateUpdated')
            .subscribe(date => {
                this.currentDate = new Date(date);
                this.showUpdate = true;
                setTimeout(() => this.showUpdate = false, 4000);
            });
    }

    ngOnDestroy() {
        this.spidereye.removeHandler('dateUpdated');
        this.dateUpdated.unsubscribe();
    }
}
