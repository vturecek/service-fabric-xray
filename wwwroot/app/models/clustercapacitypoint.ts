import { ClusterCapacity } from './clustercapacity';

export class ClusterCapacityPoint {

    public constructor(
        public capacity: ClusterCapacity,
        public timestamp: Date) {
    }
}