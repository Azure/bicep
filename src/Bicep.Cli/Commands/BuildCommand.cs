// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core;
using Bicep.IO.Abstraction;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Commands;

public class BuildCommand(
    ILogger logger,
    InputOutputArgumentsResolver inputOutputArgumentsResolver,
    DiagnosticLogger diagnosticLogger,
    BicepCompiler compiler,
    OutputWriter writer) : ICommand
{
    public async Task<int> RunAsync(BuildArguments args)
    {
        var hasErrors = false;
        var inputOutputUriPairs = inputOutputArgumentsResolver.ResolveFilePatternInputOutputArguments(args);
        var outputToStdOut = inputOutputUriPairs.Count == 1 && args.OutputToStdOut; // If there are multiple input files, we ignore the args.OutputToStdOut flag.

        foreach (var (inputUri, outputUri) in inputOutputUriPairs)
        {
            ArgumentHelper.ValidateBicepFile(inputUri);

            var result = await Compile(inputUri, outputUri, args.NoRestore, args.DiagnosticsFormat, outputToStdOut);
            hasErrors |= result.HasErrors;
        }

        var summary = new DiagnosticSummary(hasErrors);

        return CommandHelper.GetExitCode(summary);
    }

    private async Task<DiagnosticSummary> Compile(IOUri inputUri, IOUri outputUri, bool noRestore, DiagnosticsFormat? diagnosticsFormat, bool outputToStdOut)
    {
        var compilation = await compiler.CreateCompilation(inputUri.ToUri(), skipRestore: noRestore);
        CommandHelper.LogExperimentalWarning(logger, compilation);

        var summary = diagnosticLogger.LogDiagnostics(ArgumentHelper.GetDiagnosticOptions(diagnosticsFormat), compilation);

        if (!summary.HasErrors)
        {
            if (outputToStdOut)
            {
                writer.TemplateToStdout(compilation);
            }
            else
            {
                await writer.TemplateToFileAsync(compilation, outputUri);
            }
        }

        return summary;
    }
}
