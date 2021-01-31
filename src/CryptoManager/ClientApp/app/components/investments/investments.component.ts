import { Component, OnInit } from '@angular/core';
import { CryptoApiClient, InvestmentDTO, AggrInvestmentDTO } from '../../services/api-client';


@Component({
  selector: 'app-investments',
  templateUrl: './investments.component.html',
  styleUrls: ['./investments.component.scss']  
})

export class InvestmentsComponent implements OnInit {
  investment: AggrInvestmentDTO;
  investments: InvestmentDTO[];

  constructor(private apiClient: CryptoApiClient) {
  }



  ngOnInit() {
    this.apiClient.apiInvestmentsGet().subscribe(i => {
      this.investment = i;
      this.investments = <InvestmentDTO[]>(i.investments);
    });
  }
 
}

