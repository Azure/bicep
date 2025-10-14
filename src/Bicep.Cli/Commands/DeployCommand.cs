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

public class DeployCommand(
    DeploymentRenderer deploymentRenderer,
    IDeploymentProcessor deploymentProcessor,
    ILogger logger,
    IEnvironment environment,
    DiagnosticLogger diagnosticLogger,
    BicepCompiler compiler,
    InputOutputArgumentsResolver inputOutputArgumentsResolver) : DeploymentsCommandsBase<DeployArguments>(logger, diagnosticLogger, compiler, inputOutputArgumentsResolver)
{
    protected override async Task<int> RunInternal(DeployArguments args, SemanticModel model, ParametersResult result, CancellationToken cancellationToken)
    {
        var config = await DeploymentProcessor.GetDeployCommandsConfig(environment, args.AdditionalArguments, result, model.TargetScope);

        var success = await deploymentRenderer.RenderDeployment(
            DeploymentRenderer.RefreshInterval,
            (onUpdate) => deploymentProcessor.Deploy(model.Configuration, config, onUpdate, cancellationToken),
            args.OutputFormat ?? DeploymentOutputFormat.Default,
            cancellationToken);

        return success ? 0 : 1;
    }
}
