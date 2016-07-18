import {Component, EventEmitter, ElementRef, Output, ChangeDetectorRef, ChangeDetectionStrategy, OnInit, OnDestroy, OnChanges, SimpleChange, Input} from 'angular2/core';
import {Observable, Subscription}     from 'rxjs/rx';
import {NodeViewModel} from './../viewmodels/nodeviewmodel';
import {NodeCapacityViewModel} from './../viewmodels/nodecapacityviewmodel';
import {ClusterCapacityViewModel} from './../viewmodels/clustercapacityviewmodel';
import {DeployedApplicationViewModel} from './../viewmodels/deployedapplicationviewmodel';
import {DeployedServiceViewModel} from './../viewmodels/deployedserviceviewmodel';
import {DeployedReplicaViewModel} from './../viewmodels/deployedreplicaviewmodel';
import {List} from './../viewmodels/list';
import {LoadMetric} from './../models/loadmetric';
import {DataService} from './../services/data.service';
import {Selectable} from './../viewmodels/selectable';
import {NodeCapacityInfoDirective} from './../directives/nodecapacityinfo.directive';

@Component({
    selector: 'node-component',
    templateUrl: 'app/components/node.component.html',
    styleUrls: ['app/components/node.component.css'],
    changeDetection: ChangeDetectionStrategy.OnPush,
    directives: [NodeCapacityInfoDirective]
})
export class NodeComponent implements OnInit, OnDestroy {

    private DefaultCapacitySize: number = 500;

    @Output()
    private capacityCountChange: EventEmitter<number> = new EventEmitter();

    @Input()
    private capacityCount: number;

    @Input()
    private selectedClusterCapacity: ClusterCapacityViewModel;

    @Input()
    private scaleFactor: number;

    @Input()
    private applicationsExpanded: boolean;

    @Input()
    private servicesExpanded: boolean;

    @Input()
    private selectedColors: string;

    @Input()
    private selectedApplicationTypes: Selectable[];

    @Input()
    private nodeName: string;

    @Input()
    private nodeType: string;

    @Input()
    private health: string;

    @Input()
    private status: string;

    @Input()
    private upTime: string;

    @Input()
    private address: string;
    
    private elementHeight: number;
    private nodeHeaderHeight: number;
    private applicationSubscription: Subscription;
    private applications: DeployedApplicationViewModel[];
    private nodeCapacitySubscription: Subscription;
    private selectedNodeCapacity: NodeCapacityViewModel;
    private nodeCapacities: NodeCapacityViewModel[];

    // element spacing in pixel values. 
    // Margin is outer spacing: margin-top + margin-bottom
    // PaddingAndBorder is inner spacing: padding-top + padding-bottom + border-width * 2
    // these are not computed at runtime so they need to match CSS set on the corresponding elements.
    private nodeMargin: number = 6;
    private nodePaddingAndBorder: number = 6;
    private applicationMargin: number = 4;
    private applicationPaddingAndBorder: number = 2;
    private serviceMargin: number = 2;
    private servicePaddingAndBorder: number = 2;
    private replicaMargin: number = 0;

    constructor(
        private changeDetector: ChangeDetectorRef,
        private dataService: DataService) {
        this.nodeCapacities = [];
        this.applications = [];
    }

    public ngOnChanges(changes: { [propertyName: string]: SimpleChange }): void {

        if (changes['capacityCount']) {
            console.log(this.capacityCount);
            this.nodeHeaderHeight = 120 + this.capacityCount * 7;
        }

        if (changes['applicationsExpanded']) {
            for (var appView of this.applications) {
                appView.expanded = this.applicationsExpanded;
            }
        }

        if (changes['servicesExpanded']) {
            for (var appView of this.applications) {
                for (var serviceView of appView.services) {
                    serviceView.expanded = this.servicesExpanded;
                }
            }
        }

        if (changes['selectedClusterCapacity']) {
            for (var appView of this.applications) {
                appView.selectedMetric = this.getSelectedMetricValue(appView.application.metrics);

                for (var serviceView of appView.services) {
                    serviceView.selectedMetric = this.getSelectedMetricValue(serviceView.service.metrics);

                    for (var replicaView of serviceView.replicas) {
                        replicaView.selectedMetric = this.getSelectedMetricValue(replicaView.replica.metrics);
                    }
                }
            }
        }

        if (changes['selectedColors']) {
            for (var appView of this.applications) {
                appView.selectedClass = this.getSelectedColors(appView.application);

                for (var serviceView of appView.services) {
                    serviceView.selectedClass = this.getSelectedColors(serviceView.service);

                    for (var replicaView of serviceView.replicas) {
                        replicaView.selectedClass = this.getSelectedColors(replicaView.replica);
                    }
                }
            }
        }

        this.computeElementHeights();
    }
    
    public ngOnInit(): void {
        this.nodeCapacitySubscription = this.dataService.getNodeCapacity(this.nodeName).subscribe(
            result => {
                if (!result) {
                    return;
                }

                let currentCount = this.nodeCapacities.length;

                List.updateList(this.nodeCapacities, result.map(x =>
                    new NodeCapacityViewModel(
                        x.isCapacityViolation,
                        x.name,
                        x.bufferedCapacity,
                        x.capacity,
                        x.load,
                        x.remainingBufferedCapacity,
                        x.remainingCapacity)));

                this.computeElementHeights();
                this.changeDetector.markForCheck();

                if (currentCount != this.nodeCapacities.length) {
                    this.capacityCountChange.emit(this.nodeCapacities.length);
                }
            },
            error => console.log("error from observable: " + error)

        );

        this.applicationSubscription = this.dataService.getApplicationModels(this.nodeName, () => this.selectedApplicationTypes.filter(x => !x.selected).map(x => x.name)).subscribe(
            result => {
                if (!result) {
                    return;
                }

                List.updateList(this.applications, result.map(x =>
                    new DeployedApplicationViewModel(
                        true,
                        this.getSelectedMetricValue(x.application.metrics),
                        this.getSelectedColors(x.application),
                        x.application,
                        x.services.map(y =>
                            new DeployedServiceViewModel(
                                true,
                                this.getSelectedMetricValue(y.service.metrics),
                                this.getSelectedColors(y.service),
                                y.service,
                                y.replicas.map(z =>
                                    new DeployedReplicaViewModel(
                                        this.getSelectedMetricValue(z.metrics),
                                        this.getSelectedColors(z),
                                        z.role ? z.role.toLowerCase() : 'unknown',
                                        z)))))));

                this.computeElementHeights();
                this.changeDetector.markForCheck();

            },
            error => console.log("error from observable: " + error));
    }

    public ngOnDestroy(): void {
        if (this.applicationSubscription) {
            this.applicationSubscription.unsubscribe();
        }

        if (this.nodeCapacitySubscription) {
            this.nodeCapacitySubscription.unsubscribe();
        }
    }

    private getPercentage(item: NodeCapacityViewModel, cap: boolean = false): string {
        if (!item) return '0';
        let capacity: number = item.capacity > 0 ? item.capacity : this.selectedClusterCapacity.load;
        let result = capacity > 0 ? item.load / capacity * 100 : 0;

        if (cap && result > 100.0) {
            return '100.0';
        }

        return result.toFixed(1);
    }
    
    private getSelectedMetricValue(metrics: LoadMetric[]): number {

        if (this.selectedClusterCapacity) {
            let metric: LoadMetric = metrics.find(x => x.name == this.selectedClusterCapacity.name);

            if (metric) {
                return metric.value;
            }
        }
        
        return 0;
    }

    private getSelectedColors(model: any): string {
        let colors: string;
        switch (this.selectedColors) {
            case "status":
                colors = model.status;
                break;
            case "health":
                colors = model.healthState;
                break;
        }

        return colors
            ? colors.toLowerCase()
            : 'unknown';
    }

    private computeElementHeights(): void {

        if (!this.selectedClusterCapacity) {
            return;
        }

        this.selectedNodeCapacity = this.nodeCapacities.find(x => x.name == this.selectedClusterCapacity.name);
        
        if (this.selectedNodeCapacity) {

            var nodeCapacity: number;
            var nodeContainerSize: number;
            
            if (this.selectedNodeCapacity.capacity <= 0) {
                this.elementHeight = -1; // lets the browser auto scale height  
                nodeCapacity = this.DefaultCapacitySize / 20;
                nodeContainerSize = this.DefaultCapacitySize * this.scaleFactor;
            }
            else {
                this.elementHeight = Math.max(0, (this.selectedNodeCapacity.capacity * this.scaleFactor) - this.nodeMargin);
                nodeCapacity = this.selectedNodeCapacity.capacity;
                nodeContainerSize = Math.max(0, this.elementHeight - this.nodePaddingAndBorder);
            }


            for (var appView of this.applications) {
                if (appView.selectedMetric <= 0) {
                    continue;
                }

                appView.elementHeight =
                    Math.max(0, ((appView.selectedMetric / nodeCapacity) * nodeContainerSize) - this.applicationMargin)

                for (var serviceView of appView.services) {
                    if (serviceView.selectedMetric <= 0) {
                        continue;
                    }

                    serviceView.elementHeight =
                        Math.max(0, ((serviceView.selectedMetric / appView.selectedMetric) * (appView.elementHeight - this.applicationPaddingAndBorder)) - this.serviceMargin);

                    for (var replicaView of serviceView.replicas) {
                        if (replicaView.selectedMetric <= 0) {
                            continue;
                        }

                        replicaView.elementHeight =
                            Math.max(0, ((replicaView.selectedMetric / serviceView.selectedMetric) * (serviceView.elementHeight - this.servicePaddingAndBorder)) - this.replicaMargin);
                    }
                }
            }
        }
    }
}
