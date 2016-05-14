import {Component, OnInit, Inject} from 'angular2/core';
import {Router} from 'angular2/router'; 
import {ClusterCapacityGraph} from './clustercapacitygraph.component';
import {ClusterCapacityDonut} from './clustercapacitydonut.component';
import {DataService} from './../services/data.service';
import {ClusterCapacityViewModel} from './../viewmodels/clustercapacityviewmodel';
import {List} from './../viewmodels/list';
import {ClusterCapacityHistoryViewModel} from './../viewmodels/clustercapacityhistoryviewmodel';

@Component({
    selector: 'dashboard-component',
    templateUrl: 'app/components/dashboard.component.html',
    styleUrls: ['app/components/dashboard.component.css'],
    directives: [ClusterCapacityGraph, ClusterCapacityDonut]
})

export class DashboardComponent implements OnInit {
    
    private capacities: ClusterCapacityViewModel[] = [];

    private capacityHistory: ClusterCapacityHistoryViewModel[] = [];
    
    public constructor(
        private dataService: DataService,
        private router: Router)
    {
    }
    
    public ngOnInit() {
       
        this.dataService.getClusterCapacityHistory().subscribe(
            result => {
                if (!result) {
                    return;
                }

                this.capacityHistory = result.map(x =>
                    new ClusterCapacityHistoryViewModel(x.name, true, x.current, x.history));
            },
            error => console.log("error from observable: " + error));
    }
}