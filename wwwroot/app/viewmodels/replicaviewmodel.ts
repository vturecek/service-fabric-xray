import {LoadMetric} from './../models/loadmetric';
import {DeployedEntityViewModel} from './deployedentityviewmodel';

export class ReplicaViewModel extends DeployedEntityViewModel<ReplicaViewModel> {

    public constructor(
        public id: number,
        public partitionId: string,
        public status: string,
        public healthState: string,
        public role: string,
        public metrics: LoadMetric[]) {
        super(status, healthState, partitionId + id);
    }
    
    public copyFrom(other: ReplicaViewModel) {
        this.id = other.id;
        this.partitionId = other.partitionId;
        this.status = other.status;
        this.healthState = other.healthState;
        this.role = other.role;
        this.metrics = other.metrics;
    }
}