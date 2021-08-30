// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core.FileSystem;
using System.Threading.Tasks;

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

        public async Task<int> RunAsync(BuildArguments args)
        {
            var inputPath = PathHelper.ResolvePath(args.InputFile);

            var compilation = await compilationService.CompileAsync(inputPath, args.NoRestore);

            if (diagnosticLogger.ErrorCount < 1)
            {
                if (args.OutputToStdOut)
                {
                    writer.ToStdout(compilation);
                }
                else
                {
                    static string DefaultOutputPath(string path) => PathHelper.GetDefaultBuildOutputPath(path);

                    var outputPath = PathHelper.ResolveDefaultOutputPath(inputPath, args.OutputDir, args.OutputFile, DefaultOutputPath);

                    writer.ToFile(compilation, outputPath);
                }
            }

            // return non-zero exit code on errors
           return diagnosticLogger.ErrorCount > 0 ? 1 : 0;
        }
    }
}
