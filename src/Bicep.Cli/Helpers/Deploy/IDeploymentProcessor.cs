// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Deployments.Core.Definitions;
using Bicep.Core.Configuration;
using Bicep.Core.Emit;
using Bicep.Core.Utils;

namespace Bicep.Cli.Helpers.Deploy;

public interface IDeploymentProcessor
{
    Task Deploy(RootConfiguration bicepConfig, DeployCommandsConfig config, Action<DeploymentWrapperView> onRefresh, CancellationToken cancellationToken);

    Task Teardown(RootConfiguration bicepConfig, DeployCommandsConfig config, Action<GeneralOperationView> onRefresh, CancellationToken cancellationToken);

    Task<DeploymentWhatIfResponseDefinition> WhatIf(RootConfiguration bicepConfig, DeployCommandsConfig config, CancellationToken cancellationToken);
}
