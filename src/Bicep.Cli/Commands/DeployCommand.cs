// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json.Nodes;
using Azure.ResourceManager.Resources.Models;
using Bicep.Cli.Arguments;
using Bicep.Cli.Commands.Helpers.Deploy;
using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.Core.AzureApi;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem;
using Bicep.Core.Utils;
using Bicep.Local.Deploy;
using Bicep.Local.Deploy.Extensibility;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;

namespace Bicep.Cli.Commands;

public class DeployCommand(
    IOContext io,
    ILogger logger,
    IEnvironment environment,
    IArmClientProvider armClientProvider,
    DiagnosticLogger diagnosticLogger,
    BicepCompiler compiler,
    LocalExtensionDispatcherFactory dispatcherFactory,
    InputOutputArgumentsResolver inputOutputArgumentsResolver) : DeploymentsCommandsBase<DeployArguments>(logger, diagnosticLogger, compiler, inputOutputArgumentsResolver)
{
    protected override async Task<int> RunInternal(DeployArguments args, Compilation compilation, SemanticModel model, ParametersResult result, CancellationToken cancellationToken)
    {
        var config = await DeploymentProcessor.GetDeployCommandsConfig(environment, args.AdditionalArguments, result);

        DeploymentState state = new(Deployments: [], IsActive: true);
        await Task.WhenAll([
            RenderLoop(TimeSpan.FromMilliseconds(50), () => state, cancellationToken),
            ProcessDeployment(model, config, newState => state = newState, cancellationToken),
        ]);

        return DeploymentProcessor.IsSuccess(state.Deployments) ? 0 : 1;
    }

    protected override async Task<int> RunOrchestrationInternal(DeployArguments args, Compilation compilation, SemanticModel model, CancellationToken cancellationToken)
    {
        var parameters = compilation.Emitter.Parameters();

        if (parameters.Template?.Template is not { } templateContent ||
            parameters.Parameters is not { } parametersContent)
        {
            return 1;
        }

        var renderer = new DeploymentRenderer();
        void Refresh(LocalDeploymentResult result, Action<DeploymentState> onUpdate)
        {
            var (deployment, operations) = result;

            onUpdate(new(
                Deployments: [new(
                    Id: deployment.Id,
                    Name: deployment.Name,
                    StartTime: deployment.Properties.Timestamp!.Value,
                    EndTime: DeploymentProcessor.IsTerminal(deployment.Properties.ProvisioningState!.Value.ToString()) ? deployment.Properties.Timestamp!.Value.Add(deployment.Properties.Duration!.Value) : null,
                    Operations: [..operations.Where(x => x.Properties.TargetResource is { }).Select(op => new DeploymentOperationView(
                        Id: "",
                        Name: op.Properties.TargetResource.SymbolicName,
                        Type: "",
                        State: op.Properties.ProvisioningState.ToString(),
                        StartTime: op.Properties.Timestamp!.Value,
                        EndTime: DeploymentProcessor.IsTerminal(op.Properties.ProvisioningState.ToString()) ? op.Properties.Timestamp!.Value.Add(op.Properties.Duration!.Value) : null,
                        Error: op.Properties.StatusMessage?.FromJToken<StatusMessage>()?.Error is { } error ? $"{error.Code}: {error.Message}" : null))],
                    IsEntryPoint: true,
                    State: deployment.Properties.ProvisioningState.ToString()!,
                    Error: deployment.Properties.Error is { } error ? $"{error.Code}: {error.Message}" : null,
                    Outputs: deployment.Properties.Outputs is { } outputs ? outputs.ToImmutableDictionary(
                        kvp => kvp.Key,
                        kvp => JsonNode.Parse(kvp.Value.Value.ToJson())!) : ImmutableDictionary<string, JsonNode>.Empty)
                    ],
                IsActive: DeploymentProcessor.IsTerminal(deployment.Properties.ProvisioningState.ToString()!) == false));
        }

        async Task ProcessLocal(Action<DeploymentState> onUpdate)
        {
            // this using block is intentional to ensure that the dispatcher completes running before we write the summary
            LocalDeploymentResult localDeploymentResult;
            await using (var dispatcher = dispatcherFactory.Create())
            {
                await dispatcher.InitializeExtensions(compilation);
                localDeploymentResult = await dispatcher.Deploy(templateContent, parametersContent, cancellationToken, x => Refresh(x, onUpdate));
            }

            Refresh(localDeploymentResult, onUpdate);
        }

        DeploymentState state = new(Deployments: [], IsActive: true);
        await Task.WhenAll([
            RenderLoop(TimeSpan.FromMilliseconds(50), () => state, cancellationToken),
            ProcessLocal(newState => state = newState),
        ]);

        return 0;
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
            await io.Output.FlushAsync(cancellationToken);
            await Task.Delay(refreshInterval, cancellationToken);
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