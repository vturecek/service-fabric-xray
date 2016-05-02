import {ServiceViewModel} from './serviceviewmodel';
import {ReplicaViewModel} from './replicaviewmodel';
import {ViewModel} from './viewmodel';
import {List} from './list';

export class DeployedServiceViewModel extends ViewModel<DeployedServiceViewModel> {

    public constructor(
        public selected: boolean,
        public service: ServiceViewModel,
        public replicas: ReplicaViewModel[]) {
        super(service.name);
    }

    public copyFrom(other: DeployedServiceViewModel) {
        
        this.service.copyFrom(other.service);

        if (!this.replicas) {
            this.replicas = [];
        }
        
        List.updateList(this.replicas, other.replicas);
    }
}