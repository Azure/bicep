// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Deployments.Core.Definitions;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using Bicep.Core.Models;

namespace Bicep.Deploy
{
    public interface IDeploymentManager
    {
        public Task<ArmDeploymentResource> CreateOrUpdateAsync(ArmDeploymentDefinition deploymentDefinition, CancellationToken cancellationToken);

        public Task<ArmDeploymentResource> CreateOrUpdateAsync(ArmDeploymentDefinition deploymentDefinition, Action<ImmutableSortedSet<ArmDeploymentOperation>>? onOperationsUpdated, CancellationToken cancellationToken);

        public Task<ArmDeploymentValidateResult> ValidateAsync(ArmDeploymentDefinition deploymentDefinition, CancellationToken cancellationToken);

        public Task<WhatIfOperationResult> WhatIfAsync(ArmDeploymentDefinition deploymentDefinition, CancellationToken cancellationToken);
    }
}
