import {NodeCapacityViewModel} from './nodecapacityviewmodel';
import {ViewModel} from './viewmodel';
import {List} from './list';

export class NodeViewModel extends ViewModel<NodeViewModel>{

    public constructor(
        public name: string,
        public nodeType: string,
        public status: string,
        public health: string,
        public upTime: string,
        public address: string,
        public faultDomain: number,
        public upgradeDomain: number,
        public applicationsExpanded: boolean,
        public servicesExpanded: boolean) {
        super(name);
    }
    
    public copyFrom(other: NodeViewModel) {
        this.name = other.name;
        this.nodeType = other.nodeType;
        this.health = other.health;
        this.status = other.status;
        this.upTime = other.upTime;
        this.address = other.address;
        this.faultDomain = other.faultDomain;
        this.upgradeDomain = other.upgradeDomain;
    }
}
