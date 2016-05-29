import {LoadMetric} from './../models/loadmetric';
import {ViewModel} from './viewmodel';

export class ApplicationViewModel extends ViewModel<ApplicationViewModel>
{
    public elementHeight: number;
    
    public constructor(
        public name: string,
        public type: string,
        public version: string,
        public status: string,
        public health: string,
        public selectedMetric: LoadMetric,
        public metrics: LoadMetric[]) {
        super(name);

    }

    public copyFrom(other: ApplicationViewModel) {
        this.name = other.name;
        this.type = other.type;
        this.version = other.version;
        this.status = other.status;
        this.health = other.health;
        this.metrics = other.metrics;
    }
}