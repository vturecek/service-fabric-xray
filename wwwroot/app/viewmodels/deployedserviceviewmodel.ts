import {Service} from './../models/service';
import {LoadMetric} from './../models/loadmetric';
import {DeployedReplicaViewModel} from './deployedreplicaviewmodel';
import {ViewModel} from './viewmodel';
import {List} from './list';

export class DeployedServiceViewModel extends ViewModel<DeployedServiceViewModel> {

    public elementHeight: number;

    public constructor(
        public selected: boolean,
        public selectedMetric: LoadMetric,
        public selectedClass: string,
        public service: Service,
        public replicas: DeployedReplicaViewModel[]) {
        super(service.name);
    }

    public copyFrom(other: DeployedServiceViewModel): void {
        
        this.service = other.service;

        if (!this.replicas) {
            this.replicas = [];
        }
        
        List.updateList(this.replicas, other.replicas);
    }
}