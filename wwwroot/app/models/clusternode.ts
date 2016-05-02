import {ClusterNodeCapacity} from './clusternodecapacity';

export class ClusterNode{
    public name: string;
    public status: string;
    public healthState: string;
    public faultDomain: number;
    public upgradeDomain: number;
    public capacities: ClusterNodeCapacity[];
}
