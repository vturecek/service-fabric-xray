import {Component, Input, ViewChild, ElementRef} from 'angular2/core';
import {MetricComponent} from './metriccomponent';
import {Router} from 'angular2/router';
import {ReplicaViewModel} from './../viewmodels/replicaviewmodel';
import {LoadMetric} from './../models/loadmetric';

@Component({
    selector: 'replica-component',
    templateUrl: 'app/components/replica.component.html',
    styleUrls: ['app/components/replica.component.css']
})

export class ReplicaComponent extends MetricComponent  {

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
    protected replicaRole: string;

    @Input()
    protected health: string;

    @Input()
    protected status: string;

    @Input()
    protected metrics: LoadMetric[];


    constructor(
        private router: Router)
    {
        super();
    }

    protected getMetrics(): LoadMetric[] {
        return this.metrics || [];
    }


    private onselect(item: ReplicaViewModel) {
        //item.selected = !item.selected;
    }
    
    
}
