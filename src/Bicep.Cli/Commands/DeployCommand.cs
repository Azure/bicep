// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers.Deploy;
using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.Core.AzureApi;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Bicep.Core.Utils;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Commands;

public class DeployCommand(
    DeploymentRenderer deploymentRenderer,
    ILogger logger,
    IEnvironment environment,
    IArmClientProvider armClientProvider,
    DiagnosticLogger diagnosticLogger,
    BicepCompiler compiler,
    InputOutputArgumentsResolver inputOutputArgumentsResolver) : DeploymentsCommandsBase<DeployArguments>(logger, diagnosticLogger, compiler, inputOutputArgumentsResolver)
{
    protected override async Task<int> RunInternal(DeployArguments args, SemanticModel model, ParametersResult result, CancellationToken cancellationToken)
    {
        var config = await DeploymentProcessor.GetDeployCommandsConfig(environment, args.AdditionalArguments, result);

        var success = await deploymentRenderer.RenderDeployment(
            TimeSpan.FromMilliseconds(50),
            (onUpdate) => ProcessDeployment(model, config, onUpdate, cancellationToken),
            cancellationToken);

        return success ? 0 : 1;
    }

    private async Task ProcessDeployment(SemanticModel model, DeployCommandsConfig config, Action<DeploymentWrapperView> onUpdate, CancellationToken cancellationToken)
    {
        try
        {
            var armClient = armClientProvider.CreateArmClient(model.Configuration, null);
            var processor = new DeploymentProcessor(armClient);

            await processor.Deploy(config, deployment => onUpdate(deployment), cancellationToken);
        }
        catch (Exception exception)
        {
            onUpdate(new(null, exception.Message));
        }
    }
}