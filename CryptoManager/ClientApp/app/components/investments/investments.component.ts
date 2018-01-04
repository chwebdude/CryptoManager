import { Component, OnInit } from '@angular/core';
import { CryptoApiClient, InvestmentDTO } from '../../services/api-client';


@Component({
  selector: 'app-investments',
  templateUrl: './investments.component.html',
  styleUrls: ['./investments.component.scss']  
})

export class InvestmentsComponent implements OnInit {
  investments: InvestmentDTO[];

  constructor(private apiClient: CryptoApiClient) {
  }



  ngOnInit() {
    this.apiClient.apiInvestmentsGet().subscribe(i => this.investments = i);
  }
 
}

