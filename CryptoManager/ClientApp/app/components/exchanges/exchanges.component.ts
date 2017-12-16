import { Component, OnInit } from '@angular/core';
import { SelectItem } from 'primeng/primeng';
import { CryptoApiClient, IExchangeMeta, Exchange, ExchangeDto } from '../../services/api-client';

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

  publicKey: string;
  privateKey: string;
  comment: string;
  showForm: boolean;

  ownExchanges: ExchangeDto[];

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

  addExchange() {
    let exchange = new Exchange({
      exchangeId: <any>(this.selectedAvailableExchange).exchangeId,
      comment: this.comment,
      privateKey: this.privateKey,
      publicKey: this.publicKey
    });

    this.apiClient.apiExchangesPost(exchange).subscribe(res => {
      this.comment = "";
      this.publicKey = "";
      this.privateKey = "";
      this.showForm = false;
      this.refreshOwnExchanges();
    });
  }

  exchangeChanged() {
    if (this.selectedAvailableExchange == null) {
      this.showForm = false;
    } else {
      this.showForm = true;
    }
  }

  ngOnInit() {
    this.refreshOwnExchanges();
  }

  refreshOwnExchanges() {
    this.apiClient.apiExchangesGet().subscribe(res => this.ownExchanges = res);
  }
}

