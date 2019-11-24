import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { HomeComponent } from './components/home/home.component';
import { ApiComponent } from './components/api/api.component';
import { BridgeComponent } from './components/bridge/bridge.component';

const routes: Routes = [{
    path: '',
    pathMatch: 'full',
    redirectTo: 'Home'
}, {
    path: 'Home',
    component: HomeComponent,
}, {
    path: 'Api',
    component: ApiComponent,
}, {
    path: 'Bridge',
    component: BridgeComponent,
}, {
    path: '**',
    redirectTo: '/'
}];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }
