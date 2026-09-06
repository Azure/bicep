// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core;
using Bicep.Core.Emit.Options;
using Bicep.Core.Features;
using Bicep.IO.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Option = Bicep.Cli.Constants.Option;

namespace Bicep.Cli.Commands
{
    public class GenerateParametersFileCommand : ICommand
    {
        private readonly ILogger logger;
        private readonly IFileExplorer fileExplorer;
        private readonly DiagnosticLogger diagnosticLogger;
        private readonly BicepCompiler compiler;
        private readonly PlaceholderParametersWriter writer;
        private readonly InputOutputArgumentsResolver inputOutputArgumentsResolver;

        public GenerateParametersFileCommand(
            ILogger logger,
            IFileExplorer fileExplorer,
            DiagnosticLogger diagnosticLogger,
            BicepCompiler compiler,
            PlaceholderParametersWriter writer,
            InputOutputArgumentsResolver inputOutputArgumentsResolver)
        {
            this.logger = logger;
            this.diagnosticLogger = diagnosticLogger;
            this.compiler = compiler;
            this.writer = writer;
            this.inputOutputArgumentsResolver = inputOutputArgumentsResolver;
            this.fileExplorer = fileExplorer;
        }

        public async Task<int> RunAsync(GenerateParametersFileArguments args)
        {
            var (inputUri, outputUri) = this.inputOutputArgumentsResolver.ResolveInputOutputArguments(args);
            ArgumentHelper.ValidateBicepFile(inputUri);

            var compilation = await compiler.CreateCompilation(inputUri, forceRestore: args.NoRestore);
            CommandHelper.LogExperimentalWarning(logger, compilation);

            var summary = diagnosticLogger.LogDiagnostics(DiagnosticOptions.Default, compilation);

            if (!summary.HasErrors)
            {
                if (args.OutputToStdOut)
                {
                    writer.ToStdout(compilation, args.OutputFormat, args.IncludeParams);
                }
                else
                {
                    var outputFile = this.fileExplorer.GetFile(outputUri);
                    writer.ToFile(compilation, outputFile, args.OutputFormat, args.IncludeParams);
                }
            }

            // return non-zero exit code on errors
            return summary.HasErrors ? 1 : 0;
        }

        internal static System.CommandLine.Command CreateCommand(CommandLineBuilderContext context)
        {
            var command = new System.CommandLine.Command(Constants.Command.GenerateParamsFile, "Builds parameters file from the given bicep file, updates if there is an existing parameters file.")
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
                Description = "Generates the parameters file without restoring external modules.",
            };
            var outDirOption = new System.CommandLine.Option<string?>(Option.OutDir)
            {
                Description = "Saves the output at the specified directory.",
            };
            var outFileOption = new System.CommandLine.Option<string?>(Option.OutFile)
            {
                Description = "Saves the output as the specified file path.",
            };
            var outputFormatOption = new System.CommandLine.Option<OutputFormatOption>(Option.OutputFormat)
            {
                Description = "Selects the output format (json, bicepparam).",
            };
            var includeParamsOption = new System.CommandLine.Option<IncludeParamsOption>(Option.IncludeParams)
            {
                Description = "Selects which parameters to include into output (requiredonly, all).",
            };

            command.Add(inputFileArgument);
            command.Add(stdoutOption);
            command.Add(noRestoreOption);
            command.Add(outDirOption);
            command.Add(outFileOption);
            command.Add(outputFormatOption);
            command.Add(includeParamsOption);
            command.Validators.Add((System.CommandLine.Parsing.CommandResult result) => CommandLineBuilderContext.ValidatePositionalArgument(result, inputFileArgument));

            command.SetAction((result, ct) => context.RunCommandAsync(async () =>
            {
                var outputToStdOut = result.GetValue(stdoutOption);
                var outputDir = result.GetValue(outDirOption);
                var outputFile = result.GetValue(outFileOption);

                ArgumentHelper.ValidateOutputOptions(outputToStdOut, outputDir, outputFile);

                var inputFile = result.GetValue(inputFileArgument)
                    ?? throw new CommandLineException("The input file path was not specified");
                var args = new GenerateParametersFileArguments(
                    inputFile,
                    outputToStdOut,
                    result.GetValue(noRestoreOption),
                    outputDir,
                    outputFile,
                    result.GetValue(outputFormatOption),
                    result.GetValue(includeParamsOption));

                return await context.GetCommand<GenerateParametersFileCommand>().RunAsync(args);
            }));

            return command;
        }
    }
}
