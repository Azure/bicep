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
    public class TestCommand : ICommand
    {
        private const string SuccessSymbol = "[✓]";
        private const string FailureSymbol = "[✗]";
        private const string SkippedSymbol = "[-]";

        private readonly ILogger logger;
        private readonly IOContext io;
        private readonly DiagnosticLogger diagnosticLogger;
        private readonly BicepCompiler compiler;
        private readonly IFeatureProviderFactory featureProviderFactory;
        private readonly InputOutputArgumentsResolver inputOutputArgumentsResolver;

        public TestCommand(
            IOContext io,
            ILogger logger,
            DiagnosticLogger diagnosticLogger,
            BicepCompiler compiler,
            IFeatureProviderFactory featureProviderFactory,
            InputOutputArgumentsResolver inputOutputArgumentsResolver)
        {
            this.logger = logger;
            this.diagnosticLogger = diagnosticLogger;
            this.compiler = compiler;
            this.featureProviderFactory = featureProviderFactory;
            this.io = io;
            this.inputOutputArgumentsResolver = inputOutputArgumentsResolver;
        }

        public async Task<int> RunAsync(TestArguments args)
        {
            var inputUri = this.inputOutputArgumentsResolver.ResolveInputArguments(args);
            ArgumentHelper.ValidateBicepFile(inputUri);
            var features = featureProviderFactory.GetFeatureProvider(inputUri);

            if (!features.TestFrameworkEnabled)
            {
                await io.Error.Writer.WriteLineAsync("TestFrameWork not enabled");

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
                    io.Output.Writer.WriteLine($"{SuccessSymbol} Evaluation {testDeclaration.Name} Passed!");
                }
                else if (evaluation.Skip)
                {
                    io.Error.Writer.WriteLine($"{SkippedSymbol} Evaluation {testDeclaration.Name} Skipped!");
                    io.Error.Writer.WriteLine($"Reason: {evaluation.Error}");
                }
                else
                {
                    io.Error.Writer.WriteLine($"{FailureSymbol} Evaluation {testDeclaration.Name} Failed at {evaluation.FailedAssertions.Length} / {evaluation.AllAssertions.Length} assertions!");
                    foreach (var (assertion, _) in evaluation.FailedAssertions)
                    {
                        io.Error.Writer.WriteLine($"\t{FailureSymbol} Assertion {assertion} failed!");
                    }
                }
            }
            if (testResults.Success)
            {
                io.Output.Writer.WriteLine($"All {testResults.TotalEvaluations} evaluations passed!");
            }
            else
            {
                io.Error.Writer.WriteLine($"Evaluation Summary: Failure!");
                io.Error.Writer.WriteLine($"Total: {testResults.TotalEvaluations} - Success: {testResults.SuccessfulEvaluations} - Skipped: {testResults.SkippedEvaluations} - Failed: {testResults.FailedEvaluations}");
            }
        }

        private DiagnosticOptions GetDiagnosticOptions(TestArguments args)
            => new(
                Format: args.DiagnosticsFormat ?? DiagnosticsFormat.Default,
                SarifToStdout: false);
    }
}
