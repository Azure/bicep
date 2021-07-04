// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Cli.Arguments;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core.FileSystem;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace Bicep.Cli.Commands
{
    public class DecompileCommand : ICommand
    {
        private readonly ILogger logger;
        private readonly IDiagnosticLogger diagnosticLogger;
        private readonly InvocationContext invocationContext;
        private readonly CompilationService compilationService;
        private readonly DecompilationWriter writer;

        public DecompileCommand(
            ILogger logger, 
            IDiagnosticLogger diagnosticLogger, 
            InvocationContext invocationContext, 
            CompilationService compilationService,
            DecompilationWriter writer) 
        {
            this.logger = logger;
            this.diagnosticLogger = diagnosticLogger;
            this.invocationContext = invocationContext;
            this.compilationService = compilationService;
            this.writer = writer;
        }

        public int Run(DecompileArguments args)
        {
            logger.LogWarning(CliResources.DecompilerDisclaimerMessage);

            var inputPath = PathHelper.ResolvePath(args.InputFile);
            var outputPath = ResolveOutputPath(inputPath, args.OutputDir, args.OutputFile);

            try
            {
                var decompilation = compilationService.Decompile(inputPath, outputPath);

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
                invocationContext.ErrorWriter.WriteLine(string.Format(CliResources.DecompilationFailedFormat, PathHelper.ResolvePath(args.InputFile), exception.Message));
                return 1;
            }

            // return non-zero exit code on errors
            return diagnosticLogger.ErrorCount > 0 ? 1 : 0;
        }

        private static string ResolveOutputPath(string inputPath, string? outputDir, string? outputFile)
        {
            if (outputDir is not null)
            {
                var dir = PathHelper.ResolvePath(outputDir);
                var file = Path.GetFileName(inputPath);
                var path = Path.Combine(dir, file);

                return PathHelper.GetDefaultDecompileOutputPath(path);
            }
            else if (outputFile is not null)
            {
                return PathHelper.ResolvePath(outputFile);
            }
            else
            {
                return PathHelper.GetDefaultDecompileOutputPath(inputPath);
            }
        }
    }
}
