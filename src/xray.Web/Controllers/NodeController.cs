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
        
        [HttpGet("info/{nodeTypeFilter?}")]
        public Task<HttpResponseMessage> Info(string nodeTypeFilter = null)
        {
            HttpClient client = new HttpClient(new HttpServiceClientHandler());

            return client.GetAsync(new HttpServiceUriBuilder()
                .SetServiceName(new ServiceUriBuilder("Data").Build())
                .SetPartitionKey(0)
                .SetServicePathAndQuery($"api/node/info/{nodeTypeFilter ?? ""}").Build());
        }

        [HttpGet("capacity/{nodeName}")]
        public Task<HttpResponseMessage> Capacity(string nodeName)
        {
            HttpClient client = new HttpClient(new HttpServiceClientHandler());

            return client.GetAsync(new HttpServiceUriBuilder()
                .SetServiceName(new ServiceUriBuilder("Data").Build())
                .SetPartitionKey(0)
                .SetServicePathAndQuery($"api/node/capacity/{nodeName}").Build());
        }
    }
}