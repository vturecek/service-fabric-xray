import { ClusterCapacityPoint } from './../models/clustercapacitypoint';
import { ClusterCapacity } from './../models/clustercapacity';
import { ViewModel } from './viewmodel';

export class ClusterCapacityHistoryViewModel extends ViewModel<ClusterCapacityHistoryViewModel> {

    public constructor(
        public name: string,
        public selected: boolean,
        public current: ClusterCapacity,
        public history: ClusterCapacityPoint[]) {
        super(name);
    }

    public copyFrom(other: ClusterCapacityHistoryViewModel) {
        this.name = other.name;
        this.current = other.current;
        this.history = other.history;

    }
}