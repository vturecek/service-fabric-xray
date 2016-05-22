import {Component, AfterViewInit, OnDestroy, Inject} from 'angular2/core';
import {Router} from 'angular2/router';
import {Observable, Subscription, BehaviorSubject, ReplaySubject} from "rxjs/Rx";
import {ClusterCapacityGraph, DataStream} from './clustercapacitygraph.component';
import {ClusterCapacityDonut} from './clustercapacitydonut.component';
import {DataService} from './../services/data.service';
import {ClusterCapacityViewModel} from './../viewmodels/clustercapacityviewmodel';
import {ClusterCapacityHistory} from './../models/clustercapacityhistory';
import {List} from './../viewmodels/list';

declare var Chart: any;

@Component({
    selector: 'dashboard-component',
    templateUrl: 'app/components/dashboard.component.html',
    styleUrls: ['app/components/dashboard.component.css'],
    directives: [ClusterCapacityGraph, ClusterCapacityDonut]
})
export class DashboardComponent implements AfterViewInit, OnDestroy {

    private clusterCapacities: ClusterCapacityViewModel[] = [];

    private clusterCapacityStream: BehaviorSubject<DataStream>;

    private dataStreams: DataStreamSubscription[] = [];

    private clusterSubscription: Subscription;

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

        this.clusterSubscription = this.dataService.getClusterCapacity().subscribe(
            result => {
                if (!result) {
                    return;
                }

                List.updateList(this.clusterCapacities, result.map(x =>
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
                    )));

                for (let i = 0; i < this.dataStreams.length; ++i) {
                    let item = this.dataStreams[i];

                    if (!this.clusterCapacities.find(x => x.name == item.name)) {
                        this.removeDataStream(item.name);
                    }
                }

                this.clusterCapacities.forEach(x => {
                    if (x.selected) {
                        this.createDataStream(x.name);
                    }
                });
            },
            error => console.log("error from observable: " + error));
    }

    public ngOnDestroy() {
        if (this.clusterSubscription) {
            this.clusterSubscription.unsubscribe();
        }

        for (let i = 0; i < this.dataStreams.length; ++i) {
            let item = this.dataStreams[i];
            
            this.removeDataStream(item.name);
        }
    }

    private createDataStream(name: string) {
        if (!this.dataStreams.find(item => item.name == name)) {
            console.log("subscribing to " + name);

            let replay = new ReplaySubject<ClusterCapacityHistory[]>();

            let subscription = this.dataService.getClusterCapacityHistory(name).subscribe(
                next => replay.next(next),
                error => replay.error(error));

            this.dataStreams.push(new DataStreamSubscription(name, subscription, replay));
            this.clusterCapacityStream.next(new DataStream(name, replay));
        }
    }

    private removeDataStream(name:string) {
        let ix: number = this.dataStreams.findIndex(x => x.name == name);
        if (ix >= 0) {
            let dataStream = this.dataStreams[ix];
            dataStream.subject.complete();
            dataStream.subject.unsubscribe();
            dataStream.subscription.unsubscribe();
            this.dataStreams.splice(ix, 1);
        }
    }

    private onSelectCapacity(name: string, event) {
        let isChecked: boolean = event.currentTarget.checked;

        if (isChecked) {
            this.createDataStream(name);
        }
        else {
            this.removeDataStream(name);
        }
    }

    private isCapacityWarning(item: ClusterCapacityViewModel): boolean {
        return item.load / item.capacity > 0.9;
    }
}

class DataStreamSubscription {

    public constructor(
        public name: string,
        public subscription: Subscription,
        public subject: ReplaySubject<ClusterCapacityHistory[]>)
    { }
}