// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.Core.Utils;

namespace Bicep.Cli.Commands;

public class RestoreCommand : ICommand
{
    private readonly IEnvironment environment;
    private readonly BicepCompiler compiler;
    private readonly DiagnosticLogger diagnosticLogger;

    public RestoreCommand(
        IEnvironment environment,
        BicepCompiler compiler,
        DiagnosticLogger diagnosticLogger)
    {
        this.environment = environment;
        this.compiler = compiler;
        this.diagnosticLogger = diagnosticLogger;
    }

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
        var restoreDiagnostics = await this.compiler.Restore(compilation, forceRestore: force);

        var summary = diagnosticLogger.LogDiagnostics(DiagnosticOptions.Default, restoreDiagnostics);

        return summary;
    }

    public async Task<DiagnosticSummary> RestoreMultiple(RestoreArguments args)
    {
        var hasErrors = false;

        foreach (var inputUri in CommandHelper.GetFilesMatchingPattern(environment, args.FilePatternRoot, args.FilePatterns))
        {
            ArgumentHelper.ValidateBicepOrBicepParamFile(inputUri);

            var result = await Restore(inputUri, args.ForceModulesRestore);
            hasErrors |= result.HasErrors;
        }

        return new(hasErrors);
    }
}
