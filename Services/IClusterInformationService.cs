// ------------------------------------------------------------
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Xray.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xray.Models;

    public interface IClusterInformationService
    {
        Task<IEnumerable<DeployedApplicationModel>> GetApplicationMetrics(string nodeName, string appTypeFilter);

        Task<IEnumerable<ClusterNode>> GetNodeCapacity(string nodeTypeFilter);

        Task<ClusterInfo> GetClusterInfo();

        Task<IEnumerable<ClusterCapacity>> GetClusterCapacities();

        Task<IEnumerable<ClusterCapacityHistory>> GetClusterCapacityHistory(string capacityName, DateTimeOffset startTime);
    }
}
