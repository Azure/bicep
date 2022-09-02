// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core.FileSystem;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Bicep.Cli.Commands
{
    public class GenerateParametersFileCommand : ICommand
    {
        private readonly ILogger logger;
        private readonly IDiagnosticLogger diagnosticLogger;
        private readonly InvocationContext invocationContext;
        private readonly CompilationService compilationService;
        private readonly PlaceholderParametersWriter writer;

        public GenerateParametersFileCommand(
            ILogger logger,
            IDiagnosticLogger diagnosticLogger,
            InvocationContext invocationContext,
            CompilationService compilationService,
            PlaceholderParametersWriter writer)
        {
            this.logger = logger;
            this.diagnosticLogger = diagnosticLogger;
            this.invocationContext = invocationContext;
            this.compilationService = compilationService;
            this.writer = writer;
        }

        public async Task<int> RunAsync(GenerateParametersFileArguments args)
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

            if (!IsBicepFile(inputPath))
            {
                logger.LogError(CliResources.UnrecognizedFileExtensionMessage, inputPath);
                return 1;
            }

            var compilation = await compilationService.CompileAsync(inputPath, args.NoRestore);

            if (diagnosticLogger.ErrorCount < 1)
            {
                if (args.OutputToStdOut)
                {
                    writer.ToStdout(compilation);
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

                    outputPath = PathHelper.ResolveParametersFileOutputPath(outputPath);

                    writer.ToFile(compilation, outputPath);
                }
            }

            // return non-zero exit code on errors
            return diagnosticLogger.ErrorCount > 0 ? 1 : 0;
        }

        private bool IsBicepFile(string inputPath) => PathHelper.HasBicepExtension(PathHelper.FilePathToFileUrl(inputPath));
    }
}
