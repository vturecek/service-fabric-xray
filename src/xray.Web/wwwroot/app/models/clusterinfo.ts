export class ClusterInfo{
    public healthStatus: string;
    public version: string;
    public nodeTypes: string[];
    public applicationTypes: string[];
    public faultDomains: number[];
    public upgradeDomains: number[];
    public lastBalanceStartTime: Date;
    public lastBalanceEndTime: Date;
    public nodes: number;
    public applications: number;
    public services: number;
    public partitions: number;
    public replicas: number;
}
