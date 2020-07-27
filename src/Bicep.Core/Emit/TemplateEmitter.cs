using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using Bicep.Core.Extensions;
using Bicep.Core.Parser;
using Bicep.Core.Resources;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Newtonsoft.Json;

namespace Bicep.Core.Emit
{
    // TODO: Are there discrepancies between parameter, variable, and output names between bicep and ARM?
    public class TemplateEmitter
    {
        // these are top-level parameter modifier properties whose values can be emitted without any modifications
        private static readonly ImmutableArray<string> ParameterModifierPropertiesToEmitDirectly = new[]
        {
            "defaultValue",
            "allowedValues",
            "minValue",
            "maxValue",
            "minLength",
            "maxLength",
            "metadata"
        }.ToImmutableArray();

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

        /// <summary>
        /// Emits a template to the specified text writer if there are no errors. No writes are made to the writer if there are compilation errors.
        /// </summary>
        /// <param name="writer">The text writer to write the template</param>
        public EmitResult Emit(TextWriter writer)
        {
            // collect all the errors
            var diagnostics = this.semanticModel.GetAllDiagnostics();

            if (diagnostics.Any())
            {
                // TODO: This needs to account for warnings when we add severity.
                return new EmitResult(EmitStatus.Failed, diagnostics);
            }

            this.EmitInternal(writer);

            return new EmitResult(EmitStatus.Succeeded, new Error[0]);
        }

        private void EmitInternal(Stream stream)
        {
            using var writer = new JsonTextWriter(new StreamWriter(stream, Encoding.UTF8, 4096, true))
            {
                Formatting = Formatting.Indented
            };

            EmitInternal(writer);
        }

        private void EmitInternal(TextWriter textWriter)
        {
            using var writer = new JsonTextWriter(textWriter)
            {
                Formatting = Formatting.Indented
            };

            EmitInternal(writer);
        }

        private void EmitInternal(JsonTextWriter writer)
        {
            writer.WriteStartObject();

            // TODO: Select by scope type
            this.EmitPropertyValue(writer, "$schema", "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#");

            this.EmitPropertyValue(writer, "contentVersion", "1.0.0.0");

            writer.WritePropertyName("parameters");
            this.EmitParameters(writer);
            
            writer.WritePropertyName("functions");
            writer.WriteStartArray();
            writer.WriteEndArray();

            writer.WritePropertyName("variables");
            this.EmitVariables(writer);

            writer.WritePropertyName("resources");
            this.EmitResources(writer);

            writer.WritePropertyName("outputs");
            this.EmitOutputs(writer);

            writer.WriteEndObject();
        }

        private void EmitParameters(JsonTextWriter writer)
        {
            writer.WriteStartObject();

            foreach (var parameterSymbol in this.semanticModel.Root.ParameterDeclarations)
            {
                writer.WritePropertyName(parameterSymbol.Name);
                this.EmitParameter(writer, parameterSymbol);
            }

            writer.WriteEndObject();
        }

        private void EmitParameter(JsonTextWriter writer, ParameterSymbol parameterSymbol)
        {
            // local function
            bool IsSecure(SyntaxBase? value) => value is BooleanLiteralSyntax boolLiteral && boolLiteral.Value;

            writer.WriteStartObject();

            switch (parameterSymbol.Modifier)
            {
                case null:
                    this.EmitPropertyValue(writer, "type", GetTemplateTypeName(parameterSymbol.Type, secure: false));

                    break;

                case ParameterDefaultValueSyntax defaultValueSyntax:
                    this.EmitPropertyValue(writer, "type", GetTemplateTypeName(parameterSymbol.Type, secure: false));
                    this.EmitPropertyExpression(writer, "defaultValue", defaultValueSyntax.DefaultValue);

                    break;

                case ObjectSyntax modifierSyntax:
                    // this would throw on invalid object node - we are relying on emitter checking for errors at the beginning
                    var properties = modifierSyntax.ToPropertyDictionary();

                    this.EmitPropertyValue(writer, "type", GetTemplateTypeName(parameterSymbol.Type, IsSecure(properties.TryGetValue("secure"))));

                    // relying on validation here as well (not all of the properties are valid in all contexts)
                    foreach (string modifierPropertyName in ParameterModifierPropertiesToEmitDirectly)
                    {
                        this.EmitOptionalPropertyExpression(writer, modifierPropertyName, properties.TryGetValue(modifierPropertyName));
                    }
                    
                    break;
            }

            writer.WriteEndObject();
        }

        private void EmitVariables(JsonTextWriter writer)
        {
            writer.WriteStartObject();

            foreach (var variableSymbol in this.semanticModel.Root.VariableDeclarations)
            {
                writer.WritePropertyName(variableSymbol.Name);
                this.EmitVariable(writer, variableSymbol);
            }

            writer.WriteEndObject();
        }

        private void EmitVariable(JsonTextWriter writer, VariableSymbol variableSymbol)
        {
            // TODO: When we have expressions, only expressions without runtime functions can be emitted this way. Everything else will need to be inlined.
            this.EmitExpression(writer, variableSymbol.Value);
        }

        private void EmitResources(JsonTextWriter writer)
        {
            writer.WriteStartArray();

            foreach (var resourceSymbol in this.semanticModel.Root.ResourceDeclarations)
            {
                this.EmitResource(writer, resourceSymbol);
            }

            writer.WriteEndArray();
        }

        private void EmitResource(JsonTextWriter writer, ResourceSymbol resourceSymbol)
        {
            writer.WriteStartObject();

            // using the throwing variant here because the semantic model should be completely valid at this point
            // (it's a code defect if it some errors were not emitted)
            ResourceTypeReference typeReference = ResourceTypeReference.Parse(resourceSymbol.Type.Name);

            this.EmitPropertyValue(writer, "type", typeReference.FullyQualifiedType);
            this.EmitPropertyValue(writer, "apiVersion", typeReference.ApiVersion);
            this.EmitObjectProperties(writer, (ObjectSyntax) resourceSymbol.Body);

            writer.WriteEndObject();
        }

        private void EmitOutputs(JsonTextWriter writer)
        {
            writer.WriteStartObject();

            foreach (var outputSymbol in this.semanticModel.Root.OutputDeclarations)
            {
                writer.WritePropertyName(outputSymbol.Name);
                this.EmitOutput(writer, outputSymbol);
            }

            writer.WriteEndObject();
        }

        private void EmitOutput(JsonTextWriter writer, OutputSymbol outputSymbol)
        {
            writer.WriteStartObject();

            this.EmitPropertyValue(writer, "type", outputSymbol.Type.Name);
            this.EmitPropertyExpression(writer, "value", outputSymbol.Value);

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
                    // using the throwing method to get semantic value of the string because
                    // error checking should have caught any errors by now
                    writer.WriteValue(stringSyntax.GetValue());
                    break;

                case ObjectSyntax objectSyntax:
                    writer.WriteStartObject();
                    this.EmitObjectProperties(writer, objectSyntax);
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

        private void EmitObjectProperties(JsonTextWriter writer, ObjectSyntax objectSyntax)
        {
            foreach (ObjectPropertySyntax propertySyntax in objectSyntax.Properties)
            {
                this.EmitPropertyExpression(writer, propertySyntax.Identifier.IdentifierName, propertySyntax.Value);
            }
        }

        private void EmitPropertyValue(JsonTextWriter writer, string name, string value)
        {
            writer.WritePropertyName(name);
            writer.WriteValue(value);
        }

        private void EmitPropertyExpression(JsonTextWriter writer, string name, SyntaxBase expression)
        {
            writer.WritePropertyName(name);
            this.EmitExpression(writer, expression);
        }

        private void EmitOptionalPropertyExpression(JsonTextWriter writer, string name, SyntaxBase? expression)
        {
            if (expression != null)
            {
                this.EmitPropertyExpression(writer, name, expression);
            }
        }

        private string GetTemplateTypeName(TypeSymbol type, bool secure)
        {
            if (secure)
            {
                if (ReferenceEquals(type, LanguageConstants.String))
                {
                    return "secureString";
                }

                if (ReferenceEquals(type, LanguageConstants.Object))
                {
                    return "secureObject";
                }
            }

            return type.Name;
        }
    }
}
