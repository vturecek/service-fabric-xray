using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xray.Models;

namespace Xray.Services
{
    public interface ICapacityHistoryService
    {
        Task<IEnumerable<ClusterCapacityHistory>> GetHistory(string capacityName, DateTimeOffset startTime);
    }
}
