// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Cli.Arguments;
using Bicep.Cli.Services;
using Bicep.Core.FileSystem;
using Bicep.Decompiler;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Commands
{
    public class DecompileParamsCommand(
        ILogger logger,
        IOContext io,
        IFileResolver fileResolver,
        BicepDecompiler decompiler,
        OutputWriter writer) : ICommand
    {
        private readonly ILogger logger = logger;
        private readonly IOContext io = io;
        private readonly IFileResolver fileResolver = fileResolver;
        private readonly BicepDecompiler decompiler = decompiler;
        private readonly OutputWriter writer = writer;

        public int Run(DecompileParamsArguments args)
        {
            logger.LogWarning(BicepDecompiler.DecompilerDisclaimerMessage);

            var inputUri = PathHelper.FilePathToFileUrl(PathHelper.ResolvePath(args.InputFile));
            var outputPath = PathHelper.ResolveDefaultOutputPath(inputUri.LocalPath, args.OutputDir, args.OutputFile, PathHelper.GetDefaultDecompileparamOutputPath);
            var outputUri = PathHelper.FilePathToFileUrl(outputPath);
            var bicepUri = args.BicepFilePath is { } ? PathHelper.FilePathToFileUrl(args.BicepFilePath) : null;

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
