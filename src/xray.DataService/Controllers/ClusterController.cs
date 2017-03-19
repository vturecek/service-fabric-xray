// ------------------------------------------------------------
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace xray.DataService.Controllers
{
    using Common;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Data.Collections;
    using System;
    using System.Collections.Generic;
    using System.Fabric.Health;
    using System.Fabric.Query;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using xray.Models;

    [Route("api/[controller]")]
    public class ClusterController : Controller
    {
        private readonly IServiceFabricQuery query;
        private readonly IReliableStateManager stateManager;

        public ClusterController(IServiceFabricQuery query, IReliableStateManager stateManager)
        {
            this.query = query;
            this.stateManager = stateManager;
        }

        [HttpGet("info")]
        public async Task<ClusterInfo> Info()
        {
            NodeList nodes = await this.query.GetNodesAsync();
            ApplicationTypeList appTypes = await this.query.GetApplicationTypesAsync();
            ApplicationList applications = await this.query.GetApplicationsAsync();
            ClusterLoadInformation clusterLoadInfo = await this.query.GetClusterLoadAsync();
            ClusterHealth clusterHealth = await this.query.GetClusterHealthAsync();
            ProvisionedFabricCodeVersion version = await this.query.GetFabricVersion();

            long serviceCount = 0;
            long partitionCount = 0;
            long replicaCount = 0;

            foreach (Node node in nodes)
            {
                DeployedApplicationList deployedApplicationList = await this.query.GetDeployedApplicationsAsync(node.NodeName);

                foreach (DeployedApplication deployedApplication in deployedApplicationList)
                {
                    DeployedServiceReplicaList deployedReplicas =
                        await this.query.GetDeployedReplicasAsync(node.NodeName, deployedApplication.ApplicationName);

                    replicaCount += deployedReplicas.Count;
                }
            }

            foreach (Application application in applications)
            {
                ServiceList services = await this.query.GetServicesAsync(application.ApplicationName);
                serviceCount += services.Count;
            }

            return new ClusterInfo(
                clusterHealth.AggregatedHealthState.ToString(),
                version != null ? version.CodeVersion : "not based",
                nodes.Select(x => x.NodeType).Distinct().Count(),
                appTypes.Count(),
                nodes.Select(x => x.FaultDomain.ToString()).Distinct().Count(),
                nodes.Select(x => x.UpgradeDomain).Distinct().Count(),
                nodes.Count,
                applications.Count,
                serviceCount,
                partitionCount, // TODO: partition count
                replicaCount,
                clusterLoadInfo.LastBalancingStartTimeUtc,
                clusterLoadInfo.LastBalancingEndTimeUtc);
        }

        [HttpGet("filters")]
        public async Task<ClusterFilters> Filters()
        {
            NodeList nodes = await this.query.GetNodesAsync();
            ApplicationTypeList appTypes = await this.query.GetApplicationTypesAsync();

            return new ClusterFilters(
                nodes.Select(x => x.NodeType).Distinct(),
                appTypes.Select(x => x.ApplicationTypeName),
                nodes.Select(x => x.FaultDomain.ToString()).Distinct(),
                nodes.Select(x => x.UpgradeDomain).Distinct());
        }

        [HttpGet("capacity")]
        public async Task<IEnumerable<ClusterCapacity>> Capacity()
        {
            ClusterLoadInformation loadInfo = await this.query.GetClusterLoadAsync();
            NodeList nodes = await this.query.GetNodesAsync();

            return loadInfo.LoadMetricInformationList.Select(
                x => new ClusterCapacity(
                    x.Name,
                    x.ClusterBufferedCapacity,
                    x.ClusterCapacity,
                    x.ClusterLoad,
                    x.ClusterRemainingBufferedCapacity,
                    x.ClusterRemainingCapacity,
                    x.IsClusterCapacityViolation,
                    x.NodeBufferPercentage,
                    x.IsBalancedBefore,
                    x.IsBalancedAfter,
                    x.DeviationBefore,
                    x.DeviationAfter,
                    x.BalancingThreshold,
                    nodes.FirstOrDefault(node => node.NodeId == x.MaxNodeLoadNodeId)?.NodeName,
                    nodes.FirstOrDefault(node => node.NodeId == x.MinNodeLoadNodeId)?.NodeName));
        }

        [HttpGet("history/{capacityName}/{startTime}")]
        public async Task<IEnumerable<ClusterCapacityHistory>> History(string capacityName, DateTimeOffset startTime)
        {
            IReliableDictionary<DateTimeOffset, Dictionary<string, long>> dictionary = await
                   this.stateManager.GetOrAddAsync<IReliableDictionary<DateTimeOffset, Dictionary<string, long>>>($"history:/hourly");

            List<ClusterCapacityHistory> result = new List<ClusterCapacityHistory>();

            using (ITransaction tx = this.stateManager.CreateTransaction())
            {
                var enumerable = await dictionary.CreateEnumerableAsync(tx);
                var enumerator = enumerable.GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync(CancellationToken.None))
                {
                    var current = enumerator.Current;

                    if (current.Key > startTime)
                    {
                        long capacity;
                        current.Value.TryGetValue(capacityName, out capacity);
                        result.Add(new ClusterCapacityHistory(capacity, current.Key));
                    }
                }

                return result.OrderBy(x => x.Timestamp);
            }
        }
    }
}