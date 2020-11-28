import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgModule, APP_INITIALIZER } from '@angular/core';
import { SpiderEye } from 'spidereye';

import { AppRoutingModule } from './app-routing.module';

import { AppComponent } from './app.component';
import { HomeComponent } from './components/home/home.component';
import { ApiComponent } from './components/api/api.component';
import { BridgeComponent } from './components/bridge/bridge.component';

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
        useFactory: () => SpiderEye.onReadyAsync,
        multi: true
    }],
    bootstrap: [AppComponent]
})
export class AppModule { }
