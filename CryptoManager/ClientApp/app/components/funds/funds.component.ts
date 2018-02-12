import { Component, OnInit } from '@angular/core';
import { CryptoApiClient, FundDTO, FiatDTO } from '../../services/api-client';


@Component({
    selector: 'app-funds',
    templateUrl: './funds.component.html',
    styleUrls: ['./funds.component.scss']
})

export class FundsComponent implements OnInit {
    funds: FundDTO[];
    fiat: FiatDTO[];
    totalWorth: number = 0;

    constructor(private apiClient: CryptoApiClient) {
    }



    ngOnInit() {
        this.apiClient.apiFundsGet().subscribe(res => {
            this.funds = res;
            res.forEach((value, index, array) => {
                if (value.worthFiat != undefined)
                    this.totalWorth += value.worthFiat;
            });
        });
        this.apiClient.apiFiatBalancesGet().subscribe(res => this.fiat = res);
    }

}

