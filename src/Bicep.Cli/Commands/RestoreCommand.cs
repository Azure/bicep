// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Bicep.Cli.Arguments;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core.FileSystem;

namespace Bicep.Cli.Commands
{
    public class RestoreCommand : ICommand
    {
        private readonly CompilationService compilationService;
        private readonly DiagnosticLogger diagnosticLogger;

        public RestoreCommand(CompilationService compilationService, DiagnosticLogger diagnosticLogger)
        {
            this.compilationService = compilationService;
            this.diagnosticLogger = diagnosticLogger;
        }

        public async Task<int> RunAsync(RestoreArguments args)
        {
            var inputPath = PathHelper.ResolvePath(args.InputFile);
            var diagnostics = await this.compilationService.RestoreAsync(inputPath, args.ForceModulesRestore);

            var summary = diagnosticLogger.LogDiagnostics(DiagnosticOptions.Default, diagnostics);

            // return non-zero exit code on errors
            return summary.HasErrors ? 1 : 0;
        }
    }
}
