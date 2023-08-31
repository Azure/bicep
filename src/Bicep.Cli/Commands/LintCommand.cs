// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

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

        if (IsBicepFile(inputPath))
        {
            diagnosticLogger.SetupFormat(args.DiagnosticsFormat);
            var compilation = await compilationService.CompileAsync(inputPath, args.NoRestore);

            if (ExperimentalFeatureWarningProvider.TryGetEnabledExperimentalFeatureWarningMessage(compilation.SourceFileGrouping, featureProviderFactory) is { } warningMessage)
            {
                logger.LogWarning(warningMessage);
            }

            diagnosticLogger.FlushLog();

            // return non-zero exit code on errors
            return diagnosticLogger.ErrorCount > 0 ? 1 : 0;
        }

        logger.LogError(CliResources.UnrecognizedBicepFileExtensionMessage, inputPath);
        return 1;
    }

    private bool IsBicepFile(string inputPath) => PathHelper.HasBicepExtension(PathHelper.FilePathToFileUrl(inputPath));
}