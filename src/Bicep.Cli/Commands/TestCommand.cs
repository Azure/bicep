// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Bicep.Cli.Arguments;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core.Emit;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Workspaces;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Commands
{
    public class TestCommand : ICommand
    {
        private readonly ILogger logger;
        private readonly IDiagnosticLogger diagnosticLogger;
        private readonly CompilationService compilationService;
        private readonly CompilationWriter writer;
        private readonly IFeatureProviderFactory featureProviderFactory;
        private const string SuccessSymbol = "[✓]";
        private const string FailureSymbol = "[✗]";
        private const string SkippedSymbol = "[-]";

        public TestCommand(
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

        public async Task<int> RunAsync(TestArguments args)
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

            if(!features.TestFrameworkEnabled) 
            {
                logger.LogError("TestFrameWork not enabled");
                
                return 1;
            }

            if (IsBicepFile(inputPath))
            {
                diagnosticLogger.SetupFormat(args.DiagnosticsFormat);
                var validation = await compilationService.TestAsync(inputPath, args.NoRestore);
                LogResults(validation);
                diagnosticLogger.FlushLog();

                // return non-zero exit code on errors
                return diagnosticLogger.ErrorCount > 0 ? 1 : 0;
            }

            logger.LogError(CliResources.UnrecognizedBicepFileExtensionMessage, inputPath);
            return 1;
        }

        private bool IsBicepFile(string inputPath) => PathHelper.HasBicepExtension(PathHelper.FilePathToFileUrl(inputPath));
        private void LogResults(Validation validation){
            foreach(var (name, evaluation) in validation.SuccessfullEvaluations){
                logger.LogInformation($"[✓] Evaluation {name} passed!");
            }
            foreach(var (name, evaluation) in validation.SkippedEvaluations){
                logger.LogError($"[-] Evaluation {name} skipped!");
                logger.LogError($"Reason: {evaluation.Error?.Message}");

            }
            foreach(var (name, evaluation) in validation.FailedEvaluations){
                logger.LogError($"[✗] Evaluation {name} failed at {evaluation.FailedAssertions.Count} / {evaluation.Assertions.Count} assertions!");
                foreach(var (assertion, _) in evaluation.FailedAssertions){
                    logger.LogError($"\t{FailureSymbol} Assertion {assertion} failed!");
                }
                
            }
            if (validation.Success)
            {
                logger.LogInformation($"All {validation.TotalEvaluations} evaluations passed!");
            }
            else 
            {
                logger.LogError($"Evaluation Summary: Failure!");
                logger.LogError($"Total: {validation.TotalEvaluations} - Success: {validation.SuccessfullEvaluations.Count} - Skipped: {validation.SkippedEvaluations.Count} - Failed: {validation.FailedEvaluations.Count}");
            }
            
        }
    }
}
