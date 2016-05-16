import {Component, AfterViewInit, Inject} from 'angular2/core';
import {Router} from 'angular2/router';
import {Observable, BehaviorSubject} from "rxjs/Rx";
import {ClusterCapacityGraph, DataStream} from './clustercapacitygraph.component';
import {ClusterCapacityDonut} from './clustercapacitydonut.component';
import {DataService} from './../services/data.service';
import {ClusterCapacityViewModel} from './../viewmodels/clustercapacityviewmodel';
import {List} from './../viewmodels/list';
import {ClusterCapacityHistory} from './../models/clustercapacityhistory';

declare var Chart: any;

@Component({
    selector: 'dashboard-component',
    templateUrl: 'app/components/dashboard.component.html',
    styleUrls: ['app/components/dashboard.component.css'],
    directives: [ClusterCapacityGraph, ClusterCapacityDonut]
})
export class DashboardComponent implements AfterViewInit {

    private clusterCapacities: ClusterCapacityViewModel[] = [];

    private clusterCapacityStream: BehaviorSubject<DataStream>;

    private dataStreams: DataStream[] = [];

    public constructor(
        private dataService: DataService,
        private router: Router) {
        this.clusterCapacityStream = new BehaviorSubject(null);

        Chart.defaults.global.maintainAspectRatio = false;
        Chart.defaults.global.defaultFontFamily = "'Segoe UI', 'Segoe', Arial, sans-serif";
        Chart.defaults.global.defaultFontSize = 10;
        Chart.defaults.global.defaultFontColor = "#AAAAAA";
        Chart.defaults.global.tooltips.cornerRadius = 0;
    }

    public ngAfterViewInit() {

        this.dataService.getClusterCapacity().subscribe(
            result => {
                if (!result) {
                    return;
                }

                this.clusterCapacities = result.map(x =>
                    new ClusterCapacityViewModel(
                        x.bufferedCapacity,
                        x.capacity,
                        x.load,
                        x.remainingBufferedCapacity,
                        x.remainingCapacity,
                        x.isClusterCapacityViolation,
                        x.name,
                        x.bufferPercentage,
                        true
                    ));

                for (let i = 0; i < this.dataStreams.length; ++i) {
                    let item = this.dataStreams[i];

                    if (!this.clusterCapacities.find(x => x.name == item.name)) {
                        //TODO: dispose of the stream here
                        this.dataStreams.splice(i, 1);
                    }
                }

                this.clusterCapacities.forEach(x => {

                    if (!this.dataStreams.find(item => item.name == x.name)) {
                        console.log("subscribing to " + x.name);

                        let newStream = new DataStream(x.name, this.dataService.getClusterCapacityHistory(x.name));

                        this.dataStreams.push(newStream);
                        this.clusterCapacityStream.next(newStream);
                    }
                });
            },
            error => console.log("error from observable: " + error));
    }
}