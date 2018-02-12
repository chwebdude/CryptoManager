import { Component, OnInit } from '@angular/core';
import { CryptoApiClient } from '../../services/api-client';

const defaultColors = [
    '#3366CC', '#DC3912', '#FF9900', '#109618', '#990099',
    '#3B3EAC', '#0099C6', '#DD4477', '#66AA00', '#B82E2E',
    '#316395', '#994499', '#22AA99', '#AAAA11', '#6633CC',
    '#E67300', '#8B0707', '#329262', '#5574A6', '#3B3EAC'
];

@Component({
    selector: 'home',
    templateUrl: './home.component.html'
})
export class HomeComponent implements OnInit {
    fundings: any;

    constructor(private apiClient: CryptoApiClient) {
    }


    ngOnInit() {
        this.apiClient.apiFundsGet().subscribe(res => {
            var labels: string[] | any = [];
            var data: string[] | any = [];
            var backgroundColors: string[] | any = [];
            for (var i = 0; i < res.length; i++) {
                labels.push(res[i].currency);
                data.push(res[i].worthFiat);
                backgroundColors.push(defaultColors[i % 20]);
            }
            this.fundings = {
                labels: labels,
                datasets: [
                    {
                        data: data,
                        backgroundColor: backgroundColors
                    }]
            }
        });
    }

}
