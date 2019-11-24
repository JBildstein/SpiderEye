import { Component } from '@angular/core';

import { SpiderEyeService } from '../../services/spidereye.service';

import { SomeDataModel } from '../../models/some-data-model';
import { PowerModel } from '../../models/power-model';

@Component({
    selector: 'app-bridge',
    templateUrl: './bridge.component.html',
    styleUrls: ['./bridge.component.scss']
})
export class BridgeComponent {

    longRunningTaskState: string;
    longRunningState: string;
    getDataState: string;
    powerState: string;
    getErrorState: string;

    powerValue = 2;
    powerPower = 4;

    constructor(private spidereye: SpiderEyeService) {
    }

    startLongRunningTask() {
        this.longRunningTaskState = 'Running...';
        this.spidereye.invokeApi<void>('UiBridge.runLongProcedureOnTask')
            .subscribe(() => this.longRunningTaskState = 'Done!',
                error => this.longRunningTaskState = 'Error: ' + error.message);
    }

    startLongRunning() {
        this.longRunningState = 'Running...';
        this.spidereye.invokeApi<void>('UiBridge.runLongProcedure')
            .subscribe(() => this.longRunningState = 'Done!',
                error => this.longRunningState = 'Error: ' + error.message);
    }

    getData() {
        this.getDataState = 'Getting...';
        this.spidereye.invokeApi<SomeDataModel>('UiBridge.getSomeData')
            .subscribe(data => this.getDataState = 'Result: ' + JSON.stringify(data),
                error => this.getDataState = 'Error: ' + error.message);
    }

    power() {
        const parameters: PowerModel = {
            value: this.powerValue,
            power: this.powerPower,
        };

        this.powerState = 'Calculating...';
        this.spidereye.invokeApi<number, PowerModel>('UiBridge.power', parameters)
            .subscribe(msg => this.powerState = 'Result: ' + msg,
                error => this.powerState = 'Error: ' + error.message);
    }

    getError() {
        this.getErrorState = 'Throwing...';
        this.spidereye.invokeApi<void>('UiBridge.produceError')
            .subscribe(() => this.getErrorState = 'Everything went well, that shouldn\'t happen here',
                error => this.getErrorState = 'Thrown Error: ' + error.message);
    }
}
