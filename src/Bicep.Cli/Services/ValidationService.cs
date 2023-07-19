// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.IO;
using System.Text;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Microsoft.WindowsAzure.ResourceStack.Common.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Cli.Services
{
    public class ValidationService
    {
        public static void Validate(ImmutableArray<TestSymbol> testDeclarations)
        {
            var templateOutputBuffer = new StringBuilder();
            using var textWriter = new StringWriter(templateOutputBuffer);

            EvaluateTemplates(testDeclarations, textWriter);

            
        }
        private static void EvaluateTemplates(ImmutableArray<TestSymbol> testDeclarations, StringWriter textWriter)
        {

            var evaluatedTemplates =  new InsensitiveDictionary<JToken>();

            foreach(var testDeclaration  in testDeclarations)
            {
                testDeclaration.TryGetSemanticModel(out var semanticModel, out var failureDiagnostic);

                var parameters = testDeclaration.DeclaringTest.TryGetParameters();

                if (semanticModel is SemanticModel testSemanticModel)
                {
                    var sourceFileToTrack = testSemanticModel.Features.SourceMappingEnabled ? testSemanticModel.SourceFile : default;

                    using var writer = new SourceAwareJsonTextWriter(testSemanticModel.FileResolver, textWriter, sourceFileToTrack)
                    {
                        // don't close the textWriter when writer is disposed
                        CloseOutput = false,
                        Formatting = Formatting.Indented

                    };

                    var templateWriter = new TemplateWriter(testSemanticModel);

                    var (_, templateJtoken) = templateWriter.GetTemplate(writer);

                    var evaluatedTemplate = TemplateEvaluatorService.Evaluate(templateJtoken, parameters);

                    evaluatedTemplates.Add(testDeclaration.Name, evaluatedTemplate);

                }
                
            }

        }
    }
}