// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.Core.Features;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Commands;

public class LintCommand : ICommand
{
    private readonly ILogger logger;
    private readonly DiagnosticLogger diagnosticLogger;
    private readonly BicepCompiler compiler;
    private readonly IFeatureProviderFactory featureProviderFactory;

    public LintCommand(
        ILogger logger,
        DiagnosticLogger diagnosticLogger,
        BicepCompiler compiler,
        IFeatureProviderFactory featureProviderFactory)
    {
        this.logger = logger;
        this.diagnosticLogger = diagnosticLogger;
        this.compiler = compiler;
        this.featureProviderFactory = featureProviderFactory;
    }

    public async Task<int> RunAsync(LintArguments args)
    {
        var inputUri = ArgumentHelper.GetFileUri(args.InputFile);
        ArgumentHelper.ValidateBicepOrBicepParamFile(inputUri);

        var compilation = await compiler.CreateCompilation(inputUri, skipRestore: args.NoRestore);

        if (ExperimentalFeatureWarningProvider.TryGetEnabledExperimentalFeatureWarningMessage(compilation.SourceFileGrouping, featureProviderFactory) is { } warningMessage)
        {
            logger.LogWarning(warningMessage);
        }

        var summary = diagnosticLogger.LogDiagnostics(GetDiagnosticOptions(args), compilation);

        // return non-zero exit code on errors
        return summary.HasErrors ? 1 : 0;
    }

    private DiagnosticOptions GetDiagnosticOptions(LintArguments args)
        => new(
            Format: args.DiagnosticsFormat ?? DiagnosticsFormat.Default,
            SarifToStdout: true);
}
