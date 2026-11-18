// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;
using Bicep.Cli.Arguments;
using Bicep.Cli.Constants;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.Core.Features;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Commands;

public class LintCommand(
    ILogger logger,
    DiagnosticLogger diagnosticLogger,
    BicepCompiler compiler,
    InputOutputArgumentsResolver inputOutputArgumentsResolver) : ICommand
{
    public async Task<int> RunAsync(LintArguments args)
    {
        var hasErrors = false;

        foreach (var inputUri in inputOutputArgumentsResolver.ResolveFilePatternInputArguments(args))
        {
            ArgumentHelper.ValidateBicepOrBicepParamFile(inputUri);

            var result = await Lint(inputUri, args.NoRestore, args.DiagnosticsFormat);
            hasErrors |= result.HasErrors;
        }

        return CommandHelper.GetExitCode(new(hasErrors));
    }

    private async Task<DiagnosticSummary> Lint(IOUri inputUri, bool noRestore, DiagnosticsFormat? diagnosticsFormat)
    {
        var compilation = await compiler.CreateCompilation(inputUri, skipRestore: noRestore);
        CommandHelper.LogExperimentalWarning(logger, compilation);

        var summary = diagnosticLogger.LogDiagnostics(ArgumentHelper.GetDiagnosticOptions(diagnosticsFormat) with { SarifToStdout = true }, compilation);

        return summary;
    }

    internal static Command CreateCommand(CommandLineBuilderContext context)
    {
        var inputFileArgument = CommonArguments.OptionalInputFile("The path to the .bicep file.");
        var filePatternOption = CommonOptions.Pattern("Lints all files matching the specified glob pattern.");
        var noRestoreOption = CommonOptions.NoRestore("Skips restoring external modules.");
        var diagnosticsFormatOption = CommonOptions.DiagnosticsFormat();
        var configOption = CommonOptions.Config();

        var command = new Command(CommandNames.Lint, "Lints a .bicep file.")
        {
            inputFileArgument,
            filePatternOption,
            noRestoreOption,
            diagnosticsFormatOption,
            configOption,
        };
        command.Validators.Add(result => CommandLineBuilderContext.ValidatePositionalArgument(result, inputFileArgument));

        command.SetAction((result, ct) => context.RunCommandAsync(result.GetValue(configOption), async () =>
        {
            var diagnosticsFormat = result.GetValue(diagnosticsFormatOption) ?? Arguments.DiagnosticsFormat.Default;
            var args = new LintArguments(
                result.GetValue(inputFileArgument),
                result.GetValue(filePatternOption),
                diagnosticsFormat,
                result.GetValue(noRestoreOption));

            return await context.GetCommand<LintCommand>().RunAsync(args);
        }));

        return command;
    }
}
