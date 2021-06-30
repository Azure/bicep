// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Cli.Arguments;
using Bicep.Cli.Services;
using Bicep.Cli.Logging;
using Bicep.Cli.Helpers;
using System.IO;
using System.Collections.Generic;
using Bicep.Core.FileSystem;

namespace Bicep.Cli.Commands
{
    public class BuildCommand : ICommand
    {
        private readonly IDiagnosticLogger diagnosticLogger;
        private readonly ICompilationService compilationService;
        private readonly IEnumerable<IWriter> writers;

        public BuildCommand(
            IDiagnosticLogger diagnosticLogger, 
            ICompilationService compilationService,
            IEnumerable<IWriter> writers) 
        {
            this.diagnosticLogger = diagnosticLogger;
            this.compilationService = compilationService;
            this.writers = writers;
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

        private void ToStdout(string inputPath)
        {
            var compilation = compilationService.Compile(inputPath);

            if(diagnosticLogger.ErrorCount < 1)
            {
                writers.ResolveService<ConsoleWriter>()
                    .WriteCompilation(compilation);
            }
        }

        private void ToFile(string inputPath, string outputPath)
        {
            var compilation = compilationService.Compile(inputPath);

            if(diagnosticLogger.ErrorCount < 1)
            {
                writers.ResolveService<FileWriter>()
                    .CreateFileStream(outputPath)
                    .WriteCompilation(compilation);
            }
        }
    }
}
