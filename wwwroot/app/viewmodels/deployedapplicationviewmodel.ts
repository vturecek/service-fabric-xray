import {DeployedServiceViewModel} from './deployedserviceviewmodel';
import {Application} from './../models/application';
import {LoadMetric} from './../models/loadmetric';
import {ViewModel} from './viewmodel';
import {List} from './list';

export class DeployedApplicationViewModel extends ViewModel<DeployedApplicationViewModel> {

    public elementHeight: number;

    public constructor(
        public selected: boolean,
        public selectedMetric: LoadMetric,
        public selectedClass: string,
        public application: Application,
        public services: DeployedServiceViewModel[]) {
        super(application.name);
    }

    public copyFrom(other: DeployedApplicationViewModel) {

        this.application = other.application;

        if (!this.services) {
            this.services = [];
        }

        List.updateList(this.services, other.services);
    }
    
}