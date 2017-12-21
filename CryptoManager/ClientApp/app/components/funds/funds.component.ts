import { Component, OnInit } from '@angular/core';
import { CryptoApiClient, FundDTO } from '../../services/api-client';


@Component({
  selector: 'app-funds',
  templateUrl: './funds.component.html',
  styleUrls: ['./funds.component.scss']  
})

export class FundsComponent implements OnInit {
  funds: FundDTO[];

  constructor(private apiClient: CryptoApiClient) {
  }



  ngOnInit() {
    this.apiClient.apiFundsGet().subscribe(res => this.funds = res);
  }
 
}

