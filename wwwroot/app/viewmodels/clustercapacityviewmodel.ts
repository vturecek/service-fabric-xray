import {ViewModel} from './viewmodel';

export class ClusterCapacityViewModel extends ViewModel<ClusterCapacityViewModel>{

    public constructor(
        public bufferedCapacity: number,
        public capacity: number,
        public load: number,
        public remainingBufferedCapacity: number,
        public remainingCapacity: number,
        public isClusterCapacityViolation: boolean,
        public name: string,
        public bufferPercentage: number) {

        super(name);
    }

    public copyFrom(other: ClusterCapacityViewModel) {
        this.bufferedCapacity = other.bufferedCapacity;
        this.bufferPercentage = other.bufferPercentage;
        this.capacity = other.capacity;
        this.isClusterCapacityViolation = other.isClusterCapacityViolation;
        this.load = other.load;
        this.remainingBufferedCapacity = other.remainingBufferedCapacity;
        this.remainingCapacity = other.remainingCapacity;
    }
}