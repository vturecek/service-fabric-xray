import {NodeCapacityViewModel} from './nodecapacityviewmodel';
import {ViewModel} from './viewmodel';

export class NodeViewModel extends ViewModel<NodeViewModel>{

    public constructor(
        public name: string,
        public nodeType: string,
        public status: string,
        public health: string,
        public faultDomain: number,
        public upgradeDomain: number,
        public selected: boolean,
        public capacities: NodeCapacityViewModel[]) {
        super(name);
    }
    
    public copyFrom(other: NodeViewModel) {
        this.name = other.name;
        this.nodeType = other.nodeType;
        this.health = other.health;
        this.status = other.status;
        this.capacities = other.capacities;
        this.faultDomain = other.faultDomain;
        this.upgradeDomain = other.upgradeDomain;
    }
}
