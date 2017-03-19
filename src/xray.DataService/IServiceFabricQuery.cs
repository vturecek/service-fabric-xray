// ------------------------------------------------------------
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace xray.DataService
{
    using System;
    using System.Collections.Generic;
    using System.Fabric.Health;
    using System.Fabric.Query;
    using System.Threading.Tasks;

    public interface IServiceFabricQuery
    {
        Task<Application> GetApplicationAsync(Uri applicationName);

        Task<ApplicationList> GetApplicationsAsync();

        Task<DeployedApplicationList> GetDeployedApplicationsAsync(string nodeName);

        Task<DeployedServiceReplicaList> GetDeployedReplicasAsync(string nodeName, Uri applicationName);

        Task<string> GetReplicaHealthAsync(Guid partitionId, long replicaId);

        Task<IEnumerable<LoadMetricReport>> GetReplicaLoad(string nodeName, Guid partitionId, long replicaId);

        Task<Service> GetServiceAsync(Uri applicationName, Uri serviceName);

        Task<ServiceList> GetServicesAsync(Uri applicationName);
        
        Task<NodeList> GetNodesAsync();

        Task<IEnumerable<NodeLoadMetricInformation>> GetNodeLoadAsync(string nodeName);

        Task<ClusterHealth> GetClusterHealthAsync();

        Task<ClusterLoadInformation> GetClusterLoadAsync();

        Task<ApplicationTypeList> GetApplicationTypesAsync();

        Task<ProvisionedFabricCodeVersion> GetFabricVersion();
    }
}
