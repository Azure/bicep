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
    public class DecompileCommand : ICommand
    {
        private readonly ILogger logger;
        private readonly IDiagnosticLogger diagnosticLogger;
        private readonly IOContext io;
        private readonly CompilationService compilationService;
        private readonly DecompilationWriter writer;

        public DecompileCommand(
            ILogger logger,
            IDiagnosticLogger diagnosticLogger,
            IOContext io,
            CompilationService compilationService,
            DecompilationWriter writer)
        {
            this.logger = logger;
            this.diagnosticLogger = diagnosticLogger;
            this.io = io;
            this.compilationService = compilationService;
            this.writer = writer;
        }

        public async Task<int> RunAsync(DecompileArguments args)
        {
            logger.LogWarning(BicepDecompiler.DecompilerDisclaimerMessage);

            var inputPath = PathHelper.ResolvePath(args.InputFile);

            static string DefaultOutputPath(string path) => PathHelper.GetDefaultDecompileOutputPath(path);

            var outputPath = PathHelper.ResolveDefaultOutputPath(inputPath, args.OutputDir, args.OutputFile, DefaultOutputPath);

            try
            {
                var decompilation = await compilationService.DecompileAsync(inputPath, outputPath);

                if (args.OutputToStdOut)
                {
                    writer.ToStdout(decompilation);
                }
                else
                {
                    writer.ToFile(decompilation);
                }
            }
            catch (Exception exception)
            {
                await io.Error.WriteLineAsync(string.Format(CliResources.DecompilationFailedFormat, PathHelper.ResolvePath(args.InputFile), exception.Message));
                return 1;
            }

            // return non-zero exit code on errors
            return diagnosticLogger.ErrorCount > 0 ? 1 : 0;
        }
    }
}
