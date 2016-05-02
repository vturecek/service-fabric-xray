using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Xray.Models
{
    public struct DeployedApplicationModel
    {
        public DeployedApplicationModel(
            ApplicationModel application,
            IEnumerable<DeployedServiceModel> services)
        {
            this.Application = application;
            this.Services = services;
        }

        public ApplicationModel Application { get; }

        public IEnumerable<DeployedServiceModel> Services { get; }
    }
}
