// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core;
using Bicep.Core.Features;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Commands
{
    public class TestCommand(
        IOContext io,
        ILogger logger,
        DiagnosticLogger diagnosticLogger,
        BicepCompiler compiler,
        IFeatureProviderFactory featureProviderFactory) : ICommand
    {
        private readonly ILogger logger = logger;
        private readonly IOContext io = io;
        private readonly DiagnosticLogger diagnosticLogger = diagnosticLogger;
        private readonly BicepCompiler compiler = compiler;
        private readonly IFeatureProviderFactory featureProviderFactory = featureProviderFactory;
        private const string SuccessSymbol = "[✓]";
        private const string FailureSymbol = "[✗]";
        private const string SkippedSymbol = "[-]";

        public async Task<int> RunAsync(TestArguments args)
        {
            var inputUri = ArgumentHelper.GetFileUri(args.InputFile);
            ArgumentHelper.ValidateBicepFile(inputUri);
            var features = featureProviderFactory.GetFeatureProvider(inputUri);

            if (!features.TestFrameworkEnabled)
            {
                await io.Error.WriteLineAsync("TestFrameWork not enabled");

                return 1;
            }

            logger.LogWarning(string.Format(CliResources.ExperimentalFeaturesDisclaimerMessage, "TestFramework"));

            var compilation = await compiler.CreateCompilation(inputUri, skipRestore: args.NoRestore);

            var summary = diagnosticLogger.LogDiagnostics(GetDiagnosticOptions(args), compilation);

            var semanticModel = compilation.GetEntrypointSemanticModel();

            var declarations = semanticModel.Root.TestDeclarations;
            var testResults = TestRunner.Run(declarations);

            LogResults(testResults);

            // return non-zero exit code on errors
            return testResults.Success ? 0 : 1;
        }

        private void LogResults(TestResults testResults)
        {
            foreach (var (testDeclaration, evaluation) in testResults.Results)
            {
                if (evaluation.Success)
                {
                    io.Output.WriteLine($"{SuccessSymbol} Evaluation {testDeclaration.Name} Passed!");
                }
                else if (evaluation.Skip)
                {
                    io.Error.WriteLine($"{SkippedSymbol} Evaluation {testDeclaration.Name} Skipped!");
                    io.Error.WriteLine($"Reason: {evaluation.Error}");
                }
                else
                {
                    io.Error.WriteLine($"{FailureSymbol} Evaluation {testDeclaration.Name} Failed at {evaluation.FailedAssertions.Length} / {evaluation.AllAssertions.Length} assertions!");
                    foreach (var (assertion, _) in evaluation.FailedAssertions)
                    {
                        io.Error.WriteLine($"\t{FailureSymbol} Assertion {assertion} failed!");
                    }
                }
            }
            if (testResults.Success)
            {
                io.Output.WriteLine($"All {testResults.TotalEvaluations} evaluations passed!");
            }
            else
            {
                io.Error.WriteLine($"Evaluation Summary: Failure!");
                io.Error.WriteLine($"Total: {testResults.TotalEvaluations} - Success: {testResults.SuccessfullEvaluations} - Skipped: {testResults.SkippedEvaluations} - Failed: {testResults.FailedEvaluations}");
            }
        }

        private DiagnosticOptions GetDiagnosticOptions(TestArguments args)
            => new(
                Format: args.DiagnosticsFormat ?? DiagnosticsFormat.Default,
                SarifToStdout: false);
    }
}
