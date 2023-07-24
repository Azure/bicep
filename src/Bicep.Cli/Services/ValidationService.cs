// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
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
    public class ValidationService
    {
        public static void Validate(ImmutableArray<TestSymbol> testDeclarations)
        {
            var templateOutputBuffer = new StringBuilder();
            using var textWriter = new StringWriter();

            EvaluateTemplates(testDeclarations, textWriter);

            
        }
        private static void EvaluateTemplates(ImmutableArray<TestSymbol> testDeclarations, StringWriter textWriter)
        {
            var evaluatedTemplates =  new InsensitiveDictionary<JToken>();

            foreach(var testDeclaration  in testDeclarations)
            {
                if (testDeclaration.TryGetSemanticModel(out var semanticModel, out var failureDiagnostic) &&
                    semanticModel is SemanticModel testSemanticModel)
                {
                    var parameters = TryGetParameters(testSemanticModel, testDeclaration);
                    var template = GetTemplate(testSemanticModel, testDeclaration);

                    var evaluatedTemplate = TemplateEvaluatorService.Evaluate(template, parameters);

                    evaluatedTemplates.Add(testDeclaration.Name, evaluatedTemplate);
                
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