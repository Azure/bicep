// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Bicep.Core.Utils;

namespace Bicep.Cli.Commands;

public class BuildCommand : ICommand
{
    private readonly ILogger logger;
    private readonly IEnvironment environment;
    private readonly DiagnosticLogger diagnosticLogger;
    private readonly BicepCompiler compiler;
    private readonly OutputWriter writer;

    public BuildCommand(
        ILogger logger,
        IEnvironment environment,
        DiagnosticLogger diagnosticLogger,
        BicepCompiler compiler,
        OutputWriter writer)
    {
        this.logger = logger;
        this.environment = environment;
        this.diagnosticLogger = diagnosticLogger;
        this.compiler = compiler;
        this.writer = writer;
    }

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

        if (ExperimentalFeatureWarningProvider.TryGetEnabledExperimentalFeatureWarningMessage(compilation.SourceFileGrouping) is { } warningMessage)
        {
            logger.LogWarning(warningMessage);
        }

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

        foreach (var inputUri in CommandHelper.GetFilesMatchingPattern(environment, args.FilePatternRoot, args.FilePatterns))
        {
            ArgumentHelper.ValidateBicepFile(inputUri);

            var outputUri = CommandHelper.GetJsonOutputUri(inputUri, null, null);

            var result = await Compile(inputUri, outputUri, args.NoRestore, args.DiagnosticsFormat, false);
            hasErrors |= result.HasErrors;
        }

        return new(hasErrors);
    }
}
