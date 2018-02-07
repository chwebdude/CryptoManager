import { Component, OnInit } from '@angular/core';
import *  as shape from 'd3-shape';
import { CryptoApiClient, FlowNode } from '../../services/api-client';


@Component({
    selector: 'app-flow',
    templateUrl: './flow.component.html',
    styleUrls: ['./flow.component.scss']
})

export class FlowComponent implements OnInit {

    constructor(private apiClient: CryptoApiClient) {
        this.hierarchialGraph = this.getTurbineData();
        //this.hierarchialGraph = { links: [], nodes: [] };
    }


    ngOnInit() {
        this.apiClient.apiFlowsGet().subscribe(nodes => {
            for (var i = 0; i < nodes.length; i++) {
                var label = nodes[i].comment == null
                    ? <number>nodes[i].amount + " " + nodes[i].currency
                    : <string>nodes[i].comment + " " + nodes[i].amount + " " + nodes[i].currency;

                this.hierarchialGraph.nodes.push({
                    id: i.toString(),
                    label: label
                });

            }
            this.hierarchialGraph.nodes = [...this.hierarchialGraph.nodes];
            console.log(this.hierarchialGraph);
        });
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
        const links: any[] = [];



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
            source: 'a0',
            target: 'a1',
            label: 'links to'
        }, {
            source: 'a0',
            target: 'a2'
        }, {
            source: 'a2',
            target: 'a4'
        }, {
            source: 'a1',
            target: 'a4'
        }

    ];

    nodes = [
        {
            id: 'a0',
            label: '500 EUR'
        }, {
            id: 'a1',
            label: '0.1 BTC',
        }, {
            id: 'a2',
            label: '100 EUR',
        }, {
            id: 'a4',
            label: '550 EUR'
        }
    ];




    hierarchialGraph: { links: any[], nodes: any[] };


    view: any[];
    width: number = 1200;
    height: number = 1200;
    fitContainer: boolean = false;
    autoZoom: boolean = true;

    // options
    showLegend = false;
    orientation: string = 'TB'; // LR, RL, TB, BT


}

