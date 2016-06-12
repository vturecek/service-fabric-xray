import {Replica} from './replica'

export class Partition {
    id: string;
}

export class StatelessPartition extends Partition {
    instanceIds: string[];
}

export class StatefulPartition extends Partition {
    replicaIds: string[];
}

export class Int64StatefulPartition extends StatefulPartition {
    lowKey: number;
    highKey: number;
}