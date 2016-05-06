import {Component, SimpleChange, OnChanges, OnInit, Input, ElementRef, ViewChildren, ViewChild, QueryList} from 'angular2/core';
import {Observable}     from 'rxjs/Observable';
import {MetricComponent} from './metriccomponent';
import {ApplicationComponent} from './application.component';

import {NodeViewModel} from './../viewmodels/nodeviewmodel';
import {NodeCapacityViewModel} from './../viewmodels/nodecapacityviewmodel';
import {ClusterCapacityViewModel} from './../viewmodels/clustercapacityviewmodel';
import {DeployedApplicationViewModel} from './../viewmodels/deployedapplicationviewmodel';
import {DeployedServiceViewModel} from './../viewmodels/deployedserviceviewmodel';
import {ApplicationViewModel} from './../viewmodels/applicationviewmodel';
import {ServiceViewModel} from './../viewmodels/serviceviewmodel';
import {ReplicaViewModel} from './../viewmodels/replicaviewmodel';
import {List} from './../viewmodels/list';

import {DataService} from './../services/data.service';

@Component({
    selector: 'node-component',
    templateUrl: 'app/components/node.component.html',
    styleUrls: ['app/components/node.component.css'],
    directives: [ApplicationComponent]
})

export class NodeComponent extends MetricComponent implements OnInit {

    private DefaultCapacitySize: number = 500;

    @ViewChild("container")
    protected container: ElementRef;

    @ViewChildren(ApplicationComponent)
    private applicationComponents: QueryList<ApplicationComponent>

    @Input()
    private node: NodeViewModel;

    @Input()
    private scaleFactor: number;
    
    @Input()
    protected selectedColors: string;

    @Input()
    protected selectedMetricName: string;
    
    private applications: DeployedApplicationViewModel[];
    protected selectedCapacity: NodeCapacityViewModel;
    protected parentContainerSize: number;
    protected parentCapacity: number;
    protected elementHeight: number;
    protected expanded: boolean;
    private loadPercent: number;

    constructor(
        private dataService: DataService)
    {
        super();
        
        this.applications = [];
        this.expanded = true;
    }

    public ngOnChanges(changes: { [propertyName: string]: SimpleChange }) {

        this.selectedCapacity = this.node.capacities.find(x => x.name == this.selectedMetricName) || null;

        if (this.selectedCapacity) {
            this.parentCapacity = this.selectedCapacity.capacity < 0
                ? this.DefaultCapacitySize
                : this.selectedCapacity.capacity;

            this.elementHeight = this.selectedCapacity.capacity < 0
                ? -1
                : Math.max(0, (this.selectedCapacity.capacity * this.scaleFactor) - super.getOuterVerticalSpacing(this.container));

            this.parentContainerSize = this.selectedCapacity.capacity < 0
                ? this.DefaultCapacitySize * this.scaleFactor
                : Math.max(0, this.elementHeight - super.getInnerVerticalSpacing(this.container));

            this.loadPercent = Math.round(this.selectedCapacity.load / this.selectedCapacity.capacity * 100);
        }

        console.log("parentCapacity: " + this.parentCapacity);
        console.log("parentContainerSize: " + this.parentContainerSize);
        console.log("selectedCapacity: " + this.selectedCapacity.capacity);
    }
    

    public ngOnInit() {
        this.dataService.getApplicationModels(this.node.name).subscribe(
            result => {
                if (!result) {
                    return;
                }

                this.applications = result.map(x =>
                    new DeployedApplicationViewModel(
                        true,
                        new ApplicationViewModel(
                            x.application.name,
                            x.application.type,
                            x.application.version,
                            x.application.status,
                            x.application.healthState,
                            x.application.metrics),
                        x.services.map(y =>
                            new DeployedServiceViewModel(
                                true,
                                new ServiceViewModel(
                                    y.service.name,
                                    y.service.type,
                                    y.service.version,
                                    y.service.status,
                                    y.service.healthState,
                                    y.service.metrics),
                                y.replicas.map(z =>
                                    new ReplicaViewModel(
                                        z.id,
                                        z.partitionId,
                                        z.status,
                                        z.healthState,
                                        z.role,
                                        z.metrics))))));
            },
            error => console.log("error from observable: " + error));
    }
}
