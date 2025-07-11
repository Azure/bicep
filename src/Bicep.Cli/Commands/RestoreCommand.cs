// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.IO.Abstraction;

namespace Bicep.Cli.Commands;

public class RestoreCommand(
    BicepCompiler compiler,
    DiagnosticLogger diagnosticLogger,
    InputOutputArgumentsResolver inputOutputArgumentsResolver) : ICommand
{
    public async Task<int> RunAsync(RestoreArguments args)
    {
        var hasErrors = false;

        foreach (var inputUri in inputOutputArgumentsResolver.ResolveFilePatternInputArguments(args))
        {
            ArgumentHelper.ValidateBicepOrBicepParamFile(inputUri);

            var result = await Restore(inputUri, args.ForceModulesRestore);
            hasErrors |= result.HasErrors;
        }

        return CommandHelper.GetExitCode(new(hasErrors));
    }

    private async Task<DiagnosticSummary> Restore(IOUri inputUri, bool force)
    {
        var compilation = compiler.CreateCompilationWithoutRestore(inputUri.ToUri(), markAllForRestore: force);
        var restoreDiagnostics = await compiler.Restore(compilation, forceRestore: force);

        var summary = diagnosticLogger.LogDiagnostics(DiagnosticOptions.Default, restoreDiagnostics);

        return summary;
    }
}
