// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;
using Bicep.Cli.Arguments;
using Bicep.Cli.Constants;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core;
using Bicep.Core.Emit.Options;
using Bicep.Core.Features;
using Bicep.IO.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

        internal static Command CreateCommand(CommandLineBuilderContext context)
        {
            var inputFileArgument = CommonArguments.OptionalInputFile("The path to the input .bicep file.");
            var stdoutOption = CommonOptions.Stdout();
            var noRestoreOption = CommonOptions.NoRestore("Generates the parameters file without restoring external modules.");
            var outDirOption = CommonOptions.OutDir();
            var outFileOption = CommonOptions.OutFile();
            var outputFormatOption = new Option<OutputFormatOption>(OptionNames.OutputFormat)
            {
                Description = "Selects the output format (json, bicepparam).",
            };
            var includeParamsOption = new Option<IncludeParamsOption>(OptionNames.IncludeParams)
            {
                Description = "Selects which parameters to include into output (requiredonly, all).",
            };
            var configOption = CommonOptions.Config();

            var command = new Command(CommandNames.GenerateParamsFile, "Builds parameters file from the given bicep file, updates if there is an existing parameters file.")
            {
                inputFileArgument,
                stdoutOption,
                noRestoreOption,
                outDirOption,
                outFileOption,
                outputFormatOption,
                includeParamsOption,
                configOption,
            };
            command.Validators.Add(result => CommandLineBuilderContext.ValidatePositionalArgument(result, inputFileArgument));

            command.SetAction((result, ct) => context.RunCommandAsync(result.GetValue(configOption), async () =>
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
