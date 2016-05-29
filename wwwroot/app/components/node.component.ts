import {Component, ChangeDetectorRef, ChangeDetectionStrategy, OnInit, OnDestroy, OnChanges, SimpleChange, Input} from 'angular2/core';
import {Observable, Subscription}     from 'rxjs/rx';
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
import {Selectable} from './../viewmodels/selectable';

@Component({
    selector: 'node-component',
    templateUrl: 'app/components/node.component.html',
    styleUrls: ['app/components/node.component.css'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class NodeComponent implements OnInit, OnDestroy {

    private DefaultCapacitySize: number = 500;
    
    @Input()
    private nodeCapacities: NodeCapacityViewModel[];

    @Input()
    private scaleFactor: number;

    @Input()
    private selected: boolean;

    @Input()
    private selectedColors: string;

    @Input()
    private selectedMetricName: string;

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

    private selectedCapacity: NodeCapacityViewModel;
    private nodeContainerSize: number;
    private nodeCapacity: number;
    private elementHeight: number;
    private applicationSubscription: Subscription;
    private applications: DeployedApplicationViewModel[];
    private loadPercent: number;

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

        this.applications = [];
    }

    public ngOnChanges(changes: { [propertyName: string]: SimpleChange }): void {

        if (changes['selected']) {
            for (var a of this.applications) {
                a.selected = this.selected;
            }
        }

        if (changes['selectedMetricName']) {

            this.selectedCapacity = this.nodeCapacities.find(x => x.name == this.selectedMetricName) || null;

            for (var appView of this.applications) {
                appView.application.selectedMetric = appView.application.metrics.find(x => x.name == this.selectedMetricName);

                for (var serviceView of appView.services) {
                    serviceView.service.selectedMetric = serviceView.service.metrics.find(x => x.name == this.selectedMetricName);

                    for (var replicaView of serviceView.replicas) {
                        replicaView.selectedMetric = replicaView.metrics.find(x => x.name == this.selectedMetricName);
                    }
                }
            }
        }

        this.redraw();
    }


    public ngOnInit(): void{
        this.applicationSubscription = this.dataService.getApplicationModels(this.nodeName, () => this.selectedApplicationTypes.filter(x => !x.selected).map(x => x.name)).subscribe(
            result => {
                if (!result) {
                    return;
                }
                
                List.updateList(this.applications, result.map(x =>
                    new DeployedApplicationViewModel(
                        true,
                        new ApplicationViewModel(
                            x.application.name,
                            x.application.type,
                            x.application.version,
                            x.application.status.toLowerCase(),
                            x.application.healthState.toLowerCase(),
                            x.application.metrics.find(x => x.name == this.selectedMetricName),
                            x.application.metrics),
                        x.services.map(y =>
                            new DeployedServiceViewModel(
                                true,
                                new ServiceViewModel(
                                    y.service.name,
                                    y.service.type,
                                    y.service.version,
                                    y.service.status.toLowerCase(),
                                    y.service.healthState.toLowerCase(),
                                    y.service.metrics.find(x => x.name == this.selectedMetricName),
                                    y.service.metrics),
                                y.replicas.map(z =>
                                    new ReplicaViewModel(
                                        z.id,
                                        z.partitionId,
                                        z.status.toLowerCase(),
                                        z.healthState.toLowerCase(),
                                        z.role.toLowerCase(),
                                        z.metrics.find(x => x.name == this.selectedMetricName),
                                        z.metrics)))))));
                this.redraw();

            },
            error => console.log("error from observable: " + error));
    }

    public ngOnDestroy(): void {
        if (this.applicationSubscription) {
            this.applicationSubscription.unsubscribe();
        }
    }

    private redraw(): void {

        if (this.selectedCapacity) {
            this.nodeCapacity = this.selectedCapacity.capacity <= 0
                ? this.DefaultCapacitySize
                : this.selectedCapacity.capacity;

            this.elementHeight = this.selectedCapacity.capacity <= 0
                ? -1
                : Math.max(0, (this.selectedCapacity.capacity * this.scaleFactor) - this.nodeMargin);

            this.nodeContainerSize = this.selectedCapacity.capacity <= 0
                ? this.DefaultCapacitySize * this.scaleFactor
                : Math.max(0, this.elementHeight - this.nodePaddingAndBorder);

            this.loadPercent = Math.round(this.selectedCapacity.load / this.selectedCapacity.capacity * 100);

            for (var appView of this.applications) {
                if (!appView.application.selectedMetric) {
                    continue;
                }

                appView.application.elementHeight =
                    Math.max(0, ((appView.application.selectedMetric.value / this.nodeCapacity) * this.nodeContainerSize) - this.applicationMargin);

                for (var serviceView of appView.services) {
                    if (!serviceView.service.selectedMetric) {
                        continue;
                    }

                    serviceView.service.elementHeight =
                        Math.max(0, ((serviceView.service.selectedMetric.value / appView.application.selectedMetric.value) * (appView.application.elementHeight - this.applicationPaddingAndBorder)) - this.serviceMargin);

                    for (var replicaView of serviceView.replicas) {
                        replicaView.elementHeight =
                            Math.max(0, ((replicaView.selectedMetric.value / serviceView.service.selectedMetric.value) * (serviceView.service.elementHeight - this.servicePaddingAndBorder)) - this.replicaMargin);
                    }
                }
            }
        }
    }
}
