// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json.Nodes;
using Azure.ResourceManager.Resources.Models;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers.Deploy;
using Bicep.Cli.Logging;
using Bicep.Core;
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
    DeploymentRenderer deploymentRenderer,
    IDeploymentProcessor deploymentProcessor,
    ILogger logger,
    IEnvironment environment,
    DiagnosticLogger diagnosticLogger,
    BicepCompiler compiler,
    LocalExtensionDispatcherFactory dispatcherFactory,
    InputOutputArgumentsResolver inputOutputArgumentsResolver) : DeploymentsCommandsBase<DeployArguments>(logger, diagnosticLogger, compiler, inputOutputArgumentsResolver)
{
    protected override async Task<int> RunInternal(DeployArguments args, Compilation compilation, SemanticModel model, ParametersResult result, CancellationToken cancellationToken)
    {
        var config = await DeploymentProcessor.GetDeployCommandsConfig(environment, args.AdditionalArguments, result, model.TargetScope);

        var success = await deploymentRenderer.RenderDeployment(
            DeploymentRenderer.RefreshInterval,
            (onUpdate) => deploymentProcessor.Deploy(model.Configuration, config, onUpdate, cancellationToken),
            args.OutputFormat ?? DeploymentOutputFormat.Default,
            cancellationToken);

        return success ? 0 : 1;
    }

    protected override async Task<int> RunOrchestrationInternal(DeployArguments args, Compilation compilation, SemanticModel model, CancellationToken cancellationToken)
    {
        var parameters = compilation.Emitter.Parameters();

        if (parameters.Template?.Template is not { } templateContent ||
            parameters.Parameters is not { } parametersContent)
        {
            return 1;
        }

        async Task ProcessLocal(Action<DeploymentWrapperView> onUpdate)
        {
            try
            {
                await using var dispatcher = dispatcherFactory.Create();

                await dispatcher.InitializeExtensions(compilation);
                await dispatcher.Deploy(templateContent, parametersContent, result => onUpdate(new(DeploymentProcessor.GetDeploymentView(result.Deployment, result.Operations), null)), cancellationToken);
            }
            catch (Exception exception)
            {
                onUpdate(new(null, exception.Message));
            }
        }

        var success = await deploymentRenderer.RenderDeployment(
            DeploymentRenderer.RefreshInterval,
            (onUpdate) => ProcessLocal(onUpdate),
            args.OutputFormat ?? DeploymentOutputFormat.Default,
            cancellationToken);

        return success ? 0 : 1;
    }

    private record DeploymentState(ImmutableArray<DeploymentView> Deployments, bool IsActive);
}