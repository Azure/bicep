// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Linq;
using System.Threading.Tasks;
using Bicep.Cli.Arguments;
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
    private readonly IDiagnosticLogger diagnosticLogger;
    private readonly CompilationService compilationService;
    private readonly IFeatureProviderFactory featureProviderFactory;

    public LintCommand(
        ILogger logger,
        IDiagnosticLogger diagnosticLogger,
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
        var features = featureProviderFactory.GetFeatureProvider(PathHelper.FilePathToFileUrl(inputPath));
        var emitterSettings = new EmitterSettings(features, BicepSourceFileKind.BicepFile);

        if (emitterSettings.EnableSymbolicNames)
        {
            logger.LogWarning(CliResources.SymbolicNamesDisclaimerMessage);
        }

        if (features.ResourceTypedParamsAndOutputsEnabled)
        {
            logger.LogWarning(CliResources.ResourceTypesDisclaimerMessage);
        }

        if (IsBicepFile(inputPath))
        {
            diagnosticLogger.SetupFormat(args.DiagnosticsFormat);
            await compilationService.CompileAsync(inputPath, args.NoRestore);

            diagnosticLogger.FlushLog();

            // return non-zero exit code on errors
            return diagnosticLogger.ErrorCount > 0 ? 1 : 0;
        }

        logger.LogError(CliResources.UnrecognizedBicepFileExtensionMessage, inputPath);

        return 1;
    }

    private bool IsBicepFile(string inputPath) => PathHelper.HasBicepExtension(PathHelper.FilePathToFileUrl(inputPath));
}