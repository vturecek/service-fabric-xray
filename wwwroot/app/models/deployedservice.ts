import {Service} from './service';
import {Replica} from './replica';

export class DeployedService {
    public service: Service;
    public replicas: Replica[];
}