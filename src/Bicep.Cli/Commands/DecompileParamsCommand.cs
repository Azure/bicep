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

        internal static Command CreateCommand(CommandLineBuilderContext context)
        {
            var inputFileArgument = new Argument<string>(ArgumentNames.InputFile)
            {
                Description = "The path to the parameters .json file.",
            };
            var stdoutOption = CommonOptions.Stdout();
            var forceOption = CommonOptions.Force("Allows overwriting the output file if it exists (applies only to 'bicep decompile' or 'bicep decompile-params').");
            var outDirOption = CommonOptions.OutDir();
            var outFileOption = CommonOptions.OutFile();
            var bicepFileOption = new Option<string?>(OptionNames.BicepFile)
            {
                Description = "Path to the bicep template file that will be referenced in the using declaration.",
            };

            var command = new Command(CommandNames.DecompileParams, "Attempts to decompile a parameters .json file to .bicepparam.")
            {
                inputFileArgument,
                stdoutOption,
                forceOption,
                outDirOption,
                outFileOption,
                bicepFileOption,
            };
            command.Validators.Add(result => CommandLineBuilderContext.ValidateRequiredPositionalArgument(result, inputFileArgument));

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
