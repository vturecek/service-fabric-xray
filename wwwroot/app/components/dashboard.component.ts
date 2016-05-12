import {Component, OnInit, Inject} from 'angular2/core';
import {Router} from 'angular2/router'; 
import {ClusterCapacityGraph} from './clustercapacitygraph.component';
import {ClusterCapacityDonut} from './clustercapacitydonut.component';
import {DataService} from './../services/data.service';
import {ClusterCapacityViewModel} from './../viewmodels/clustercapacityviewmodel';
import {List} from './../viewmodels/list';
import {ClusterCapacityHistory} from './../models/clustercapacityhistory';

@Component({
    selector: 'dashboard-component',
    templateUrl: 'app/components/dashboard.component.html',
    styleUrls: ['app/components/dashboard.component.css'],
    directives: [ClusterCapacityGraph, ClusterCapacityDonut]
})

export class DashboardComponent implements OnInit {

    private capacities: ClusterCapacityViewModel[] = [];

    private capacityHistory: ClusterCapacityHistory[] = [];

    public constructor(
        private dataService: DataService,
        private router: Router)
    {
    }

    public ngOnInit() {
        this.dataService.getClusterCapacity().subscribe(
            result => {
                if (!result) {
                    return;
                }

                List.updateList(this.capacities, result.map(x =>
                    new ClusterCapacityViewModel(
                        x.bufferedCapacity,
                        x.capacity,
                        x.load,
                        x.remainingBufferedCapacity,
                        x.remainingCapacity,
                        x.isClusterCapacityViolation,
                        x.name,
                        x.bufferPercentage)));
            },
            error => console.log("error from observable: " + error));

        this.dataService.getClusterCapacityHistory().subscribe(
            result => {
                if (!result) {
                    return;
                }

                this.capacityHistory = result;
            },
            error => console.log("error from observable: " + error));
    }
}