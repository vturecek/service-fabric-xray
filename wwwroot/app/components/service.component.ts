import {Component, Input, ElementRef, ViewChild} from 'angular2/core';
import {MetricComponent} from './metriccomponent';
import {DeployedServiceViewModel} from './../viewmodels/deployedserviceviewmodel';
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
    private serviceViewModel: DeployedServiceViewModel;

    protected getMetrics(): LoadMetric[] {
        return this.serviceViewModel.service.metrics;
    }

    constructor()
    {
        super();
    }

    private selectService(item: DeployedServiceViewModel) {
        item.selected = !item.selected;
    }
}
