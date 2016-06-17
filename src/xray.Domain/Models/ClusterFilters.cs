// ------------------------------------------------------------
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace xray.Models
{
    using System;
    using System.Collections.Generic;

    public struct ClusterFilters
    {
        public ClusterFilters(
            IEnumerable<string> nodeTypes,
            IEnumerable<string> applicationTypes,
            IEnumerable<string> faultDomains,
            IEnumerable<string> upgradeDomains)
        {
            this.NodeTypes = nodeTypes;
            this.ApplicationTypes = applicationTypes;
            this.FaultDomains = faultDomains;
            this.UpgradeDomains = upgradeDomains;
        }

        public IEnumerable<string> NodeTypes { get; }

        public IEnumerable<string> ApplicationTypes { get; }

        public IEnumerable<string> FaultDomains { get; }

        public IEnumerable<string> UpgradeDomains { get; }
    }
}
