import {LoadMetric} from './../models/loadmetric';
import {ViewModel} from './viewmodel';

export class ReplicaViewModel extends ViewModel<ReplicaViewModel> {

    public constructor(
        public id: number,
        public partitionId: string,
        public status: string,
        public health: string,
        public role: string,
        public metrics: LoadMetric[]) {
        super(partitionId + id);
    }
    
    public copyFrom(other: ReplicaViewModel) {
        this.id = other.id;
        this.partitionId = other.partitionId;
        this.status = other.status;
        this.health = other.health;
        this.role = other.role;
        this.metrics = other.metrics;
    }
}