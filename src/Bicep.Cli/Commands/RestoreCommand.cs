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
using Option = Bicep.Cli.Constants.Option;

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

    internal static System.CommandLine.Command CreateCommand(CommandLineBuilderContext context)
    {
        var command = new System.CommandLine.Command(Constants.Command.Restore, "Restores external modules from the specified Bicep file to the local module cache.");

        var inputFileArgument = new System.CommandLine.Argument<string?>(Constants.Argument.InputFile)
        {
            Description = "The path to the .bicep file.",
            Arity = ArgumentArity.ZeroOrOne,
        };
        var filePatternOption = new System.CommandLine.Option<string?>(Option.Pattern)
        {
            Description = "Restores all files matching the specified glob pattern.",
        };
        var forceOption = new System.CommandLine.Option<bool>(Option.Force)
        {
            Description = "Force restore even if modules are already cached.",
        };

        command.Add(inputFileArgument);
        command.Add(filePatternOption);
        command.Add(forceOption);
        command.Validators.Add((System.CommandLine.Parsing.CommandResult result) => CommandLineBuilderContext.ValidatePositionalArgument(result, inputFileArgument));

        command.SetAction((result, ct) => context.RunCommandAsync(async () =>
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
