export class ClusterCapacity{
    public bufferedCapacity: number;
    public capacity: number;
    public load: number;
    public remainingBufferedCapacity: number;
    public remainingCapacity: number
    public isClusterCapacityViolation: boolean;
    public name: string;
    public bufferPercentage: number;
    public balancedBefore: boolean;
    public balancedAfter: boolean;
    public deviationBefore: number;
    public deviationAfter: number;
    public balancingThreshold: number;
    public maxLoadedNode: string;
    public minLoadedNode: string;
}