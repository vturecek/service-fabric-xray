import {DeployedService} from './deployedservice';
import {Application} from './application';

export class DeployedApplication {

    public application: Application;
    public services: DeployedService[];

}