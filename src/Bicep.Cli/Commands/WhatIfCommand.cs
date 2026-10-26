// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;
using Azure.Deployments.Core.Entities;
using Bicep.Cli.Arguments;
using Bicep.Cli.Constants;
using Bicep.Cli.Helpers.Deploy;
using Bicep.Cli.Helpers.WhatIf;
using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Bicep.Core.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Commands;

public class WhatIfCommand(
    IDeploymentProcessor deploymentProcessor,
    IOContext io,
    ILogger logger,
    IEnvironment environment,
    DiagnosticLogger diagnosticLogger,
    BicepCompiler compiler,
    InputOutputArgumentsResolver inputOutputArgumentsResolver) : DeploymentsCommandsBase<WhatIfArguments>(logger, diagnosticLogger, compiler, inputOutputArgumentsResolver)
{
    protected override async Task<int> RunInternal(WhatIfArguments args, SemanticModel model, ParametersResult result, CancellationToken cancellationToken)
    {
        var config = await DeploymentProcessor.GetDeployCommandsConfig(environment, args.AdditionalArguments, result, model.TargetScope);

        await WhatIf(model, config, cancellationToken);

        return 0;
    }

    private async Task WhatIf(SemanticModel model, DeployCommandsConfig config, CancellationToken cancellationToken)
    {
        var result = await deploymentProcessor.WhatIf(model.Configuration, config, cancellationToken);

        var changes = result.Properties.Changes.Where(x => x.ChangeType != DeploymentWhatIfChangeType.Ignore);

        await io.Output.Writer.WriteAsync(WhatIfOperationResultFormatter.Format([.. changes]));
    }

    internal static Command CreateCommand(CommandLineBuilderContext context)
    {
        var inputFileArgument = CommonArguments.ParametersFile();
        var noRestoreOption = CommonOptions.NoRestore("Do not restore modules prior to running what-if.");
        var configOption = CommonOptions.Config();

        var command = new Command(CommandNames.WhatIf, "[Experimental] Previews the changes a deployment would make.")
        {
            inputFileArgument,
            noRestoreOption,
            configOption,
        };
        command.TreatUnmatchedTokensAsErrors = false;
        command.Validators.Add(result => CommandLineBuilderContext.ValidateRequiredPositionalArgument(result, inputFileArgument));

        command.SetAction((result, ct) => context.RunCommandAsync(result.GetValue(configOption), async () =>
        {
            var additionalArguments = CommandLineBuilderContext.ParseAdditionalArguments(result.UnmatchedTokens);
            var args = new WhatIfArguments(
                result.GetRequiredValue(inputFileArgument),
                result.GetValue(noRestoreOption),
                additionalArguments);

            return await context.GetCommand<WhatIfCommand>().RunAsync(args, ct);
        }));

        return command;
    }
}
