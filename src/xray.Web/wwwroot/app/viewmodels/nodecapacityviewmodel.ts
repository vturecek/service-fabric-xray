import {ViewModel} from './viewmodel';

export class NodeCapacityViewModel extends ViewModel<NodeCapacityViewModel> {

    public constructor(
        public isCapacityViolation: boolean,
        public name: string,
        public bufferedCapacity: number,
        public capacity: number,
        public load: number,
        public remainingBufferedCapacity: number,
        public remainingCapacity: number) {
        super(name);
    }
    
    public copyFrom(other: NodeCapacityViewModel) {
        this.name = other.name;
        this.bufferedCapacity = other.bufferedCapacity;
        this.capacity = other.capacity;
        this.isCapacityViolation = other.isCapacityViolation;
        this.load = other.load;
        this.remainingBufferedCapacity = other.remainingBufferedCapacity;
        this.remainingCapacity = other.remainingCapacity;
    }
}