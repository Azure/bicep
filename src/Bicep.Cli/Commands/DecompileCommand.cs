// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.IO;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core.FileSystem;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Commands
{
    public class DecompileCommand : ICommand
    {
        private readonly ILogger logger;
        private readonly IDiagnosticLogger diagnosticLogger;
        private readonly InvocationContext invocationContext;
        private readonly ICompilationService compilationService;
        private readonly IEnumerable<IWriter> writers;

        public DecompileCommand(
            ILogger logger, 
            IDiagnosticLogger diagnosticLogger, 
            InvocationContext invocationContext, 
            ICompilationService compilationService,
            IEnumerable<IWriter> writers) 
        {
            this.logger = logger;
            this.diagnosticLogger = diagnosticLogger;
            this.invocationContext = invocationContext;
            this.compilationService = compilationService;
            this.writers = writers;
        }

        public int Run(DecompileArguments args)
        {
            logger.LogWarning(CliResources.DecompilerDisclaimerMessage);

            var inputPath = PathHelper.ResolvePath(args.InputFile);
            var outputPath = args.OutputDir == null
                ? PathHelper.GetDefaultDecompileOutputPath(inputPath) // use the inputPath's directory.
                : PathHelper.GetDefaultDecompileOutputPath(Path.Combine(PathHelper.ResolvePath(args.OutputDir), Path.GetFileName(inputPath))); // otherwise resolve to the outputDir.

            try
            {
                if (args.OutputToStdOut)
                {
                    ToStdout(inputPath); // --stdout
                }
                else
                {
                    ToFile(inputPath, args.OutputFile ?? outputPath); // --output-file or --output-dir
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

         private void ToStdout(string inputPath)
        {
            var decompilation = compilationService.Decompile(inputPath);

            // it's intended that we write here regardless of errors
            writers.ResolveService<ConsoleWriter>()
                .WriteDecompilation(decompilation);
        }

        private void ToFile(string inputPath, string outputPath)
        {
            var decompilation = compilationService.Decompile(inputPath, outputPath);

            // it's intended that we write here regardless of errors
            writers.ResolveService<FileWriter>()
                .WriteDecompilation(decompilation);
        }
    }
}
