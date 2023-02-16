// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Bicep.Cli.Arguments;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core.Configuration;
using Bicep.Core.Emit;
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
            var bicepInputPath = args.BicepFile != null ? PathHelper.ResolvePath(args.BicepFile) : null;

            var features = featureProviderFactory.GetFeatureProvider(PathHelper.FilePathToFileUrl(paramsInputPath));
            var emitterSettings = new EmitterSettings(features);

            if (emitterSettings.EnableSymbolicNames)
            {
                logger.LogWarning(CliResources.SymbolicNamesDisclaimerMessage);
            }

            if (features.ResourceTypedParamsAndOutputsEnabled)
            {
                logger.LogWarning(CliResources.ResourceTypesDisclaimerMessage);
            }

            if(bicepInputPath != null && !IsBicepFile(bicepInputPath))
            {
                throw new InvalidOperationException($"{bicepInputPath} is not is bicep file");
            }

            if(features.ParamsFilesEnabled && IsBicepparamsFile(paramsInputPath)) 
            {
                var paramsCompilation = await compilationService.CompileAsync(paramsInputPath, args.NoRestore);

                var bicepSemanticModel = GetBicepSemanticModelFromParamsCompilation(paramsCompilation);

                if(bicepSemanticModel != null && bicepInputPath != null) 
                {   
                    var bicepFilePath = bicepSemanticModel.Root.FileUri;
                    
                    if(!bicepFilePath.Equals(PathHelper.FilePathToFileUrl(bicepInputPath)))
                    {
                        throw new InvalidOperationException($"Bicep file {bicepInputPath} provided with --bicep-file option doesn't match the Bicep file {bicepFilePath} referenced by the using declaration in the parameters file");
                    }
                }

                if (diagnosticLogger.ErrorCount < 1)
                {
                    if (args.OutputToStdOut)
                    {   
                        if(bicepSemanticModel != null) 
                        {
                            var paramsSemanticModel = paramsCompilation.GetEntrypointSemanticModel();
                            writer.ToStdout(bicepSemanticModel, paramsSemanticModel);
                        }
                    }
                    else
                    {
                        static string DefaultOutputPath(string path) => PathHelper.GetDefaultBuildOutputPath(path);
                        var paramsOutputPath = PathHelper.ResolveDefaultOutputPath(paramsInputPath, null, args.OutputParamsFile, DefaultOutputPath);
                        var bicepOutputPath = bicepInputPath != null ? 
                                              PathHelper.ResolveDefaultOutputPath(bicepInputPath, null, args.OutputParamsFile, DefaultOutputPath)
                                              : null;

                        if(bicepOutputPath != null && (paramsOutputPath == bicepOutputPath))
                        {
                            throw new Exception("JSON files for bicep template and parameters can not be generated with the same name");
                        }

                        writer.ToFile(paramsCompilation, paramsOutputPath);

                        if(bicepSemanticModel != null && bicepOutputPath != null)
                        {
                            writer.ToFile(bicepSemanticModel, bicepOutputPath);
                        }
                    }
                }

                return diagnosticLogger.ErrorCount > 0 ? 1 : 0;
            }


            if(!features.ParamsFilesEnabled) 
            {
                logger.LogError(CliResources.UnableToCompileParamsFile, paramsInputPath, nameof(ExperimentalFeaturesEnabled.ParamsFiles));
            }

            if (!IsBicepparamsFile(paramsInputPath))
            {
                logger.LogError(CliResources.UnrecognizedFileExtensionMessage, paramsInputPath);
            }

            return 1;
        }

        private SemanticModel? GetBicepSemanticModelFromParamsCompilation(Compilation paramsCompilation) 
        {
            var paramSemanticModel = paramsCompilation.GetEntrypointSemanticModel();
            
            if(paramSemanticModel.Root.TryGetBicepFileSemanticModelViaUsing(out var bicepSemanticModel, out _))
            {
                return bicepSemanticModel;
            }
           
           throw new InvalidOperationException($"Unable to read corresponding bicep file for the params file");
        }

        private bool IsBicepFile(string inputPath) => PathHelper.HasBicepExtension(PathHelper.FilePathToFileUrl(inputPath));

        private bool IsBicepparamsFile(string inputPath) => PathHelper.HasBicepparamsExension(PathHelper.FilePathToFileUrl(inputPath));
    }
}
