import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';

import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { DropdownModule, ButtonModule, DataTableModule, SharedModule, ConfirmDialogModule, ConfirmationService } from 'primeng/primeng';


import { AppComponent } from './components/app/app.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { ExchangesComponent } from './components/exchanges/exchanges.component';
import { TransactionsComponent } from './components/transactions/transactions.component';
import { FundsComponent } from './components/funds/funds.component';
import { InvestmentsComponent } from './components/investments/investments.component'
import { FlowComponent } from './components/flow/flow.component'

import { CryptoApiClient } from './services/api-client';

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        HomeComponent,
        ExchangesComponent,
        TransactionsComponent,
        FundsComponent,
        InvestmentsComponent,
        FlowComponent
    ],
    imports: [
        CommonModule,
        HttpModule,
        FormsModule,

        DropdownModule,
        ButtonModule,
        DataTableModule,
        SharedModule,
        ConfirmDialogModule,

        BrowserModule,
        BrowserAnimationsModule,
        HttpClientModule,

        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'home', component: HomeComponent },
            { path: 'transactions', component: TransactionsComponent },
            { path: 'funds', component: FundsComponent },
            { path: 'investments', component: InvestmentsComponent },
            { path: 'exchanges', component: ExchangesComponent },
            { path: 'flow', component: FlowComponent },
            { path: '**', redirectTo: 'home' }
        ])
    ],
    providers: [
        CryptoApiClient,

        ConfirmationService
    ]
})
export class AppModuleShared {
}
