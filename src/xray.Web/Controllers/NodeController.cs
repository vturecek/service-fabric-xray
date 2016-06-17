// ------------------------------------------------------------
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace xray.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using xray.Models;

    [Route("api/[controller]")]
    public class NodeController : Controller
    {

        [HttpGet("info")]
        public Task<IEnumerable<ClusterNode>> Info()
        {
           // return this.clusterInfoService.GetNodes(null);
        }

        [HttpGet("info/{nodeTypeFilter}")]
        public Task<IEnumerable<ClusterNode>> Info(string nodeTypeFilter)
        {
           // return this.clusterInfoService.GetNodes(nodeTypeFilter);
        }

        [HttpGet("capacity/{nodeName}")]
        public Task<IEnumerable<ClusterNodeCapacity>> Capacity(string nodeName)
        {
           // return this.clusterInfoService.GetNodeCapacity(nodeName);
        }
    }
}