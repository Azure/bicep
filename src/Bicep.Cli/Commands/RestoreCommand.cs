// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Core;

namespace Bicep.Cli.Commands
{
    public class RestoreCommand : ICommand
    {
        private readonly BicepCompiler compiler;
        private readonly DiagnosticLogger diagnosticLogger;

        public RestoreCommand(BicepCompiler compiler, DiagnosticLogger diagnosticLogger)
        {
            this.compiler = compiler;
            this.diagnosticLogger = diagnosticLogger;
        }

        public async Task<int> RunAsync(RestoreArguments args)
        {
            var inputUri = ArgumentHelper.GetFileUri(args.InputFile);

            var compilation = compiler.CreateCompilationWithoutRestore(inputUri, markAllForRestore: args.ForceModulesRestore);
            var restoreDiagnostics = await this.compiler.Restore(compilation, forceRestore: args.ForceModulesRestore);

            var summary = diagnosticLogger.LogDiagnostics(DiagnosticOptions.Default, restoreDiagnostics);

            // return non-zero exit code on errors
            return summary.HasErrors ? 1 : 0;
        }
    }
}
