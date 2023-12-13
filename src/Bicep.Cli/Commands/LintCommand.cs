// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Linq;
using System.Threading.Tasks;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Workspaces;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Commands;

public class LintCommand : ICommand
{
    private readonly ILogger logger;
    private readonly DiagnosticLogger diagnosticLogger;
    private readonly CompilationService compilationService;
    private readonly IFeatureProviderFactory featureProviderFactory;

    public LintCommand(
        ILogger logger,
        DiagnosticLogger diagnosticLogger,
        CompilationService compilationService,
        IFeatureProviderFactory featureProviderFactory)
    {
        this.logger = logger;
        this.diagnosticLogger = diagnosticLogger;
        this.compilationService = compilationService;
        this.featureProviderFactory = featureProviderFactory;
    }

    public async Task<int> RunAsync(LintArguments args)
    {
        var inputPath = PathHelper.ResolvePath(args.InputFile);
        var inputUri = PathHelper.FilePathToFileUrl(inputPath);

        if (!PathHelper.HasBicepExtension(inputUri) &&
            !PathHelper.HasBicepparamsExension(inputUri))
        {
            logger.LogError(CliResources.UnrecognizedBicepOrBicepparamsFileExtensionMessage, inputPath);
            return 1;
        }

        var compilation = await compilationService.CompileAsync(inputPath, args.NoRestore);

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
