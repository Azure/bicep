// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.Core.Features;
using Bicep.Core.Utils;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Commands;

public class LintCommand(
    ILogger logger,
    IEnvironment environment,
    DiagnosticLogger diagnosticLogger,
    BicepCompiler compiler) : ICommand
{
    public async Task<int> RunAsync(LintArguments args)
    {
        if (args.InputFile is null)
        {
            var summaryMultiple = await LintMultiple(args);
            return CommandHelper.GetExitCode(summaryMultiple);
        }

        var inputUri = ArgumentHelper.GetFileUri(args.InputFile);
        ArgumentHelper.ValidateBicepOrBicepParamFile(inputUri);

        var summary = await Lint(inputUri, args.NoRestore, args.DiagnosticsFormat);
        return CommandHelper.GetExitCode(summary);
    }

    private async Task<DiagnosticSummary> Lint(Uri inputUri, bool noRestore, DiagnosticsFormat? diagnosticsFormat)
    {
        var compilation = await compiler.CreateCompilation(inputUri, skipRestore: noRestore);
        CommandHelper.LogExperimentalWarning(logger, compilation);

        var summary = diagnosticLogger.LogDiagnostics(ArgumentHelper.GetDiagnosticOptions(diagnosticsFormat) with { SarifToStdout = true }, compilation);

        return summary;
    }

    public async Task<DiagnosticSummary> LintMultiple(LintArguments args)
    {
        var hasErrors = false;

        foreach (var inputUri in CommandHelper.GetInputFilesForPattern(environment, args.FilePattern))
        {
            ArgumentHelper.ValidateBicepOrBicepParamFile(inputUri);

            var result = await Lint(inputUri, args.NoRestore, args.DiagnosticsFormat);
            hasErrors |= result.HasErrors;
        }

        return new(hasErrors);
    }
}
