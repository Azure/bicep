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

            static string DefaultOutputPath(string path) => PathHelper.GetDefaultDecompileOutputPath(path);

            var outputPath = PathHelper.ResolveDefaultOutputPath(inputPath, args.OutputDir, args.OutputFile, DefaultOutputPath);

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
    }
}
