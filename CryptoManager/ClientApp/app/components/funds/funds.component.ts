import { Component, OnInit } from '@angular/core';
import { CryptoApiClient, CryptoTransaction } from '../../services/api-client';


@Component({
  selector: 'app-funds',
  templateUrl: './funds.component.html',
  styleUrls: ['./funds.component.scss']  
})

export class FundsComponent implements OnInit {
  

  constructor(private apiClient: CryptoApiClient) {
  }

 

  ngOnInit() {
  }
 
}

