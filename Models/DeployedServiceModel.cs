// ------------------------------------------------------------
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Xray.Models
{
    using System.Collections.Generic;

    public struct DeployedServiceModel
    {
        public DeployedServiceModel(
            ServiceModel service,
            IEnumerable<ReplicaModel> replicas)
        {
            this.Service = service;
            this.Replicas = replicas;
        }

        public ServiceModel Service { get; }

        public IEnumerable<ReplicaModel> Replicas { get; }
    }
}