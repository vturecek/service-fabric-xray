import { ClusterCapacity } from './clustercapacity';

export class ClusterCapacityDataPoint {

    public constructor(
        public capacity: ClusterCapacity,
        public timestamp: Date) {
    }
}