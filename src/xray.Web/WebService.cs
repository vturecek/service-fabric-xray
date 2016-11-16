// ------------------------------------------------------------
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace xray.Web
{
    using Common;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;
    using System.Collections.Generic;
    using System.Fabric;
    using System.IO;
    using System.Net.Http;

    /// <summary>
    /// A specialized stateless service for hosting ASP.NET Core web apps.
    /// </summary>
    internal sealed class WebService : StatelessService
    {
        public WebService(StatelessServiceContext serviceContext)
            : base(serviceContext)
        {
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[] 
            {
                new ServiceInstanceListener(context =>
                    new WebHostCommunicationListener(context, "ServiceEndpoint", uri =>
                        new WebHostBuilder().UseWebListener()
                                           .ConfigureServices(services =>
                                                services.AddSingleton(new HttpClient(new HttpServiceClientHandler())))
                                           .UseContentRoot(Directory.GetCurrentDirectory())
                                           .UseStartup<Startup>()
                                           .UseUrls(uri)
                                           .Build()))
            };
        }
    }
}
