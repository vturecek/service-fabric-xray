// ------------------------------------------------------------
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace xray.Controllers
{
    using Common;
    using Microsoft.AspNetCore.Mvc;
    using System.Net.Http;
    using System.Threading.Tasks;

    [Route("api/[controller]")]
    public class ApplicationController : Controller
    {
        private readonly HttpClient httpClient;

        public ApplicationController(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        [HttpGet("{nodeName}/{appTypeFilter?}")]
        public Task<HttpResponseMessage> Get(string nodeName, string appTypeFilter = null)
        {
            return this.httpClient.GetAsync(new HttpServiceUriBuilder()
                .SetServiceName(new ServiceUriBuilder("Data").Build())
                .SetPartitionKey(0)
                    .SetTarget(HttpServiceUriTarget.Primary)
                .SetServicePathAndQuery($"api/application/{nodeName}/{appTypeFilter ?? ""}").Build());
        }
    }
}