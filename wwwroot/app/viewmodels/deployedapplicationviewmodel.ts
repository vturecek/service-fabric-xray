import {DeployedServiceViewModel} from './deployedserviceviewmodel';
import {ApplicationViewModel} from './applicationviewmodel';
import {ViewModel} from './viewmodel';
import {List} from './list';

export class DeployedApplicationViewModel extends ViewModel<DeployedApplicationViewModel> {

    public constructor(
        public selected: boolean,
        public application: ApplicationViewModel,
        public services: DeployedServiceViewModel[]) {
        super(application.name);
    }

    public copyFrom(other: DeployedApplicationViewModel) {
        
        this.application.copyFrom(other.application);

        if (!this.services) {
            this.services = [];
        }

        List.updateList(this.services, other.services);
    }
}