// ------------------------------------------------------------
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace xray.Data.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Fabric.Query;
    using System.Linq;
    using System.Threading.Tasks;
    using xray.Models;

    [Route("api/[controller]")]
    public class NodeController : Controller
    {
        private readonly IServiceFabricQuery query;

        public NodeController(IServiceFabricQuery query)
        {
            this.query = query;
        }
        

        [HttpGet("info/{nodeTypeFilter?}")]
        public async Task<IEnumerable<ClusterNode>> Info(string nodeTypeFilter = null)
        {
            IEnumerable<Node> nodes = await this.query.GetNodesAsync();

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

        [HttpGet("capacity/{nodeName}")]
        public async Task<IEnumerable<ClusterNodeCapacity>> Capacity(string nodeName)
        {
            IEnumerable<NodeLoadMetricInformation> loadInfo = await this.query.GetNodeLoadAsync(nodeName);

            return loadInfo.Select(item =>
                new ClusterNodeCapacity(
                   item.Name,
                   item.IsCapacityViolation,
                   item.NodeBufferedCapacity,
                   item.NodeCapacity,
                   item.NodeLoad,
                   item.NodeRemainingBufferedCapacity,
                   item.NodeRemainingCapacity));
        }
        
    }
}