// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.IO;
using System.Text;
using Bicep.Core.Emit;
using Bicep.Core.Intermediate;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Microsoft.WindowsAzure.ResourceStack.Common.Collections;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Cli.Services
{
    public class Validation
    {
        public InsensitiveDictionary<TestEvaluation> SuccessfullEvaluations { get; }

        public InsensitiveDictionary<TestEvaluation> FailedEvaluations { get; }

        public InsensitiveDictionary<TestEvaluation> SkippedEvaluations { get; }

        public bool Success => FailedEvaluations.Count == 0 && SkippedEvaluations.Count == 0;

        public int TotalEvaluations => SuccessfullEvaluations.Count + FailedEvaluations.Count + SkippedEvaluations.Count;

        public Validation(ImmutableArray<TestSymbol> testDeclarations)
        {
            SuccessfullEvaluations = new InsensitiveDictionary<TestEvaluation>();
            FailedEvaluations = new InsensitiveDictionary<TestEvaluation>();
            SkippedEvaluations = new InsensitiveDictionary<TestEvaluation>();

            Validate(testDeclarations);
        }
        public void Validate(ImmutableArray<TestSymbol> testDeclarations)
        {
            var templateOutputBuffer = new StringBuilder();
            using var textWriter = new StringWriter();

            EvaluateTemplates(testDeclarations, textWriter);    
        }
        private void EvaluateTemplates(ImmutableArray<TestSymbol> testDeclarations, StringWriter textWriter)
        {
            var evaluatedTemplates =  new InsensitiveDictionary<JToken>();

            foreach(var testDeclaration  in testDeclarations)
            {
                if (testDeclaration.TryGetSemanticModel(out var semanticModel, out var failureDiagnostic) &&
                    semanticModel is SemanticModel testSemanticModel)
                {
                    var parameters = TryGetParameters(testSemanticModel, testDeclaration);
                    var template = GetTemplate(testSemanticModel, testDeclaration);

                    var evaluation = TemplateEvaluator.Evaluate(template, parameters);

                    var skipped = evaluation.Skip;
                    var success = evaluation.Success;

                    var evaluations = skipped ? SkippedEvaluations : (success ? SuccessfullEvaluations : FailedEvaluations);
                    evaluations.Add(testDeclaration.Name, evaluation);
                
                }
                
            }

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