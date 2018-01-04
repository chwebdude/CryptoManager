import { Component, OnInit } from '@angular/core';
import { CryptoApiClient } from '../../services/api-client';


@Component({
  selector: 'app-investments',
  templateUrl: './investments.component.html',
  styleUrls: ['./investments.component.scss']  
})

export class InvestmentsComponent implements OnInit {


  constructor(private apiClient: CryptoApiClient) {
  }



  ngOnInit() {
  }
 
}

