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
        public const string NestedDeploymentResourceType = "Microsoft.Resources/deployments";
        public const string NestedDeploymentResourceApiVersion = "2019-10-01";

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

            if (!(SyntaxHelper.TryGetPrimitiveType(parameterSymbol.DeclaringParameter) is TypeSymbol primitiveType))
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
                    var properties = modifierSyntax.ToKnownPropertyValueDictionary();

                    this.emitter.EmitPropertyValue("type", GetTemplateTypeName(primitiveType, IsSecure(properties.TryGetValue("secure"))));

                    // relying on validation here as well (not all of the properties are valid in all contexts)
                    foreach (string modifierPropertyName in ParameterModifierPropertiesToEmitDirectly)
                    {
                        this.emitter.EmitOptionalPropertyExpression(modifierPropertyName, properties.TryGetValue(modifierPropertyName));
                    }

                    this.emitter.EmitOptionalPropertyExpression("defaultValue", properties.TryGetValue(LanguageConstants.ParameterDefaultPropertyName));
                    this.emitter.EmitOptionalPropertyExpression("allowedValues", properties.TryGetValue(LanguageConstants.ParameterAllowedPropertyName));
                    
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

            foreach (var moduleSymbol in this.context.SemanticModel.Root.ModuleDeclarations)
            {
                this.EmitModule(moduleSymbol);
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

        private void EmitModule(ModuleSymbol moduleSymbol)
        {
            writer.WriteStartObject();

            // TODO: "name" implementation may change based on https://github.com/Azure/bicep/issues/614
            this.emitter.EmitPropertyValue("name", moduleSymbol.Name);
            this.emitter.EmitPropertyValue("type", NestedDeploymentResourceType);
            this.emitter.EmitPropertyValue("apiVersion", NestedDeploymentResourceApiVersion);

            writer.WritePropertyName("properties");
            {
                writer.WriteStartObject();

                writer.WritePropertyName("expressionEvaluationOptions");
                {
                    writer.WriteStartObject();
                    this.emitter.EmitPropertyValue("scope", "inner");
                    writer.WriteEndObject();
                }

                this.emitter.EmitPropertyValue("mode", "Incremental");

                writer.WritePropertyName("parameters");
                {
                    writer.WriteStartObject();

                    var moduleBody = (ObjectSyntax)moduleSymbol.Body;
                    foreach (var propertySyntax in moduleBody.Properties)
                    {
                        if (!(propertySyntax.TryGetKeyText() is string keyName))
                        {
                            // TODO improve this, add earlier validation.
                            throw new ArgumentException("Disallowed interpolation in module parameter");
                        }

                        writer.WritePropertyName(keyName);
                        {
                            writer.WriteStartObject();
                            this.emitter.EmitPropertyExpression("value", propertySyntax.Value);
                            writer.WriteEndObject();
                        }                        
                    }

                    writer.WriteEndObject();
                }

                writer.WritePropertyName("template");
                {
                    var moduleSemanticModel = moduleSymbol.TryGetSemanticModel(out _);
                    if (moduleSemanticModel == null)
                    {
                        // this should have already been checked during type assignment
                        throw new InvalidOperationException($"Unable to find referenced compilation for module {moduleSymbol.Name}");
                    }

                    var moduleWriter = new TemplateWriter(writer, moduleSemanticModel);
                    moduleWriter.Write();
                }

                writer.WriteEndObject();
            }

            this.EmitDependsOn(moduleSymbol);

            writer.WriteEndObject();
        }

        private void EmitDependsOn(DeclaredSymbol declaredSymbol)
        {
            var dependencies = context.ResourceDependencies[declaredSymbol];
            if (!dependencies.Any())
            {
                return;
            }

            writer.WritePropertyName("dependsOn");
            writer.WriteStartArray();
            // need to put dependencies in a deterministic order to generate a deterministic template
            foreach (var dependency in dependencies.OrderBy(x => x.Name))
            {
                switch (dependency)
                {
                    case ResourceSymbol resourceDependency:
                        var typeReference = EmitHelpers.GetTypeReference(resourceDependency);
                        emitter.EmitResourceIdReference(resourceDependency.DeclaringResource, typeReference);
                        break;
                    case ModuleSymbol moduleDependency:
                        emitter.EmitModuleResourceIdExpression(moduleDependency);
                        break;
                    default:
                        throw new InvalidOperationException($"Found dependency '{dependency.Name}' of unexpected type {dependency.GetType()}");
                }
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

