// ------------------------------------------------------------
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace xray.Data.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.ServiceFabric.Data;
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Fabric.Query;
    using System.Linq;
    using System.Threading.Tasks;
    using xray.Models;

    [Route("api/[controller]")]
    internal class ApplicationController : Controller
    {
        private readonly IServiceFabricQuery query;
        private readonly IReliableStateManager stateManager;

        public ApplicationController(IServiceFabricQuery query, IReliableStateManager stateManager)
        {
            this.query = query;
            this.stateManager = stateManager;
        }

        [HttpGet("{nodeName}/{appTypeFilter?}")]
        public async Task<IEnumerable<DeployedApplicationModel>> Get(string nodeName, string appTypeFilter = null)
        {
            IEnumerable<DeployedApplication> deployedApplications = await this.query.GetDeployedApplicationsAsync(nodeName);

            if (appTypeFilter != null)
            {
                deployedApplications = deployedApplications.Where(x => !appTypeFilter.Contains(x.ApplicationTypeName));
            }

            List<DeployedApplicationModel> applicationModels = new List<DeployedApplicationModel>(deployedApplications.Count());

            foreach (DeployedApplication deployedApplication in deployedApplications)
            {
                DeployedServiceReplicaList deployedReplicas =
                    await this.query.GetDeployedReplicasAsync(nodeName, deployedApplication.ApplicationName);
                
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
                                IEnumerable<LoadMetricReport> loadMetrics =
                                    await this.query.GetReplicaLoad(nodeName, statefulDeployedReplica.Partitionid, statefulDeployedReplica.ReplicaId);

                                string health =
                                    await this.query.GetReplicaHealthAsync(statefulDeployedReplica.Partitionid, statefulDeployedReplica.ReplicaId);

                                replicaModels.Add(
                                    new ReplicaModel(
                                        statefulDeployedReplica.ReplicaId,
                                        statefulDeployedReplica.Partitionid,
                                        statefulDeployedReplica.ReplicaRole.ToString(),
                                        statefulDeployedReplica.ReplicaStatus.ToString(),
                                        health,
                                        loadMetrics.Select(x => new LoadMetric(x.Name, x.Value))));
                            }
                            else
                            {
                                DeployedStatelessServiceInstance statelessDeployedInstance = item as DeployedStatelessServiceInstance;

                                if (statelessDeployedInstance != null)
                                {
                                    IEnumerable<LoadMetricReport> loadMetrics =
                                        await this.query.GetReplicaLoad(nodeName, statelessDeployedInstance.Partitionid, statelessDeployedInstance.InstanceId);

                                    string health =
                                        await this.query.GetReplicaHealthAsync(statelessDeployedInstance.Partitionid, statelessDeployedInstance.InstanceId);

                                    replicaModels.Add(
                                        new ReplicaModel(
                                            statelessDeployedInstance.InstanceId,
                                            statelessDeployedInstance.Partitionid,
                                            null,
                                            statelessDeployedInstance.ReplicaStatus.ToString(),
                                            health,
                                            loadMetrics.Select(x => new LoadMetric(x.Name, x.Value))));

                                }
                            }
                        }
                        catch (FabricException)
                        {
                            // information may not be available yet. Skip and move on.
                        }
                    }

                    Service service = await this.query.GetServiceAsync(deployedApplication.ApplicationName, group.Key);

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
                
                Application application = await this.query.GetApplicationAsync(deployedApplication.ApplicationName);

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
    }
}