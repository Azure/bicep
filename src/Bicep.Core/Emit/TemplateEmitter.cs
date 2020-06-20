using System;
using System.IO;
using System.Linq;
using Bicep.Core.Parser;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Newtonsoft.Json;

namespace Bicep.Core.Emit
{
    public class TemplateEmitter
    {
        private readonly SemanticModel.SemanticModel semanticModel;

        public TemplateEmitter(SemanticModel.SemanticModel semanticModel)
        {
            this.semanticModel = semanticModel;
        }

        public EmitResult Emit(string fileName)
        {
            // collect all the errors
            var diagnostics = this.semanticModel.GetAllDiagnostics();

            if (diagnostics.Any())
            {
                // TODO: This needs to account for warnings when we add severity.
                return new EmitResult(EmitStatus.Failed, diagnostics);
            }

            this.EmitInternal(fileName);

            return new EmitResult(EmitStatus.Succeeded, new Error[0]);
        }

        private void EmitInternal(string fileName)
        {
            using var writer = new JsonTextWriter(new StreamWriter(new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None)))
            {
                Formatting = Formatting.Indented
            };

            writer.WriteStartObject();

            writer.WritePropertyName("$schema");
            // TODO: Select by scope type
            writer.WriteValue("https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#");

            writer.WritePropertyName("contentVersion");
            writer.WriteValue("1.0.0.0");

            writer.WritePropertyName("parameters");
            writer.WriteStartObject();
            this.EmitParameters(writer);
            writer.WriteEndObjectAsync();

            writer.WritePropertyName("functions");
            writer.WriteStartArray();
            writer.WriteEndArray();

            writer.WritePropertyName("variables");
            writer.WriteStartObject();
            writer.WriteEndObjectAsync();

            writer.WritePropertyName("resources");
            writer.WriteStartArray();
            writer.WriteEndArray();

            writer.WritePropertyName("outputs");
            writer.WriteStartObject();
            writer.WriteEndObjectAsync();

            writer.WriteEndObject();
        }

        private void EmitParameters(JsonTextWriter writer)
        {
            foreach (var parameterSymbol in this.semanticModel.Root.Descendants.OfType<ParameterSymbol>())
            {
                this.EmitParameter(writer, parameterSymbol);
            }
        }

        private void EmitParameter(JsonTextWriter writer, ParameterSymbol parameterSymbol)
        {
            writer.WritePropertyName(parameterSymbol.Name);
            writer.WriteStartObject();

            writer.WritePropertyName("type");
            writer.WriteValue(parameterSymbol.Type.Name);

            if (parameterSymbol.DefaultValue != null)
            {
                writer.WritePropertyName("defaultValue");
                EmitExpression(writer, parameterSymbol.DefaultValue);
            }
            
            writer.WriteEndObject();
        }

        private void EmitExpression(JsonTextWriter writer, SyntaxBase syntax)
        {
            switch (syntax)
            {
                case BooleanLiteralSyntax boolSyntax:
                    writer.WriteValue(boolSyntax.Value);
                    break;

                case NumericLiteralSyntax numericSyntax:
                    writer.WriteValue(numericSyntax.Value);
                    break;

                case StringSyntax stringSyntax:
                    writer.WriteValue(stringSyntax.StringToken.Text);
                    break;
                    
                default:
                    throw new NotImplementedException($"Cannot emit unexpected expression of type {syntax.GetType().Name}");
            }
        }
    }
}
