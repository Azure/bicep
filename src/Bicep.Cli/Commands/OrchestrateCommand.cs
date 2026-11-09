// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.CommandLine;
using Azure.Core;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Core.Entities;
using Bicep.Cli.Arguments;
using Bicep.Cli.Constants;
using Bicep.Cli.Helpers;
using Bicep.Cli.Helpers.Deploy;
using Bicep.Cli.Helpers.WhatIf;
using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem;
using Bicep.Local.Deploy.Extensibility;
using Json.More;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Cli.Commands;

public class OrchestrateCommand(
    IOContext io,
    DeploymentRenderer deploymentRenderer,
    ArmDeploymentProviderFactory deploymentProviderFactory,
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
        DeploymentOutputFormat? OutputFormat,
        ImmutableDictionary<string, ImmutableHashSet<string>> Labels) : IInputArguments;

    private record DeployArguments(
        string InputFile,
        bool NoRestore,
        ImmutableDictionary<string, string> AdditionalArguments,
        DeploymentOutputFormat? OutputFormat,
        ImmutableDictionary<string, ImmutableHashSet<string>> Labels) : OrchestrateArguments(InputFile, NoRestore, AdditionalArguments, OutputFormat, Labels);

    private record WhatIfArguments(
        string InputFile,
        bool NoRestore,
        ImmutableDictionary<string, string> AdditionalArguments,
        DeploymentOutputFormat? OutputFormat,
        ImmutableDictionary<string, ImmutableHashSet<string>> Labels) : OrchestrateArguments(InputFile, NoRestore, AdditionalArguments, OutputFormat, Labels);

    private record TeardownArguments(
        string InputFile,
        bool NoRestore,
        ImmutableDictionary<string, string> AdditionalArguments,
        DeploymentOutputFormat? OutputFormat,
        ImmutableDictionary<string, ImmutableHashSet<string>> Labels) : OrchestrateArguments(InputFile, NoRestore, AdditionalArguments, OutputFormat, Labels);

    private async Task<Compilation> GetCompilation(OrchestrateArguments args)
    {
        var paramsFileUri = inputOutputArgumentsResolver.ResolveInputArguments(args);
        ArgumentHelper.ValidateBicepParamFile(paramsFileUri);

        var compilation = await compiler.CreateCompilation(paramsFileUri, skipRestore: args.NoRestore);
        var model = compilation.GetEntrypointSemanticModel();
        if (diagnosticLogger.LogDiagnostics(DiagnosticOptions.Default, compilation).HasErrors)
        {
            throw new CommandLineException("Orchestration compilation failed. Fix the reported diagnostics before retrying.");
        }

        if (model.TargetScope != ResourceScope.Orchestrator)
        {
            throw new CommandLineException($"The target scope of the Bicep file must be 'orchestrator'.");
        }

        return compilation;
    }

    internal static System.CommandLine.Command CreateCommand(CommandLineBuilderContext context)
    {
        var command = new System.CommandLine.Command(CommandNames.Orchestrate, "[Experimental] Orchestrates a complex infrastructure deployment.") { TreatUnmatchedTokensAsErrors = true };
        var deployCommand = new System.CommandLine.Command(CommandNames.Deploy, "[Experimental] Deploys resources.") { TreatUnmatchedTokensAsErrors = false };
        var whatIfCommand = new System.CommandLine.Command(CommandNames.WhatIf, "[Experimental] Performs a what-if analysis.") { TreatUnmatchedTokensAsErrors = false };
        var teardownCommand = new System.CommandLine.Command(CommandNames.Teardown, "[Experimental] Tears down resources.") { TreatUnmatchedTokensAsErrors = false };

        command.Add(deployCommand);
        command.Add(whatIfCommand);
        command.Add(teardownCommand);

        var inputFileArgument = new Argument<string>(ArgumentNames.ParametersFile)
        {
            Description = "The path to the .bicepparam file.",
        };
        var noRestoreOption = new System.CommandLine.Option<bool>(OptionNames.NoRestore)
        {
            Description = "Do not restore modules prior to deploying.",
        };
        var formatOption = new Option<DeploymentOutputFormat?>(OptionNames.Format)
        {
            Description = "Output format for deployment results (Default, Json).",
        };
        var labelOption = new Option<string[]>("--label")
        {
            Description = "Select stacks by label (key=value1,value2). Repeat for multiple labels. Deploy and what-if include prerequisites; teardown rejects surviving dependents.",
            Arity = ArgumentArity.OneOrMore,
            AllowMultipleArgumentsPerToken = false,
        };

        deployCommand.Add(inputFileArgument);
        deployCommand.Add(noRestoreOption);
        deployCommand.Add(formatOption);
        deployCommand.Add(labelOption);
        deployCommand.SetAction((result, ct) => context.RunCommandAsync(async () =>
        {
            var additionalArguments = CommandLineBuilderContext.ParseAdditionalArguments(result.UnmatchedTokens);
            var args = new DeployArguments(
                result.GetRequiredValue(inputFileArgument),
                result.GetValue(noRestoreOption),
                additionalArguments,
                result.GetValue(formatOption),
                OrchestrationSelection.ParseLabels(result.GetValue(labelOption) ?? []));

            return await context.GetCommand<OrchestrateCommand>().Deploy(args, ct);
        }));

        whatIfCommand.Add(inputFileArgument);
        whatIfCommand.Add(noRestoreOption);
        whatIfCommand.Add(formatOption);
        whatIfCommand.Add(labelOption);
        whatIfCommand.SetAction((result, ct) => context.RunCommandAsync(async () =>
        {
            var additionalArguments = CommandLineBuilderContext.ParseAdditionalArguments(result.UnmatchedTokens);
            var args = new WhatIfArguments(
                result.GetRequiredValue(inputFileArgument),
                result.GetValue(noRestoreOption),
                additionalArguments,
                result.GetValue(formatOption),
                OrchestrationSelection.ParseLabels(result.GetValue(labelOption) ?? []));

            return await context.GetCommand<OrchestrateCommand>().WhatIf(args, ct);
        }));

        teardownCommand.Add(inputFileArgument);
        teardownCommand.Add(noRestoreOption);
        teardownCommand.Add(formatOption);
        teardownCommand.Add(labelOption);
        teardownCommand.SetAction((result, ct) => context.RunCommandAsync(async () =>
        {
            var additionalArguments = CommandLineBuilderContext.ParseAdditionalArguments(result.UnmatchedTokens);
            var args = new TeardownArguments(
                result.GetRequiredValue(inputFileArgument),
                result.GetValue(noRestoreOption),
                additionalArguments,
                result.GetValue(formatOption),
                OrchestrationSelection.ParseLabels(result.GetValue(labelOption) ?? []));

            return await context.GetCommand<OrchestrateCommand>().Teardown(args, ct);
        }));        

        return command;
    }

    private async Task<int> Deploy(DeployArguments args, CancellationToken cancellationToken)
    {
        await using var session = deploymentProviderFactory.Create();
        var compilation = await GetCompilation(args);
        var parameters = compilation.Emitter.Parameters();

        if (parameters.Template?.Template is not { } templateContent ||
            parameters.Parameters is not { } parametersContent)
        {
            return 1;
        }

        if (args.Labels.Count > 0)
        {
            var selection = await OrchestrationSelection.Create(templateContent, parametersContent, args.Labels, teardown: false, cancellationToken);
            logger.LogInformation("Selected stacks (including prerequisites): {Stacks}", string.Join(", ", selection.Selected.Order(StringComparer.Ordinal)));
            templateContent = selection.Template;
        }

        async Task ProcessLocal(Action<DeploymentWrapperView> onUpdate)
        {
            try
            {
                await using var dispatcher = dispatcherFactory.Create(session.Provider);
                await dispatcher.InitializeExtensions(compilation);
                var result = await dispatcher.Deploy(templateContent, parametersContent, result => onUpdate(new(DeploymentProcessor.GetDeploymentView(result), null)), cancellationToken);
                if (result.Deployment.Properties.ProvisioningState != ProvisioningState.Succeeded)
                {
                    logger.LogError("Deployment failed: {Error}", result.Deployment.Properties.Error?.ToJson());
                }
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
        await using var session = deploymentProviderFactory.Create();
        var compilation = await GetCompilation(args);
        var model = compilation.GetEntrypointSemanticModel();

        var parameters = compilation.Emitter.Parameters();

        if (parameters.Template?.Template is not { } templateContent ||
            parameters.Parameters is not { } parametersContent)
        {
            return 1;
        }

        var selection = await OrchestrationSelection.Create(templateContent, parametersContent, args.Labels, teardown: false, cancellationToken);
        var stackResources = selection.GetTeardownBatches().Reverse().SelectMany(batch => batch)
            .Select(name => selection.Resources.Single(resource => resource.GetProperty("symbolicName").GetString() == name));

        foreach (var stackResource in stackResources)
        {
            var symbolicName = stackResource.GetPropertyByPath("symbolicName").GetString()!;
            var stackParameters = stackResource.GetPropertyByPath("properties.parameters").GetString()!;
            var sourceUriString = stackResource.GetPropertyByPath("properties.sourceUri").GetString()!;
            var region = stackResource.GetPropertyByPath("properties.body.region").GetString()!;
            var inputs = stackResource.GetPropertyByPath("properties.body.inputs");

            var sourceUri = new Uri(sourceUriString).ToIOUri();

            var paramsResult = await NestedDeploymentExtension.ProcessParameters(stackParameters, inputs.ToJsonString().FromJson<JObject>());
            var deploymentName = paramsResult.UsingConfig.Name ?? "main";

            var template = stackResource.GetPropertyByPath("properties.template").GetString()
                ?? throw new CommandLineException($"Stack '{symbolicName}' has no template.");
            var changes = await session.Provider.WhatIf(model.Configuration,
                GetLocator(paramsResult.UsingConfig.Scope, deploymentName),
                region, template, paramsResult.Parameters,
                paramsResult.UsingConfig.StacksConfig ?? throw new CommandLineException($"Stack '{symbolicName}' has no stacks configuration."),
                cancellationToken);

            logger.LogInformation($"WhatIf results for Stack {symbolicName} - {sourceUri}:");
            await io.Output.Writer.WriteAsync(WhatIfOperationResultFormatter.Format([.. changes.Where(change => change.ChangeType != DeploymentWhatIfChangeType.Ignore)]));
        }

        return 0;
    }

    private async Task<int> Teardown(TeardownArguments args, CancellationToken cancellationToken)
    {
        await using var session = deploymentProviderFactory.Create();
        var compilation = await GetCompilation(args);
        var model = compilation.GetEntrypointSemanticModel();

        var parameters = compilation.Emitter.Parameters();

        if (parameters.Template?.Template is not { } templateContent ||
            parameters.Parameters is not { } parametersContent)
        {
            return 1;
        }

        var selection = await OrchestrationSelection.Create(templateContent, parametersContent, args.Labels, teardown: true, cancellationToken);
        var stacksConfigs = new Dictionary<string, (DeploymentLocator Locator, NestedDeploymentExtension.StacksConfig Config)>(StringComparer.Ordinal);
        foreach (var stackResource in selection.Resources)
        {
            var symbolicName = stackResource.GetPropertyByPath("symbolicName").GetString()!;
            var stackParameters = stackResource.GetPropertyByPath("properties.parameters").GetString()!;
            var inputs = stackResource.GetPropertyByPath("properties.body.inputs");

            var paramsResult = await NestedDeploymentExtension.ProcessParameters(stackParameters, inputs.ToJsonString().FromJson<JObject>());
            var deploymentName = paramsResult.UsingConfig.Name ?? "main";

            stacksConfigs.Add(symbolicName, (
                GetLocator(paramsResult.UsingConfig.Scope, deploymentName),
                paramsResult.UsingConfig.StacksConfig ?? throw new CommandLineException($"Stack '{symbolicName}' has no stacks configuration.")));
        }

        var batches = selection.GetTeardownBatches();

        var success = await deploymentRenderer.RenderOperation(
            DeploymentRenderer.RefreshInterval,
            async (onUpdate) =>
            {
                var failures = 0;
                onUpdate(new("Teardown", "Running", null));

                foreach (var batch in batches)
                {
                    await Task.WhenAll(batch.Select(async name =>
                    {
                        try
                        {
                            var (locator, config) = stacksConfigs[name];
                            await session.Provider.TeardownStack(model.Configuration, locator, config, cancellationToken);
                        }
                        catch (Exception exception) when (exception is not OperationCanceledException)
                        {
                            Interlocked.Increment(ref failures);
                            logger.LogError("Teardown failed for stack {Stack}: {Error}", name, exception.Message);
                        }
                    }));
                    if (failures > 0)
                    {
                        break;
                    }
                }

                onUpdate(new("Teardown", failures > 0 ? "Failed" : "Succeeded",
                    failures > 0 ? "One or more stacks failed to tear down. Remaining prerequisites were not deleted." : null));
            },
            cancellationToken);

        return success ? 0 : 1;
    }

    private static DeploymentLocator GetLocator(string scope, string name)
    {
        var id = new ResourceIdentifier(scope);
        if (id.SubscriptionId is null)
        {
            throw new CommandLineException("Above-subscription orchestration scopes are not supported.");
        }
        return new("", null, id.SubscriptionId, id.ResourceGroupName, name);
    }
}
