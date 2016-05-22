using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xray.Models;

namespace Xray.Services
{
    public interface IClusterInformationService
    {
        Task<IEnumerable<DeployedApplicationModel>> GetApplicationMetrics(string nodeName);

        Task<IEnumerable<ClusterNode>> GetNodeCapacity();

        Task<IEnumerable<ClusterCapacity>> GetClusterCapacities();

        Task<IEnumerable<ClusterCapacityHistory>> GetClusterCapacityHistory(string capacityName, DateTimeOffset startTime);
    }
}
