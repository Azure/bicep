using System;
using System.Collections.Immutable;
using System.Linq;
using Arm.Expression.Expressions;
using Bicep.Core.Extensions;
using Bicep.Core.Resources;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Newtonsoft.Json;

namespace Bicep.Core.Emit
{
    // TODO: Are there discrepancies between parameter, variable, and output names between bicep and ARM?
    public class TemplateWriter
    {
        // these are top-level parameter modifier properties whose values can be emitted without any modifications
        private static readonly ImmutableArray<string> ParameterModifierPropertiesToEmitDirectly = new[]
        {
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

        public TemplateWriter(JsonTextWriter writer, SemanticModel.SemanticModel semanticModel)
        {
            this.writer = writer;
            this.semanticModel = semanticModel;
            this.emitter = new ExpressionEmitter(writer, semanticModel);
        }

        public void Write()
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

                    this.emitter.EmitOptionalPropertyExpression("defaultValue", properties.TryGetValue("default"));
                    
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
            // TODO should we merge with the supplied props? Do we need to check if it's already been set? Add a test for this scenario
            this.EmitDependsOn(resourceSymbol);
            this.emitter.EmitObjectProperties((ObjectSyntax) resourceSymbol.Body);

            writer.WriteEndObject();
        }

        private void EmitDependsOn(ResourceSymbol resourceSymbol)
        {
            var dependencies = this.semanticModel.GetDependencies(resourceSymbol);
            if (dependencies.Length == 0)
            {
                return;
            }

            writer.WritePropertyName("dependsOn");
            writer.WriteStartArray();
            foreach (var dependency in dependencies)
            {
                var typeReference = ResourceTypeReference.Parse(dependency.Type.Name);
                emitter.EmitResourceIdReference(dependency.DeclaringResource, typeReference);
            }
            writer.WriteEndArray();
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
