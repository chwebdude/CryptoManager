import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ExchangesComponent } from './exchanges/exchanges.component';
import { FlowComponent } from './flow/flow.component';
import { FundsComponent } from './funds/funds.component';
import { HomeComponent } from './home/home.component';
import { TransactionsComponent } from './transactions/transactions.component';


const routes: Routes = [  
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: 'home', component: HomeComponent },
  { path: 'transactions', component: TransactionsComponent },
  { path: 'funds', component: FundsComponent },
  // { path: 'investments', component: InvestmentsComponent },
  { path: 'exchanges', component: ExchangesComponent },
  { path: 'flow', component: FlowComponent },
  { path: '**', redirectTo: 'home' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
