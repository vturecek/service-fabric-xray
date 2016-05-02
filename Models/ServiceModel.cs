using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading.Tasks;

namespace Xray.Models
{
    public struct ServiceModel
    {
        public ServiceModel(
            string name,
            string type,
            string version,
            string status,
            string healthState,
            IEnumerable<LoadMetric> metrics)
        {
            this.Name = name;
            this.Type = type;
            this.Version = version;
            this.Status = status;
            this.HealthState = healthState;
            this.Metrics = metrics;
        }

        public string Name { get; }

        public string Status { get; }

        public string HealthState { get; }

        public string Type { get; }

        public string Version { get; }

        public IEnumerable<LoadMetric> Metrics { get; }
    }
}
