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

    internal static Command CreateCommand(CommandLineBuilderContext context)
    {
        var inputFileArgument = CommonArguments.ParametersFile();
        var noRestoreOption = CommonOptions.NoRestore("Do not restore modules prior to deploying.");
        var formatOption = CommonOptions.DeploymentOutputFormat();
        var configOption = CommonOptions.Config();

        var command = new Command(CommandNames.Deploy, "[Experimental] Deploys infrastructure using a .bicepparam file.")
        {
            inputFileArgument,
            noRestoreOption,
            formatOption,
            configOption,
        };
        command.TreatUnmatchedTokensAsErrors = false;
        command.Validators.Add(result => CommandLineBuilderContext.ValidateRequiredPositionalArgument(result, inputFileArgument));

        command.SetAction((result, ct) => context.RunCommandAsync(result.GetValue(configOption), async () =>
        {
            var additionalArguments = CommandLineBuilderContext.ParseAdditionalArguments(result.UnmatchedTokens);
            var args = new DeployArguments(
                result.GetRequiredValue(inputFileArgument),
                result.GetValue(noRestoreOption),
                additionalArguments,
                result.GetValue(formatOption));

            return await context.GetCommand<DeployCommand>().RunAsync(args, ct);
        }));

        return command;
    }
}
