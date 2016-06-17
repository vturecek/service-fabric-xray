// ------------------------------------------------------------
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace xray.Models
{
    using System;
    using System.Collections.Generic;

    public struct ClusterInfo
    {
        public ClusterInfo(
            string healthStatus,
            string version,
            long nodeTypes,
            long applicationTypes,
            long faultDomains,
            long upgradeDomains,
            long nodes,
            long applications,
            long services,
            long partitions,
            long replicas,
            DateTimeOffset lastBalanceStartTime,
            DateTimeOffset lastBalanceEndTime)
        {
            this.HealthStatus = healthStatus;
            this.Version = version;
            this.NodeTypes = nodeTypes;
            this.ApplicationTypes = applicationTypes;
            this.FaultDomains = faultDomains;
            this.UpgradeDomains = upgradeDomains;
            this.Nodes = nodes;
            this.Applications = applications;
            this.Services = services;
            this.Partitions = partitions;
            this.Replicas = replicas;
            this.LastBalanceStartTime = lastBalanceStartTime;
            this.LastBalanceEndTime = lastBalanceEndTime;
        }

        public string HealthStatus { get; }

        public string Version { get; }

        public long NodeTypes { get; }

        public long ApplicationTypes { get; }

        public long FaultDomains { get; }

        public long UpgradeDomains { get; }

        public long Nodes { get; }

        public long Applications { get; }

        public long Services { get; }

        public long Partitions { get; }

        public long Replicas { get; }

        public DateTimeOffset LastBalanceStartTime { get; }

        public DateTimeOffset LastBalanceEndTime { get; }
    }
}
