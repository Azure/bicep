// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Cli.Arguments;
using Bicep.Cli.Services;
using Bicep.Core.FileSystem;
using Bicep.Decompiler;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Commands
{
    public class DecompileParamsCommand : ICommand
    {
        private readonly ILogger logger;
        private readonly IOContext io;
        private readonly IFileResolver fileResolver;
        private readonly BicepDecompiler decompiler;
        private readonly OutputWriter writer;

        public DecompileParamsCommand(
            ILogger logger,
            IOContext io,
            IFileResolver fileResolver,
            BicepDecompiler decompiler,
            OutputWriter writer)
        {
            this.logger = logger;
            this.io = io;
            this.fileResolver = fileResolver;
            this.decompiler = decompiler;
            this.writer = writer;
        }

        public int Run(DecompileParamsArguments args)
        {
            logger.LogWarning(BicepDecompiler.DecompilerDisclaimerMessage);

            var inputUri = PathHelper.FilePathToFileUrl(PathHelper.ResolvePath(args.InputFile));
            var outputPath = PathHelper.ResolveOutputPath(inputUri.LocalPath, args.OutputDir, args.OutputFile, PathHelper.GetBicepparamOutputPath);
            var outputUri = PathHelper.FilePathToFileUrl(outputPath);
            var bicepUri = args.BicepFilePath is { } ? PathHelper.FilePathToFileUrl(PathHelper.ResolvePath(args.BicepFilePath)) : null;

            try
            {
                if (!fileResolver.TryRead(inputUri).IsSuccess(out var jsonContents))
                {
                    throw new InvalidOperationException($"Failed to read {inputUri}");
                }

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
                io.Error.WriteLine(string.Format(CliResources.DecompilationFailedFormat, inputUri.LocalPath, exception.Message));
                return 1;
            }
        }
    }
}
