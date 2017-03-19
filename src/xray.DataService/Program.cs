// ------------------------------------------------------------
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace xray.DataService
{
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Services.Runtime;
    using System.Fabric;
    using System.Threading;

    public class Program
    {
        // Entry point for the application.
        public static void Main(string[] args)
        {
            ServiceRuntime.RegisterServiceAsync("DataServiceType", context =>
                new DataService(
                    context,
                    new ReliableStateManager(context),
                    new FabricClientServiceFabricQuery(new FabricClient())))
                .GetAwaiter().GetResult();

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
