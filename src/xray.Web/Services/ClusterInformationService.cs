// ------------------------------------------------------------
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Xray.Services
{
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Data.Collections;
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Fabric.Health;
    using System.Fabric.Query;
    using System.Linq;
    using System.Runtime.Caching;
    using System.Threading;
    using System.Threading.Tasks;
    using Xray.Models;

    public class ClusterInformationService : Microsoft.ServiceFabric.Services.Runtime.StatefulService, IClusterInformationService
    {
        private const string CountMetricName = "Count";

        private static readonly TimeSpan HourlyInterval = TimeSpan.FromSeconds(10);

        private static readonly IEnumerable<LoadMetric> defaultMetrics = new[] { new LoadMetric(CountMetricName, 1) };

        private readonly MemoryCache cache = new MemoryCache("ClusterInformation");
        private readonly TimeSpan cacheDuration = TimeSpan.FromSeconds(10);

        private readonly FabricClient fabricClient = new FabricClient();

        public ClusterInformationService(StatefulServiceContext context, IReliableStateManagerReplica stateManager)
            : base(context, stateManager)
        {
            Task.Run(async () => await RunAsync(CancellationToken.None));
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            int hourlyLimit = (int)Math.Round(3600D / HourlyInterval.TotalSeconds);

            try
            {
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    IEnumerable<ClusterCapacity> capacities = await this.GetClusterCapacities();

                    using (ITransaction tx = this.StateManager.CreateTransaction())
                    {
                        DateTimeOffset utcnow = DateTimeOffset.UtcNow;
                        DateTimeOffset timestamp = new DateTimeOffset(utcnow.Year, utcnow.Month, utcnow.Day, utcnow.Hour, utcnow.Minute, utcnow.Second, utcnow.Offset);

                        foreach (var capacity in capacities)
                        {
                            IReliableDictionary<DateTimeOffset, long> dictionary = await
                                this.StateManager.GetOrAddAsync<IReliableDictionary<DateTimeOffset, long>>($"history:/{capacity.Name}/hourly");

                            long count = await dictionary.GetCountAsync(tx);

                            if (count >= hourlyLimit)
                            {
                                var min = await (await dictionary.CreateLinqAsyncEnumerable(tx)).Min(x => x.Key);

                                await dictionary.TryRemoveAsync(tx, min);
                            }

                            await dictionary.SetAsync(tx, timestamp, capacity.Load);

                        }

                        await tx.CommitAsync();
                    }

                    await Task.Delay(HourlyInterval, cancellationToken);
                }
            }
            finally
            {
                if (this.fabricClient != null)
                {
                    this.fabricClient.Dispose();
                }
            }
        }

        public async Task<IEnumerable<ClusterCapacity>> GetClusterCapacities()
        {
            ClusterLoadInformation loadInfo = await this.fabricClient.QueryManager.GetClusterLoadInformationAsync();
            NodeList nodes = await this.fabricClient.QueryManager.GetNodeListAsync();

            return loadInfo.LoadMetricInformationList.Where(x => x.ClusterCapacity > 0).Select(
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
                    nodes.FirstOrDefault(node => node.NodeId == x.MinNodeLoadNodeId)?.NodeName))
                .Concat(new[] { await this.GetClusterReplicaCount() });
        }

        public async Task<IEnumerable<ClusterCapacityHistory>> GetClusterCapacityHistory(string capacityName, DateTimeOffset startTime)
        {
            ConditionalValue<IReliableDictionary<DateTimeOffset, long>> result = await
                this.StateManager.TryGetAsync<IReliableDictionary<DateTimeOffset, long>>($"history:/{capacityName}/hourly");

            if (!result.HasValue)
            {
                throw new ArgumentException($"capacity history doesn't exist for {capacityName}", "capacityName");
            }

            IReliableDictionary<DateTimeOffset, long> dictionary = result.Value;

            using (ITransaction tx = this.StateManager.CreateTransaction())
            {
                return (await dictionary.CreateLinqAsyncEnumerable(tx))
                    .Where(x => x.Key > startTime)
                    .Select(x => new ClusterCapacityHistory(x.Value, x.Key))
                    .OrderBy(x => x.Timestamp)
                    .ToEnumerable();
            }
        }

        public async Task<IEnumerable<ClusterNodeCapacity>> GetNodeCapacity(string nodeName)
        {
            ClusterNodeCapacity replicaCount = await this.GetNodeReplicaCount(nodeName);

            NodeLoadInformation loadInfo = await this.fabricClient.QueryManager.GetNodeLoadInformationAsync(nodeName);

            return loadInfo.NodeLoadMetricInformationList.Select(item =>
                new ClusterNodeCapacity(
                   item.Name,
                   item.IsCapacityViolation,
                   item.NodeBufferedCapacity,
                   item.NodeCapacity,
                   item.NodeLoad,
                   item.NodeRemainingBufferedCapacity,
                   item.NodeRemainingCapacity))
                .Concat(new[] { replicaCount });
        }

        public async Task<IEnumerable<ClusterNode>> GetNodes(string nodeTypeFilter)
        {
            IEnumerable<Node> nodes = await this.fabricClient.QueryManager.GetNodeListAsync();

            if (nodeTypeFilter != null)
            {
                nodes = nodes.Where(x => !nodeTypeFilter.Contains(x.NodeType));
            }

            return nodes.Select(node =>
                new ClusterNode(
                    node.NodeName,
                    node.NodeType,
                    node.NodeStatus.ToString(),
                    node.HealthState.ToString(),
                    node.NodeUpTime,
                    node.IpAddressOrFQDN,
                    node.FaultDomain.ToString(),
                    node.UpgradeDomain));
        }

        private async Task<Application> GetApplication(Uri applicationName)
        {
            ApplicationList applications = this.cache["ApplicationList"] as ApplicationList;

            if (applications == null)
            {
                applications = await this.fabricClient.QueryManager.GetApplicationListAsync();

                this.cache.Set(new CacheItem("ApplicationList", applications), new CacheItemPolicy()
                {
                    AbsoluteExpiration = DateTimeOffset.UtcNow + this.cacheDuration
                });
            }

            return applications.FirstOrDefault(x => x.ApplicationName == applicationName);
        }

        private async Task<Service> GetService(Uri applicationName, Uri serviceName)
        {
            try
            {
                string key = "Services-" + applicationName.ToString();
                ServiceList services = this.cache[key] as ServiceList;

                if (services == null)
                {
                    services = await this.fabricClient.QueryManager.GetServiceListAsync(applicationName);

                    this.cache.Set(new CacheItem(key, services), new CacheItemPolicy()
                    {
                        AbsoluteExpiration = DateTimeOffset.UtcNow + this.cacheDuration
                    });
                }

                return services.FirstOrDefault(x => x.ServiceName == serviceName);
            }
            catch (FabricObjectClosedException)
            {
                return null;
            }
        }

        public async Task<IEnumerable<DeployedApplicationModel>> GetApplicationMetrics(string nodeName, string appTypeFilter)
        {
            IEnumerable<DeployedApplication> deployedApplications = await this.fabricClient.QueryManager.GetDeployedApplicationListAsync(nodeName);

            if (appTypeFilter != null)
            {
                deployedApplications = deployedApplications.Where(x => !appTypeFilter.Contains(x.ApplicationTypeName));
            }

            List<DeployedApplicationModel> applicationModels = new List<DeployedApplicationModel>(deployedApplications.Count());

            foreach (DeployedApplication deployedApplication in deployedApplications)
            {
                DeployedServiceReplicaList deployedReplicas =
                    await this.fabricClient.QueryManager.GetDeployedReplicaListAsync(nodeName, deployedApplication.ApplicationName);

                IEnumerable<IGrouping<Uri, DeployedServiceReplica>> groups = deployedReplicas.GroupBy(x => x.ServiceName);
                List<DeployedServiceModel> serviceModels = new List<DeployedServiceModel>(groups.Count());

                foreach (IGrouping<Uri, DeployedServiceReplica> group in groups)
                {
                    List<ReplicaModel> replicaModels = new List<ReplicaModel>(group.Count());
                    foreach (DeployedServiceReplica item in group)
                    {
                        try
                        {
                            DeployedStatefulServiceReplica statefulDeployedReplica = item as DeployedStatefulServiceReplica;

                            if (statefulDeployedReplica != null)
                            {
                                DeployedStatefulServiceReplicaDetail detail =
                                    await
                                        this.fabricClient.QueryManager.GetDeployedReplicaDetailAsync(
                                            nodeName,
                                            statefulDeployedReplica.Partitionid,
                                            statefulDeployedReplica.ReplicaId) as DeployedStatefulServiceReplicaDetail;

                                ServiceReplicaList replicaList = await this.fabricClient.QueryManager.GetReplicaListAsync(detail.PartitionId, detail.ReplicaId);

                                Replica replica = replicaList.FirstOrDefault();

                                replicaModels.Add(
                                    new ReplicaModel(
                                        statefulDeployedReplica.ReplicaId,
                                        statefulDeployedReplica.Partitionid,
                                        statefulDeployedReplica.ReplicaRole.ToString(),
                                        statefulDeployedReplica.ReplicaStatus.ToString(),
                                        replica?.HealthState.ToString(),
                                        detail.ReportedLoad.Select(x => new LoadMetric(x.Name, x.Value)).Concat(defaultMetrics)));

                                continue;
                            }

                            DeployedStatelessServiceInstance statelessDeployedInstance = item as DeployedStatelessServiceInstance;

                            if (statelessDeployedInstance != null)
                            {
                                DeployedStatelessServiceInstanceDetail instanceDetail =
                                    await
                                        this.fabricClient.QueryManager.GetDeployedReplicaDetailAsync(
                                            nodeName,
                                            statelessDeployedInstance.Partitionid,
                                            statelessDeployedInstance.InstanceId) as DeployedStatelessServiceInstanceDetail;

                                ServiceReplicaList replicaList = await this.fabricClient.QueryManager.GetReplicaListAsync(instanceDetail.PartitionId, instanceDetail.InstanceId);

                                Replica replica = replicaList.FirstOrDefault();

                                replicaModels.Add(
                                    new ReplicaModel(
                                        statelessDeployedInstance.InstanceId,
                                        statelessDeployedInstance.Partitionid,
                                        null,
                                        statelessDeployedInstance.ReplicaStatus.ToString(),
                                        replica?.HealthState.ToString(),
                                        instanceDetail.ReportedLoad.Select(x => new LoadMetric(x.Name, x.Value)).Concat(defaultMetrics)));

                            }
                        }
                        catch (FabricException)
                        {
                            // information may not be available yet. Skip and move on.
                        }
                    }

                    Service service = await this.GetService(deployedApplication.ApplicationName, group.Key);

                    serviceModels.Add(
                        new DeployedServiceModel(
                            new ServiceModel(
                                group.Key.ToString(),
                                service?.ServiceTypeName,
                                service?.ServiceManifestVersion,
                                service?.ServiceStatus.ToString(),
                                service?.HealthState.ToString(),
                                replicaModels
                                    .SelectMany(r => r.Metrics)
                                    .GroupBy(k => k.Name, v => v.Value)
                                    .Select(g => new LoadMetric(g.Key, g.Sum()))),
                            replicaModels));
                }


                Application application = await this.GetApplication(deployedApplication.ApplicationName);

                applicationModels.Add(
                    new DeployedApplicationModel(
                        new ApplicationModel(
                            deployedApplication.ApplicationName.ToString(),
                            application?.ApplicationTypeName,
                            application?.ApplicationTypeVersion,
                            application?.ApplicationStatus.ToString(),
                            application?.HealthState.ToString(),
                            serviceModels
                                .SelectMany(s => s.Service.Metrics)
                                .GroupBy(k => k.Name, v => v.Value)
                                .Select(g => new LoadMetric(g.Key, g.Sum()))),
                        serviceModels));
            }

            return applicationModels;
        }

        private async Task<ClusterCapacity> GetClusterReplicaCount()
        {
            long count = 0;
            NodeList nodes = await this.fabricClient.QueryManager.GetNodeListAsync();
            foreach (Node node in nodes)
            {
                count += (await this.GetNodeReplicaCount(node.NodeName)).Load;
            }

            return new ClusterCapacity(CountMetricName, 0, 0, count, -1, -1, false, 0, true, true, 0, 0, 0, "", "");
        }

        private async Task<ClusterNodeCapacity> GetNodeReplicaCount(string nodeName)
        {
            long count = 0;
            DeployedApplicationList deployedApplications = await this.fabricClient.QueryManager.GetDeployedApplicationListAsync(nodeName);

            foreach (DeployedApplication application in deployedApplications)
            {
                DeployedServiceReplicaList replicas = await this.fabricClient.QueryManager.GetDeployedReplicaListAsync(nodeName, application.ApplicationName);

                count += replicas.Count;
            }

            return new ClusterNodeCapacity(CountMetricName, false, 0, 0, count, -1, -1);
        }

        public async Task<ClusterFilters> GetClusterFilters()
        {
            NodeList nodes = await this.fabricClient.QueryManager.GetNodeListAsync();
            ApplicationTypeList appTypes = await this.fabricClient.QueryManager.GetApplicationTypeListAsync();

            return new ClusterFilters(
                nodes.Select(x => x.NodeType).Distinct(),
                appTypes.Select(x => x.ApplicationTypeName),
                nodes.Select(x => x.FaultDomain.ToString()).Distinct(),
                nodes.Select(x => x.UpgradeDomain).Distinct());
        }

        public async Task<ClusterInfo> GetClusterInfo()
        {
            NodeList nodes = await this.fabricClient.QueryManager.GetNodeListAsync();
            ApplicationTypeList appTypes = await this.fabricClient.QueryManager.GetApplicationTypeListAsync();
            ApplicationList applications = await this.fabricClient.QueryManager.GetApplicationListAsync();
            ClusterLoadInformation clusterLoadInfo = await fabricClient.QueryManager.GetClusterLoadInformationAsync();
            ClusterHealth clusterHealth = await fabricClient.HealthManager.GetClusterHealthAsync();
            ProvisionedFabricCodeVersionList codeVersionList = await fabricClient.QueryManager.GetProvisionedFabricCodeVersionListAsync();
            ProvisionedFabricCodeVersion version = codeVersionList.FirstOrDefault();
            
            long serviceCount = 0;
            long partitionCount = 0;
            long replicaCount = 0;

            foreach (Node node in nodes)
            {
                DeployedApplicationList deployedApplicationList = await this.fabricClient.QueryManager.GetDeployedApplicationListAsync(node.NodeName);

                foreach (DeployedApplication deployedApplication in deployedApplicationList)
                {
                    DeployedServiceReplicaList deployedReplicas =
                        await this.fabricClient.QueryManager.GetDeployedReplicaListAsync(node.NodeName, deployedApplication.ApplicationName);
                    
                    replicaCount += deployedReplicas.Count;
                }
            }

            foreach (Application application in applications)
            {
                ServiceList services = await this.fabricClient.QueryManager.GetServiceListAsync(application.ApplicationName);
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

    }
}
