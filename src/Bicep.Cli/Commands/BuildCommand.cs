// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Utils;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Cli.Commands;

public class BuildCommand(
    ILogger logger,
    IEnvironment environment,
    DiagnosticLogger diagnosticLogger,
    BicepCompiler compiler,
    OutputWriter writer) : ICommand
{
    public async Task<int> RunAsync(BuildArguments args)
    {
        if (args.InputFile is null)
        {
            var summaryMultiple = await CompileMultiple(args);
            return CommandHelper.GetExitCode(summaryMultiple);
        }

        var inputUri = ArgumentHelper.GetFileUri(args.InputFile);
        ArgumentHelper.ValidateBicepFile(inputUri);

        var outputUri = CommandHelper.GetJsonOutputUri(inputUri, args.OutputDir, args.OutputFile);

        var summary = await Compile(inputUri, outputUri, args.NoRestore, args.DiagnosticsFormat, args.OutputToStdOut);
        return CommandHelper.GetExitCode(summary);
    }

    private async Task<DiagnosticSummary> Compile(Uri inputUri, Uri outputUri, bool noRestore, DiagnosticsFormat? diagnosticsFormat, bool outputToStdOut)
    {
        var compilation = await compiler.CreateCompilation(inputUri, skipRestore: noRestore);
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
                writer.TemplateToFile(compilation, outputUri);
            }
        }

        return summary;
    }

    public async Task<DiagnosticSummary> CompileMultiple(BuildArguments args)
    {
        var hasErrors = false;

        foreach (var (inputUri, outputUri) in CommandHelper.GetInputAndOutputFilesForPattern(environment, args.FilePattern, args.OutputDir, PathHelper.GetJsonOutputPath))
        {
            ArgumentHelper.ValidateBicepFile(inputUri);

            var result = await Compile(inputUri, outputUri, args.NoRestore, args.DiagnosticsFormat, false);
            hasErrors |= result.HasErrors;
        }

        return new(hasErrors);
    }
}
