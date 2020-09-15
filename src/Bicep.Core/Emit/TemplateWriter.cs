// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Extensions;
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
            "minValue",
            "maxValue",
            "minLength",
            "maxLength",
            "metadata"
        }.ToImmutableArray();

        private static ImmutableHashSet<string> ResourcePropertiesToOmit = new [] {
            "dependsOn"
        }.ToImmutableHashSet();

        private readonly JsonTextWriter writer;
        private readonly EmitterContext context;
        private readonly ExpressionEmitter emitter;

        public TemplateWriter(JsonTextWriter writer, SemanticModel.SemanticModel semanticModel)
        {
            this.writer = writer;
            this.context = new EmitterContext(semanticModel);
            this.emitter = new ExpressionEmitter(writer, context);
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

            foreach (var parameterSymbol in this.context.SemanticModel.Root.ParameterDeclarations)
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
            var primitiveType = parameterSymbol.TryGetPrimitiveType();

            if (primitiveType == null)
            {
                // this should have been caught by the type checker long ago
                throw new ArgumentException($"Unable to find primitive type for parameter {parameterSymbol.Name}");
            }

            writer.WriteStartObject();

            switch (parameterSymbol.Modifier)
            {
                case null:
                    this.emitter.EmitPropertyValue("type", GetTemplateTypeName(primitiveType, secure: false));

                    break;

                case ParameterDefaultValueSyntax defaultValueSyntax:
                    this.emitter.EmitPropertyValue("type", GetTemplateTypeName(primitiveType, secure: false));
                    this.emitter.EmitPropertyExpression("defaultValue", defaultValueSyntax.DefaultValue);

                    break;

                case ObjectSyntax modifierSyntax:
                    // this would throw on duplicate properties in the object node - we are relying on emitter checking for errors at the beginning
                    var properties = modifierSyntax.ToPropertyValueDictionary();

                    this.emitter.EmitPropertyValue("type", GetTemplateTypeName(primitiveType, IsSecure(properties.TryGetValue("secure"))));

                    // relying on validation here as well (not all of the properties are valid in all contexts)
                    foreach (string modifierPropertyName in ParameterModifierPropertiesToEmitDirectly)
                    {
                        this.emitter.EmitOptionalPropertyExpression(modifierPropertyName, properties.TryGetValue(modifierPropertyName));
                    }

                    this.emitter.EmitOptionalPropertyExpression("defaultValue", properties.TryGetValue("default"));
                    this.emitter.EmitOptionalPropertyExpression("allowedValues", properties.TryGetValue("allowed"));
                    
                    break;
            }

            writer.WriteEndObject();
        }

        private void EmitVariables()
        {
            writer.WriteStartObject();

            foreach (var variableSymbol in this.context.SemanticModel.Root.VariableDeclarations)
            {
                if (!this.context.VariablesToInline.Contains(variableSymbol))
                {
                    writer.WritePropertyName(variableSymbol.Name);
                    this.EmitVariable(variableSymbol);
                }
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

            foreach (var resourceSymbol in this.context.SemanticModel.Root.ResourceDeclarations)
            {
                this.EmitResource(resourceSymbol);
            }

            writer.WriteEndArray();
        }

        private void EmitResource(ResourceSymbol resourceSymbol)
        {
            writer.WriteStartObject();

            var typeReference = EmitHelpers.GetTypeReference(resourceSymbol);

            this.emitter.EmitPropertyValue("type", typeReference.FullyQualifiedType);
            this.emitter.EmitPropertyValue("apiVersion", typeReference.ApiVersion);
            this.emitter.EmitObjectProperties((ObjectSyntax) resourceSymbol.Body, ResourcePropertiesToOmit);

            // dependsOn is currently not allowed as a top-level resource property in bicep
            // we will need to revisit this and probably merge the two if we decide to allow it
            this.EmitDependsOn(resourceSymbol);

            writer.WriteEndObject();
        }

        private void EmitDependsOn(ResourceSymbol resourceSymbol)
        {
            var dependencies = context.ResourceDependencies[resourceSymbol];
            if (!dependencies.Any())
            {
                return;
            }

            writer.WritePropertyName("dependsOn");
            writer.WriteStartArray();
            // need to put dependencies in a deterministic order to generate a deterministic template
            foreach (var dependency in dependencies.OrderBy(x => x.Name))
            {
                var typeReference = EmitHelpers.GetTypeReference(dependency);
                emitter.EmitResourceIdReference(dependency.DeclaringResource, typeReference);
            }
            writer.WriteEndArray();
        }

        private void EmitOutputs()
        {
            writer.WriteStartObject();

            foreach (var outputSymbol in this.context.SemanticModel.Root.OutputDeclarations)
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

