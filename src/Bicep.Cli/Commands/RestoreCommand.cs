// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;
using Bicep.Cli.Arguments;
using Bicep.Cli.Constants;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.IO.Abstraction;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Cli.Commands;

public class RestoreCommand(
    BicepCompiler compiler,
    DiagnosticLogger diagnosticLogger,
    InputOutputArgumentsResolver inputOutputArgumentsResolver) : ICommand
{
    public async Task<int> RunAsync(RestoreArguments args)
    {
        var hasErrors = false;

        foreach (var inputUri in inputOutputArgumentsResolver.ResolveFilePatternInputArguments(args))
        {
            ArgumentHelper.ValidateBicepOrBicepParamFile(inputUri);

            var result = await Restore(inputUri, args.ForceModulesRestore);
            hasErrors |= result.HasErrors;
        }

        return CommandHelper.GetExitCode(new(hasErrors));
    }

    private async Task<DiagnosticSummary> Restore(IOUri inputUri, bool force)
    {
        var compilation = compiler.CreateCompilationWithoutRestore(inputUri, markAllForRestore: force);
        var restoreDiagnostics = await compiler.Restore(compilation, forceRestore: force);

        var summary = diagnosticLogger.LogDiagnostics(DiagnosticOptions.Default, restoreDiagnostics);

        return summary;
    }

    internal static Command CreateCommand(CommandLineBuilderContext context)
    {
        var inputFileArgument = CommonArguments.OptionalInputFile("The path to the .bicep file.");
        var filePatternOption = CommonOptions.Pattern("Restores all files matching the specified glob pattern.");
        var forceOption = CommonOptions.Force("Force restore even if modules are already cached.");
        var configOption = CommonOptions.Config();

        var command = new Command(CommandNames.Restore, "Restores external modules from the specified Bicep file to the local module cache.")
        {
            inputFileArgument,
            filePatternOption,
            forceOption,
            configOption,
        };
        command.Validators.Add(result => CommandLineBuilderContext.ValidatePositionalArgument(result, inputFileArgument));

        command.SetAction((result, ct) => context.RunCommandAsync(result.GetValue(configOption), async () =>
        {
            var args = new RestoreArguments(
                result.GetValue(inputFileArgument),
                result.GetValue(filePatternOption),
                result.GetValue(forceOption));

            return await context.GetCommand<RestoreCommand>().RunAsync(args);
        }));

        return command;
    }
}
