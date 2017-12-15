import { Component, OnInit } from '@angular/core';
import { DropdownModule, SelectItem } from 'primeng/primeng';

import '../../../../node_modules/primeng/resources/themes/omega/theme.css';
import '../../../../node_modules/primeng/resources/primeng.min.css';
import '../../../../node_modules/font-awesome/css/font-awesome.min.css';

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

  cities1: SelectItem[];
  selectedCity1: City;


  constructor() {

    this.cities1 = [
      { label: 'Select City', value: null },
      { label: 'New York', value: { id: 1, name: 'New York', code: 'NY' } },
      { label: 'Rome', value: { id: 2, name: 'Rome', code: 'RM' } },
      { label: 'London', value: { id: 3, name: 'London', code: 'LDN' } },
      { label: 'Istanbul', value: { id: 4, name: 'Istanbul', code: 'IST' } },
      { label: 'Paris', value: { id: 5, name: 'Paris', code: 'PRS' } }
    ];

  }

  ngOnInit() {
  }

}

