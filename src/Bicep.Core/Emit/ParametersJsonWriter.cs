// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Syntax;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Immutable;
using System.IO;
using Bicep.Core.Semantics;
using Azure.Deployments.Templates.Expressions;
using Azure.Deployments.Expression.Expressions;

namespace Bicep.Core.Emit
{
    public class ParametersJsonWriter
    {
        private readonly SemanticModel paramSemanticModel;
        private readonly ExpressionEvaluationContext expressionEvaluationContext;
        public ParametersJsonWriter(SemanticModel paramSemanticModel)
        {
            this.paramSemanticModel = paramSemanticModel;
            this.expressionEvaluationContext = GetExpressionEvaluationContext(paramSemanticModel);
        }

        public void Write(JsonTextWriter writer) => GenerateTemplate().WriteTo(writer);
        
        public JToken GenerateTemplate()
        {
            using var stringWriter = new StringWriter();
            using var jsonWriter = new JsonTextWriter(stringWriter);

            jsonWriter.WriteStartObject();

            jsonWriter.WritePropertyName("$schema");
            jsonWriter.WriteValue("https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#");

            jsonWriter.WritePropertyName("contentVersion");
            jsonWriter.WriteValue("1.0.0.0");

            //TODO: Update after param semantic model is complete
            var syntax = paramSemanticModel.SourceFile.ProgramSyntax;
            var parameters = syntax.Children.OfType<ParameterAssignmentSyntax>().ToImmutableList();

            if (parameters.Count > 0)
            {
                jsonWriter.WritePropertyName("parameters");
                jsonWriter.WriteStartObject();

                foreach (var parameter in parameters)
                {
                    jsonWriter.WritePropertyName(parameter.Name.IdentifierName);

                    jsonWriter.WriteStartObject();

                    jsonWriter.WritePropertyName("value");
                    this.EmitExpression(parameter.Value, jsonWriter);
                    jsonWriter.WriteEndObject();
                }
                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndObject();

            var content = stringWriter.ToString();

            return content.FromJson<JToken>();
        }

        public void EmitExpression(SyntaxBase syntax, JsonTextWriter jsonWriter)
        {
            var converter = new ExpressionConverter(new(paramSemanticModel));
            var expression = converter.ConvertExpression(syntax);
            
            var value = expression.EvaluateExpression(expressionEvaluationContext);
            value.WriteTo(jsonWriter);
        }

        private static ExpressionEvaluationContext GetExpressionEvaluationContext(SemanticModel semanticModel)
        {
            var converter = new ExpressionConverter(new(semanticModel));
            var paramsByName = semanticModel.Root.ParameterAssignments
                .ToImmutableDictionary(x => x.Name, LanguageConstants.IdentifierComparer);

            var helper = new TemplateExpressionEvaluationHelper();
            helper.OnGetParameter = (name, info) => {
                var param = paramsByName[name];
                var referenceExpression = converter.ConvertExpression(param.DeclaringParameterAssignment.Value);

                return referenceExpression.EvaluateExpression(helper.EvaluationContext, null);
            };

            return helper.EvaluationContext;
        }
    }
}
