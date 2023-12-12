// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Threading.Tasks;
using Bicep.Cli.Arguments;
using Bicep.Cli.Logging;
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
        private readonly CompilationService compilationService;
        private readonly DecompilationWriter writer;

        public DecompileParamsCommand(
            ILogger logger,
            IOContext io,
            CompilationService compilationService,
            DecompilationWriter writer)
        {
            this.logger = logger;
            this.io = io;
            this.compilationService = compilationService;
            this.writer = writer;
        }

        public int Run(DecompileParamsArguments args)
        {
            logger.LogWarning(BicepDecompiler.DecompilerDisclaimerMessage);

            var inputPath = PathHelper.ResolvePath(args.InputFile);

            static string DefaultOutputPath(string path) => PathHelper.GetDefaultDecompileparamOutputPath(path);

            var outputPath = PathHelper.ResolveDefaultOutputPath(inputPath, args.OutputDir, args.OutputFile, DefaultOutputPath);

            try
            {
                var decompilation = compilationService.DecompileParams(inputPath, outputPath, args.BicepFilePath);

                if (args.OutputToStdOut)
                {
                    writer.ToStdout(decompilation);
                }
                else
                {
                    writer.ToFile(decompilation);
                }

                return 0;
            }
            catch (Exception exception)
            {
                io.Error.WriteLine(string.Format(CliResources.DecompilationFailedFormat, PathHelper.ResolvePath(args.InputFile), exception.Message));
                return 1;
            }
        }
    }
}
