// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Bicep.Cli.Arguments;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core.Configuration;
using Bicep.Core.Emit;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Workspaces;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Commands
{
    public class BuildCommand : ICommand
    {
        private readonly ILogger logger;
        private readonly IDiagnosticLogger diagnosticLogger;
        private readonly CompilationService compilationService;
        private readonly CompilationWriter writer;
        private readonly IFeatureProviderFactory featureProviderFactory;

        public BuildCommand(
            ILogger logger,
            IDiagnosticLogger diagnosticLogger,
            CompilationService compilationService,
            CompilationWriter writer,
            IFeatureProviderFactory featureProviderFactory)
        {
            this.logger = logger;
            this.diagnosticLogger = diagnosticLogger;
            this.compilationService = compilationService;
            this.writer = writer;
            this.featureProviderFactory = featureProviderFactory;
        }

        public async Task<int> RunAsync(BuildArguments args)
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
                var compilation = await compilationService.CompileAsync(inputPath, args.NoRestore);

                if (diagnosticLogger.ErrorCount < 1)
                {
                    if (args.OutputToStdOut)
                    {
                        writer.ToStdout(compilation);
                    }
                    else
                    {
                        static string DefaultOutputPath(string path) => PathHelper.GetDefaultBuildOutputPath(path);
                        var outputPath = PathHelper.ResolveDefaultOutputPath(inputPath, args.OutputDir, args.OutputFile, DefaultOutputPath);

                        writer.ToFile(compilation, outputPath);
                    }
                }

                // return non-zero exit code on errors
                return diagnosticLogger.ErrorCount > 0 ? 1 : 0;
            }

            logger.LogError(CliResources.UnrecognizedBicepFileExtensionMessage, inputPath);
            return 1;
        }

        private bool IsBicepFile(string inputPath) => PathHelper.HasBicepExtension(PathHelper.FilePathToFileUrl(inputPath));
    }
}
