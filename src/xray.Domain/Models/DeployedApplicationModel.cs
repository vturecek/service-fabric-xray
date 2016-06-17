// ------------------------------------------------------------
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace xray.Models
{
    using System.Collections.Generic;

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