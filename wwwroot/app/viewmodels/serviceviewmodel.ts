import {LoadMetric} from './../models/loadmetric';
import {ViewModel} from './viewmodel';

export class ServiceViewModel extends ViewModel<ServiceViewModel> {

    public constructor(
        public name: string,
        public type: string,
        public version: string,
        public status: string,
        public health: string,
        public metrics: LoadMetric[]) {
        super(name);
    }
    
    public copyFrom(other: ServiceViewModel) {
        this.name = other.name;
        this.type = other.type;
        this.version = other.version;
        this.status = other.status;
        this.health = other.health;
        this.metrics = other.metrics;
    }
}
