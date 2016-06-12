import {ClusterNodeCapacity} from './clusternodecapacity';

export class ClusterNode{
    public name: string;
    public nodeType: string;
    public status: string;
    public healthState: string;
    public upTime: string;
    public address: string;
    public faultDomain: number;
    public upgradeDomain: number;
}
