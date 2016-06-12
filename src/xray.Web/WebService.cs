// ------------------------------------------------------------
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace xray.Web
{
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;
    using System.Collections.Generic;
    using System.Fabric;

    /// <summary>
    /// A specialized stateless service for hosting ASP.NET Core web apps.
    /// </summary>
    internal sealed class WebHostingService : StatelessService
    {
        public WebHostingService(StatelessServiceContext serviceContext)
            : base(serviceContext)
        {
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[] { new ServiceInstanceListener(context => new WebCommunicationListener(context, "ServiceEndpoint")) };
        }
    }
}
