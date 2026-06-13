// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.CommandLine;
using Azure.Core;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Core.Entities;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using Bicep.Cli.Arguments;
using Bicep.Cli.Constants;
using Bicep.Cli.Helpers;
using Bicep.Cli.Helpers.Deploy;
using Bicep.Cli.Helpers.WhatIf;
using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.Core.AzureApi;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem;
using Bicep.Core.Utils;
using Bicep.Core.Utils.Snapshots;
using Bicep.Local.Deploy.Extensibility;
using Json.More;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;
using Option = Bicep.Cli.Constants.Option;

namespace Bicep.Cli.Commands;

public class OrchestrateCommand(
    IOContext io,
    DeploymentRenderer deploymentRenderer,
    IDeploymentProcessor deploymentProcessor,
    IArmClientProvider armClientProvider,
    ILogger logger,
    DiagnosticLogger diagnosticLogger,
    BicepCompiler compiler,
    LocalExtensionDispatcherFactory dispatcherFactory,
    InputOutputArgumentsResolver inputOutputArgumentsResolver) : ICommand
{
    private record OrchestrateArguments(
        string InputFile,
        bool NoRestore,
        ImmutableDictionary<string, string> AdditionalArguments,
        DeploymentOutputFormat? OutputFormat) : IInputArguments;

    private record DeployArguments(
        string InputFile,
        bool NoRestore,
        ImmutableDictionary<string, string> AdditionalArguments,
        DeploymentOutputFormat? OutputFormat) : OrchestrateArguments(InputFile, NoRestore, AdditionalArguments, OutputFormat);

    private record WhatIfArguments(
        string InputFile,
        bool NoRestore,
        ImmutableDictionary<string, string> AdditionalArguments,
        DeploymentOutputFormat? OutputFormat) : OrchestrateArguments(InputFile, NoRestore, AdditionalArguments, OutputFormat);

    private record TeardownArguments(
        string InputFile,
        bool NoRestore,
        ImmutableDictionary<string, string> AdditionalArguments,
        DeploymentOutputFormat? OutputFormat) : OrchestrateArguments(InputFile, NoRestore, AdditionalArguments, OutputFormat);

    private async Task<Compilation> GetCompilation(OrchestrateArguments args)
    {
        var paramsFileUri = inputOutputArgumentsResolver.ResolveInputArguments(args);
        ArgumentHelper.ValidateBicepParamFile(paramsFileUri);

        var compilation = await compiler.CreateCompilation(paramsFileUri, skipRestore: args.NoRestore);
        var model = compilation.GetEntrypointSemanticModel();

        if (model.TargetScope != ResourceScope.Orchestrator)
        {
            throw new CommandLineException($"The target scope of the Bicep file must be 'orchestrator'.");
        }

        return compilation;
    }

    internal static System.CommandLine.Command CreateCommand(CommandLineBuilderContext context)
    {
        var command = new System.CommandLine.Command(Constants.Command.Orchestrate, "[Experimental] Orchestrates a complex infrastructure deployment.") { TreatUnmatchedTokensAsErrors = true };
        var deployCommand = new System.CommandLine.Command(Constants.Command.Deploy, "[Experimental] Deploys resources.") { TreatUnmatchedTokensAsErrors = false };
        var whatIfCommand = new System.CommandLine.Command(Constants.Command.WhatIf, "[Experimental] Performs a what-if analysis.") { TreatUnmatchedTokensAsErrors = false };
        var teardownCommand = new System.CommandLine.Command(Constants.Command.Teardown, "[Experimental] Tears down resources.") { TreatUnmatchedTokensAsErrors = false };

        command.Add(deployCommand);
        command.Add(whatIfCommand);
        command.Add(teardownCommand);

        var inputFileArgument = new Argument<string>(Constants.Argument.ParametersFile)
        {
            Description = "The path to the .bicepparam file.",
        };
        var noRestoreOption = new System.CommandLine.Option<bool>(Option.NoRestore)
        {
            Description = "Do not restore modules prior to deploying.",
        };
        var formatOption = new Option<DeploymentOutputFormat?>(Option.Format)
        {
            Description = "Output format for deployment results (Default, Json).",
        };

        deployCommand.Add(inputFileArgument);
        deployCommand.Add(noRestoreOption);
        deployCommand.Add(formatOption);
        deployCommand.SetAction((result, ct) => context.RunCommandAsync(async () =>
        {
            var additionalArguments = CommandLineBuilderContext.ParseAdditionalArguments(result.UnmatchedTokens);
            var args = new DeployArguments(
                result.GetRequiredValue(inputFileArgument),
                result.GetValue(noRestoreOption),
                additionalArguments,
                result.GetValue(formatOption));

            return await context.GetCommand<OrchestrateCommand>().Deploy(args, ct);
        }));

        whatIfCommand.Add(inputFileArgument);
        whatIfCommand.Add(noRestoreOption);
        whatIfCommand.Add(formatOption);
        whatIfCommand.SetAction((result, ct) => context.RunCommandAsync(async () =>
        {
            var additionalArguments = CommandLineBuilderContext.ParseAdditionalArguments(result.UnmatchedTokens);
            var args = new WhatIfArguments(
                result.GetRequiredValue(inputFileArgument),
                result.GetValue(noRestoreOption),
                additionalArguments,
                result.GetValue(formatOption));

            return await context.GetCommand<OrchestrateCommand>().WhatIf(args, ct);
        }));

        teardownCommand.Add(inputFileArgument);
        teardownCommand.Add(noRestoreOption);
        teardownCommand.Add(formatOption);
        teardownCommand.SetAction((result, ct) => context.RunCommandAsync(async () =>
        {
            var additionalArguments = CommandLineBuilderContext.ParseAdditionalArguments(result.UnmatchedTokens);
            var args = new TeardownArguments(
                result.GetRequiredValue(inputFileArgument),
                result.GetValue(noRestoreOption),
                additionalArguments,
                result.GetValue(formatOption));

            return await context.GetCommand<OrchestrateCommand>().Teardown(args, ct);
        }));        

        return command;
    }

    private async Task<int> Deploy(DeployArguments args, CancellationToken cancellationToken)
    {
        var compilation = await GetCompilation(args);
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

    private async Task<int> WhatIf(WhatIfArguments args, CancellationToken cancellationToken)
    {
        var compilation = await GetCompilation(args);
        var model = compilation.GetEntrypointSemanticModel();

        var parameters = compilation.Emitter.Parameters();

        if (parameters.Template?.Template is not { } templateContent ||
            parameters.Parameters is not { } parametersContent)
        {
            return 1;
        }

        var snapshot = await SnapshotHelper.GetSnapshot(
            targetScope: model.TargetScope,
            templateContent: templateContent,
            parametersContent: parametersContent,
            tenantId: null,
            managementGroupId: null,
            subscriptionId: null,
            resourceGroup: null,
            location: null,
            deploymentName: null,
            cancellationToken: cancellationToken,
            includeSymbolicNames: true,
            externalInputs: []);

        var armClient = armClientProvider.CreateArmClient(model.Configuration, null);

        var hasFailures = false;
        foreach (var stackResource in snapshot.PredictedResources)
        {
            var symbolicName = stackResource.GetPropertyByPath("symbolicName").GetString()!;
            var stackParameters = stackResource.GetPropertyByPath("properties.parameters").GetString()!;
            var sourceUriString = stackResource.GetPropertyByPath("properties.sourceUri").GetString()!;
            var region = stackResource.GetPropertyByPath("properties.body.region").GetString()!;
            var inputs = stackResource.GetPropertyByPath("properties.body.inputs");

            var sourceUri = new Uri(sourceUriString).ToIOUri();

            var paramsResult = await NestedDeploymentExtension.ProcessParameters(stackParameters, inputs.ToJsonString().FromJson<JObject>());
            var deploymentName = paramsResult.UsingConfig.Name ?? "main";

            var stackCompilation = await compiler.CreateCompilation(sourceUri);
            var stackSummary = diagnosticLogger.LogDiagnostics(DiagnosticOptions.Default, stackCompilation);

            if (stackSummary.HasErrors ||
                stackCompilation.Emitter.Parameters() is not { } stackResult ||
                stackResult.Template?.Template is not { } stackTemplateContent ||
                stackResult.Parameters is not { } stackParametersContent)
            {
                hasFailures = true;
                continue;
            }

            var deploymentResource = armClient.GetArmDeploymentResource(new ResourceIdentifier($"{paramsResult.UsingConfig.Scope}/providers/Microsoft.Resources/deployments/{deploymentName}"));
            var paramsDefinition = paramsResult.Parameters.FromJson<DeploymentParametersDefinition>();
            var deploymentProperties = new ArmDeploymentWhatIfProperties(ArmDeploymentMode.Incremental)
            {
                Template = BinaryData.FromString(stackTemplateContent),
                Parameters = BinaryData.FromString(paramsDefinition.Parameters.ToJson()),
            };
            var armDeploymentContent = new ArmDeploymentWhatIfContent(deploymentProperties);

            var response = await deploymentResource.WhatIfAsync(Azure.WaitUntil.Completed, armDeploymentContent, cancellationToken);

            var definition = response.GetRawResponse().Content.ToString().FromJson<DeploymentWhatIfResponseDefinition>();
            var changes = definition.Properties.Changes.Where(x => x.ChangeType != DeploymentWhatIfChangeType.Ignore);

            logger.LogInformation($"WhatIf results for Stack {symbolicName} - {sourceUri}:");
            await io.Output.Writer.WriteAsync(WhatIfOperationResultFormatter.Format([.. changes]));
        }

        return hasFailures ? 1 : 0;
    }

    private async Task<int> Teardown(TeardownArguments args, CancellationToken cancellationToken)
    {
        var compilation = await GetCompilation(args);
        var model = compilation.GetEntrypointSemanticModel();

        var parameters = compilation.Emitter.Parameters();

        if (parameters.Template?.Template is not { } templateContent ||
            parameters.Parameters is not { } parametersContent)
        {
            return 1;
        }

        var snapshot = await SnapshotHelper.GetSnapshot(
            targetScope: model.TargetScope,
            templateContent: templateContent,
            parametersContent: parametersContent,
            tenantId: null,
            managementGroupId: null,
            subscriptionId: null,
            resourceGroup: null,
            location: null,
            deploymentName: null,
            includeSymbolicNames: true,
            cancellationToken: cancellationToken,
            externalInputs: []);

        var stacksConfigs = new List<DeployCommandsConfig>();
        var hasFailures = false;
        foreach (var stackResource in snapshot.PredictedResources)
        {
            var symbolicName = stackResource.GetPropertyByPath("symbolicName").GetString()!;
            var stackParameters = stackResource.GetPropertyByPath("properties.parameters").GetString()!;
            var sourceUriString = stackResource.GetPropertyByPath("properties.sourceUri").GetString()!;
            var region = stackResource.GetPropertyByPath("properties.body.region").GetString()!;
            var inputs = stackResource.GetPropertyByPath("properties.body.inputs");

            var sourceUri = new Uri(sourceUriString).ToIOUri();

            var paramsResult = await NestedDeploymentExtension.ProcessParameters(stackParameters, inputs.ToJsonString().FromJson<JObject>());
            var deploymentName = paramsResult.UsingConfig.Name ?? "main";

            var stackCompilation = await compiler.CreateCompilation(sourceUri);
            var stackSummary = diagnosticLogger.LogDiagnostics(DiagnosticOptions.Default, stackCompilation);

            if (stackSummary.HasErrors ||
                stackCompilation.Emitter.Parameters() is not { } stackResult ||
                stackResult.Template?.Template is not { } stackTemplateContent ||
                stackResult.Parameters is not { } stackParametersContent)
            {
                hasFailures = true;
                continue;
            }

            stacksConfigs.Add(new(stackTemplateContent, stackParametersContent, new(
                paramsResult.UsingConfig.Name,
                paramsResult.UsingConfig.Scope,
                region,
                stackCompilation.GetEntrypointSemanticModel().TargetScope,
                paramsResult.UsingConfig.StacksConfig is { } stacksConfig ? new(
                    stacksConfig.Description,
                    stacksConfig.ActionOnUnmanage,
                    stacksConfig.DenySettings) : null)));
        }

        if (hasFailures)
        {
            return 1;
        }

        var armClient = armClientProvider.CreateArmClient(model.Configuration, null);

        var success = await deploymentRenderer.RenderOperation(
            DeploymentRenderer.RefreshInterval,
            async (onUpdate) =>
            {
                var hasFailures = false;
                onUpdate(new("Teardown", "Running", null));

                await Task.WhenAll(stacksConfigs.Select(async config =>
                {
                    await deploymentProcessor.Teardown(model.Configuration, config, _ => { }, cancellationToken);
                }));

                onUpdate(new("Teardown", hasFailures ? "Failed" : "Succeeded", null));
            },
            cancellationToken);

        return success ? 0 : 1;
    }
}
