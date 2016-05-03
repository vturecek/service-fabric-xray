// ------------------------------------------------------------
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Xray.Controllers
{
    using System.Collections.Generic;
    using System.Fabric;
    using System.Fabric.Query;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Mvc;
    using Xray.Models;

    [Route("api/[controller]")]
    public class ClusterController : Controller
    {
        private static readonly IEnumerable<ClusterCapacity> defaultCapacities = new[]
        {
            new ClusterCapacity("Default Replica Count", 0, -1, 0, 0, 0, false, 0)
        };

        private readonly FabricClient client;

        public ClusterController(FabricClient client)
        {
            this.client = client;
        }

        [HttpGet("capacity")]
        public async Task<IEnumerable<ClusterCapacity>> Capacity()
        {
            ClusterLoadInformation loadInfo = await this.client.QueryManager.GetClusterLoadInformationAsync();

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
                .Concat(defaultCapacities);
        }
    }
}