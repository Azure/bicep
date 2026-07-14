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
using Option = Bicep.Cli.Constants.Option;

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

    internal static System.CommandLine.Command CreateCommand(CommandLineBuilderContext context)
    {
        var command = new System.CommandLine.Command(Constants.Command.Build, "Builds a .bicep file.")
        {
            TreatUnmatchedTokensAsErrors = true,
        };

        var inputFileArgument = new System.CommandLine.Argument<string?>(Constants.Argument.InputFile)
        {
            Description = "The path to the input .bicep file.",
            Arity = ArgumentArity.ZeroOrOne,
        };
        var stdoutOption = new System.CommandLine.Option<bool>(Option.Stdout)
        {
            Description = "Prints the output to stdout.",
        };
        var noRestoreOption = new System.CommandLine.Option<bool>(Option.NoRestore)
        {
            Description = "Builds the bicep file without restoring external modules.",
        };
        var outDirOption = new System.CommandLine.Option<string?>(Option.OutDir)
        {
            Description = "Saves the output at the specified directory.",
        };
        var outFileOption = new System.CommandLine.Option<string?>(Option.OutFile)
        {
            Description = "Saves the output as the specified file path.",
        };
        var filePatternOption = new System.CommandLine.Option<string?>(Option.Pattern)
        {
            Description = "Builds all files matching the specified glob pattern.",
        };
        var diagnosticsFormatOption = new System.CommandLine.Option<DiagnosticsFormat?>(Option.DiagnosticsFormat)
        {
            Description = "Sets the diagnostics format. Valid values are (Default, SARIF).",
        };

        command.Add(inputFileArgument);
        command.Add(stdoutOption);
        command.Add(noRestoreOption);
        command.Add(outDirOption);
        command.Add(outFileOption);
        command.Add(filePatternOption);
        command.Add(diagnosticsFormatOption);
        command.Validators.Add((System.CommandLine.Parsing.CommandResult result) => CommandLineBuilderContext.ValidatePositionalArgument(result, inputFileArgument));

        command.SetAction((result, ct) => context.RunCommandAsync(async () =>
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
