import {Component, OnInit, Input, ElementRef, ViewChild} from 'angular2/core';
import {MetricComponent} from './metriccomponent';
import {ServiceComponent} from './service.component';

import {ApplicationViewModel} from './../viewmodels/applicationviewmodel';
import {ServiceViewModel} from './../viewmodels/serviceviewmodel';
import {ReplicaViewModel} from './../viewmodels/replicaviewmodel';
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

export class ApplicationComponent extends MetricComponent implements OnInit {

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
    private nodeName: string;
    
    private applications: DeployedApplicationViewModel[];


    constructor(
        private dataService: DataService)
    {
        super();
        this.applications = [];
    }

    public ngOnInit() {
        console.log("subscribing to getApplications()");

        this.dataService.getApplicationModels(this.nodeName).subscribe(
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
                                        z.metrics)))))));
            },
            error => console.log("error from observable: " + error));
    }
    
    private selectApplication(item: DeployedApplicationViewModel) {
        item.selected = !item.selected;
    }

    public toggleSelectAll(selected:boolean) {
        this.applications.forEach(application => {
            application.selected = selected;
            application.services.forEach(service => service.selected = selected);
        });
    }
}