// ------------------------------------------------------------
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace xray.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using xray.Models;

    [Route("api/[controller]")]
    public class ApplicationController : Controller
    {

        [HttpGet("{nodeName}")]
        public Task<IEnumerable<DeployedApplicationModel>> Get(string nodeName)
        {
           // return this.clusterInfoService.GetApplicationMetrics(nodeName, null);
        }

        [HttpGet("{nodeName}/{appTypeFilter}")]
        public Task<IEnumerable<DeployedApplicationModel>> Get(string nodeName, string appTypeFilter)
        {
           // return this.clusterInfoService.GetApplicationMetrics(nodeName, appTypeFilter);
        }
    }
}