// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Cli.Arguments;
using Bicep.Cli.Services;
using Bicep.Decompiler;
using Bicep.IO.Abstraction;
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
    }
}
