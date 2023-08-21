// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;
using System.Threading.Tasks;
using Bicep.Cli.Arguments;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Commands
{
    public class TestCommand : ICommand
    {
        private readonly ILogger logger;
        private readonly TextWriter outputWriter;
        private readonly TextWriter errorWriter;
        private readonly IDiagnosticLogger diagnosticLogger;
        private readonly CompilationService compilationService;
        private readonly CompilationWriter writer;
        private readonly IFeatureProviderFactory featureProviderFactory;
        private const string SuccessSymbol = "[✓]";
        private const string FailureSymbol = "[✗]";
        private const string SkippedSymbol = "[-]";

        public TestCommand(
            IOContext io,
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
            this.outputWriter = io.Output;
            this.errorWriter = io.Error;
        }

        public async Task<int> RunAsync(TestArguments args)
        {
            var inputPath = PathHelper.ResolvePath(args.InputFile);
            var features = featureProviderFactory.GetFeatureProvider(PathHelper.FilePathToFileUrl(inputPath));

            if(!features.TestFrameworkEnabled)
            {
                await errorWriter.WriteLineAsync("TestFrameWork not enabled");

                return 1;
            }

            logger.LogWarning(string.Format(CliResources.ExperimentalFeaturesDisclaimerMessage, "TestFramework"));

            if (IsBicepFile(inputPath))
            {
                diagnosticLogger.SetupFormat(args.DiagnosticsFormat);
                var testResults = await compilationService.TestAsync(inputPath, args.NoRestore);
                LogResults(testResults);
                diagnosticLogger.FlushLog();

                // return non-zero exit code on errors
                return testResults.Success? 0 : 1;
            }

            await errorWriter.WriteLineAsync(string.Format(CliResources.UnrecognizedBicepFileExtensionMessage, inputPath));
            return 1;
        }

        private bool IsBicepFile(string inputPath) => PathHelper.HasBicepExtension(PathHelper.FilePathToFileUrl(inputPath));
        private void LogResults(TestResults testResults){
            foreach(var (testDeclaration, evaluation) in testResults.Results )
            {
                if(evaluation.Success)
                {
                    outputWriter.WriteLine($"{SuccessSymbol} Evaluation {testDeclaration.Name} Passed!");
                }
                else if(evaluation.Skip)
                {
                    errorWriter.WriteLine($"{SkippedSymbol} Evaluation {testDeclaration.Name} Skipped!");
                    errorWriter.WriteLine($"Reason: {evaluation.Error}");
                }
                else
                {
                    errorWriter.WriteLine($"{FailureSymbol} Evaluation {testDeclaration.Name} Failed at {evaluation.FailedAssertions.Length} / {evaluation.AllAssertions.Length} assertions!");
                    foreach(var (assertion, _) in evaluation.FailedAssertions){
                        errorWriter.WriteLine($"\t{FailureSymbol} Assertion {assertion} failed!");
                    }
                }
            }
            if (testResults.Success)
            {
                outputWriter.WriteLine($"All {testResults.TotalEvaluations} evaluations passed!");
            }
            else
            {
                errorWriter.WriteLine($"Evaluation Summary: Failure!");
                errorWriter.WriteLine($"Total: {testResults.TotalEvaluations} - Success: {testResults.SuccessfullEvaluations} - Skipped: {testResults.SkippedEvaluations} - Failed: {testResults.FailedEvaluations}");
            }
        }
    }
}
