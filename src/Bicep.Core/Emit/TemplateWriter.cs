// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.Linq;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics;
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
            LanguageConstants.ResourceScopePropertyName,
            LanguageConstants.ResourceDependsOnPropertyName,
        }.ToImmutableHashSet();

        private static ImmutableHashSet<string> ModulePropertiesToOmit = new [] {
            LanguageConstants.ModuleParamsPropertyName,
            LanguageConstants.ResourceScopePropertyName,
            LanguageConstants.ResourceDependsOnPropertyName,
        }.ToImmutableHashSet();

        private static SemanticModel GetModuleSemanticModel(ModuleSymbol moduleSymbol)
        {
            if (!moduleSymbol.TryGetSemanticModel(out var moduleSemanticModel, out _))
            {
                // this should have already been checked during type assignment
                throw new InvalidOperationException($"Unable to find referenced compilation for module {moduleSymbol.Name}");
            }
            
            return moduleSemanticModel;
        }

        private readonly JsonTextWriter writer;
        private readonly EmitterContext context;
        private readonly ExpressionEmitter emitter;

        public TemplateWriter(JsonTextWriter writer, SemanticModel semanticModel)
        {
            this.writer = writer;
            this.context = new EmitterContext(semanticModel);
            this.emitter = new ExpressionEmitter(writer, context);
        }

        private static string GetSchema(ResourceScope targetScope)
        {
            if (targetScope.HasFlag(ResourceScope.Tenant))
            {
                return "https://schema.management.azure.com/schemas/2019-08-01/tenantDeploymentTemplate.json#";
            }

            if (targetScope.HasFlag(ResourceScope.ManagementGroup))
            {
                return "https://schema.management.azure.com/schemas/2019-08-01/managementGroupDeploymentTemplate.json#";
            }

            if (targetScope.HasFlag(ResourceScope.Subscription))
            {
                return "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#";
            }

            return "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#";
        }

        public void Write()
        {
            writer.WriteStartObject();

            this.emitter.EmitProperty("$schema", GetSchema(context.SemanticModel.TargetScope));

            this.emitter.EmitProperty("contentVersion", "1.0.0.0");

            this.EmitParametersIfPresent();

            writer.WritePropertyName("functions");
            writer.WriteStartArray();
            writer.WriteEndArray();

            this.EmitVariablesIfPresent();

            this.EmitResources();

            this.EmitOutputsIfPresent();

            writer.WriteEndObject();
        }

        private void EmitParametersIfPresent()
        {
            if (this.context.SemanticModel.Root.ParameterDeclarations.Length == 0)
            {
                return;
            }

            writer.WritePropertyName("parameters");
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

            if (parameterSymbol.DeclaringParameter.Decorators.Any())
            {
                var parameterType = SyntaxFactory.CreateStringLiteral(primitiveType.Name);
                var parameterObject = SyntaxFactory.CreateObject(SyntaxFactory.CreateObjectProperty("type", parameterType).AsEnumerable());

                if (parameterSymbol.Modifier is ParameterDefaultValueSyntax defaultValueSyntax)
                {
                    parameterObject.MergeProperty("defaultValue", defaultValueSyntax.DefaultValue);
                }

                foreach (var decoratorSyntax in parameterSymbol.DeclaringParameter.Decorators.Reverse())
                {

                    var symbol = this.context.SemanticModel.GetSymbolInfo(decoratorSyntax.Expression);

                    if (symbol is FunctionSymbol decoratorSymbol)
                    {
                        var argumentTypes = decoratorSyntax.Arguments
                            .Select(argument => this.context.SemanticModel.TypeManager.GetTypeInfo(argument))
                            .ToArray();

                        // There should be exact one matching decorator since there's no errors.
                        Decorator decorator = this.context.SemanticModel.Root.ImportedNamespaces
                            .SelectMany(ns => ns.Value.Type.DecoratorResolver.GetMatches(decoratorSymbol, argumentTypes))
                            .Single();
                        
                        parameterObject = decorator.Evaluate(decoratorSyntax, primitiveType, parameterObject);
                    }
                }

                foreach (var property in parameterObject.Properties)
                {
                    if (property.TryGetKeyText() is string propertyName)
                    {
                        this.emitter.EmitProperty(propertyName, property.Value);
                    }
                }
            }
            else
            {
                // TODO: remove this before the 0.3 release.
                switch (parameterSymbol.Modifier)
                {
                    case null:
                        this.emitter.EmitProperty("type", GetTemplateTypeName(primitiveType, secure: false));

                        break;

                    case ParameterDefaultValueSyntax defaultValueSyntax:
                        this.emitter.EmitProperty("type", GetTemplateTypeName(primitiveType, secure: false));
                        this.emitter.EmitProperty("defaultValue", defaultValueSyntax.DefaultValue);

                        break;

                    case ObjectSyntax modifierSyntax:
                        // this would throw on duplicate properties in the object node - we are relying on emitter checking for errors at the beginning
                        var properties = modifierSyntax.ToKnownPropertyValueDictionary();

                        this.emitter.EmitProperty("type", GetTemplateTypeName(primitiveType, IsSecure(properties.TryGetValue("secure"))));

                        // relying on validation here as well (not all of the properties are valid in all contexts)
                        foreach (string modifierPropertyName in ParameterModifierPropertiesToEmitDirectly)
                        {
                            this.emitter.EmitOptionalPropertyExpression(modifierPropertyName, properties.TryGetValue(modifierPropertyName));
                        }

                        this.emitter.EmitOptionalPropertyExpression("defaultValue", properties.TryGetValue(LanguageConstants.ParameterDefaultPropertyName));
                        this.emitter.EmitOptionalPropertyExpression("allowedValues", properties.TryGetValue(LanguageConstants.ParameterAllowedPropertyName));
                        
                        break;
                }
            }

            writer.WriteEndObject();
        }

        private void EmitVariablesIfPresent()
        {
            if (!this.context.SemanticModel.Root.VariableDeclarations.Any(symbol => !this.context.VariablesToInline.Contains(symbol)))
            {
                return;
            }

            writer.WritePropertyName("variables");
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
            writer.WritePropertyName("resources");
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
            if (resourceSymbol.DeclaringResource.IfCondition is IfConditionSyntax ifCondition)
            {
                this.emitter.EmitProperty("condition", ifCondition.ConditionExpression);
            }

            this.emitter.EmitProperty("type", typeReference.FullyQualifiedType);
            this.emitter.EmitProperty("apiVersion", typeReference.ApiVersion);
            if (context.SemanticModel.EmitLimitationInfo.ResoureScopeData[resourceSymbol] is ResourceSymbol scopeResource)
            {
                this.emitter.EmitProperty("scope", () => this.emitter.EmitUnqualifiedResourceId(scopeResource));
            }
            this.emitter.EmitObjectProperties((ObjectSyntax)resourceSymbol.DeclaringResource.Body, ResourcePropertiesToOmit);

            // dependsOn is currently not allowed as a top-level resource property in bicep
            // we will need to revisit this and probably merge the two if we decide to allow it
            this.EmitDependsOn(resourceSymbol);

            writer.WriteEndObject();
        }

        private void EmitModuleParameters(ModuleSymbol moduleSymbol)
        {
            var paramsValue = moduleSymbol.SafeGetBodyPropertyValue(LanguageConstants.ModuleParamsPropertyName);
            if (paramsValue is not ObjectSyntax paramsObjectSyntax)
            {
                // 'params' is optional if the module has no required params
                return;
            }

            writer.WritePropertyName("parameters");

            writer.WriteStartObject();

            foreach (var propertySyntax in paramsObjectSyntax.Properties)
            {
                if (!(propertySyntax.TryGetKeyText() is string keyName))
                {
                    // should have been caught by earlier validation
                    throw new ArgumentException("Disallowed interpolation in module parameter");
                }

                writer.WritePropertyName(keyName);
                {
                    writer.WriteStartObject();
                    this.emitter.EmitProperty("value", propertySyntax.Value);
                    writer.WriteEndObject();
                }
            }

            writer.WriteEndObject();
        }

        private void EmitModule(ModuleSymbol moduleSymbol)
        {
            writer.WriteStartObject();

            if (moduleSymbol.DeclaringModule.IfCondition is IfConditionSyntax ifCondition)
            {
                this.emitter.EmitProperty("condition", ifCondition.ConditionExpression);
            }

            this.emitter.EmitProperty("type", NestedDeploymentResourceType);
            this.emitter.EmitProperty("apiVersion", NestedDeploymentResourceApiVersion);

            // emit all properties apart from 'params'. In practice, this currrently only allows 'name', but we may choose to allow other top-level resource properties in future.
            // params requires special handling (see below).
            this.emitter.EmitObjectProperties((ObjectSyntax)moduleSymbol.DeclaringModule.Body, ModulePropertiesToOmit);


            var scopeData = context.ModuleScopeData[moduleSymbol];
            ScopeHelper.EmitModuleScopeProperties(context.SemanticModel.TargetScope, scopeData, emitter);

            if (scopeData.RequestedScope != ResourceScope.ResourceGroup)
            {
                // if we're deploying to a scope other than resource group, we need to supply a location
                if (this.context.SemanticModel.TargetScope == ResourceScope.ResourceGroup)
                {
                    // the deployment() object at resource group scope does not contain a property named 'location', so we have to use resourceGroup().location
                    this.emitter.EmitProperty("location", new FunctionExpression(
                        "resourceGroup",
                        new LanguageExpression[] { },
                        new LanguageExpression[] { new JTokenExpression("location") }));
                }
                else
                {
                    // at all other scopes we can just use deployment().location
                    this.emitter.EmitProperty("location", new FunctionExpression(
                        "deployment",
                        new LanguageExpression[] { },
                        new LanguageExpression[] { new JTokenExpression("location") }));
                }
            }

            writer.WritePropertyName("properties");
            {
                writer.WriteStartObject();

                writer.WritePropertyName("expressionEvaluationOptions");
                {
                    writer.WriteStartObject();
                    this.emitter.EmitProperty("scope", "inner");
                    writer.WriteEndObject();
                }

                this.emitter.EmitProperty("mode", "Incremental");

                EmitModuleParameters(moduleSymbol);

                writer.WritePropertyName("template");
                {
                    var moduleSemanticModel = GetModuleSemanticModel(moduleSymbol);
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
                        emitter.EmitResourceIdReference(resourceDependency);
                        break;
                    case ModuleSymbol moduleDependency:
                        emitter.EmitResourceIdReference(moduleDependency);
                        break;
                    default:
                        throw new InvalidOperationException($"Found dependency '{dependency.Name}' of unexpected type {dependency.GetType()}");
                }
            }
            writer.WriteEndArray();
        }

        private void EmitOutputsIfPresent()
        {
            if (this.context.SemanticModel.Root.OutputDeclarations.Length == 0)
            {
                return;
            }

            writer.WritePropertyName("outputs");
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

            this.emitter.EmitProperty("type", outputSymbol.Type.Name);
            this.emitter.EmitProperty("value", outputSymbol.Value);

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

