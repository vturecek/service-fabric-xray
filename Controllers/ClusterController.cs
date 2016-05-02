using Xray.Models;
using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Query;
using System.Linq;
using System.Threading.Tasks;

namespace Xray.Controllers
{
    [Route("api/[controller]")]
    public class ClusterController : Controller
    {
        private static readonly IEnumerable<ClusterCapacity> defaultCapacities = new[] 
        {
            new ClusterCapacity("Default Replica Count", 0, 500, 0, 0, 0, false, 0)
        };

        private readonly FabricClient client;

        public ClusterController(FabricClient client)
        {
            this.client = client;
        }

        [HttpGet("capacity")]
        public async Task<IEnumerable<ClusterCapacity>> Capacity()
        {
            ClusterLoadInformation loadInfo = await client.QueryManager.GetClusterLoadInformationAsync();

            return loadInfo.LoadMetricInformationList.Where(x => x.ClusterCapacity > 0).Select(x => new ClusterCapacity(
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
