import { ClusterCapacityDataPoint } from './clustercapacitydatapoint';

export class ClusterCapacityHistory {

    public constructor(
        public capacityName: string,
        public capacityData: ClusterCapacityDataPoint[]) {
    }
}