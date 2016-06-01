import {Observable}     from 'rxjs/Observable';
import {Injectable} from 'angular2/core';
import {ClusterCapacity} from './../models/clustercapacity';
import {ClusterNode} from './../models/clusternode';
import {ClusterNodeCapacity} from './../models/clusternodecapacity';
import {DeployedApplication} from './../models/deployedapplication';
import {ClusterCapacityHistory} from './../models/clustercapacityhistory';
import {ClusterInfo} from './../models/clusterinfo';

@Injectable()
export abstract class DataService {

    public abstract getApplicationModels(nodeName: string, appTypeFilter: () => string[]): Observable<DeployedApplication[]>;

    public abstract getClusterInfo(): Observable<ClusterInfo>;

    public abstract getClusterCapacity(): Observable<ClusterCapacity[]>;

    public abstract getClusterCapacityHistory(capacityName: string, startDate?: Date): Observable<ClusterCapacityHistory[]>;

    public abstract getNodes(nodeTypeFilter: () => string[]): Observable<ClusterNode[]>;

    public abstract getNodeCapacity(nodeName: string): Observable<ClusterNodeCapacity[]>;

}
