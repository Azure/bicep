// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Cli.Arguments;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core.Diagnostics;
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

        public DecompileCommand(ILogger logger, IDiagnosticLogger diagnosticLogger, InvocationContext invocationContext, CompilationService compilationService) 
        {
            this.logger = logger;
            this.diagnosticLogger = diagnosticLogger;
            this.invocationContext = invocationContext;
            this.compilationService = compilationService;
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

        private int ToStdout(string inputPath)
        {
            return compilationService
                .Decompile(inputPath)
                .PrintDecompilation()
                .CompileDecompilationOutput()
                .LogDiagnostics()
                .GetResult() ? 0 : 1;
        }

        private int ToFile(string inputPath, string outputPath)
        {
            return compilationService
                .Decompile(inputPath, outputPath)
                .WriteDecompilationFile()
                .CompileDecompilationOutput()
                .LogDiagnostics()
                .GetResult() ? 0 : 1;
        }
    }
}