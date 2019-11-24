import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgModule, APP_INITIALIZER } from '@angular/core';
import { SpiderEye } from 'spidereye';

import { AppRoutingModule } from './app-routing.module';

import { AppComponent } from './app.component';
import { HomeComponent } from './components/home/home.component';
import { ApiComponent } from './components/api/api.component';
import { BridgeComponent } from './components/bridge/bridge.component';

export function init() {
    return async () => {
        if (!SpiderEye.isReady) {
            await SpiderEye.onReadyAsync();
        }
    };
}

@NgModule({
    declarations: [
        AppComponent,
        HomeComponent,
        ApiComponent,
        BridgeComponent,
    ],
    imports: [
        BrowserModule,
        FormsModule,
        ReactiveFormsModule,
        AppRoutingModule,
    ],
    providers: [{
        provide: APP_INITIALIZER,
        useFactory: init,
        multi: true
    }],
    bootstrap: [AppComponent]
})
export class AppModule { }
