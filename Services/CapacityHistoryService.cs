using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading.Tasks;
using Xray.Models;
using System.Threading;
using Query = System.Fabric.Query;
using Microsoft.ServiceFabric.Data.Collections;

namespace Xray.Services
{
    public class CapacityHistoryService : StatefulService, ICapacityHistoryService
    {
        public CapacityHistoryService(StatefulServiceContext context, IReliableStateManagerReplica stateManager)
            : base(context, stateManager)
        {
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            using (FabricClient fabricClient = new FabricClient())
            {
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    Query.ClusterLoadInformation loadInfo = await fabricClient.QueryManager.GetClusterLoadInformationAsync();

                    using (ITransaction tx = this.StateManager.CreateTransaction())
                    {
                        foreach (var capacity in loadInfo.LoadMetricInformationList)
                        {
                            IReliableDictionary<DateTimeOffset, long> dictionary = await
                                this.StateManager.GetOrAddAsync<IReliableDictionary<DateTimeOffset, long>>($"history:/hourly/{capacity.Name}");

                            await dictionary.SetAsync(tx, DateTimeOffset.UtcNow, capacity.ClusterLoad);
                        }

                        await tx.CommitAsync();
                    }

                    await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                }
            }
        }

        public async Task<IEnumerable<ClusterCapacityHistory>> GetHistory(string capacityName, DateTimeOffset startTime)
        {
            ConditionalValue<IReliableDictionary<DateTimeOffset, long>> result = await
                this.StateManager.TryGetAsync<IReliableDictionary<DateTimeOffset, long>>($"history:/hourly/{capacityName}");

            if (!result.HasValue)
            {
                throw new ArgumentException($"capacity history doesn't exist for {capacityName}", "capacityName");
            }

            IReliableDictionary<DateTimeOffset, long> dictionary = result.Value;

            using (ITransaction tx = this.StateManager.CreateTransaction())
            {
                return (await dictionary.CreateLinqAsyncEnumerable(tx))
                    .Where(x => x.Key > startTime)
                    .Select(x => new ClusterCapacityHistory(x.Value, x.Key))
                    .ToEnumerable();
            }
        }
    }
}
