import {NodeCapacityViewModel} from './nodecapacityviewmodel';
import {DeployedEntityViewModel} from './deployedentityviewmodel';

export class NodeViewModel extends DeployedEntityViewModel<NodeViewModel>{

    public constructor(public name: string,
        public status: string,
        public healthState: string,
        public faultDomain: number,
        public upgradeDomain: number,
        public capacities: NodeCapacityViewModel[]) {
        super(status, healthState, name);
    }
    
    public copyFrom(other: NodeViewModel) {
        this.name = other.name;
        this.healthState = other.healthState;
        this.status = other.status;
        this.capacities = other.capacities;
        this.faultDomain = other.faultDomain;
        this.upgradeDomain = other.upgradeDomain;
    }
}
