// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.CommandLine;
using Bicep.Cli.Arguments;
using Bicep.Cli.Constants;
using Bicep.Cli.Helpers;
using Bicep.Cli.Services;
using Bicep.Decompiler;
using Bicep.IO.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Option = Bicep.Cli.Constants.Option;

namespace Bicep.Cli.Commands
{
    public class DecompileParamsCommand : ICommand
    {
        private readonly ILogger logger;
        private readonly IOContext io;
        private readonly IFileExplorer fileExplorer;
        private readonly BicepDecompiler decompiler;
        private readonly OutputWriter writer;
        private readonly InputOutputArgumentsResolver inputOutputArgumentsResolver;

        public DecompileParamsCommand(
            ILogger logger,
            IOContext io,
            IFileExplorer fileExplorer,
            BicepDecompiler decompiler,
            OutputWriter writer,
            InputOutputArgumentsResolver inputOutputArgumentsResolver)
        {
            this.logger = logger;
            this.io = io;
            this.fileExplorer = fileExplorer;
            this.decompiler = decompiler;
            this.writer = writer;
            this.inputOutputArgumentsResolver = inputOutputArgumentsResolver;
        }

        public int Run(DecompileParamsArguments args)
        {
            logger.LogWarning(BicepDecompiler.DecompilerDisclaimerMessage);

            var (inputUri, outputUri) = this.inputOutputArgumentsResolver.ResolveInputOutputArguments(args);

            if (!args.OutputToStdOut && !args.AllowOverwrite && this.fileExplorer.GetFile(outputUri).Exists())
            {
                throw new CommandLineException($"The output file \"{outputUri}\" already exists. Use --force to overwrite the existing file.");
            }

            var bicepUri = args.BicepFilePath is not null ? this.inputOutputArgumentsResolver.PathToUri(args.BicepFilePath) : null;

            try
            {
                var jsonContents = this.fileExplorer.GetFile(inputUri).ReadAllText();
                var decompilation = decompiler.DecompileParameters(jsonContents, outputUri, bicepUri);

                if (args.OutputToStdOut)
                {
                    writer.DecompileResultToStdout(decompilation);
                }
                else
                {
                    writer.DecompileResultToFile(decompilation);
                }

                return 0;
            }
            catch (Exception exception)
            {
                io.Error.Writer.WriteLine(string.Format(CliResources.DecompilationFailedFormat, inputUri, exception.Message));
                return 1;
            }
        }

        internal static System.CommandLine.Command CreateCommand(CommandLineBuilderContext context)
        {
            var command = new System.CommandLine.Command(Constants.Command.DecompileParams, "Attempts to decompile a parameters .json file to .bicepparam.")
            {
                TreatUnmatchedTokensAsErrors = true,
            };

            var inputFileArgument = new System.CommandLine.Argument<string>(Constants.Argument.InputFile)
            {
                Description = "The path to the parameters .json file.",
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
            var bicepFileOption = new System.CommandLine.Option<string?>(Option.BicepFile)
            {
                Description = "Path to the bicep template file that will be referenced in the using declaration.",
            };

            command.Add(inputFileArgument);
            command.Add(stdoutOption);
            command.Add(forceOption);
            command.Add(outDirOption);
            command.Add(outFileOption);
            command.Add(bicepFileOption);
            command.Validators.Add((System.CommandLine.Parsing.CommandResult result) => CommandLineBuilderContext.ValidateRequiredPositionalArgument(result, inputFileArgument));

            command.SetAction((result, ct) => context.RunCommandAsync(async () =>
            {
                var outputToStdOut = result.GetValue(stdoutOption);
                var outputDir = result.GetValue(outDirOption);
                var outputFile = result.GetValue(outFileOption);

                ArgumentHelper.ValidateOutputOptions(outputToStdOut, outputDir, outputFile);

                var args = new DecompileParamsArguments(
                    result.GetRequiredValue(inputFileArgument),
                    outputToStdOut,
                    result.GetValue(forceOption),
                    outputDir,
                    outputFile,
                    result.GetValue(bicepFileOption));

                return await Task.FromResult(context.GetCommand<DecompileParamsCommand>().Run(args));
            }));

            return command;
        }
    }
}
