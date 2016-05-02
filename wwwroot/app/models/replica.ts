import {LoadMetric} from './loadmetric';

export class Replica {
    public id: number;
    public partitionId: string;
    public status: string;
    public healthState: string;
    public role: string;
    public metrics: LoadMetric[];
}