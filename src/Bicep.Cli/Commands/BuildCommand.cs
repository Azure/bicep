// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Commands
{
    public class BuildCommand : ICommand
    {
        private readonly ILogger logger;
        private readonly DiagnosticLogger diagnosticLogger;
        private readonly BicepCompiler compiler;
        private readonly OutputWriter writer;
        private readonly IFeatureProviderFactory featureProviderFactory;

        public BuildCommand(
            ILogger logger,
            DiagnosticLogger diagnosticLogger,
            BicepCompiler compiler,
            OutputWriter writer,
            IFeatureProviderFactory featureProviderFactory)
        {
            this.logger = logger;
            this.diagnosticLogger = diagnosticLogger;
            this.compiler = compiler;
            this.writer = writer;
            this.featureProviderFactory = featureProviderFactory;
        }

        public async Task<int> RunAsync(BuildArguments args)
        {
            var inputUri = ArgumentHelper.GetFileUri(args.InputFile);
            ArgumentHelper.ValidateBicepFile(inputUri);

            var compilation = await compiler.CreateCompilation(inputUri, skipRestore: args.NoRestore);

            if (ExperimentalFeatureWarningProvider.TryGetEnabledExperimentalFeatureWarningMessage(compilation.SourceFileGrouping, featureProviderFactory) is { } warningMessage)
            {
                logger.LogWarning(warningMessage);
            }

            var summary = diagnosticLogger.LogDiagnostics(GetDiagnosticOptions(args), compilation);

            if (!summary.HasErrors)
            {
                if (args.OutputToStdOut)
                {
                    writer.TemplateToStdout(compilation);
                }
                else
                {
                    var outputPath = PathHelper.ResolveDefaultOutputPath(inputUri.LocalPath, args.OutputDir, args.OutputFile, PathHelper.GetDefaultBuildOutputPath);

                    writer.TemplateToFile(compilation, PathHelper.FilePathToFileUrl(outputPath));
                }
            }

            // return non-zero exit code on errors
            return summary.HasErrors ? 1 : 0;
        }

        private DiagnosticOptions GetDiagnosticOptions(BuildArguments args)
            => new(
                Format: args.DiagnosticsFormat ?? DiagnosticsFormat.Default,
                SarifToStdout: false);
    }
}
