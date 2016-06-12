// ------------------------------------------------------------
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace xray.Web
{
    using Microsoft.AspNetCore.Hosting;
    using System;
    using System.IO;

    public class LocalServer : IDisposable
    {
        private IWebHost webHost;
        
        public void Open()
        {
            string serverUrl = "http://localhost:5000";

            this.webHost = new WebHostBuilder().UseKestrel()
                                           .UseContentRoot(Directory.GetCurrentDirectory())
                                           .UseStartup<Startup>()
                                           .UseUrls(serverUrl)
                                           .Build();

            this.webHost.Start();
        }

        public void Dispose()
        {
            this.webHost?.Dispose();
        }
    }
}
