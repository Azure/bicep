// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;
using Bicep.Cli.Arguments;
using Bicep.Cli.Constants;
using Bicep.Cli.Helpers;
using Bicep.Cli.Helpers.Deploy;
using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem;
using Bicep.Local.Deploy.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Option = Bicep.Cli.Constants.Option;

namespace Bicep.Cli.Commands;

public class LocalDeployCommand(
    DeploymentRenderer deploymentRenderer,
    ILogger logger,
    DiagnosticLogger diagnosticLogger,
    BicepCompiler compiler,
    LocalExtensionDispatcherFactory dispatcherFactory,
    InputOutputArgumentsResolver inputOutputArgumentsResolver) : ICommand
{
    public async Task<int> RunAsync(LocalDeployArguments args, CancellationToken cancellationToken)
    {
        var paramsFileUri = inputOutputArgumentsResolver.PathToUri(args.ParamsFile);
        ArgumentHelper.ValidateBicepParamFile(paramsFileUri);

        var compilation = await compiler.CreateCompilation(paramsFileUri, skipRestore: args.NoRestore);

        var summary = diagnosticLogger.LogDiagnostics(DiagnosticOptions.Default, compilation);
        var parameters = compilation.Emitter.Parameters();

        if (summary.HasErrors ||
            parameters.Parameters is not { } parametersString ||
            parameters.Template?.Template is not { } templateString)
        {
            return 1;
        }

        if (!compilation.GetEntrypointSemanticModel().Features.LocalDeployEnabled)
        {
            logger.LogError("Experimental feature 'localDeploy' is not enabled.");
            return 1;
        }

        if (compilation.GetEntrypointSemanticModel().Root.TryGetBicepFileSemanticModelViaUsing().IsSuccess(out var usingModel) &&
            usingModel.TargetScope is not ResourceScope.Local)
        {
            logger.LogError($"The referenced .bicep file must have targetScope set to 'local'.");
            return 1;
        }

        var success = await deploymentRenderer.RenderDeployment(
            DeploymentRenderer.RefreshInterval,
            (onUpdate) => ProcessDeployment(compilation, templateString, parametersString, onUpdate, cancellationToken),
            args.OutputFormat ?? DeploymentOutputFormat.Default,
            cancellationToken);

        return success ? 0 : 1;
    }

    private async Task ProcessDeployment(Compilation compilation, string templateString, string parametersString, Action<DeploymentWrapperView> onUpdate, CancellationToken cancellationToken)
    {
        try
        {
            await using var dispatcher = dispatcherFactory.Create();

            await dispatcher.InitializeExtensions(compilation);
            await dispatcher.Deploy(templateString, parametersString, result => onUpdate(new(DeploymentProcessor.GetDeploymentView(result), null)), cancellationToken);
        }
        catch (Exception exception)
        {
            onUpdate(new(null, exception.Message));
        }
    }

    internal static System.CommandLine.Command CreateCommand(CommandLineBuilderContext context)
    {
        var command = new System.CommandLine.Command(Constants.Command.LocalDeploy, "[Experimental] Performs a local deployment.");

        var inputFileArgument = new System.CommandLine.Argument<string>(Constants.Argument.ParametersFile)
        {
            Description = "The path to the .bicepparam file.",
        };
        var noRestoreOption = new System.CommandLine.Option<bool>(Option.NoRestore)
        {
            Description = "Do not restore modules prior to deploying.",
        };
        var formatOption = new System.CommandLine.Option<DeploymentOutputFormat?>(Option.Format)
        {
            Description = "Output format for deployment results (Default, Json).",
        };

        command.Add(inputFileArgument);
        command.Add(noRestoreOption);
        command.Add(formatOption);
        command.Validators.Add((System.CommandLine.Parsing.CommandResult result) => CommandLineBuilderContext.ValidateRequiredPositionalArgument(result, inputFileArgument));

        command.SetAction((result, ct) => context.RunCommandAsync(async () =>
        {
            var args = new LocalDeployArguments(
                result.GetRequiredValue(inputFileArgument),
                result.GetValue(noRestoreOption),
                result.GetValue(formatOption));

            return await context.GetCommand<LocalDeployCommand>().RunAsync(args, ct);
        }));

        return command;
    }
}
