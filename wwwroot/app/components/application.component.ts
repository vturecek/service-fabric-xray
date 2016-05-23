import {Component, Input, ElementRef, ViewChild} from 'angular2/core';
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
    protected applicationName: string;

    @Input()
    protected health: string;

    @Input()
    protected status: string;

    @Input()
    protected services: DeployedServiceViewModel[];

    @Input()
    protected metrics: LoadMetric[];

    @Input()
    protected selected: boolean;

    constructor(
        private dataService: DataService)
    {
        super();
    }
    
    private selectApplication() {
        this.selected = !this.selected;
    }
}