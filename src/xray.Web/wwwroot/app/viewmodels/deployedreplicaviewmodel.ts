import {Replica} from './../models/replica';
import {ViewModel} from './viewmodel';

export class DeployedReplicaViewModel extends ViewModel<DeployedReplicaViewModel> {

    public elementHeight: number;

    public constructor(
        public highlighted: boolean,
        public selectedMetric: number,
        public selectedClass: string,
        public roleClass: string,
        public replica: Replica) {
        super(replica.partitionId + replica.id);
    }

    public copyFrom(other: DeployedReplicaViewModel): void {
        this.replica = other.replica;
        this.roleClass = other.roleClass;
        this.selectedMetric = other.selectedMetric;
        this.selectedClass = other.selectedClass;
    }
}