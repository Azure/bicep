// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers.Deploy;
using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Bicep.Core.Utils;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Commands;

public class TeardownCommand(
    IDeploymentProcessor deploymentProcessor,
    DeploymentRenderer deploymentRenderer,
    ILogger logger,
    IEnvironment environment,
    DiagnosticLogger diagnosticLogger,
    BicepCompiler compiler,
    InputOutputArgumentsResolver inputOutputArgumentsResolver) : DeploymentsCommandsBase<TeardownArguments>(logger, diagnosticLogger, compiler, inputOutputArgumentsResolver)
{
    protected override async Task<int> RunInternal(TeardownArguments args, SemanticModel model, ParametersResult result, CancellationToken cancellationToken)
    {
        var config = await DeploymentProcessor.GetDeployCommandsConfig(environment, args.AdditionalArguments, result, model.TargetScope);

        var success = await deploymentRenderer.RenderOperation(
            DeploymentRenderer.RefreshInterval,
            (onUpdate) => deploymentProcessor.Teardown(model.Configuration, config, onUpdate, cancellationToken),
            cancellationToken);

        return success ? 0 : 1;
    }
}
