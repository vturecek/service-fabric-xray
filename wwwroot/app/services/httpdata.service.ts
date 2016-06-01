
import {DataService} from './data.service';
import {Injectable} from 'angular2/core';
import {Http, Response} from 'angular2/http';
import {Observable}     from 'rxjs/Observable';
import {ClusterCapacity} from './../models/clustercapacity';
import {ClusterNode} from './../models/clusternode';
import {ClusterNodeCapacity} from './../models/clusternodecapacity';
import {Replica} from './../models/replica';
import {DeployedApplication} from './../models/deployedapplication';
import {DeployedService} from './../models/deployedservice';
import {ClusterCapacityHistory} from './../models/clustercapacityhistory';
import {ClusterInfo} from './../models/clusterinfo';

@Injectable()
export class HttpDataService extends DataService {

    public constructor(
        private http: Http)
    { super(); }

    private refreshInterval: number = 6;

    private apiUrl:string = "api/";

    public getApplicationModels(nodeName: string, appTypeFilter: () => string[]): Observable<DeployedApplication[]> {

        return Observable
            .interval(this.refreshInterval * 1000)
            .startWith(-1)
            .flatMap(() => {
                let filterArray: string[] = appTypeFilter();
                let filterString: string = filterArray
                    ? filterArray.join(",")
                    : "";

                return this.http.get(this.apiUrl + 'application/' + nodeName + '/' + filterString).catch(this.handleError);
            })
            .map(this.extractData)
            .catch(this.handleError)
    }


    public getClusterInfo(): Observable<ClusterInfo> {
        return Observable
            .interval(this.refreshInterval * 1000)
            .startWith(-1)
            .flatMap(() => this.http.get(this.apiUrl + 'cluster/info'))
            .map(this.extractData)
            .catch(this.handleError);
    }
    
    public getClusterCapacity(): Observable<ClusterCapacity[]> {
        return Observable
            .interval(this.refreshInterval * 1000)
            .startWith(-1)
            .flatMap(() => this.http.get(this.apiUrl + 'cluster/capacity').catch(this.handleError))
            .map(this.extractData)
            .catch(this.handleError);
    }

    public getClusterCapacityHistory(capacityName: string, startDate?: Date): Observable<ClusterCapacityHistory[]> {
        let start = startDate ?
            startDate :
            new Date(Date.now() - 3600000);

        return Observable
            .interval(this.refreshInterval * 1000)
            .startWith(-1)
            .flatMap(() => {
                let result = this.http.get(this.apiUrl + 'cluster/history/' + capacityName + '/' + start.toISOString()).catch(this.handleError);
                start = new Date(Date.now());
                return result;
            })
            .map(this.extractData)
            .catch(this.handleError);
    }

    public getNodes(nodeTypeFilter: () => string[]): Observable<ClusterNode[]> {
        return Observable
            .interval(this.refreshInterval * 1000)
            .startWith(-1)
            .flatMap(() => {
                let nodeTypeArray: string[] = nodeTypeFilter();
                let nodeTypeString: string = nodeTypeArray
                    ? nodeTypeArray.join(",")
                    : "";

                return this.http.get(this.apiUrl + 'node/info/' + nodeTypeString).catch(this.handleError);
            })
            .map(this.extractData)
            .catch(this.handleError);
    }

    public getNodeCapacity(nodeName: string): Observable<ClusterNodeCapacity[]> {
        return Observable
            .interval(this.refreshInterval * 1000)
            .startWith(-1)
            .flatMap(() => {
                return this.http.get(this.apiUrl + 'node/capacity/' + nodeName).catch(this.handleError);
            })
            .map(this.extractData)
            .catch(this.handleError);
    }


    private extractData(res: Response) {
        if (res.status < 200 || res.status >= 400) {
            return null;
        }
        let body = res.json();
        return body;
    }

    private handleError(error: any) {
        // In a real world app, we might send the error to remote logging infrastructure
        let errMsg = error.message || 'Server error';
        console.log(errMsg); // log to console instead
        return Observable.empty();
    }
}
