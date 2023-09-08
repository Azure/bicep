// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Commands
{
    public class BuildParamsCommand : ICommand
    {
        private readonly ILogger logger;
        private readonly IDiagnosticLogger diagnosticLogger;
        private readonly IOContext io;
        private readonly CompilationService compilationService;
        private readonly CompilationWriter writer;
        private readonly IFeatureProviderFactory featureProviderFactory;

        public BuildParamsCommand(
            ILogger logger,
            IDiagnosticLogger diagnosticLogger,
            IOContext io,
            CompilationService compilationService,
            CompilationWriter writer,
            IFeatureProviderFactory featureProviderFactory)
        {
            this.logger = logger;
            this.diagnosticLogger = diagnosticLogger;
            this.io = io;
            this.compilationService = compilationService;
            this.writer = writer;
            this.featureProviderFactory = featureProviderFactory;
        }

        public async Task<int> RunAsync(BuildParamsArguments args)
        {
            var paramsInputPath = PathHelper.ResolvePath(args.ParamsFile);
            var bicepFileArgPath = args.BicepFile != null ? PathHelper.ResolvePath(args.BicepFile) : null;

            if (bicepFileArgPath != null && !IsBicepFile(bicepFileArgPath))
            {
                throw new CommandLineException($"{bicepFileArgPath} is not a bicep file");
            }

            if (!IsBicepparamsFile(paramsInputPath))
            {
                logger.LogError(CliResources.UnrecognizedBicepparamsFileExtensionMessage, paramsInputPath);
                return 1;
            }

            var paramsCompilation = await compilationService.CompileAsync(
                paramsInputPath,
                args.NoRestore,
                compilation => {
                    if (bicepFileArgPath is not null &&
                        compilation.GetEntrypointSemanticModel().Root.TryGetBicepFileSemanticModelViaUsing().IsSuccess(out var usingModel))
                    {
                        if (usingModel is not SemanticModel bicepSemanticModel)
                        {
                            throw new CommandLineException($"Bicep file {bicepFileArgPath} provided with --bicep-file can only be used if the Bicep parameters \"using\" declaration refers to a Bicep file on disk.");
                        }

                        if (!bicepSemanticModel.Root.FileUri.Equals(PathHelper.FilePathToFileUrl(bicepFileArgPath)))
                        {
                            throw new CommandLineException($"Bicep file {bicepFileArgPath} provided with --bicep-file option doesn't match the Bicep file {bicepSemanticModel.Root.Name} referenced by the \"using\" declaration in the parameters file");
                        }
                    }
                });

            if (ExperimentalFeatureWarningProvider.TryGetEnabledExperimentalFeatureWarningMessage(paramsCompilation.SourceFileGrouping, featureProviderFactory) is {} message)
            {
                logger.LogWarning(message);
            }

            var paramsModel = paramsCompilation.GetEntrypointSemanticModel();

            //Failure scenario is ignored since a diagnostic for it would be emitted during semantic analysis
            if (paramsModel.Root.TryGetBicepFileSemanticModelViaUsing().IsSuccess(out var usingModel))
            {
                if (diagnosticLogger.ErrorCount < 1)
                {
                    if (args.OutputToStdOut)
                    {
                        writer.ToStdout(paramsModel, usingModel);
                    }
                    else
                    {
                        static string DefaultOutputPath(string path) => PathHelper.GetDefaultBuildOutputPath(path);
                        var paramsOutputPath = PathHelper.ResolveDefaultOutputPath(paramsInputPath, null, args.OutputFile, DefaultOutputPath);

                        writer.ToFile(paramsCompilation, paramsOutputPath);
                    }
                }
            }

            return diagnosticLogger.ErrorCount > 0 ? 1 : 0;
        }

        private bool IsBicepFile(string inputPath) => PathHelper.HasBicepExtension(PathHelper.FilePathToFileUrl(inputPath));

        private bool IsBicepparamsFile(string inputPath) => PathHelper.HasBicepparamsExension(PathHelper.FilePathToFileUrl(inputPath));
    }
}
