/// <reference path="../../../typings/jquery/jquery.d.ts" />
import {Component, ChangeDetectorRef, ChangeDetectionStrategy, OnInit, OnDestroy, OnChanges, SimpleChange, Input, ElementRef, ViewChildren, ViewChild, QueryList} from 'angular2/core';
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

    @ViewChild("container")
    protected container: ElementRef;

    @Input()
    protected nodeCapacities: NodeCapacityViewModel[];

    @Input()
    protected scaleFactor: number;

    @Input()
    protected selected: boolean;

    @Input()
    protected selectedColors: string;

    @Input()
    protected selectedMetricName: string;

    @Input()
    protected selectedApplicationTypes: Selectable[];

    @Input()
    protected nodeName: string;

    @Input()
    protected nodeType: string;

    @Input()
    protected health: string;

    @Input()
    protected status: string;

    @Input()
    protected upTime: string;

    @Input()
    protected address: string;

    protected selectedCapacity: NodeCapacityViewModel;
    protected nodeContainerSize: number;
    protected nodeCapacity: number;
    protected elementHeight: number;

    private applicationSubscription: Subscription;
    private applications: DeployedApplicationViewModel[];
    private loadPercent: number;

    constructor(
        private changeDetector: ChangeDetectorRef,
        private dataService: DataService) {

        this.applications = [];
        this.selected = true;
    }

    public ngOnChanges(changes: { [propertyName: string]: SimpleChange }) {

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

        if (this.selectedCapacity) {
            this.nodeCapacity = this.selectedCapacity.capacity <= 0
                ? this.DefaultCapacitySize
                : this.selectedCapacity.capacity;

            this.elementHeight = this.selectedCapacity.capacity <= 0
                ? -1
                : Math.max(0, (this.selectedCapacity.capacity * this.scaleFactor) - this.getOuterVerticalSpacing(this.container));

            this.nodeContainerSize = this.selectedCapacity.capacity <= 0
                ? this.DefaultCapacitySize * this.scaleFactor
                : Math.max(0, this.elementHeight - this.getInnerVerticalSpacing(this.container));

            this.loadPercent = Math.round(this.selectedCapacity.load / this.selectedCapacity.capacity * 100);
            
            for (var appView of this.applications) {
                if (!appView.application.selectedMetric) {
                    continue;
                }

                appView.application.elementHeight =
                    Math.max(0, ((appView.application.selectedMetric.value / this.nodeCapacity) * this.nodeContainerSize));

                for (var serviceView of appView.services) {
                    if (!serviceView.service.selectedMetric) {
                        continue;
                    }

                    serviceView.service.elementHeight =
                        Math.max(0, ((serviceView.service.selectedMetric.value / appView.application.selectedMetric.value) * (appView.application.elementHeight - 2)) - 2);

                    for (var replicaView of serviceView.replicas) {
                        replicaView.elementHeight =
                            Math.max(0, ((replicaView.selectedMetric.value / serviceView.service.selectedMetric.value) * (serviceView.service.elementHeight - 2)));
                    }
                }
            }
        }
    }

    public ngOnInit() {
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

            },
            error => console.log("error from observable: " + error));
    }

    public ngOnDestroy() {
        if (this.applicationSubscription) {
            this.applicationSubscription.unsubscribe();
        }
    }

    protected getDeployedEntityClass<T>(classes: string[]): string[] {
        let result: string = "";
        switch (this.selectedColors) {
            case "status":
                result = this.status;
                break;
            case "health":
                result = this.health;
                break;
        }

        result = result ?
            result.toLowerCase() :
            "unknown";

        return [result].concat(classes);
    }


    /**
     * Gets the height of the top and bottom margin of the given element.
     * @param el
     */
    private getOuterVerticalSpacing(el: ElementRef) {
        if (el) {
            return jQuery(el.nativeElement).outerHeight(true) - jQuery(el.nativeElement).outerHeight();
        }
        return 0;
    }

    /**
     * Gets the height of the top and bottom padding and border width of the given element.
     * @param el
     */
    private getInnerVerticalSpacing(el: ElementRef) {
        if (el) {
            return jQuery(el.nativeElement).outerHeight(false) - jQuery(el.nativeElement).height();
        }
        return 0;
    }
}
