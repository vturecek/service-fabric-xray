export class ClusterInfoViewModel{

    public constructor(
        public healthStatus: string,
        public version: string,
        public nodeTypes: string[],
        public applicationTypes: string[],
        public faultDomains: number[],
        public upgradeDomains: number[],
        public lastBalanceStartTime: string,
        public lastBalanceEndTime: string,
        public nodes: number,
        public applications: number,
        public services: number,
        public partitions: number,
        public replicas: number) {
    }
}
