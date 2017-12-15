import { Component, OnInit } from '@angular/core';
import { SelectItem, ButtonModule, PanelModule } from 'primeng/primeng';
import { CryptoApiClient, IExchangeMeta } from '../../services/api-client';

interface City {
  name: string;
  code: string;
}

@Component({
  selector: 'app-exchanges',
  templateUrl: './exchanges.component.html',
  styleUrls: ['./exchanges.component.css']
})
export class ExchangesComponent implements OnInit {
  availableExchanges: SelectItem[] = [{ label: 'Select Exchange Plugin', value: null }];
  selectedAvailableExchange: IExchangeMeta;

  constructor(private apiClient: CryptoApiClient) {

    apiClient.apiExchangesAvailableExchangesGet().subscribe(ex => {
      for (let entry of ex) {
        this.availableExchanges.push({
          label: <string>entry.name,
          value: entry
        });
      }
    });


  }

  ngOnInit() {
  }

}

