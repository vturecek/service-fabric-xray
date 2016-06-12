// ------------------------------------------------------------
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace xray.Web
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using System.Fabric;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public class WebCommunicationListener : ICommunicationListener
    {
        private readonly string endpointName;

        private readonly StatelessServiceContext serviceContext;

        private IWebHost webHost;

        public WebCommunicationListener(StatelessServiceContext serviceContext, string endpointName)
        {
            this.serviceContext = serviceContext;
            this.endpointName = endpointName;
        }
        

        void ICommunicationListener.Abort()
        {
            this.webHost?.Dispose();
        }

        Task ICommunicationListener.CloseAsync(CancellationToken cancellationToken)
        {
            this.webHost?.Dispose();

            return Task.FromResult(true);
        }

        Task<string> ICommunicationListener.OpenAsync(CancellationToken cancellationToken)
        {
            var endpoint = this.serviceContext.CodePackageActivationContext.GetEndpoint(endpointName);

            string serverUrl = $"{endpoint.Protocol}://{FabricRuntime.GetNodeContext().IPAddressOrFQDN}:{endpoint.Port}";

            this.webHost = new WebHostBuilder().UseKestrel()
                                           .UseContentRoot(Directory.GetCurrentDirectory())
                                           .UseStartup<Startup>()
                                           .UseUrls(serverUrl)
                                           .Build();

            this.webHost.Start();

            return Task.FromResult(serverUrl);
        }
        
    }
}
