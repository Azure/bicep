// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.Core.Features;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;
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
        var compilation = await compiler.CreateCompilation(inputUri.ToUri(), skipRestore: noRestore);
        CommandHelper.LogExperimentalWarning(logger, compilation);

        var summary = diagnosticLogger.LogDiagnostics(ArgumentHelper.GetDiagnosticOptions(diagnosticsFormat) with { SarifToStdout = true }, compilation);

        return summary;
    }
}
