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

            var compilation = compilationService.Compile(inputPath);

            if (diagnosticLogger.ErrorCount < 1)
            {
                if (args.OutputToStdOut)
                {
                    writer.ToStdout(compilation);
                }
                else
                {
                    var outputPath = ResolveOutputPath(inputPath, args.OutputDir, args.OutputFile);

                    writer.ToFile(compilation, outputPath);
                }
            }

            if (args.NoSummary is false)
            {
                diagnosticLogger.LogSummary();
            }

            // return non-zero exit code on errors
           return diagnosticLogger.ErrorCount > 0 ? 1 : 0;
        }

        private static string ResolveOutputPath(string inputPath, string? outputDir, string? outputFile)
        {
            if(outputDir is not null)
            {
                var dir = PathHelper.ResolvePath(outputDir);
                var file = Path.GetFileName(inputPath);
                var path = Path.Combine(dir, file);

                return PathHelper.GetDefaultBuildOutputPath(path);
            }
            else if (outputFile is not null)
            {
                return PathHelper.ResolvePath(outputFile);
            }
            else
            {
                return PathHelper.GetDefaultBuildOutputPath(inputPath);
            }
        }
    }
}
