using System;
using System.IO;
using System.Linq;
using System.Text;
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

        /// <summary>
        /// Emits a template to the specified file if there are no errors. The specified file is not touched if there are compilation errors.
        /// </summary>
        /// <param name="fileName">The path to the file.</param>
        public EmitResult Emit(string fileName)
        {
            // collect all the errors
            var diagnostics = this.semanticModel.GetAllDiagnostics();

            if (diagnostics.Any())
            {
                // TODO: This needs to account for warnings when we add severity.
                return new EmitResult(EmitStatus.Failed, diagnostics);
            }

            using var stream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            this.EmitInternal(stream);

            return new EmitResult(EmitStatus.Succeeded, new Error[0]);
        }

        /// <summary>
        /// Emits a template to the specified stream if there are no errors. No writes are made to the stream if there are compilation errors.
        /// </summary>
        /// <param name="stream">The stream to write the template</param>
        public EmitResult Emit(Stream stream)
        {
            // collect all the errors
            var diagnostics = this.semanticModel.GetAllDiagnostics();

            if (diagnostics.Any())
            {
                // TODO: This needs to account for warnings when we add severity.
                return new EmitResult(EmitStatus.Failed, diagnostics);
            }

            this.EmitInternal(stream);

            return new EmitResult(EmitStatus.Succeeded, new Error[0]);
        }

        private void EmitInternal(Stream stream)
        {
            using var writer = new JsonTextWriter(new StreamWriter(stream, Encoding.UTF8, 4096, true))
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
                    writer.WriteValue(stringSyntax.GetValue());
                    break;

                case ObjectSyntax objectSyntax:
                    writer.WriteStartObject();

                    foreach (ObjectPropertySyntax propertySyntax in objectSyntax.Properties)
                    {
                        writer.WritePropertyName(propertySyntax.Identifier.IdentifierName);
                        this.EmitExpression(writer, propertySyntax.Value);
                    }

                    writer.WriteEndObject();

                    break;

                case ArraySyntax arraySyntax:
                    writer.WriteStartArray();

                    foreach (ArrayItemSyntax itemSyntax in arraySyntax.Items)
                    {
                        this.EmitExpression(writer, itemSyntax.Value);
                    }

                    writer.WriteEndArray();

                    break;
                    
                default:
                    throw new NotImplementedException($"Cannot emit unexpected expression of type {syntax.GetType().Name}");
            }
        }
    }
}
