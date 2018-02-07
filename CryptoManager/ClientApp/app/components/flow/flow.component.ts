import { Component, OnInit } from '@angular/core';
import *  as shape from 'd3-shape';


@Component({
    selector: 'app-flow',
    templateUrl: './flow.component.html',
    styleUrls: ['./flow.component.scss']
})

export class FlowComponent implements OnInit {

    constructor() {
        this.hierarchialGraph = this.getTurbineData();
    }


    ngOnInit() {

        if (!this.fitContainer) {
            this.applyDimensions();
        }
    }

    applyDimensions() {
        this.view = [this.width, this.height];
    }

    cache: any = {};

    getTurbineData() {
        const nodes = [];
        const links = [];

       

        for (var node in this.nodes) {
            nodes.push(this.nodes[node]);
        }

        for (var key in this.links) {
            links.push(this.links[key]);
        }

        

        return { nodes, links };
    }

    
    colorScheme = {
        name: 'picnic',
        selectable: false,
        group: 'Ordinal',
        domain: [
            '#FAC51D', '#66BD6D', '#FAA026', '#29BB9C', '#E96B56', '#55ACD2', '#B7332F', '#2C83C9', '#9166B8', '#92E7E8'
        ]
    };
    schemeType: string = 'ordinal';

    curveType: string = 'Linear';
    curve: any = shape.curveBundle.beta(1);


    links = [
        {
            source: 'start',
            target: '1',
            label: 'links to'
        }, {
            source: 'start',
            target: '2'
        }, {
            source: '2',
            target: '4'
        }, {
            source: '1',
            target: '4'
        }
        
    ];

    nodes = [
        {
            id: 'start',
            label: '500 EUR'
        }, {
            id: '1',
            label: '0.1 BTC',
        }, {
            id: '2',
            label: '100 EUR',
        },  {
            id: '4',
            label: '550 EUR'
        }
    ];


    

    hierarchialGraph: { links: any[], nodes: any[] };


    view: any[];
    width: number = 700;
    height: number = 700;
    fitContainer: boolean = true;
    autoZoom: boolean = true;

    // options
    showLegend = false;
    orientation: string = 'TB'; // LR, RL, TB, BT


}

