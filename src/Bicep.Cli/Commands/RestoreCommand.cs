// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core.FileSystem;
using System.Threading.Tasks;

namespace Bicep.Cli.Commands
{
    public class RestoreCommand : ICommand
    {
        private readonly CompilationService compilationService;
        private readonly IDiagnosticLogger diagnosticLogger;

        public RestoreCommand(CompilationService compilationService, IDiagnosticLogger diagnosticLogger)
        {
            this.compilationService = compilationService;
            this.diagnosticLogger = diagnosticLogger;
        }

        public async Task<int> RunAsync(RestoreArguments args)
        {
            var inputPath = PathHelper.ResolvePath(args.InputFile);
            await this.compilationService.RestoreAsync(inputPath);

            // return non-zero exit code on errors
            return diagnosticLogger.ErrorCount > 0 ? 1 : 0;
        }
    }
}
