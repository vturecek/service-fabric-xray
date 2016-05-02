using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading.Tasks;

namespace Xray.Models
{
    public class ReplicaModel
    {
        public ReplicaModel(
            long id,
            Guid partitionId,
            string role,
            string status,
            string healthState,
            IEnumerable<LoadMetric> metrics)
        {
            this.Role = role;
            this.Status = status;
            this.HealthState = healthState;
            this.Id = id;
            this.PartitionId = partitionId;
            this.Metrics = metrics;
        }

        public string Role { get; }

        public string Status { get; }

        public string HealthState { get; }

        public long Id { get; }

        public Guid PartitionId { get; }

        public IEnumerable<LoadMetric> Metrics { get; }
    }
}
