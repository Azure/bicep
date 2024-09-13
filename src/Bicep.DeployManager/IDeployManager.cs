// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Deployments.Core.Definitions;

namespace Bicep.DeployManager
{
    public interface IDeployManager
    {
        public Task StartDeploymentAsync(DeploymentContent deploymentContent, CancellationToken cancellationToken);
    }
}
