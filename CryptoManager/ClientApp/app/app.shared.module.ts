import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';

import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { DropdownModule } from 'primeng/primeng';     


import { AppComponent } from './components/app/app.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { FetchDataComponent } from './components/fetchdata/fetchdata.component';
import { CounterComponent } from './components/counter/counter.component';
import { ExchangesComponent } from './components/exchanges/exchanges.component';

import { CryptoApiClient } from './services/api-client';

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        CounterComponent,
        FetchDataComponent,
      HomeComponent,
      ExchangesComponent
    ],
    imports: [
        CommonModule,
        HttpModule,
      FormsModule,

      DropdownModule,
      BrowserModule,
      BrowserAnimationsModule,
      HttpClientModule,

        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'home', component: HomeComponent },
            { path: 'counter', component: CounterComponent },
            { path: 'fetch-data', component: FetchDataComponent },
            { path: 'exchanges', component: ExchangesComponent },
            { path: '**', redirectTo: 'home' }
        ])
    ],
    providers: [
      CryptoApiClient
    ]
})
export class AppModuleShared {
}
