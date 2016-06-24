// ------------------------------------------------------------
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace xray.Controllers
{
    using Common;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using xray.Models;

    [Route("api/[controller]")]
    public class ClusterController : Controller
    {

        [HttpGet("info")]
        public Task<HttpResponseMessage> Info()
        {
            HttpClient client = new HttpClient(new HttpServiceClientHandler());

            return client.GetAsync(new HttpServiceUriBuilder()
                    .SetServiceName(new ServiceUriBuilder("Data").Build())
                    .SetPartitionKey(0)
                    .SetTarget(HttpServiceUriTarget.Primary)
                    .SetServicePathAndQuery("api/cluster/info").Build());
        }

        [HttpGet("filters")]
        public Task<HttpResponseMessage> Filters()
        {
            HttpClient client = new HttpClient(new HttpServiceClientHandler());

            return client.GetAsync(new HttpServiceUriBuilder()
                .SetServiceName(new ServiceUriBuilder("Data").Build())
                .SetPartitionKey(0)
                    .SetTarget(HttpServiceUriTarget.Primary)
                .SetServicePathAndQuery("api/cluster/filters").Build());
        }

        [HttpGet("capacity")]
        public Task<HttpResponseMessage> Capacity()
        {
            HttpClient client = new HttpClient(new HttpServiceClientHandler());

            return client.GetAsync(new HttpServiceUriBuilder()
                .SetServiceName(new ServiceUriBuilder("Data").Build())
                .SetPartitionKey(0)
                    .SetTarget(HttpServiceUriTarget.Primary)
                .SetServicePathAndQuery("api/cluster/capacity").Build());
        }

        [HttpGet("history/{capacityName}/{startTime}")]
        public Task<HttpResponseMessage> History(string capacityName, DateTimeOffset startTime)
        {
            HttpClient client = new HttpClient(new HttpServiceClientHandler());

            return client.GetAsync(new HttpServiceUriBuilder()
                .SetServiceName(new ServiceUriBuilder("Data").Build())
                .SetPartitionKey(0)
                    .SetTarget(HttpServiceUriTarget.Primary)
                .SetServicePathAndQuery($"api/cluster/history/{capacityName}/{startTime}").Build());
        }
    }
}