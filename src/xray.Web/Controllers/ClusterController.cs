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
        private readonly HttpClient httpClient;

        public ClusterController(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        [HttpGet("info")]
        public Task<HttpResponseMessage> Info()
        {
            return this.httpClient.GetAsync(new HttpServiceUriBuilder()
                    .SetServiceName(new ServiceUriBuilder("Data").Build())
                    .SetPartitionKey(0)
                    .SetTarget(HttpServiceUriTarget.Primary)
                    .SetServicePathAndQuery("api/cluster/info").Build());
        }

        [HttpGet("filters")]
        public Task<HttpResponseMessage> Filters()
        {
            return this.httpClient.GetAsync(new HttpServiceUriBuilder()
                .SetServiceName(new ServiceUriBuilder("Data").Build())
                .SetPartitionKey(0)
                    .SetTarget(HttpServiceUriTarget.Primary)
                .SetServicePathAndQuery("api/cluster/filters").Build());
        }

        [HttpGet("capacity")]
        public Task<HttpResponseMessage> Capacity()
        {
            return this.httpClient.GetAsync(new HttpServiceUriBuilder()
                .SetServiceName(new ServiceUriBuilder("Data").Build())
                .SetPartitionKey(0)
                    .SetTarget(HttpServiceUriTarget.Primary)
                .SetServicePathAndQuery("api/cluster/capacity").Build());
        }

        [HttpGet("history/{capacityName}/{startTime}")]
        public Task<HttpResponseMessage> History(string capacityName, DateTimeOffset startTime)
        {
            return this.httpClient.GetAsync(new HttpServiceUriBuilder()
                .SetServiceName(new ServiceUriBuilder("Data").Build())
                .SetPartitionKey(0)
                    .SetTarget(HttpServiceUriTarget.Primary)
                .SetServicePathAndQuery($"api/cluster/history/{capacityName}/{startTime.ToString("o")}").Build());
        }
    }
}