// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Core;

namespace Bicep.Cli.Commands
{
    public class RestoreCommand(BicepCompiler compiler, DiagnosticLogger diagnosticLogger) : ICommand
    {
        private readonly BicepCompiler compiler = compiler;
        private readonly DiagnosticLogger diagnosticLogger = diagnosticLogger;

        public async Task<int> RunAsync(RestoreArguments args)
        {
            var inputUri = ArgumentHelper.GetFileUri(args.InputFile);

            var compilation = compiler.CreateCompilationWithoutRestore(inputUri, markAllForRestore: args.ForceModulesRestore);
            var restoreDiagnostics = await this.compiler.Restore(compilation, force: args.ForceModulesRestore);

            var summary = diagnosticLogger.LogDiagnostics(DiagnosticOptions.Default, restoreDiagnostics);

            // return non-zero exit code on errors
            return summary.HasErrors ? 1 : 0;
        }
    }
}
