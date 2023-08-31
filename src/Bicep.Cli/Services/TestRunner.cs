// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Emit;
using Bicep.Core.Intermediate;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;

namespace Bicep.Cli.Services
{
    public record TestResult(TestSymbol Source, TestEvaluation Result);

    public record TestResults(ImmutableArray<TestResult> Results)
    {
        public int TotalEvaluations => Results.Length;

        public int SuccessfullEvaluations => Results.Count(x => x.Result.Success);

        public int FailedEvaluations => Results.Count(x => !x.Result.Success);

        public int SkippedEvaluations => Results.Count(x => x.Result.Skip);

        public bool Success => FailedEvaluations == 0 && SkippedEvaluations == 0;
    }
    public class TestRunner
    {
        public static TestResults Run(ImmutableArray<TestSymbol> testDeclarations)
        {
            var templateOutputBuffer = new StringBuilder();
            using var textWriter = new StringWriter();

            return EvaluateTemplates(testDeclarations, textWriter);
        }
        private static TestResults EvaluateTemplates(ImmutableArray<TestSymbol> testDeclarations, StringWriter textWriter)
        {
            var testResults = ImmutableArray.CreateBuilder<TestResult>(); ;
            foreach (var testDeclaration in testDeclarations)
            {
                if (testDeclaration.TryGetSemanticModel(out var semanticModel, out var failureDiagnostic) &&
                    semanticModel is SemanticModel testSemanticModel)
                {
                    var parameters = TryGetParameters(testSemanticModel, testDeclaration);
                    var template = GetTemplate(testSemanticModel, testDeclaration);

                    var evaluation = TemplateEvaluator.Evaluate(template, parameters);
                    var testResult = new TestResult(testDeclaration, evaluation);

                    testResults.Add(testResult);

                }

            }
            return new TestResults(testResults.ToImmutable());


        }

        private static JToken GetTemplate(SemanticModel model, TestSymbol test)
        {
            var textWriter = new StringWriter();
            using var writer = new SourceAwareJsonTextWriter(model.FileResolver, textWriter)
            {
                // don't close the textWriter when writer is disposed
                CloseOutput = false,
                Formatting = Formatting.Indented
            };
            var (_, template) = new TemplateWriter(model).GetTemplate(writer);

            return template;
        }

        private static JObject? TryGetParameters(SemanticModel model, TestSymbol test)
        {
            if (test.DeclaringTest.GetBody() is { } body &&
                body.TryGetPropertyByName("params") is { } paramsProperty)
            {
                var textWriter = new StringWriter();
                using var writer = new PositionTrackingJsonTextWriter(model.FileResolver, textWriter)
                {
                    // don't close the textWriter when writer is disposed
                    CloseOutput = false,
                    Formatting = Formatting.Indented
                };

                var emitter = new ExpressionEmitter(writer, new(model));
                var parametersExpression = new ExpressionBuilder(new(model)).Convert(paramsProperty.Value);
                new TemplateWriter(model).EmitTestParameters(emitter, parametersExpression);
                writer.Flush();

                return textWriter.ToString().FromJson<JObject>();
            }

            return null;
        }
    }
}