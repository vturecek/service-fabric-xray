import {Service} from './../../models/service';
import {Application} from './../../models/application';
import {ClusterCapacity} from './../../models/clustercapacity';
import {ClusterNode} from './../../models/clusternode';
import {ClusterInfo} from './../../models/clusterinfo';
import {Replica} from './../../models/replica';

export var ClusterCapacityList: ClusterCapacity[] = [
    { 'name': 'MemoryKB', 'capacity': 20000, 'bufferedCapacity': 20000, 'load': 10000, 'remainingBufferedCapacity': 4800, 'remainingCapacity': 10000, 'isClusterCapacityViolation': false, 'bufferPercentage': 20 },
    { 'name': 'DiskKB', 'capacity': 50000, 'bufferedCapacity': 50000, 'load': 20000, 'remainingBufferedCapacity': 19600, 'remainingCapacity': 30000, 'isClusterCapacityViolation': false, 'bufferPercentage': 20 },
    { 'name': 'Count', 'capacity': -1, 'bufferedCapacity': -1, 'load': 3, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0, 'isClusterCapacityViolation': false, 'bufferPercentage': 20 },
    { 'name': 'WTFBBQ', 'capacity': 24800, 'bufferedCapacity': 24800, 'load': 24000, 'remainingBufferedCapacity': 4800, 'remainingCapacity': 800, 'isClusterCapacityViolation': true, 'bufferPercentage': 20 },
    { 'name': 'Precise fit', 'capacity': 5000, 'bufferedCapacity': 5000, 'load': 5000, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0, 'isClusterCapacityViolation': false, 'bufferPercentage': 0 }
];

export var ClusterInfoData: ClusterInfo = {
    'nodeTypes': ['FrontEnd', 'ProcessingNode', 'DataNode', 'StatisticsNode'],
    'applicationTypes': ['App1Type', 'App2Type'],
    'faultDomains': [1, 2, 3, 4, 5],
    'upgradeDomains': [1, 2, 3, 4, 5]
};

export var ClusterNodeList: ClusterNode[] = [
    {
        'name': 'node1', 'nodeType': 'FrontEnd', 'capacities': [
            { 'name': 'MemoryKB', 'capacity': 5000, 'isCapacityViolation': false, 'bufferedCapacity': 0, 'load': 2000, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0 },
            { 'name': 'DiskKB', 'capacity': 10000, 'isCapacityViolation': false, 'bufferedCapacity': 0, 'load': 10000, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0 },
            { 'name': 'Count', 'capacity': -1, 'isCapacityViolation': false, 'bufferedCapacity': 0, 'load': 0, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0 },
            { 'name': 'WTFBBQ', 'capacity': 5000, 'isCapacityViolation': false, 'bufferedCapacity': 0, 'load': 0, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0 },
            { 'name': 'Precise fit', 'capacity': 1000, 'isCapacityViolation': false, 'bufferedCapacity': 0, 'load': 1000, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0 }
        ], 'faultDomain': 1, 'upgradeDomain': 1, 'status': 'Up', 'healthState': 'Ok', 'upTime' : '00:12:22', 'address': '10.0.0.1'
    },
    {
        'name': 'node2', 'nodeType': 'FrontEnd', 'capacities': [
            { 'name': 'MemoryKB', 'capacity': 5000, 'isCapacityViolation': false, 'bufferedCapacity': 0, 'load': 4000, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0 },
            { 'name': 'DiskKB', 'capacity': 10000, 'isCapacityViolation': true, 'bufferedCapacity': 0, 'load': 2000, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0 },
            { 'name': 'Count', 'capacity': -1, 'isCapacityViolation': false, 'bufferedCapacity': 0, 'load': 0, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0 },
            { 'name': 'Precise fit', 'capacity': 1000, 'isCapacityViolation': false, 'bufferedCapacity': 0, 'load': 1000, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0 }
        ], 'faultDomain': 2, 'upgradeDomain': 2, 'status': 'Down', 'healthState': 'Warning', 'upTime': '00:12:22', 'address': '10.0.0.2'
    }, 
    {
        'name': 'node3', 'nodeType': 'ProcessingNode', 'capacities': [
            { 'name': 'MemoryKB', 'capacity': 3000, 'isCapacityViolation': false, 'bufferedCapacity': 0, 'load': 1000, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0 },
            { 'name': 'DiskKB', 'capacity': 10000, 'isCapacityViolation': false, 'bufferedCapacity': 0, 'load': 2000, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0 },
            { 'name': 'Count', 'capacity': -1, 'isCapacityViolation': false, 'bufferedCapacity': 0, 'load': 0, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0 },
            { 'name': 'Precise fit', 'capacity': 1000, 'isCapacityViolation': false, 'bufferedCapacity': 0, 'load': 1000, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0 }
        ], 'faultDomain': 3, 'upgradeDomain': 3, 'status': 'Enabling', 'healthState': 'Error', 'upTime': '00:12:22', 'address': '10.0.0.3'
    },
    { 
        'name': 'node4', 'nodeType': 'DataNode', 'capacities': [
            { 'name': 'MemoryKB', 'capacity': 4000, 'isCapacityViolation': false, 'bufferedCapacity': 0, 'load': 2000, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0 },
            { 'name': 'DiskKB', 'capacity': 10000, 'isCapacityViolation': false, 'bufferedCapacity': 0, 'load': 1000, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0 },
            { 'name': 'Count', 'capacity': -1, 'isCapacityViolation': false, 'bufferedCapacity': 0, 'load': 0, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0 },
            { 'name': 'Precise fit', 'capacity': 1000, 'isCapacityViolation': false, 'bufferedCapacity': 0, 'load': 1000, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0 }
        ], 'faultDomain': 4, 'upgradeDomain': 4, 'status': 'Disabled', 'healthState': 'Unknown', 'upTime': '00:12:22', 'address': '10.0.0.4'
    },
    {
        'name': 'node5', 'nodeType': 'StatisticsNode', 'capacities': [
            { 'name': 'MemoryKB', 'capacity': 8000, 'isCapacityViolation': false, 'bufferedCapacity': 0, 'load': 1000, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0 },
            { 'name': 'DiskKB', 'capacity': 10000, 'isCapacityViolation': false, 'bufferedCapacity': 0, 'load': 5000, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0 },
            { 'name': 'Count', 'capacity': -1, 'isCapacityViolation': false, 'bufferedCapacity': 0, 'load': 0, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0 },
            { 'name': 'Precise fit', 'capacity': 1000, 'isCapacityViolation': false, 'bufferedCapacity': 0, 'load': 1000, 'remainingBufferedCapacity': 0, 'remainingCapacity': 0 }
        ], 'faultDomain': 5, 'upgradeDomain': 5, 'status': 'Removed', 'healthState': 'Invalid', 'upTime': '00:12:22', 'address': '10.0.0.5'
    }
];

export var ApplicationList: Application[] = [
    {
        'name': 'fabric:/App1', 'type': 'App1Type', 'version': '1.0', 'status': 'Ready', 'healthState': 'OK', 'metrics': [
            { 'name': 'Count', 'value': 120 },
            { 'name': 'MemoryKB', 'value': 600 },
            { 'name': 'DiskKB', 'value': 10000 },
            { 'name': 'Precise fit', 'value': 500 }]
    },
    {
        'name': 'fabric:/App2', 'type': 'App2Type', 'version': '2.0.0.0', 'status': 'Deleting', 'healthState': 'Warning', 'metrics': [
            { 'name': 'Count', 'value': 120 },
            { 'name': 'MemoryKB', 'value': 400 },
            { 'name': 'DiskKB', 'value': 10000 },
            { 'name': 'Precise fit', 'value': 500 }]
    }
];

export var ServiceList =
    {
        'fabric:/App1':
        [
            {
                'name': 'fabric:/App1/Service11', 'type': 'serviceType', 'version': '1.0', 'status': 'Upgrading', 'healthState': 'OK', 'metrics': [
                    { 'name': 'Count', 'value': 60 },
                    { 'name': 'MemoryKB', 'value': 400 },
                    { 'name': 'DiskKB', 'value': 5000 },
                    { 'name': 'Precise fit', 'value': 250 }]
            },
            {
                'name': 'fabric:/App1/Service12', 'type': 'serviceType', 'version': '1.0', 'status': 'Active', 'healthState': 'Warning', 'metrics': [
                    { 'name': 'Count', 'value': 60 },
                    { 'name': 'MemoryKB', 'value': 200 },
                    { 'name': 'DiskKB', 'value': 5000 },
                    { 'name': 'Precise fit', 'value': 250 }]
            },
        ],
        'fabric:/App2':
        [
            {
                'name': 'fabric:/App2/Service13', 'type': 'serviceType', 'version': '1.0', 'status': 'Unknown', 'healthState': 'Error', 'metrics': [
                    { 'name': 'Count', 'value': 60 },
                    { 'name': 'MemoryKB', 'value': 100 },
                    { 'name': 'DiskKB', 'value': 6000 },
                    { 'name': 'Precise fit', 'value': 250 }]
            },
            {
                'name': 'fabric:/App2/Service14', 'type': 'serviceType', 'version': '1.0', 'status': 'Deleting', 'healthState': 'Unknown', 'metrics': [
                    { 'name': 'Count', 'value': 60 },
                    { 'name': 'MemoryKB', 'value': 300 },
                    { 'name': 'DiskKB', 'value': 4000 },
                    { 'name': 'Precise fit', 'value': 250 }]
            }
        ]
    };

export var ReplicaList =
    {
        'fabric:/App1/Service11':
        [
            {
                'id': 1, 'partitionId': '1', 'healthState': 'OK', 'status': 'Ready', 'role': 'Primary', 'metrics': [
                    { 'name': 'Count', 'value': 20 },
                    { 'name': 'MemoryKB', 'value': 200 },
                    { 'name': 'DiskKB', 'value': 3000 },
                    { 'name': 'Precise fit', 'value': 100 }]
            },
            {
                'id': 2, 'partitionId': '2', 'healthState': 'Warning', 'status': 'Ready', 'role': 'ActiveSecondary', 'metrics': [
                    { 'name': 'Count', 'value': 20 },
                    { 'name': 'MemoryKB', 'value': 100 },
                    { 'name': 'DiskKB', 'value': 2000 },
                    { 'name': 'Precise fit', 'value': 100 }]
            },
            {
                'id': 3, 'partitionId': '3', 'healthState': 'Error', 'status': 'Ready', 'role': 'IdleSecondary', 'metrics': [
                    { 'name': 'Count', 'value': 20 },
                    { 'name': 'MemoryKB', 'value': 100 },
                    { 'name': 'DiskKB', 'value': 2000 },
                    { 'name': 'Precise fit', 'value': 50 }]
            },
        ],
        'fabric:/App1/Service12':
        [
            {
                'id': 4, 'partitionId': '1', 'healthState': 'OK', 'status': 'Ready', 'role': 'Primary', 'metrics': [
                    { 'name': 'Count', 'value': 20 },
                    { 'name': 'MemoryKB', 'value': 100 },
                    { 'name': 'DiskKB', 'value': 3000 },
                    { 'name': 'Precise fit', 'value': 100 }]
            },
            {
                'id': 5, 'partitionId': '2', 'healthState': 'Invalid', 'status': 'Standby', 'role': 'ActiveSecondary', 'metrics': [
                    { 'name': 'Count', 'value': 20 },
                    { 'name': 'MemoryKB', 'value': 50 },
                    { 'name': 'DiskKB', 'value': 2000 },
                    { 'name': 'Precise fit', 'value': 100 }]
            },
            {
                'id': 6, 'partitionId': '3', 'healthState': 'Unknown', 'status': 'Standby', 'role': 'IdleSecondary', 'metrics': [
                    { 'name': 'Count', 'value': 20 },
                    { 'name': 'MemoryKB', 'value': 50 },
                    { 'name': 'DiskKB', 'value': 2000 },
                    { 'name': 'Precise fit', 'value': 50 }]
            },
        ],
        'fabric:/App2/Service13':
        [
            {
                'id': 7, 'partitionId': '1', 'healthState': 'OK', 'status': 'Invalid', 'role': 'Invalid', 'metrics': [
                    { 'name': 'Count', 'value': 20 },
                    { 'name': 'MemoryKB', 'value': 50 },
                    { 'name': 'DiskKB', 'value': 3000 },
                    { 'name': 'Precise fit', 'value': 100 }]
            },
            {
                'id': 8, 'partitionId': '2', 'healthState': 'Warning', 'status': 'InBuild', 'role': 'Unknown', 'metrics': [
                    { 'name': 'Count', 'value': 20 },
                    { 'name': 'MemoryKB', 'value': 0 },
                    { 'name': 'DiskKB', 'value': 0 },
                    { 'name': 'Precise fit', 'value': 100 }]
            },
            {
                'id': 9, 'partitionId': '3', 'healthState': 'Error', 'status': 'Dropped', 'role': 'None', 'metrics': [
                    { 'name': 'Count', 'value': 20 },
                    { 'name': 'MemoryKB', 'value': 50 },
                    { 'name': 'DiskKB', 'value': 3000 },
                    { 'name': 'Precise fit', 'value': 50 }]
            },
        ],
        'fabric:/App2/Service14':
        [
            {
                'id': 10, 'partitionId': '1', 'healthState': 'OK', 'status': 'Down', 'role': 'Primary', 'metrics': [
                    { 'name': 'Count', 'value': 20 },
                    { 'name': 'MemoryKB', 'value': 100 },
                    { 'name': 'DiskKB', 'value': 1000 },
                    { 'name': 'Precise fit', 'value': 100 }]
            },
            {
                'id': 12, 'partitionId': '2', 'healthState': 'Invalid', 'status': 'Down', 'role': 'ActiveSecondary', 'metrics': [
                    { 'name': 'Count', 'value': 20 },
                    { 'name': 'MemoryKB', 'value': 0 },
                    { 'name': 'DiskKB', 'value': 1000 },
                    { 'name': 'Precise fit', 'value': 100 }]
            },
            {
                'id': 11, 'partitionId': '3', 'healthState': 'Unknown', 'status': 'Down', 'role': 'IdleSecondary', 'metrics': [
                    { 'name': 'Count', 'value': 20 },
                    { 'name': 'MemoryKB', 'value': 200 },
                    { 'name': 'DiskKB', 'value': 2000 },
                    { 'name': 'Precise fit', 'value': 50 }]
            }
        ]
    };
