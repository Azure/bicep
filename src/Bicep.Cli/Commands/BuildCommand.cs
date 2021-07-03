// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core.FileSystem;
using System.IO;

namespace Bicep.Cli.Commands
{
    public class BuildCommand : ICommand
    {
        private readonly IDiagnosticLogger diagnosticLogger;
        private readonly CompilationService compilationService;
        private readonly CompilationWriter writer;

        public BuildCommand(
            IDiagnosticLogger diagnosticLogger,
            CompilationService compilationService,
            CompilationWriter writer)
        {
            this.diagnosticLogger = diagnosticLogger;
            this.compilationService = compilationService;
            this.writer = writer;
        }

        public int Run(BuildArguments args)
        {
            var inputPath = PathHelper.ResolvePath(args.InputFile);
            var outputPath = args.OutputDir == null
                ? PathHelper.GetDefaultBuildOutputPath(inputPath) // use the inputPath's directory.
                : PathHelper.GetDefaultBuildOutputPath(Path.Combine(PathHelper.ResolvePath(args.OutputDir), Path.GetFileName(inputPath))); // otherwise resolve to the outputDir.

            var compilation = compilationService.Compile(inputPath);

            if (diagnosticLogger.ErrorCount < 1)
            {
                if (args.OutputToStdOut)
                {
                    writer.ToStdout(compilation);
                }
                else
                {
                    writer.ToFile(compilation, args.OutputFile ?? outputPath);
                }
            }

            if (args.NoSummary is false)
            {
                diagnosticLogger.LogSummary();
            }

            // return non-zero exit code on errors
           return diagnosticLogger.ErrorCount > 0 ? 1 : 0;
        }
    }
}
