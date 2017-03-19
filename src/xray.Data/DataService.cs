// ------------------------------------------------------------
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace xray.Data
{
    using Common;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Data.Collections;
    using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Fabric.Query;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    internal class DataService : Microsoft.ServiceFabric.Services.Runtime.StatefulService
    {
        private static readonly TimeSpan HourlyInterval = TimeSpan.FromMinutes(1);
        private readonly IServiceFabricQuery query;

        public DataService(StatefulServiceContext context, IReliableStateManagerReplica stateManager, IServiceFabricQuery query)
            : base(context, stateManager)
        {
            this.query = query;
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[]
            {
                new ServiceReplicaListener(context =>
                    new KestrelCommunicationListener(context, (uri, listener) =>
                        new WebHostBuilder().UseWebListener()
                                           .UseStartup<Startup>()
                                           .UseUrls(uri)
                                           .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.UseUniqueServiceUrl)
                                           .ConfigureServices(services => services
                                               .AddSingleton<IReliableStateManager>(this.StateManager)
                                               .AddSingleton<IServiceFabricQuery>(this.query))
                                           .Build()))
            };
        }


        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            int hourlyLimit = (int)Math.Round(3600D / HourlyInterval.TotalSeconds);

            IReliableDictionary<DateTimeOffset, Dictionary<string, long>> dictionary = await
                this.StateManager.GetOrAddAsync<IReliableDictionary<DateTimeOffset, Dictionary<string, long>>>($"history:/hourly");

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    ClusterLoadInformation capacities = await this.query.GetClusterLoadAsync();

                    DateTimeOffset utcnow = DateTimeOffset.UtcNow;
                    DateTimeOffset timestamp = new DateTimeOffset(utcnow.Year, utcnow.Month, utcnow.Day, utcnow.Hour, utcnow.Minute, utcnow.Second, utcnow.Offset);
                    Dictionary<string, long> values = new Dictionary<string, long>();

                    foreach (var capacity in capacities.LoadMetricInformationList)
                    {
                        values[capacity.Name] = capacity.ClusterLoad;
                    }

                    using (ITransaction tx = this.StateManager.CreateTransaction())
                    {
                        long count = await dictionary.GetCountAsync(tx);
                        if (count >= hourlyLimit)
                        {
                            var min = await (await dictionary.CreateLinqAsyncEnumerable(tx)).Min(x => x.Key);
                            await dictionary.TryRemoveAsync(tx, min);
                        }
                        
                        await dictionary.SetAsync(tx, timestamp, values);
                        await tx.CommitAsync();
                    }
                }
                catch (FabricTransientException)
                {
                    // retry
                }

                await Task.Delay(HourlyInterval, cancellationToken);
            }
        }
    }
}
