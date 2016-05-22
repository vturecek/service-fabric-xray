// ------------------------------------------------------------
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Xray.Controllers
{
    using Microsoft.AspNet.Mvc;
    using Services;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xray.Models;

    [Route("api/[controller]")]
    public class NodeController : Controller
    {
        private readonly IClusterInformationService clusterInfoService;

        public NodeController(IClusterInformationService historyService)
        {
            this.clusterInfoService = historyService;
        }

        [HttpGet("capacity")]
        public Task<IEnumerable<ClusterNode>> Capacity()
        {
            return this.clusterInfoService.GetNodeCapacity();
        }
    }
}