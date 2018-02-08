import { Component, OnInit } from '@angular/core';
import *  as shape from 'd3-shape';
import { CryptoApiClient } from '../../services/api-client';


@Component({
    selector: 'app-flow',
    templateUrl: './flow.component.html',
    styleUrls: ['./flow.component.scss']
})

export class FlowComponent implements OnInit {

    constructor(private apiClient: CryptoApiClient) {
        const nodes: any[] = [];
        const links: any[] = [];
        this.hierarchialGraph = { nodes, links };
    }


    ngOnInit() {
        this.apiClient.apiFlowsNodesGet().subscribe(nodes => {
            for (var i = 0; i < nodes.length; i++) {
                var label = nodes[i].comment == null
                    ? <number>nodes[i].amount + " " + nodes[i].currency
                    : <string>nodes[i].comment + " " + nodes[i].amount + " " + nodes[i].currency;


                this.hierarchialGraph.nodes.push({
                    id: nodes[i].id,
                    label: label
                });

            }
            this.hierarchialGraph.nodes = [...this.hierarchialGraph.nodes];

            this.apiClient.apiFlowsLinksGet().subscribe(links => {
                for (var i = 0; i < links.length; i++) {

                    this.hierarchialGraph.links.push({
                        source: links[i].flowNodeSource,
                        target: links[i].flowNodeTarget,
                        label: links[i].comment
                    });

                }

                this.hierarchialGraph.links = [...this.hierarchialGraph.links];
            });
        });



        if (!this.fitContainer) {
            this.applyDimensions();
        }
    }

    applyDimensions() {
        this.view = [this.width, this.height];
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

