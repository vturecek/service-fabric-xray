import { ClusterCapacityPoint } from './clustercapacitypoint';
import { ClusterCapacity} from './clustercapacity';

export class ClusterCapacityHistory {

    public constructor(
        public name: string,
        public current: ClusterCapacity,
        public history: ClusterCapacityPoint[]) {
    }
}