// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.Core.Configuration;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Commands;

public abstract class DeploymentsCommandsBase<TArgs>(
    ILogger logger,
    DiagnosticLogger diagnosticLogger,
    BicepCompiler compiler,
    InputOutputArgumentsResolver inputOutputArgumentsResolver) : ICommand
    where TArgs : DeployArgumentsBase
{
    public async Task<int> RunAsync(TArgs args, CancellationToken cancellationToken)
    {
        var paramsFileUri = inputOutputArgumentsResolver.ResolveInputArguments(args);
        ArgumentHelper.ValidateBicepParamFile(paramsFileUri);

        var compilation = await compiler.CreateCompilation(paramsFileUri, skipRestore: args.NoRestore);
        var model = compilation.GetEntrypointSemanticModel();

        if (model.TargetScope == ResourceScope.Orchestrator)
        {
            return await RunOrchestrationInternal(args, compilation, model, cancellationToken);
        }

        if (!model.Features.DeployCommandsEnabled)
        {
            throw new CommandLineException($"The '{nameof(ExperimentalFeaturesEnabled.DeployCommands)}' experimental feature must be enabled to use the '{args.CommandName}' command.");
        }

        if (!model.HasAzureTargetScope())
        {
            throw new CommandLineException($"The '{args.CommandName}' command only supports Bicep files with an Azure target scope.");
        }

        CommandHelper.LogExperimentalWarning(logger, compilation);

        var summary = diagnosticLogger.LogDiagnostics(DiagnosticOptions.Default, compilation);
        var parameters = compilation.Emitter.Parameters();

        if (summary.HasErrors)
        {
            return 1;
        }

        return await RunInternal(args, compilation, model, parameters, cancellationToken);
    }

    protected abstract Task<int> RunInternal(TArgs args, Compilation compilation, SemanticModel model, ParametersResult parameters, CancellationToken cancellationToken);

    protected abstract Task<int> RunOrchestrationInternal(TArgs args, Compilation compilation, SemanticModel model, CancellationToken cancellationToken);
}
