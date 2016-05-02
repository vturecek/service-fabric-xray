export class ClusterNodeCapacity {
    public isCapacityViolation: boolean;
    public name: string
    public bufferedCapacity: number;
    public capacity: number;
    public load: number;
    public remainingBufferedCapacity: number;
    public remainingCapacity: number;
}