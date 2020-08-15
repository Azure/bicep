using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
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

        private readonly JsonTextWriter writer;
        private readonly SemanticModel.SemanticModel semanticModel;
        private readonly ExpressionEmitter emitter;

        public TemplateEmitter(JsonTextWriter writer, SemanticModel.SemanticModel semanticModel)
        {
            this.writer = writer;
            this.semanticModel = semanticModel;
            this.emitter = new ExpressionEmitter(writer, semanticModel);
        }

        /// <summary>
        /// Emits a template to the specified file if there are no errors. The specified file is not touched if there are compilation errors.
        /// </summary>
        /// <param name="fileName">The path to the file.</param>
        public EmitResult Emit(string fileName)
        {
            // collect all the diagnostics
            var diagnostics = this.semanticModel.GetAllDiagnostics();

            if (diagnostics.Any(d => d.Level == DiagnosticLevel.Error))
            {
                return new EmitResult(EmitStatus.Failed, diagnostics);
            }

            using var stream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            this.EmitInternal(stream);

            return new EmitResult(EmitStatus.Succeeded, diagnostics);
        }

        /// <summary>
        /// Emits a template to the specified stream if there are no errors. No writes are made to the stream if there are compilation errors.
        /// </summary>
        /// <param name="stream">The stream to write the template</param>
        public EmitResult Emit(Stream stream)
        {
            // collect all the diagnostics
            var diagnostics = this.semanticModel.GetAllDiagnostics();

            if (diagnostics.Any(d => d.Level == DiagnosticLevel.Error))
            {
                return new EmitResult(EmitStatus.Failed, diagnostics);
            }

            this.EmitInternal(stream);

            return new EmitResult(EmitStatus.Succeeded, diagnostics);
        }

        /// <summary>
        /// Emits a template to the specified text writer if there are no errors. No writes are made to the writer if there are compilation errors.
        /// </summary>
        /// <param name="writer">The text writer to write the template</param>
        public EmitResult Emit(TextWriter writer)
        {
            // collect all the diagnostics
            var diagnostics = this.semanticModel.GetAllDiagnostics();

            if (diagnostics.Any(d => d.Level == DiagnosticLevel.Error))
            {
                return new EmitResult(EmitStatus.Failed, diagnostics);
            }

            this.EmitInternal(writer);

            return new EmitResult(EmitStatus.Succeeded, diagnostics);
        }

        /// <summary>
        /// Emits a template to the specified json writer if there are no errors. No writes are made to the writer if there are compilation errors.
        /// </summary>
        /// <param name="writer">The json writer to write the template</param>
        public EmitResult Emit(JsonTextWriter writer)
        {
            // collect all the diagnostics
            var diagnostics = this.semanticModel.GetAllDiagnostics();

            if (diagnostics.Any(d => d.Level == DiagnosticLevel.Error))
            {
                return new EmitResult(EmitStatus.Failed, diagnostics);
            }

            this.EmitInternal(writer);

            return new EmitResult(EmitStatus.Succeeded, diagnostics);
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

        private void EmitInternal()
        {
            writer.WriteStartObject();

            // TODO: Select by scope type
            this.emitter.EmitPropertyValue("$schema", "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#");

            this.emitter.EmitPropertyValue("contentVersion", "1.0.0.0");

            writer.WritePropertyName("parameters");
            this.EmitParameters();
            
            writer.WritePropertyName("functions");
            writer.WriteStartArray();
            writer.WriteEndArray();

            writer.WritePropertyName("variables");
            this.EmitVariables();

            writer.WritePropertyName("resources");
            this.EmitResources();

            writer.WritePropertyName("outputs");
            this.EmitOutputs();

            writer.WriteEndObject();
        }

        private void EmitParameters()
        {
            writer.WriteStartObject();

            foreach (var parameterSymbol in this.semanticModel.Root.ParameterDeclarations)
            {
                writer.WritePropertyName(parameterSymbol.Name);
                this.EmitParameter(parameterSymbol);
            }

            writer.WriteEndObject();
        }

        private void EmitParameter(ParameterSymbol parameterSymbol)
        {
            // local function
            bool IsSecure(SyntaxBase? value) => value is BooleanLiteralSyntax boolLiteral && boolLiteral.Value;

            writer.WriteStartObject();

            switch (parameterSymbol.Modifier)
            {
                case null:
                    this.emitter.EmitPropertyValue("type", GetTemplateTypeName(parameterSymbol.Type, secure: false));

                    break;

                case ParameterDefaultValueSyntax defaultValueSyntax:
                    this.emitter.EmitPropertyValue("type", GetTemplateTypeName(parameterSymbol.Type, secure: false));
                    this.emitter.EmitPropertyExpression("defaultValue", defaultValueSyntax.DefaultValue);

                    break;

                case ObjectSyntax modifierSyntax:
                    // this would throw on duplicate properties in the object node - we are relying on emitter checking for errors at the beginning
                    var properties = modifierSyntax.ToPropertyValueDictionary();

                    this.emitter.EmitPropertyValue("type", GetTemplateTypeName(parameterSymbol.Type, IsSecure(properties.TryGetValue("secure"))));

                    // relying on validation here as well (not all of the properties are valid in all contexts)
                    foreach (string modifierPropertyName in ParameterModifierPropertiesToEmitDirectly)
                    {
                        this.emitter.EmitOptionalPropertyExpression(modifierPropertyName, properties.TryGetValue(modifierPropertyName));
                    }
                    
                    break;
            }

            writer.WriteEndObject();
        }

        private void EmitVariables()
        {
            writer.WriteStartObject();

            foreach (var variableSymbol in this.semanticModel.Root.VariableDeclarations)
            {
                writer.WritePropertyName(variableSymbol.Name);
                this.EmitVariable(variableSymbol);
            }

            writer.WriteEndObject();
        }

        private void EmitVariable(VariableSymbol variableSymbol)
        {
            // TODO: When we have expressions, only expressions without runtime functions can be emitted this way. Everything else will need to be inlined.
            this.emitter.EmitExpression(variableSymbol.Value);
        }

        private void EmitResources()
        {
            writer.WriteStartArray();

            foreach (var resourceSymbol in this.semanticModel.Root.ResourceDeclarations)
            {
                this.EmitResource(resourceSymbol);
            }

            writer.WriteEndArray();
        }

        private void EmitResource(ResourceSymbol resourceSymbol)
        {
            writer.WriteStartObject();

            // using the throwing variant here because the semantic model should be completely valid at this point
            // (it's a code defect if it some errors were not emitted)
            ResourceTypeReference typeReference = ResourceTypeReference.Parse(resourceSymbol.Type.Name);

            this.emitter.EmitPropertyValue("type", typeReference.FullyQualifiedType);
            this.emitter.EmitPropertyValue("apiVersion", typeReference.ApiVersion);
            this.emitter.EmitObjectProperties((ObjectSyntax) resourceSymbol.Body);

            writer.WriteEndObject();
        }

        private void EmitOutputs()
        {
            writer.WriteStartObject();

            foreach (var outputSymbol in this.semanticModel.Root.OutputDeclarations)
            {
                writer.WritePropertyName(outputSymbol.Name);
                this.EmitOutput(outputSymbol);
            }

            writer.WriteEndObject();
        }

        private void EmitOutput(OutputSymbol outputSymbol)
        {
            writer.WriteStartObject();

            this.emitter.EmitPropertyValue("type", outputSymbol.Type.Name);
            this.emitter.EmitPropertyExpression("value", outputSymbol.Value);

            writer.WriteEndObject();
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
