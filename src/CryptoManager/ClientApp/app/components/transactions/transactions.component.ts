import { Component, OnInit } from '@angular/core';
import { CryptoApiClient, CryptoTransaction } from '../../services/api-client';
import { DecimalPipe } from '@angular/common';


@Component({
  selector: 'app-transactions',
  templateUrl: './transactions.component.html',
  styleUrls: ['./transactions.component.css']  
})

export class TransactionsComponent implements OnInit {

  transactions: CryptoTransaction[];

  constructor(private apiClient: CryptoApiClient) {
  }

 

  ngOnInit() {
    this.apiClient.apiTransactionsGet().subscribe(res => this.transactions = res);
  }
 
}

