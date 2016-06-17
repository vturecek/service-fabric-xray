using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace xray.Models
{
    public struct ClusterCapacityHistory
    {
        public long Data { get; }

        public DateTimeOffset Timestamp { get; }

        public ClusterCapacityHistory(long data, DateTimeOffset timestamp)
        {
            this.Data = data;
            this.Timestamp = timestamp;
        }
    }
}
