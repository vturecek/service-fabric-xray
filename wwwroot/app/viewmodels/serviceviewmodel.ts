import {LoadMetric} from './../models/loadmetric';
import {DeployedEntityViewModel} from './deployedentityviewmodel';

export class ServiceViewModel extends DeployedEntityViewModel<ServiceViewModel> {

    public constructor(
        public name: string,
        public type: string,
        public version: string,
        public status: string,
        public healthState: string,
        public metrics: LoadMetric[]) {
        super(status, healthState, name);
    }
    
    public copyFrom(other: ServiceViewModel) {
        this.name = other.name;
        this.type = other.type;
        this.version = other.version;
        this.status = other.status;
        this.healthState = other.healthState;
        this.metrics = other.metrics;
    }
}
