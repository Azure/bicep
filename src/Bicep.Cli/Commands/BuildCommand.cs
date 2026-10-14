// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;
using Bicep.Cli.Arguments;
using Bicep.Cli.Constants;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core;
using Bicep.IO.Abstraction;
using Microsoft.Extensions.DependencyInjection;
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
                await writer.TemplateToFileAsync(compilation, outputUri);
            }
        }

        return summary;
    }

    internal static Command CreateCommand(CommandLineBuilderContext context)
    {
        var inputFileArgument = CommonArguments.OptionalInputFile("The path to the input .bicep file.");
        var stdoutOption = CommonOptions.Stdout();
        var noRestoreOption = CommonOptions.NoRestore("Builds the bicep file without restoring external modules.");
        var outDirOption = CommonOptions.OutDir();
        var outFileOption = CommonOptions.OutFile();
        var filePatternOption = CommonOptions.Pattern("Builds all files matching the specified glob pattern.");
        var diagnosticsFormatOption = CommonOptions.DiagnosticsFormat();
        var configOption = CommonOptions.Config();

        var command = new Command(CommandNames.Build, "Builds a .bicep file.")
        {
            inputFileArgument,
            stdoutOption,
            noRestoreOption,
            outDirOption,
            outFileOption,
            filePatternOption,
            diagnosticsFormatOption,
            configOption,
        };
        command.Validators.Add(result => CommandLineBuilderContext.ValidatePositionalArgument(result, inputFileArgument));

        command.SetAction((result, ct) => context.RunCommandAsync(result.GetValue(configOption), async () =>
        {
            var outputToStdOut = result.GetValue(stdoutOption);
            var outputDir = result.GetValue(outDirOption);
            var outputFile = result.GetValue(outFileOption);
            var filePattern = result.GetValue(filePatternOption);

            ArgumentHelper.ValidateOutputOptions(outputToStdOut, outputDir, outputFile, filePattern);

            var args = new BuildArguments(
                result.GetValue(inputFileArgument),
                outputToStdOut,
                result.GetValue(noRestoreOption),
                outputDir,
                outputFile,
                filePattern,
                result.GetValue(diagnosticsFormatOption));

            return await context.GetCommand<BuildCommand>().RunAsync(args);
        }));

        return command;
    }
}
