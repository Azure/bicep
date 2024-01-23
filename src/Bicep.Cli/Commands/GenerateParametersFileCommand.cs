// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Commands
{
    public class GenerateParametersFileCommand : ICommand
    {
        private readonly ILogger logger;
        private readonly DiagnosticLogger diagnosticLogger;
        private readonly IOContext io;
        private readonly CompilationService compilationService;
        private readonly PlaceholderParametersWriter writer;
        private readonly IFeatureProviderFactory featureProviderFactory;

        public GenerateParametersFileCommand(
            ILogger logger,
            DiagnosticLogger diagnosticLogger,
            IOContext io,
            CompilationService compilationService,
            PlaceholderParametersWriter writer,
            IFeatureProviderFactory featureProviderFactory)
        {
            this.logger = logger;
            this.diagnosticLogger = diagnosticLogger;
            this.io = io;
            this.compilationService = compilationService;
            this.writer = writer;
            this.featureProviderFactory = featureProviderFactory;
        }

        public async Task<int> RunAsync(GenerateParametersFileArguments args)
        {
            var inputPath = PathHelper.ResolvePath(args.InputFile);

            if (!IsBicepFile(inputPath))
            {
                logger.LogError(CliResources.UnrecognizedBicepFileExtensionMessage, inputPath);
                return 1;
            }

            var compilation = await compilationService.CompileAsync(inputPath, args.NoRestore);

            if (ExperimentalFeatureWarningProvider.TryGetEnabledExperimentalFeatureWarningMessage(compilation.SourceFileGrouping, featureProviderFactory) is { } warningMessage)
            {
                logger.LogWarning(warningMessage);
            }

            var summary = diagnosticLogger.LogDiagnostics(DiagnosticOptions.Default, compilation);

            if (!summary.HasErrors)
            {
                if (args.OutputToStdOut)
                {
                    writer.ToStdout(compilation, args.OutputFormat, args.IncludeParams);
                }
                else
                {
                    var outputPath = string.Empty;
                    if (!string.IsNullOrWhiteSpace(args.OutputDir))
                    {
                        outputPath = args.OutputDir;
                    }
                    else if (!string.IsNullOrWhiteSpace(args.OutputFile))
                    {
                        outputPath = args.OutputFile;
                    }
                    else
                    {
                        outputPath = inputPath;
                    }

                    outputPath = PathHelper.ResolveParametersFileOutputPath(outputPath, args.OutputFormat);

                    writer.ToFile(compilation, outputPath, args.OutputFormat, args.IncludeParams);
                }
            }

            // return non-zero exit code on errors
            return summary.HasErrors ? 1 : 0;
        }

        private bool IsBicepFile(string inputPath) => PathHelper.HasBicepExtension(PathHelper.FilePathToFileUrl(inputPath));
    }
}
