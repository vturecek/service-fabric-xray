import {LoadMetric} from './loadmetric';

export class Service {
    public name: string;
    public type: string;
    public version: string;
    public status: string;
    public healthState: string;
    public metrics: LoadMetric[];
}
