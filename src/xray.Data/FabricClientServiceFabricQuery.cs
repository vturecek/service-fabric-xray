// ------------------------------------------------------------
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace xray.Data
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Fabric.Health;
    using System.Fabric.Query;
    using System.Linq;
    using System.Runtime.Caching;
    using System.Threading.Tasks;

    internal class FabricClientServiceFabricQuery : IServiceFabricQuery
    {
        private const string CountMetricName = "Count";
        private static readonly IEnumerable<string> systemMetrics = new[]
        {
            "__FaultAnalysisServicePrimaryCount__",
            "__FaultAnalysisServiceReplicaCount__",
            "__ClusterManagerServiceReplicaCount__",
            "__ClusterManagerServicePrimaryCount__",
            "__FaultAnalysisServicePrimaryCount__",
            "__NamingServicePrimaryCount__",
            "__NamingServiceReplicaCount__",
            "__FileStoreServicePrimaryCount__",
            "__FileStoreServiceReplicaCount__",
            "PrimaryCount",
            "Count",
            "ReplicaCount"
        };

        private static readonly IEnumerable<LoadMetric> defaultMetrics = new[] { new LoadMetric(CountMetricName, 1) };

        private readonly TimeSpan cacheDuration = TimeSpan.FromSeconds(10);
        private readonly FabricClient fabricClient;
        private readonly MemoryCache cache;

        public FabricClientServiceFabricQuery(FabricClient fabricClient)
        {
            this.cache = new MemoryCache("FabricClientServiceFabricQuery");
            this.fabricClient = fabricClient;
        }

        public async Task<Application> GetApplicationAsync(Uri applicationName)
        {
            ApplicationList applications = this.cache["ApplicationList"] as ApplicationList;

            if (applications == null)
            {
                applications = await this.GetApplicationsAsync();

                this.cache.Set(new CacheItem("ApplicationList", applications), new CacheItemPolicy()
                {
                    AbsoluteExpiration = DateTimeOffset.UtcNow + this.cacheDuration
                });
            }
            
            return applications.FirstOrDefault(x => x.ApplicationName == applicationName);
        }

        public Task<ApplicationList> GetApplicationsAsync()
        {
            return this.fabricClient.QueryManager.GetApplicationListAsync();
        }

        public Task<ApplicationTypeList> GetApplicationTypesAsync()
        {
            return this.fabricClient.QueryManager.GetApplicationTypeListAsync();
        }

        public Task<ClusterHealth> GetClusterHealthAsync()
        {
            return fabricClient.HealthManager.GetClusterHealthAsync();
        }

        public Task<DeployedApplicationList> GetDeployedApplicationsAsync(string nodeName)
        {
            return this.fabricClient.QueryManager.GetDeployedApplicationListAsync(nodeName);
        }

        public Task<DeployedServiceReplicaList> GetDeployedReplicasAsync(string nodeName, Uri applicationName)
        {
            return this.fabricClient.QueryManager.GetDeployedReplicaListAsync(nodeName, applicationName);
        }

        public async Task<ProvisionedFabricCodeVersion> GetFabricVersion()
        {
            ProvisionedFabricCodeVersionList codeVersionList = await fabricClient.QueryManager.GetProvisionedFabricCodeVersionListAsync();
            return codeVersionList.FirstOrDefault();
        }

        public async Task<ClusterLoadInformation> GetClusterLoadAsync()
        {
            ClusterLoadInformation loadInfo = await this.fabricClient.QueryManager.GetClusterLoadInformationAsync();
            long replicaCount = await this.GetClusterReplicaCountAsync();

            foreach (string systemService in systemMetrics)
            {
                LoadMetricInformation item = loadInfo.LoadMetricInformationList.FirstOrDefault(x => String.Equals(x.Name, systemService, StringComparison.OrdinalIgnoreCase));
                if (item != null)
                {
                    loadInfo.LoadMetricInformationList.Remove(item);
                }
            }

            Type t = typeof(LoadMetricInformation);
            LoadMetricInformation countMetric = new LoadMetricInformation();
            t.GetProperty("Name").SetValue(countMetric, CountMetricName);
            t.GetProperty("ClusterBufferedCapacity").SetValue(countMetric, 0);
            t.GetProperty("ClusterCapacity").SetValue(countMetric, 0);
            t.GetProperty("ClusterLoad").SetValue(countMetric, replicaCount);
            t.GetProperty("ClusterRemainingBufferedCapacity").SetValue(countMetric, -1);
            t.GetProperty("ClusterRemainingCapacity").SetValue(countMetric, -1);
            t.GetProperty("IsClusterCapacityViolation").SetValue(countMetric, false);
            t.GetProperty("NodeBufferPercentage").SetValue(countMetric, 0);
            t.GetProperty("IsBalancedBefore").SetValue(countMetric, true);
            t.GetProperty("IsBalancedAfter").SetValue(countMetric, true);
            t.GetProperty("DeviationBefore").SetValue(countMetric, 0);
            t.GetProperty("DeviationAfter").SetValue(countMetric, 0);
            t.GetProperty("BalancingThreshold").SetValue(countMetric, 0);
            t.GetProperty("MaxNodeLoadNodeId").SetValue(countMetric, new NodeId(new System.Numerics.BigInteger(0), new System.Numerics.BigInteger(0)));
            t.GetProperty("MinNodeLoadNodeId").SetValue(countMetric, new NodeId(new System.Numerics.BigInteger(0), new System.Numerics.BigInteger(0)));

            loadInfo.LoadMetricInformationList.Add(countMetric);

            return loadInfo;
        }

        public async Task<IEnumerable<NodeLoadMetricInformation>> GetNodeLoadAsync(string nodeName)
        {
            long replicaCount = await this.GetNodeReplicaCountAsync(nodeName);
            NodeLoadInformation loadInfo = await this.fabricClient.QueryManager.GetNodeLoadInformationAsync(nodeName);


            foreach (string systemService in systemMetrics)
            {
                NodeLoadMetricInformation item = loadInfo.NodeLoadMetricInformationList.FirstOrDefault(x => String.Equals(x.Name, systemService, StringComparison.OrdinalIgnoreCase));
                if (item != null)
                {
                    loadInfo.NodeLoadMetricInformationList.Remove(item);
                }
            }

            Type t = typeof(NodeLoadMetricInformation);
            NodeLoadMetricInformation countMetric = new NodeLoadMetricInformation();
            t.GetProperty("Name").SetValue(countMetric, CountMetricName);
            t.GetProperty("IsCapacityViolation").SetValue(countMetric, false);
            t.GetProperty("NodeBufferedCapacity").SetValue(countMetric, 0);
            t.GetProperty("NodeCapacity").SetValue(countMetric, 0);
            t.GetProperty("NodeLoad").SetValue(countMetric, replicaCount);
            t.GetProperty("NodeRemainingBufferedCapacity").SetValue(countMetric, -1);
            t.GetProperty("NodeRemainingCapacity").SetValue(countMetric, -1);

            IList<NodeLoadMetricInformation> metrics = loadInfo.NodeLoadMetricInformationList;
            metrics.Add(countMetric);

            return metrics;
        }


        public Task<NodeList> GetNodesAsync()
        {
            return this.fabricClient.QueryManager.GetNodeListAsync();
        }

        public async Task<string> GetReplicaHealthAsync(Guid partitionId, long replicaId)
        {
            ServiceReplicaList replicaList = await this.fabricClient.QueryManager.GetReplicaListAsync(partitionId, replicaId);

            Replica replica = replicaList.FirstOrDefault();

            return replica?.HealthState.ToString();
        }

        public async Task<IEnumerable<LoadMetricReport>> GetReplicaLoad(string nodeName, Guid partitionId, long replicaOrInstanceId)
        {
            DeployedServiceReplicaDetail detail = await
                this.fabricClient.QueryManager.GetDeployedReplicaDetailAsync(nodeName, partitionId, replicaOrInstanceId);

            Type t = typeof(LoadMetricReport);
            LoadMetricReport countMetric = new LoadMetricReport();
            t.GetProperty("Name").SetValue(countMetric, CountMetricName);
            t.GetProperty("Value").SetValue(countMetric, 1);

            detail.ReportedLoad.Add(countMetric);

            return detail.ReportedLoad;
        }

        public async Task<Service> GetServiceAsync(Uri applicationName, Uri serviceName)
        {
            try
            {
                string key = "Services-" + applicationName.ToString();
                ServiceList services = this.cache[key] as ServiceList;

                if (services == null)
                {
                    services = await this.GetServicesAsync(applicationName);

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

        public Task<ServiceList> GetServicesAsync(Uri applicationName)
        {
            return this.fabricClient.QueryManager.GetServiceListAsync(applicationName);
        }

        private async Task<long> GetClusterReplicaCountAsync()
        {
            long count = 0;
            NodeList nodes = await this.fabricClient.QueryManager.GetNodeListAsync();
            foreach (Node node in nodes)
            {
                count += (await this.GetNodeReplicaCountAsync(node.NodeName));
            }

            return count;
        }

        private async Task<long> GetNodeReplicaCountAsync(string nodeName)
        {
            long count = 0;
            DeployedApplicationList deployedApplications = await this.fabricClient.QueryManager.GetDeployedApplicationListAsync(nodeName);

            foreach (DeployedApplication application in deployedApplications)
            {
                DeployedServiceReplicaList replicas = await this.fabricClient.QueryManager.GetDeployedReplicaListAsync(nodeName, application.ApplicationName);

                count += replicas.Count;
            }

            return count;
        }
    }
}
