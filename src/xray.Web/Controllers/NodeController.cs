// ------------------------------------------------------------
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace xray.Controllers
{
    using Common;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using xray.Models;

    [Route("api/[controller]")]
    public class NodeController : Controller
    {
        private readonly HttpClient httpClient;

        public NodeController(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        [HttpGet("info/{nodeTypeFilter?}")]
        public Task<HttpResponseMessage> Info(string nodeTypeFilter = null)
        {
            return this.httpClient.GetAsync(new HttpServiceUriBuilder()
                .SetServiceName(new ServiceUriBuilder("Data").Build())
                .SetPartitionKey(0)
                    .SetTarget(HttpServiceUriTarget.Primary)
                .SetServicePathAndQuery($"api/node/info/{nodeTypeFilter ?? ""}").Build());
        }

        [HttpGet("capacity/{nodeName}")]
        public Task<HttpResponseMessage> Capacity(string nodeName)
        {
            return this.httpClient.GetAsync(new HttpServiceUriBuilder()
                .SetServiceName(new ServiceUriBuilder("Data").Build())
                .SetPartitionKey(0)
                    .SetTarget(HttpServiceUriTarget.Primary)
                .SetServicePathAndQuery($"api/node/capacity/{nodeName}").Build());
        }
    }
}