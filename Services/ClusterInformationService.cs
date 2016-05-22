using Microsoft.ServiceFabric.Data;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading.Tasks;
using Xray.Models;
using System.Threading;
using Microsoft.ServiceFabric.Data.Collections;
using System.Fabric.Query;

namespace Xray.Services
{
    public class ClusterInformationService : Microsoft.ServiceFabric.Services.Runtime.StatefulService, IClusterInformationService
    {
        private const string CountMetricName = "Count";

        private static readonly TimeSpan HourlyInterval = TimeSpan.FromSeconds(10);

        private static readonly IEnumerable<LoadMetric> defaultMetrics = new[] { new LoadMetric(CountMetricName, 20) };
        
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

            return loadInfo.LoadMetricInformationList.Where(x => x.ClusterCapacity > 0).Select(
                x => new ClusterCapacity(
                    x.Name,
                    x.ClusterBufferedCapacity,
                    x.ClusterCapacity,
                    x.ClusterLoad,
                    x.ClusterRemainingBufferedCapacity,
                    x.ClusterRemainingCapacity,
                    x.IsClusterCapacityViolation,
                    x.NodeBufferPercentage))
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

        public async Task<IEnumerable<ClusterNode>> GetNodeCapacity()
        {
            NodeList nodes = await this.fabricClient.QueryManager.GetNodeListAsync();

            List<ClusterNode> result = new List<ClusterNode>(nodes.Count);

            foreach (Node node in nodes)
            {
                NodeLoadInformation loadInfo = await this.fabricClient.QueryManager.GetNodeLoadInformationAsync(node.NodeName);

                result.Add(
                    new ClusterNode(
                        node.NodeName,
                        node.NodeStatus.ToString(),
                        node.HealthState.ToString(),
                        node.FaultDomain.ToString(),
                        node.UpgradeDomain,
                        loadInfo.NodeLoadMetricInformationList.Select(
                            x =>
                                new ClusterNodeCapacity(
                                    x.Name,
                                    x.IsCapacityViolation,
                                    x.NodeBufferedCapacity,
                                    x.NodeCapacity,
                                    x.NodeLoad,
                                    x.NodeRemainingBufferedCapacity,
                                    x.NodeRemainingCapacity))
                            .Concat(new[] { await this.GetNodeReplicaCount(node.NodeName) })));
            }

            return result;
        }
        
        public async Task<IEnumerable<DeployedApplicationModel>> GetApplicationMetrics(string nodeName)
        {
            DeployedApplicationList deployedApplications = await this.fabricClient.QueryManager.GetDeployedApplicationListAsync(nodeName);
            ApplicationList applications = await this.fabricClient.QueryManager.GetApplicationListAsync();

            List<DeployedApplicationModel> applicationModels = new List<DeployedApplicationModel>(deployedApplications.Count);

            foreach (DeployedApplication deployedApplication in deployedApplications)
            {
                ServiceList services = await this.fabricClient.QueryManager.GetServiceListAsync(deployedApplication.ApplicationName);
                DeployedServiceReplicaList deployedReplicas =
                    await this.fabricClient.QueryManager.GetDeployedReplicaListAsync(nodeName, deployedApplication.ApplicationName);

                IEnumerable<IGrouping<Uri, DeployedServiceReplica>> groups = deployedReplicas.GroupBy(x => x.ServiceName);

                List<DeployedServiceModel> serviceModels = new List<DeployedServiceModel>(groups.Count());

                foreach (IGrouping<Uri, DeployedServiceReplica> group in groups)
                {
                    List<ReplicaModel> replicaModels = new List<ReplicaModel>(group.Count());
                    foreach (DeployedServiceReplica item in group)
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
                        }
                    }

                    Service service = services.FirstOrDefault(x => x.ServiceName == group.Key);

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


                Application application = applications.FirstOrDefault(x => x.ApplicationName == deployedApplication.ApplicationName);

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

            return new ClusterCapacity(CountMetricName, 0, 0, count, 0, 0, false, 0);
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

            return new ClusterNodeCapacity(CountMetricName, false, 0, 0, count, 0, 0);
        }
    }
}
