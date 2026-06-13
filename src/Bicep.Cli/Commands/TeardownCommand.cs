// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;
using Bicep.Cli.Arguments;
using Bicep.Cli.Constants;
using Bicep.Cli.Helpers.Deploy;
using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Bicep.Core.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Option = Bicep.Cli.Constants.Option;

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

    internal static System.CommandLine.Command CreateCommand(CommandLineBuilderContext context)
    {
        var command = new System.CommandLine.Command(Constants.Command.Teardown, "[Experimental] Tears down resources deployed by a .bicepparam file.")
        {
            TreatUnmatchedTokensAsErrors = false,
        };

        var inputFileArgument = new System.CommandLine.Argument<string>(Constants.Argument.ParametersFile)
        {
            Description = "The path to the .bicepparam file.",
        };
        var noRestoreOption = new System.CommandLine.Option<bool>(Option.NoRestore)
        {
            Description = "Do not restore modules prior to tearing down.",
        };

        command.Add(inputFileArgument);
        command.Add(noRestoreOption);
        command.Validators.Add((System.CommandLine.Parsing.CommandResult result) => CommandLineBuilderContext.ValidateRequiredPositionalArgument(result, inputFileArgument));

        command.SetAction((result, ct) => context.RunCommandAsync(async () =>
        {
            var additionalArguments = CommandLineBuilderContext.ParseAdditionalArguments(result.UnmatchedTokens);
            var args = new TeardownArguments(
                result.GetRequiredValue(inputFileArgument),
                result.GetValue(noRestoreOption),
                additionalArguments);

            return await context.GetCommand<TeardownCommand>().RunAsync(args, ct);
        }));

        return command;
    }
}
