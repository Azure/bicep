// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
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

            if (IsBicepFile(inputPath))
            {
                diagnosticLogger.SetupFormat(args.DiagnosticsFormat);
                var compilation = await compilationService.CompileAsync(inputPath, args.NoRestore);

                foreach (var warning in ExperimentalFeatureWarningProvider.GetEnabledExperimentalFeatureWarningMessages(compilation.SourceFileGrouping, featureProviderFactory))
                {
                    logger.LogWarning(warning);
                }

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

                diagnosticLogger.FlushLog();

                // return non-zero exit code on errors
                return diagnosticLogger.ErrorCount > 0 ? 1 : 0;
            }

            logger.LogError(CliResources.UnrecognizedBicepFileExtensionMessage, inputPath);
            return 1;
        }

        private bool IsBicepFile(string inputPath) => PathHelper.HasBicepExtension(PathHelper.FilePathToFileUrl(inputPath));
    }
}
