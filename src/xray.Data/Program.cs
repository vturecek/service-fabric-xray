using Microsoft.AspNetCore.Hosting;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Collections.Generic;
using System.Fabric;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace xray.Data
{
    public class Program
    {
        // Entry point for the application.
        public static void Main(string[] args)
        {
            ServiceRuntime.RegisterServiceAsync("DataType", context => new DataService(context, new ReliableStateManager(context))).GetAwaiter().GetResult();

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
