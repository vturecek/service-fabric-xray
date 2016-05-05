import {Component, OnChanges, Input, ElementRef, ViewChild, SimpleChange} from 'angular2/core';
import {MetricComponent} from './metriccomponent';
import {ServiceComponent} from './service.component';

import {LoadMetric} from './../models/loadmetric';

import {DeployedApplicationViewModel} from './../viewmodels/deployedapplicationviewmodel';
import {DeployedServiceViewModel} from './../viewmodels/deployedserviceviewmodel';
import {List} from './../viewmodels/list';

import {DataService} from './../services/data.service';

@Component({
    selector: 'application-component',
    templateUrl: 'app/components/application.component.html',
    styleUrls: ['app/components/application.component.css'],
    directives: [ServiceComponent]
})

export class ApplicationComponent extends MetricComponent {

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
    protected nodeName: string;

    @Input()
    protected applicationViewModel: DeployedApplicationViewModel;
    
    constructor(
        private dataService: DataService)
    {
        super();
    }

    protected getMetrics(): LoadMetric[] {
        return this.applicationViewModel.application.metrics;
    }

    private selectApplication() {
        this.applicationViewModel.selected = !this.applicationViewModel.selected;
    }
    
}