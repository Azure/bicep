// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Deployments.Core.Definitions;
using Bicep.Core.Configuration;
using Bicep.Core.Emit;
using Bicep.Core.Utils;

namespace Bicep.Cli.Helpers.Deploy;

public interface IDeploymentProcessor
{
    Task Deploy(IBicepConfiguration bicepConfig, DeployCommandsConfig config, Action<DeploymentWrapperView> onRefresh, CancellationToken cancellationToken);

    Task Teardown(IBicepConfiguration bicepConfig, DeployCommandsConfig config, Action<GeneralOperationView> onRefresh, CancellationToken cancellationToken);

    Task<DeploymentWhatIfResponseDefinition> WhatIf(IBicepConfiguration bicepConfig, DeployCommandsConfig config, CancellationToken cancellationToken);
}
