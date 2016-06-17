// ------------------------------------------------------------
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace xray.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using xray.Models;

    [Route("api/[controller]")]
    public class ClusterController : Controller
    {

        [HttpGet("info")]
        public Task<ClusterInfo> Info()
        {
           // return this.clusterInfoService.GetClusterInfo();
        }

        [HttpGet("filters")]
        public Task<ClusterFilters> Filters()
        {
           // return this.clusterInfoService.GetClusterFilters();
        }

        [HttpGet("capacity")]
        public Task<IEnumerable<ClusterCapacity>> Capacity()
        {
           // return this.clusterInfoService.GetClusterCapacities();
        }

        [HttpGet("history/{capacityName}/{startTime}")]
        public Task<IEnumerable<ClusterCapacityHistory>> History(string capacityName, DateTimeOffset startTime)
        {
           // return this.clusterInfoService.GetClusterCapacityHistory(capacityName, startTime);
        }
    }
}