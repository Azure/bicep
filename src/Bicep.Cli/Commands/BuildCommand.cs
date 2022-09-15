// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Bicep.Cli.Arguments;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core.FileSystem;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Commands
{
    public class BuildCommand : ICommand
    {
        private readonly ILogger logger;
        private readonly IDiagnosticLogger diagnosticLogger;
        private readonly InvocationContext invocationContext;
        private readonly CompilationService compilationService;
        private readonly CompilationWriter writer;
        private readonly ParametersWriter paramsWriter;

        public BuildCommand(
            ILogger logger,
            IDiagnosticLogger diagnosticLogger,
            InvocationContext invocationContext,
            CompilationService compilationService,
            CompilationWriter writer,
            ParametersWriter paramsWriter)
        {
            this.logger = logger;
            this.diagnosticLogger = diagnosticLogger;
            this.invocationContext = invocationContext;
            this.compilationService = compilationService;
            this.writer = writer;
            this.paramsWriter = paramsWriter;
        }

        public async Task<int> RunAsync(BuildArguments args)
        {
            var inputPath = PathHelper.ResolvePath(args.InputFile);

            if (invocationContext.EmitterSettings.EnableSymbolicNames)
            {
                logger.LogWarning(CliResources.SymbolicNamesDisclaimerMessage);
            }

            if (invocationContext.Features.ResourceTypedParamsAndOutputsEnabled)
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
            else if (invocationContext.Features.ParamsFilesEnabled && IsBicepparamsFile(inputPath))
            {
                var model = await compilationService.CompileParams(inputPath, args.NoRestore);

                static string DefaultOutputPath(string path) => PathHelper.GetDefaultBuildOutputPath(path);
                var outputPath = PathHelper.ResolveDefaultOutputPath(inputPath, args.OutputDir, args.OutputFile, DefaultOutputPath);

                paramsWriter.ToFile(model, outputPath);

                return diagnosticLogger.ErrorCount > 0 ? 1 : 0;
            }

            logger.LogError(CliResources.UnrecognizedFileExtensionMessage, inputPath);
            return 1;
        }

        private bool IsBicepFile(string inputPath) => PathHelper.HasBicepExtension(PathHelper.FilePathToFileUrl(inputPath));

        private bool IsBicepparamsFile(string inputPath) => PathHelper.HasBicepparamsExension(PathHelper.FilePathToFileUrl(inputPath));
    }
}
