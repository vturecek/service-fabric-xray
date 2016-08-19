import {DeployedServiceViewModel} from './deployedserviceviewmodel';
import {Application} from './../models/application';
import {ViewModel} from './viewmodel';
import {List} from './list';

export class DeployedApplicationViewModel extends ViewModel<DeployedApplicationViewModel> {

    public elementHeight: number;
    public shortName: string;

    public constructor(
        public expanded: boolean,
        public selectedMetric: number,
        public selectedClass: string,
        public application: Application,
        public services: DeployedServiceViewModel[]) {
        super(application.name);
        this.shortName = application.name.replace("fabric:/", "");
    }

    public copyFrom(other: DeployedApplicationViewModel) {

        this.application = other.application;
        this.selectedMetric = other.selectedMetric;
        this.selectedClass = other.selectedClass;

        if (!this.services) {
            this.services = [];
        }

        List.updateList(this.services, other.services);
    }
    
}