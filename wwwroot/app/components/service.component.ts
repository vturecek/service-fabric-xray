import {Component, Input, ElementRef, ViewChild} from 'angular2/core';
import {MetricComponent} from './metriccomponent';
import {DeployedServiceViewModel} from './../viewmodels/deployedserviceviewmodel';
import {ReplicaViewModel} from './../viewmodels/replicaviewmodel';
import {ReplicaComponent} from './replica.component';
import {LoadMetric} from './../models/loadmetric';

@Component({
    selector: 'service-component',
    templateUrl: 'app/components/service.component.html',
    styleUrls: ['app/components/service.component.css'],
    directives: [ReplicaComponent]
})

export class ServiceComponent extends MetricComponent {

    @ViewChild("container")
    protected container: ElementRef;

    @Input()
    protected parentCapacity: number;

    @Input()
    protected parentContainerSize: number;

    @Input()
    protected selectedColors: string;

    @Input()
    protected selectedMetricName: string;

    @Input()
    protected serviceName: string;

    @Input()
    protected health: string;

    @Input()
    protected status: string;

    @Input()
    protected replicas: ReplicaViewModel[];

    @Input()
    protected metrics: LoadMetric[];

    @Input()
    protected selected: boolean;

    constructor()
    {
        super();
    }

    private selectService() {
        this.selected = !this.selected;
    }
}
