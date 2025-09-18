// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Cli.Arguments;
using Bicep.Cli.Commands.Helpers.Deploy;
using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.Core.AzureApi;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Bicep.Core.Utils;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Commands;

public class DeployCommand(
    IOContext io,
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

        DeploymentState state = new(Deployments: [], IsActive: true);
        await Task.WhenAll([
            RenderLoop(TimeSpan.FromMilliseconds(50), () => state, cancellationToken),
            ProcessDeployment(model, config, newState => state = newState, cancellationToken),
        ]);

        return DeploymentProcessor.IsSuccess(state.Deployments) ? 0 : 1;
    }

    private record DeploymentState(ImmutableArray<DeploymentView> Deployments, bool IsActive);

    private async Task ProcessDeployment(SemanticModel model, DeployCommandsConfig config, Action<DeploymentState> onUpdate, CancellationToken cancellationToken)
    {
        var armClient = armClientProvider.CreateArmClient(model.Configuration, null);
        var processor = new DeploymentProcessor(armClient);

        ImmutableArray<DeploymentView> currentDeployments = [];
        try
        {
            await processor.Deploy(config, deployments =>
            {
                currentDeployments = deployments;
                onUpdate(new(Deployments: currentDeployments, IsActive: true));
            }, cancellationToken);
        }
        finally
        {
            // ensure we always set IsActive to false, so that the render loop exits even on error
            onUpdate(new(Deployments: currentDeployments, IsActive: false));
        }
    }

    private async Task RenderLoop(TimeSpan refreshInterval, Func<DeploymentState> getCurrentState, CancellationToken cancellationToken)
    {
        var lineCount = 0;
        var isActive = true;
        while (isActive)
        {
            (var deployments, isActive) = getCurrentState();
            var output = DeploymentRenderer.Format(DateTime.UtcNow, deployments, lineCount);
            lineCount = output.Count(c => c == '\n');
            await io.Output.WriteAsync(output);
            await Task.Delay(refreshInterval, cancellationToken);
            await io.Output.FlushAsync(cancellationToken);
        }

        {
            // Always render the final state before exiting
            (var deployments, _) = getCurrentState();
            var finalOutput = DeploymentRenderer.Format(DateTime.UtcNow, deployments, lineCount);
            await io.Output.WriteAsync(finalOutput);
            await io.Output.FlushAsync(cancellationToken);
        }
    }
}
