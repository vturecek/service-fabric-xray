using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Xray.Models
{
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
