import { Component, OnInit } from '@angular/core';
import { CryptoApiClient } from '../../services/api-client';


@Component({
  selector: 'app-transactions',
  templateUrl: './transactions.component.html',
  styleUrls: ['./transactions.component.css']  
})

export class TransactionsComponent implements OnInit {

  constructor(private apiClient: CryptoApiClient) {
  }

 

  ngOnInit() {
  }
 
}

