// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.Core.Utils;

namespace Bicep.Cli.Commands;

public class RestoreCommand(
    IEnvironment environment,
    BicepCompiler compiler,
    DiagnosticLogger diagnosticLogger) : ICommand
{
    public async Task<int> RunAsync(RestoreArguments args)
    {
        if (args.InputFile is null)
        {
            var summaryMultiple = await RestoreMultiple(args);
            return CommandHelper.GetExitCode(summaryMultiple);
        }

        var inputUri = ArgumentHelper.GetFileUri(args.InputFile);
        ArgumentHelper.ValidateBicepOrBicepParamFile(inputUri);

        var summary = await Restore(inputUri, args.ForceModulesRestore);
        return CommandHelper.GetExitCode(summary);
    }

    private async Task<DiagnosticSummary> Restore(Uri inputUri, bool force)
    {
        var compilation = compiler.CreateCompilationWithoutRestore(inputUri, markAllForRestore: force);
        var restoreDiagnostics = await compiler.Restore(compilation, forceRestore: force);

        var summary = diagnosticLogger.LogDiagnostics(DiagnosticOptions.Default, restoreDiagnostics);

        return summary;
    }

    public async Task<DiagnosticSummary> RestoreMultiple(RestoreArguments args)
    {
        var hasErrors = false;

        foreach (var inputUri in CommandHelper.GetInputFilesForPattern(environment, args.FilePattern))
        {
            ArgumentHelper.ValidateBicepOrBicepParamFile(inputUri);

            var result = await Restore(inputUri, args.ForceModulesRestore);
            hasErrors |= result.HasErrors;
        }

        return new(hasErrors);
    }
}
