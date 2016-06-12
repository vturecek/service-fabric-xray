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
        public bufferPercentage: number,
        public balancedBefore: boolean,
        public balancedAfter: boolean,
        public deviationBefore: number,
        public deviationAfter: number,
        public balancingThreshold: number,
        public maxLoadedNode: string,
        public minLoadedNode: string,
        public selected: boolean) {

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
        this.balancedAfter = other.balancedAfter;
        this.balancedBefore = other.balancedBefore;
        this.deviationAfter = other.deviationAfter;
        this.deviationBefore = other.deviationBefore;
        this.balancingThreshold = other.balancingThreshold;
        this.maxLoadedNode = other.maxLoadedNode;
        this.minLoadedNode = other.minLoadedNode;
    }
}