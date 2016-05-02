import {Service} from './../../models/service';
import {Application} from './../../models/application';
import {ClusterCapacity} from './../../models/clustercapacity';
import {ClusterNode} from './../../models/clusternode';
import {Replica} from './../../models/replica';

export var ClusterCapacityList: ClusterCapacity[] = [
    { 'name': 'MemoryKB', 'capacity': 24800, 'bufferedCapacity': 24800, 'load': 20000, 'remainingBufferedCapacity': 4800, 'remainingCapacity': 4800, 'isClusterCapacityViolation': false, 'bufferPercentage': 20 },
    { 'name': 'DiskKB', 'capacity': 49600, 'bufferedCapacity': 49600, 'load': 30000, 'remainingBufferedCapacity': 19600, 'remainingCapacity': 19600, 'isClusterCapacityViolation': false, 'bufferPercentage': 20 },
    { 'name': 'Default Replica Count', 'capacity': 500, 'bufferedCapacity': 49600, 'load': 30000, 'remainingBufferedCapacity': 19600, 'remainingCapacity': 19600, 'isClusterCapacityViolation': false, 'bufferPercentage': 20 }
];

export var ClusterNodeList: ClusterNode[] = [
    {
        'name': 'node1', 'capacities': [
            { 'name': 'MemoryKB', 'capacity': 5000, 'isCapacityViolation': false, 'bufferedCapacity': 0, 'load': 0, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0 },
            { 'name': 'DiskKB', 'capacity': 10000, 'isCapacityViolation': false, 'bufferedCapacity': 0, 'load': 0, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0 },
            { 'name': 'Default Replica Count', 'capacity': 500, 'isCapacityViolation': false, 'bufferedCapacity': 0, 'load': 0, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0 }
        ], 'faultDomain': 1, 'upgradeDomain': 1, 'status': 'Up', 'healthState': 'Ok'
    },
    {
        'name': 'node2', 'capacities': [
            { 'name': 'MemoryKB', 'capacity': 5400, 'isCapacityViolation': false, 'bufferedCapacity': 0, 'load': 0, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0 },
            { 'name': 'DiskKB', 'capacity': 10800, 'isCapacityViolation': false, 'bufferedCapacity': 0, 'load': 0, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0 },
            { 'name': 'Default Replica Count', 'capacity': 500, 'isCapacityViolation': false, 'bufferedCapacity': 0, 'load': 0, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0 }
        ], 'faultDomain': 2, 'upgradeDomain': 2, 'status': 'Down', 'healthState': 'Warning'
    }, 
    {
        'name': 'node3', 'capacities': [
            { 'name': 'MemoryKB', 'capacity': 5900, 'isCapacityViolation': false, 'bufferedCapacity': 0, 'load': 0, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0 },
            { 'name': 'DiskKB', 'capacity': 11800, 'isCapacityViolation': false, 'bufferedCapacity': 0, 'load': 0, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0 },
            { 'name': 'Default Replica Count', 'capacity': 500, 'isCapacityViolation': false, 'bufferedCapacity': 0, 'load': 0, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0 }
        ], 'faultDomain': 3, 'upgradeDomain': 3, 'status': 'Enabling', 'healthState': 'Error'
    },
    { 
        'name': 'node4', 'capacities': [
            { 'name': 'MemoryKB', 'capacity': 4000, 'isCapacityViolation': false, 'bufferedCapacity': 0, 'load': 0, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0 },
            { 'name': 'DiskKB', 'capacity': 8000, 'isCapacityViolation': false, 'bufferedCapacity': 0, 'load': 0, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0 },
            { 'name': 'Default Replica Count', 'capacity': 500, 'isCapacityViolation': false, 'bufferedCapacity': 0, 'load': 0, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0 }
        ], 'faultDomain': 4, 'upgradeDomain': 4, 'status': 'Disabled', 'healthState': 'Unknown'
    },
    {
        'name': 'node5', 'capacities': [
            { 'name': 'MemoryKB', 'capacity': 4500, 'isCapacityViolation': false, 'bufferedCapacity': 0, 'load': 0, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0 },
            { 'name': 'DiskKB', 'capacity': 9000, 'isCapacityViolation': false, 'bufferedCapacity': 0, 'load': 0, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0 },
            { 'name': 'Default Replica Count', 'capacity': 500, 'isCapacityViolation': false, 'bufferedCapacity': 0, 'load': 0, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0 }
        ], 'faultDomain': 5, 'upgradeDomain': 5, 'status': 'Removed', 'healthState': 'Invalid'
    }
];

export var ApplicationList: Application[] = [
    {
        'name': 'fabric:/App1', 'type': 'App1Type', 'version': '1.0', 'status': 'Upgrading', 'healthState': 'OK', 'metrics': [{ 'name': 'Default Replica Count', 'value': 120 }, { 'name': 'MemoryKB', 'value': 2000 }, { 'name': 'DiskKB', 'value': 3000 }]
    },
    {
        'name': 'fabric:/App2', 'type': 'App2Type', 'version': '2.0.0.0', 'status': 'Deleting', 'healthState': 'Warning', 'metrics': [{ 'name': 'Default Replica Count', 'value': 120 }, { 'name': 'MemoryKB', 'value': 2000 }, { 'name': 'DiskKB', 'value': 3000 }]
    }
];

export var ServiceList =
    {
        'fabric:/App1':
        [
            { 'name': 'fabric:/App1/Service11', 'type': 'serviceType', 'version': '1.0', 'status': 'Upgrading', 'healthState': 'OK', 'metrics': [{ 'name': 'Default Replica Count', 'value': 60 }, { 'name': 'MemoryKB', 'value': 1400 }, { 'name': 'DiskKB', 'value': 1400 }] },
            { 'name': 'fabric:/App1/Service12', 'type': 'serviceType', 'version': '1.0', 'status': 'Active', 'healthState': 'Warning', 'metrics': [{ 'name': 'Default Replica Count', 'value': 60 }, { 'name': 'MemoryKB', 'value': 1000 }, { 'name': 'DiskKB', 'value': 2000 }] },
        ],
        'fabric:/App2':
        [
            { 'name': 'fabric:/App2/Service13', 'type': 'serviceType', 'version': '1.0', 'status': 'Unknown', 'healthState': 'Error', 'metrics': [{ 'name': 'Default Replica Count', 'value': 60 }, { 'name': 'MemoryKB', 'value': 1000 }, { 'name': 'DiskKB', 'value': 2000 }] },
            { 'name': 'fabric:/App2/Service14', 'type': 'serviceType', 'version': '1.0', 'status': 'Deleting', 'healthState': 'Unknown', 'metrics': [{ 'name': 'Default Replica Count', 'value': 60 }, { 'name': 'MemoryKB', 'value': 600 }, { 'name': 'DiskKB', 'value': 600 }] }
        ]
    };

export var ReplicaList =
    {
        'fabric:/App1/Service11':
        [
            { 'id': 1, 'metrics': [{ 'name': 'Default Replica Count', 'value': 20 }, { 'name': 'MemoryKB', 'value': 700 }, { 'name': 'DiskKB', 'value': 100 }], 'healthState': 'OK', 'status': 'Ready', 'role': 'Primary' },
            { 'id': 2, 'metrics': [{ 'name': 'Default Replica Count', 'value': 20 }, { 'name': 'MemoryKB', 'value': 500 }, { 'name': 'DiskKB', 'value': 300 }], 'healthState': 'Warning', 'status': 'Ready', 'role': 'ActiveSecondary' },
            { 'id': 3, 'metrics': [{ 'name': 'Default Replica Count', 'value': 20 }, { 'name': 'MemoryKB', 'value': 200 }, { 'name': 'DiskKB', 'value': 1000 }], 'healthState': 'Error', 'status': 'Ready', 'role': 'IdleSecondary' },
        ],
        'fabric:/App1/Service12':
        [
            { 'id': 4, 'metrics': [{ 'name': 'Default Replica Count', 'value': 20 }, { 'name': 'MemoryKB', 'value': 400 }, { 'name': 'DiskKB', 'value': 800 }], 'healthState': 'OK', 'status': 'Ready', 'role': 'Primary' },
            { 'id': 5, 'metrics': [{ 'name': 'Default Replica Count', 'value': 20 }, { 'name': 'MemoryKB', 'value': 300 }, { 'name': 'DiskKB', 'value': 600 }], 'healthState': 'Invalid', 'status': 'Standby', 'role': 'ActiveSecondary' },
            { 'id': 6, 'metrics': [{ 'name': 'Default Replica Count', 'value': 20 }, { 'name': 'MemoryKB', 'value': 300 }, { 'name': 'DiskKB', 'value': 600 }], 'healthState': 'Unknown', 'status': 'Standby', 'role': 'IdleSecondary' },
        ],
        'fabric:/App2/Service13':
        [
            { 'id': 7, 'metrics': [{ 'name': 'Default Replica Count', 'value': 20 }, { 'name': 'MemoryKB', 'value': 500 }, { 'name': 'DiskKB', 'value': 1000 }], 'healthState': 'OK', 'status': 'Invalid', 'role': 'Invalid' },
            { 'id': 8, 'metrics': [{ 'name': 'Default Replica Count', 'value': 20 }, { 'name': 'MemoryKB', 'value': 0 }, { 'name': 'DiskKB', 'value': 0 }], 'healthState': 'Warning', 'status': 'InBuild', 'role': 'Unknown' },
            { 'id': 9, 'metrics': [{ 'name': 'Default Replica Count', 'value': 20 }, { 'name': 'MemoryKB', 'value': 500 }, { 'name': 'DiskKB', 'value': 1000 }], 'healthState': 'Error', 'status': 'Dropped', 'role': 'None' },
        ],
        'fabric:/App2/Service14':
        [
            { 'id': 10, 'metrics': [{ 'name': 'Default Replica Count', 'value': 20 }, { 'name': 'MemoryKB', 'value': 300 }, { 'name': 'DiskKB', 'value': 300 }], 'healthState': 'OK', 'status': 'Down', 'role': 'Primary' },
            { 'id': 12, 'metrics': [{ 'name': 'Default Replica Count', 'value': 20 }, { 'name': 'MemoryKB', 'value': 0 }, { 'name': 'DiskKB', 'value': 200 }], 'healthState': 'Invalid', 'status': 'Down', 'role': 'ActiveSecondary' },
            { 'id': 11, 'metrics': [{ 'name': 'Default Replica Count', 'value': 20 }, { 'name': 'MemoryKB', 'value': 300 }, { 'name': 'DiskKB', 'value': 100 }], 'healthState': 'Unknown', 'status': 'Down', 'role': 'IdleSecondary' }
        ]
    };
