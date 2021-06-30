// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License
using System;
using System.Collections.Generic;
using System.IO;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core.Diagnostics;
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
            try
            {
                 if (args.OutputToStdOut)
                {
                    ToStdout(inputPath);
                }
                else if (args.OutputDir is not null)
                {
                    var outputDir = PathHelper.ResolvePath(args.OutputDir);

                    if (!Directory.Exists(outputDir))
                    {
                        throw new CommandLineException(string.Format(CliResources.DirectoryDoesNotExistFormat, outputDir));
                    }

                    var outputPath = Path.Combine(outputDir, Path.GetFileName(inputPath));

                    ToFile(inputPath, PathHelper.GetDefaultDecompileOutputPath(outputPath));
                }
                else
                {
                    ToFile(inputPath, args.OutputFile ?? PathHelper.GetDefaultDecompileOutputPath(inputPath));
                }
            }
            catch (Exception exception) when (
                exception is CommandLineException || 
                exception is BicepException || 
                exception is ErrorDiagnosticException) 
            {
                throw; // We pass these custom exception types back without further processing.
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

            writers.ResolveService<ConsoleWriter>()
                .WriteDecompilation(decompilation);
        }

        private void ToFile(string inputPath, string outputPath)
        {
            var decompilation = compilationService.Decompile(inputPath, outputPath);

            writers.ResolveService<FileWriter>()
                .WriteDecompilation(decompilation);
        }
    }
}
