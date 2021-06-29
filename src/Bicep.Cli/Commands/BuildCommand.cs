// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Cli.Arguments;
using Bicep.Cli.Services;
using Bicep.Cli.Logging;
using Microsoft.Extensions.Logging;
using System.IO;
using Bicep.Core.FileSystem;

namespace Bicep.Cli.Commands
{
    public class BuildCommand : ICommand
    {
        private readonly ILogger logger;
        private readonly IDiagnosticLogger diagnosticLogger;
        private readonly InvocationContext invocationContext;
        private readonly CompilationService compilationService;

        public BuildCommand(ILogger logger, IDiagnosticLogger diagnosticLogger, InvocationContext invocationContext, CompilationService compilationService) 
        {
            this.logger = logger;
            this.diagnosticLogger = diagnosticLogger;
            this.invocationContext = invocationContext;
            this.compilationService = compilationService;
        }

        public int Run(BuildArguments args)
        {
            var inputPath = PathHelper.ResolvePath(args.InputFile);

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

                ToFile(inputPath, PathHelper.GetDefaultBuildOutputPath(outputPath));
            }
            else
            {
                ToFile(inputPath, args.OutputFile ?? PathHelper.GetDefaultBuildOutputPath(inputPath));
            }
            
            // only the build command supports the --no-summary switch.
            if(!args.NoSummary)
            {
                diagnosticLogger.LogSummary();
            }
            
            // return non-zero exit code on errors
            return diagnosticLogger.ErrorCount > 0 ? 1 : 0;
        }

        private int ToStdout(string inputPath)
        {
            return compilationService
                .Compile(inputPath)
                .LogDiagnostics()
                .PrintCompilationOnSuccess()
                .GetResult() ? 0 : 1;
        }

        private int ToFile(string inputPath, string outputPath)
        {
            return compilationService
                .Compile(inputPath)
                .LogDiagnostics()
                .WriteCompilationFileOnSuccess(outputPath)
                .GetResult() ? 0 : 1;
        }
    }
}