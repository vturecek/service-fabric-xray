import {Component, OnInit, OnDestroy, ElementRef, ViewChild} from 'angular2/core';
import {Observable, Subscription}     from "rxjs/rx";
import {MetricComponent} from './metriccomponent';
import {NodeComponent} from './node.component';

import {NodeViewModel} from './../viewmodels/nodeviewmodel';
import {NodeCapacityViewModel} from './../viewmodels/nodecapacityviewmodel';
import {ClusterCapacityViewModel} from './../viewmodels/clustercapacityviewmodel';
import {List} from './../viewmodels/list';

import {DataService} from './../services/data.service';

@Component({
    selector: 'cluster-component',
    templateUrl: 'app/components/cluster.component.html',
    styleUrls: ['app/components/cluster.component.css'],
    directives: [NodeComponent]
})

export class ClusterComponent implements OnInit, OnDestroy {

    private DefaultCapacitySize: number = 500;

    @ViewChild("container")
    protected container: ElementRef;

    private selectedMetricName: string = "Default Replica Count";
    private selectedColors: string = 'status';
    private expanded: boolean;
    private scaleFactor: number;
    private nodes: NodeViewModel[];
    private capacities: ClusterCapacityViewModel[];
    private nodeSubscription: Subscription;
    private clusterSubscription: Subscription;

    constructor(
        private dataService: DataService )
    {
        this.scaleFactor = 1;
        this.expanded = true;
        this.nodes = [];
        this.capacities = [];
    }

    private hasSelectedCapacity(node: NodeViewModel): boolean {
        return node.capacities.find(x => x.name == this.selectedMetricName) != undefined;
    }

    private onChangeCapacity(newValue) {
        this.selectedMetricName = newValue;
    }

    private onChangeColors(newValue) {
        this.selectedColors = newValue;
    }


    public ngOnInit() {
        this.nodeSubscription = this.dataService.getNodes().subscribe(
            result => {
                if (!result) {
                    return;
                }

                List.updateList(this.nodes, result.map(x =>
                    new NodeViewModel(
                        x.name,
                        x.status.toLowerCase(),
                        x.healthState.toLowerCase(),
                        x.faultDomain,
                        x.upgradeDomain,
                        x.capacities.map(y =>
                            new NodeCapacityViewModel(
                                y.isCapacityViolation,
                                y.name,
                                y.bufferedCapacity,
                                y.capacity,
                                y.load,
                                y.remainingBufferedCapacity,
                                y.remainingCapacity)))));
                
            },
            error => console.log("error from observable: " + error));
                

        this.clusterSubscription = this.dataService.getClusterCapacity().subscribe(
            result => {
                if (!result) {
                    return;
                }

                List.updateList(this.capacities, result.map(x =>
                    new ClusterCapacityViewModel(
                        x.bufferedCapacity,
                        x.capacity,
                        x.load,
                        x.remainingBufferedCapacity,
                        x.remainingCapacity,
                        x.isClusterCapacityViolation,
                        x.name,
                        x.bufferPercentage,
                        true
                    )));
            },
            error => console.log("error from observable: " + error));
    }

    public ngOnDestroy() {
        if (this.nodeSubscription) {
            this.nodeSubscription.unsubscribe();
        }

        if (this.clusterSubscription) {
            this.clusterSubscription.unsubscribe();
        }
    }
}
