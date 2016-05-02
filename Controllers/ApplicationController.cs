// ------------------------------------------------------------
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Xray.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Fabric.Query;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Mvc;
    using Xray.Models;

    [Route("api/[controller]")]
    public class ApplicationController : Controller
    {
        private static readonly IEnumerable<LoadMetric> defaultMetrics = new[] {new LoadMetric("Default Replica Count", 20)};

        private readonly FabricClient client;

        public ApplicationController(FabricClient client)
        {
            this.client = client;
        }

        [HttpGet("{nodeName}")]
        public async Task<IEnumerable<DeployedApplicationModel>> Get(string nodeName)
        {
            DeployedApplicationList deployedApplications = await this.client.QueryManager.GetDeployedApplicationListAsync(nodeName);
            ApplicationList applications = await this.client.QueryManager.GetApplicationListAsync();

            List<DeployedApplicationModel> applicationModels = new List<DeployedApplicationModel>(deployedApplications.Count);

            foreach (DeployedApplication deployedApplication in deployedApplications)
            {
                ServiceList services = await this.client.QueryManager.GetServiceListAsync(deployedApplication.ApplicationName);
                DeployedServiceReplicaList deployedReplicas =
                    await this.client.QueryManager.GetDeployedReplicaListAsync(nodeName, deployedApplication.ApplicationName);

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
                                    this.client.QueryManager.GetDeployedReplicaDetailAsync(
                                        nodeName,
                                        statefulDeployedReplica.Partitionid,
                                        statefulDeployedReplica.ReplicaId) as DeployedStatefulServiceReplicaDetail;
                            ServiceReplicaList replicaList = await this.client.QueryManager.GetReplicaListAsync(detail.PartitionId, detail.ReplicaId);

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

        /*
        [HttpGet("metrics/{nodeName}")]
        public async Task<IEnumerable<ApplicationModel>> Metrics(string nodeName)
        {
            FabricClient client = new FabricClient("localhost:19000");
            DeployedApplicationList applications = await client.QueryManager.GetDeployedApplicationListAsync(nodeName);

            List<ApplicationModel> result = new List<ApplicationModel>(applications.Count);

            foreach (DeployedApplication application in applications)
            {

                Dictionary<string, int> load = new Dictionary<string, int>();
                DeployedServiceReplicaList replicas = await client.QueryManager.GetDeployedReplicaListAsync(nodeName, application.ApplicationName);

                foreach (DeployedServiceReplica item in replicas)
                {
                    DeployedStatefulServiceReplica replica = item as DeployedStatefulServiceReplica;

                    if (replica != null)
                    {
                        DeployedStatefulServiceReplicaDetail detail = await client.QueryManager.GetDeployedReplicaDetailAsync(nodeName, replica.Partitionid, replica.ReplicaId) as DeployedStatefulServiceReplicaDetail;
                        foreach (LoadMetricReport loadReport in detail.ReportedLoad)
                        {
                            if (load.ContainsKey(loadReport.Name))
                            {
                                load[loadReport.Name] += loadReport.Value;
                            }
                            else
                            {
                                load[loadReport.Name] = loadReport.Value;
                            }

                        }
                    }
                }

                result.Add(new ApplicationModel(application.ApplicationName.ToString(), application.ApplicationTypeName, "",
                    load.Select(x => new LoadMetric(x.Key, x.Value))));

            }

            return result;
        }
        */
    }
}