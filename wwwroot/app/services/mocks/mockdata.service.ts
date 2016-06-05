import {DataService} from './../data.service';
import {Injectable} from 'angular2/core';
import {Observable}     from 'rxjs/Rx';
import {ReplicaList, ServiceList, ClusterNodeList, ClusterNodeCapacityList, ClusterInfoData, ClusterFiltersData, ClusterCapacityList, ApplicationList} from './mock-data';
import {ClusterCapacity} from './../../models/clustercapacity';
import {ClusterNode} from './../../models/clusternode';
import {ClusterNodeCapacity} from './../../models/clusternodecapacity';
import {ClusterInfo} from './../../models/clusterinfo';
import {DeployedApplication} from './../../models/deployedapplication';
import {DeployedService} from './../../models/deployedservice';
import {ClusterCapacityHistory} from './../../models/clustercapacityhistory';
import {ClusterFilters} from './../../models/clusterfilters';

@Injectable()
export class MockDataService extends DataService {

    private refreshInterval: number = 20;
    private history: any;

    public constructor() {
        super();
        this.history = {};

        let times: Date[] = [];
        let now: Date = new Date(Date.now());

        for (var i = 0; i < 50; ++i) {
            times.push(new Date(Date.now() - 60000 * i));
        }

        for (var capacity of ClusterCapacityList) {

            let items: ClusterCapacityHistory[] = [];

            for (var i = 0; i < 50; ++i) {
                items.push(new ClusterCapacityHistory(times[i], Math.random() * 1000));
            }

            this.history[capacity.name] = items;
        }
    }

    public getApplicationModels(nodeName: string, appTypeFilter: () => string[]): Observable<DeployedApplication[]> {
        return Observable
            .interval(this.refreshInterval * 1000)
            .startWith(-1)
            .flatMap(() => {
                let result: DeployedApplication[] = [];

                let filter: string[] = appTypeFilter();

                console.log("getting application types: " + filter.join(','));

                let appList = filter
                    ? ApplicationList.filter(x => filter.indexOf(x.type) < 0)
                    : ApplicationList;

                for (let i = 0; i < appList.length; ++i) {
                    let app: DeployedApplication = new DeployedApplication();
                    app.application = appList[i];
                    app.services = [];

                    for (var item of ServiceList[app.application.name]) {
                        let serviceModel = new DeployedService();
                        serviceModel.service = item;
                        serviceModel.replicas = ReplicaList[serviceModel.service.name];
                        app.services.push(serviceModel);
                    }

                    result.push(app);
                }

                return Observable.of(result);
            });
    }

    public getClusterCapacityHistory(capacityName: string, startDate?: Date): Observable<ClusterCapacityHistory[]> {
        let start = startDate ?
            startDate :
            new Date(Date.now() - 3600000);

        return Observable
            .interval(this.refreshInterval * 1000)
            .startWith(-1)
            .flatMap(() => {
                let o = Observable.of(this.history[capacityName].filter(x => x.timestamp > start));
                start = new Date(Date.now());
                return o
            });
    }

    public getClusterCapacity(): Observable<ClusterCapacity[]> {
        return Observable
            .interval(this.refreshInterval * 1000)
            .startWith(-1)
            .flatMap(() => Observable.of(ClusterCapacityList));
    }

    public getNodes(nodeTypeFilter: () => string[]): Observable<ClusterNode[]> {
        return Observable
            .interval(this.refreshInterval * 1000)
            .startWith(-1)
            .flatMap(() => {
                let nodeTypes: string[] = nodeTypeFilter();
                return nodeTypes
                    ? Observable.of(ClusterNodeList.filter(x => nodeTypes.indexOf(x.nodeType) < 0))
                    : Observable.of(ClusterNodeList);
            });
    }

    public getNodeCapacity(nodeName: string): Observable<ClusterNodeCapacity[]> {
        return Observable
            .interval(this.refreshInterval * 1000)
            .startWith(-1)
            .flatMap(() => Observable.of(ClusterNodeCapacityList[nodeName]));
    }

    public getClusterFilters(): Observable<ClusterFilters> {
        return Observable
            .interval(this.refreshInterval * 1000)
            .startWith(-1)
            .flatMap(() => Observable.of(ClusterFiltersData));
    }

    public getClusterInfo(): Observable<ClusterInfo> {
        return Observable
            .interval(this.refreshInterval * 1000)
            .startWith(-1)
            .flatMap(() => Observable.of(ClusterInfoData));
    }
}