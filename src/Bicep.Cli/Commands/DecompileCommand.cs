// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.CommandLine;
using Bicep.Cli.Arguments;
using Bicep.Cli.Constants;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core;
using Bicep.Core.Extensions;
using Bicep.Core.SourceGraph;
using Bicep.Decompiler;
using Bicep.Decompiler.ArmHelpers;
using Bicep.IO.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Option = Bicep.Cli.Constants.Option;

namespace Bicep.Cli.Commands
{
    public class DecompileCommand : ICommand
    {
        private readonly ILogger logger;
        private readonly DiagnosticLogger diagnosticLogger;
        private readonly IOContext io;
        private readonly BicepDecompiler decompiler;
        private readonly BicepCompiler compiler;
        private readonly OutputWriter writer;
        private readonly ISourceFileFactory sourceFileFactory;
        private readonly InputOutputArgumentsResolver inputOutputArgumentsResolver;
        private readonly IFileExplorer fileExplorer;

        public DecompileCommand(
            ILogger logger,
            DiagnosticLogger diagnosticLogger,
            IOContext io,
            BicepDecompiler decompiler,
            BicepCompiler compiler,
            OutputWriter writer,
            ISourceFileFactory sourceFileFactory,
            InputOutputArgumentsResolver inputOutputArgumentsResolver,
            IFileExplorer fileExplorer)
        {
            this.logger = logger;
            this.diagnosticLogger = diagnosticLogger;
            this.io = io;
            this.decompiler = decompiler;
            this.compiler = compiler;
            this.writer = writer;
            this.sourceFileFactory = sourceFileFactory;
            this.inputOutputArgumentsResolver = inputOutputArgumentsResolver;
            this.fileExplorer = fileExplorer;
        }

        public async Task<int> RunAsync(DecompileArguments args)
        {
            logger.LogWarning(BicepDecompiler.DecompilerDisclaimerMessage);

            var (inputUri, outputUri) = inputOutputArgumentsResolver.ResolveInputOutputArguments(args);

            if (!args.OutputToStdOut && !args.AllowOverwrite && this.fileExplorer.GetFile(outputUri).Exists())
            {
                throw new CommandLineException($"The output file \"{outputUri}\" already exists. Use --force to overwrite the existing file.");
            }

            try
            {
                var jsonContents = await this.fileExplorer.GetFile(inputUri).ReadAllTextAsync();
                var templateObject = JTokenHelpers.LoadJson(jsonContents, Newtonsoft.Json.Linq.JObject.Load, ignoreTrailingContent: true);

                if (TemplateHelpers.IsBicepGeneratedTemplate(templateObject))
                {
                    logger.LogWarning(BicepDecompiler.BicepGeneratedTemplateWarning);
                }

                var decompilation = await decompiler.Decompile(outputUri, templateObject);

                // TODO(low-priority): It would be ideal to remove Workspace and use InMemoryFileExplorer instead.
                // This is something that should be done after the core part of file I/O abstraction migration is complete.
                var workspace = new ActiveSourceFileSet();
                foreach (var (fileUri, bicepOutput) in decompilation.FilesToSave)
                {
                    workspace.UpsertSourceFile(this.sourceFileFactory.CreateBicepFile(fileUri, bicepOutput));
                }

                // to verify success we recompile and check for syntax errors.
                var compilation = await compiler.CreateCompilation(decompilation.EntrypointUri, skipRestore: true, workspace: workspace);
                var summary = diagnosticLogger.LogDiagnostics(DiagnosticOptions.Default, compilation);

                if (args.OutputToStdOut)
                {
                    writer.DecompileResultToStdout(decompilation);
                }
                else
                {
                    await writer.DecompileResultToFileAsync(decompilation);
                }

                // return non-zero exit code on errors
                return summary.HasErrors ? 1 : 0;
            }
            catch (Exception exception)
            {
                await io.Error.Writer.WriteLineAsync(string.Format(CliResources.DecompilationFailedFormat, inputUri, exception.Message));
                return 1;
            }
        }

        internal static System.CommandLine.Command CreateCommand(CommandLineBuilderContext context)
        {
            var command = new System.CommandLine.Command(Constants.Command.Decompile, "Attempts to decompile a template .json file to .bicep.")
            {
                TreatUnmatchedTokensAsErrors = true,
            };

            var inputFileArgument = new System.CommandLine.Argument<string?>(Constants.Argument.InputFile)
            {
                Description = "The path to the ARM template .json file.",
                Arity = ArgumentArity.ZeroOrOne,
            };
            var stdoutOption = new System.CommandLine.Option<bool>(Option.Stdout)
            {
                Description = "Prints the output to stdout.",
            };
            var forceOption = new System.CommandLine.Option<bool>(Option.Force)
            {
                Description = "Allows overwriting the output file if it exists (applies only to 'bicep decompile' or 'bicep decompile-params').",
            };
            var outDirOption = new System.CommandLine.Option<string?>(Option.OutDir)
            {
                Description = "Saves the output at the specified directory.",
            };
            var outFileOption = new System.CommandLine.Option<string?>(Option.OutFile)
            {
                Description = "Saves the output as the specified file path.",
            };

            command.Add(inputFileArgument);
            command.Add(stdoutOption);
            command.Add(forceOption);
            command.Add(outDirOption);
            command.Add(outFileOption);
            command.Validators.Add((System.CommandLine.Parsing.CommandResult result) => CommandLineBuilderContext.ValidatePositionalArgument(result, inputFileArgument));

            command.SetAction((result, ct) => context.RunCommandAsync(async () =>
            {
                var outputToStdOut = result.GetValue(stdoutOption);
                var outputDir = result.GetValue(outDirOption);
                var outputFile = result.GetValue(outFileOption);

                ArgumentHelper.ValidateOutputOptions(outputToStdOut, outputDir, outputFile);

                var inputFile = result.GetValue(inputFileArgument)
                    ?? throw new CommandLineException("The input file path was not specified");
                var args = new DecompileArguments(
                    inputFile,
                    outputToStdOut,
                    result.GetValue(forceOption),
                    outputDir,
                    outputFile);

                return await context.GetCommand<DecompileCommand>().RunAsync(args);
            }));

            return command;
        }
    }
}
