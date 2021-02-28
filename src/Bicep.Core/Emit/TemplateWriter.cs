// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Azure.Deployments.Core.Extensions;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit
{
    // TODO: Are there discrepancies between parameter, variable, and output names between bicep and ARM?
    public class TemplateWriter
    {
        public const string NestedDeploymentResourceType = AzResourceTypeProvider.ResourceTypeDeployments;
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

        private static readonly ImmutableHashSet<string> ResourcePropertiesToOmit = new [] {
            LanguageConstants.ResourceScopePropertyName,
            LanguageConstants.ResourceDependsOnPropertyName,
        }.ToImmutableHashSet();

        private static readonly ImmutableHashSet<string> ModulePropertiesToOmit = new [] {
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

        private readonly JsonTextWriter writer;
        private readonly EmitterContext context;

        private readonly string assemblyFileVersion;

        public TemplateWriter(JsonTextWriter writer, SemanticModel semanticModel, string assemblyFileVersion)
        {
            this.writer = writer;
            this.context = new EmitterContext(semanticModel);
            this.assemblyFileVersion = assemblyFileVersion;
        }

        /// <summary>
        /// This emits the template into memory, calculates the template hash of the template
        /// It then adds the templateHash field to the template and finally writes it to our desired JsonTextWriter
        /// </summary>
        public void Write()
        {
            string templateString;
            using (StringWriter stringWriter = new())
            using (JsonTextWriter memoryWriter = new(stringWriter))
            {
                var emitter = new ExpressionEmitter(memoryWriter, this.context);
                // no templatehash, write to memory
                this.Write(memoryWriter, emitter, emitMetadata: true);
                // TODO: avoid reading the whole template into a string. Address this when ComputeTemplateHash 
                // has a streaming variant.
                templateString = stringWriter.ToString();
            }
            var templateHash = TemplateHashExtensions.ComputeTemplateHash(templateString);
            JObject template = (JObject)JObject.Parse(templateString);
            ((JObject) template["metadata"]!["_generator"]!).Add(new JProperty("templateHash", templateHash));
            template.WriteTo(this.writer);
        }

        /// <summary>
        /// Writing modules behaves differently than Write: we reuse the parent memoryWriter and do not generate metadata
        /// </summary>
        private void WriteModule()
        {
            this.Write(this.writer, new ExpressionEmitter(this.writer, this.context), emitMetadata: false);
        }


        private void Write(JsonTextWriter memoryWriter, ExpressionEmitter emitter, bool emitMetadata)
        {
            memoryWriter.WriteStartObject();

            emitter.EmitProperty("$schema", GetSchema(context.SemanticModel.TargetScope));

            emitter.EmitProperty("contentVersion", "1.0.0.0");

            this.EmitParametersIfPresent(memoryWriter, emitter);

            memoryWriter.WritePropertyName("functions");
            memoryWriter.WriteStartArray();
            memoryWriter.WriteEndArray();

            this.EmitVariablesIfPresent(memoryWriter, emitter);

            this.EmitResources(memoryWriter, emitter);

            this.EmitOutputsIfPresent(memoryWriter, emitter);

            // We skip emitting metadata when emitting modules (no metadata for nested deployments i.e. emitting modules)
            if (emitMetadata)
            {
                this.EmitMetadata(memoryWriter, emitter);
            }

            memoryWriter.WriteEndObject();
        }

        private void EmitParametersIfPresent(JsonTextWriter memoryWriter, ExpressionEmitter emitter)
        {
            if (this.context.SemanticModel.Root.ParameterDeclarations.Length == 0)
            {
                return;
            }

            memoryWriter.WritePropertyName("parameters");
            memoryWriter.WriteStartObject();

            foreach (var parameterSymbol in this.context.SemanticModel.Root.ParameterDeclarations)
            {
                memoryWriter.WritePropertyName(parameterSymbol.Name);
                this.EmitParameter(memoryWriter, parameterSymbol, emitter);
            }

            memoryWriter.WriteEndObject();
        }

        private void EmitParameter(JsonTextWriter memoryWriter, ParameterSymbol parameterSymbol, ExpressionEmitter emitter)
        {
            // local function
            bool IsSecure(SyntaxBase? value) => value is BooleanLiteralSyntax boolLiteral && boolLiteral.Value;

            if (!(SyntaxHelper.TryGetPrimitiveType(parameterSymbol.DeclaringParameter) is TypeSymbol primitiveType))
            {
                // this should have been caught by the type checker long ago
                throw new ArgumentException($"Unable to find primitive type for parameter {parameterSymbol.Name}");
            }

            memoryWriter.WriteStartObject();

            if (parameterSymbol.DeclaringParameter.Decorators.Any())
            {
                var parameterType = SyntaxFactory.CreateStringLiteral(primitiveType.Name);
                var parameterObject = SyntaxFactory.CreateObject(SyntaxFactory.CreateObjectProperty("type", parameterType).AsEnumerable());

                if (parameterSymbol.Modifier is ParameterDefaultValueSyntax defaultValueSyntax)
                {
                    parameterObject = parameterObject.MergeProperty("defaultValue", defaultValueSyntax.DefaultValue);
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
                        emitter.EmitProperty(propertyName, property.Value);
                    }
                }
            }
            else
            {
                // TODO: remove this before the 0.3 release.
                switch (parameterSymbol.Modifier)
                {
                    case null:
                        emitter.EmitProperty("type", GetTemplateTypeName(primitiveType, secure: false));

                        break;

                    case ParameterDefaultValueSyntax defaultValueSyntax:
                        emitter.EmitProperty("type", GetTemplateTypeName(primitiveType, secure: false));
                        emitter.EmitProperty("defaultValue", defaultValueSyntax.DefaultValue);

                        break;

                    case ObjectSyntax modifierSyntax:
                        // this would throw on duplicate properties in the object node - we are relying on emitter checking for errors at the beginning
                        var properties = modifierSyntax.ToKnownPropertyValueDictionary();

                        emitter.EmitProperty("type", GetTemplateTypeName(primitiveType, IsSecure(properties.TryGetValue("secure"))));

                        // relying on validation here as well (not all of the properties are valid in all contexts)
                        foreach (string modifierPropertyName in ParameterModifierPropertiesToEmitDirectly)
                        {
                            emitter.EmitOptionalPropertyExpression(modifierPropertyName, properties.TryGetValue(modifierPropertyName));
                        }

                        emitter.EmitOptionalPropertyExpression("defaultValue", properties.TryGetValue(LanguageConstants.ParameterDefaultPropertyName));
                        emitter.EmitOptionalPropertyExpression("allowedValues", properties.TryGetValue(LanguageConstants.ParameterAllowedPropertyName));

                        break;
                }
            }

            memoryWriter.WriteEndObject();
        }

        private void EmitVariablesIfPresent(JsonTextWriter memoryWriter, ExpressionEmitter emitter)
        {
            if (!this.context.SemanticModel.Root.VariableDeclarations.Any(symbol => !this.context.VariablesToInline.Contains(symbol)))
            {
                return;
            }

            memoryWriter.WritePropertyName("variables");
            memoryWriter.WriteStartObject();

            foreach (var variableSymbol in this.context.SemanticModel.Root.VariableDeclarations)
            {
                if (!this.context.VariablesToInline.Contains(variableSymbol))
                {
                    memoryWriter.WritePropertyName(variableSymbol.Name);
                    this.EmitVariable(variableSymbol, emitter);
                }
            }

            memoryWriter.WriteEndObject();
        }

        private void EmitVariable(VariableSymbol variableSymbol, ExpressionEmitter emitter)
        {
            emitter.EmitExpression(variableSymbol.Value);
        }

        private void EmitResources(JsonTextWriter memoryWriter, ExpressionEmitter emitter)
        {
            memoryWriter.WritePropertyName("resources");
            memoryWriter.WriteStartArray();

            foreach (var resourceSymbol in this.context.SemanticModel.Root.ResourceDeclarations)
            {
                if (resourceSymbol.DeclaringResource.IsExistingResource())
                {
                    continue;
                }

                this.EmitResource(memoryWriter, resourceSymbol, emitter);
            }

            foreach (var moduleSymbol in this.context.SemanticModel.Root.ModuleDeclarations)
            {
                this.EmitModule(memoryWriter, moduleSymbol, emitter);
            }

            memoryWriter.WriteEndArray();
        }

        private void EmitResource(JsonTextWriter memoryWriter, ResourceSymbol resourceSymbol, ExpressionEmitter emitter)
        {
            memoryWriter.WriteStartObject();

            var typeReference = EmitHelpers.GetTypeReference(resourceSymbol);
            SyntaxBase body = resourceSymbol.DeclaringResource.Value;
            switch (body)
            {
                case IfConditionSyntax ifCondition:
                    body = ifCondition.Body;
                    emitter.EmitProperty("condition", ifCondition.ConditionExpression);
                    break;

                case ForSyntax @for:
                    body = @for.Body;
                    emitter.EmitProperty("copy", () => emitter.EmitCopyObject(resourceSymbol.Name, @for, input: null));
                    break;
            }

            emitter.EmitProperty("type", typeReference.FullyQualifiedType);
            emitter.EmitProperty("apiVersion", typeReference.ApiVersion);
            if (context.SemanticModel.EmitLimitationInfo.ResourceScopeData.TryGetValue(resourceSymbol, out var scopeData) && scopeData.ResourceScopeSymbol is { } scopeResource)
            {
                emitter.EmitProperty("scope", () => emitter.EmitUnqualifiedResourceId(scopeResource));
            }
            emitter.EmitObjectProperties((ObjectSyntax)body, ResourcePropertiesToOmit);

            this.EmitDependsOn(memoryWriter, resourceSymbol, emitter, body);

            memoryWriter.WriteEndObject();
        }

        private void EmitModuleParameters(JsonTextWriter memoryWriter, ModuleSymbol moduleSymbol, ExpressionEmitter emitter)
        {
            var paramsValue = moduleSymbol.SafeGetBodyPropertyValue(LanguageConstants.ModuleParamsPropertyName);
            if (paramsValue is not ObjectSyntax paramsObjectSyntax)
            {
                // 'params' is optional if the module has no required params
                return;
            }

            memoryWriter.WritePropertyName("parameters");

            memoryWriter.WriteStartObject();

            foreach (var propertySyntax in paramsObjectSyntax.Properties)
            {
                if (!(propertySyntax.TryGetKeyText() is string keyName))
                {
                    // should have been caught by earlier validation
                    throw new ArgumentException("Disallowed interpolation in module parameter");
                }

                // we can't just call EmitObjectProperties here because the ObjectSyntax is flatter than the structure we're generating
                // because nested deployment parameters are objects with a single value property
                memoryWriter.WritePropertyName(keyName);
                memoryWriter.WriteStartObject();
                if (propertySyntax.Value is ForSyntax @for)
                {
                    // the value is a for-expression
                    // write a single property copy loop
                    emitter.EmitProperty("copy", () =>
                    {
                        memoryWriter.WriteStartArray();
                        emitter.EmitCopyObject("value", @for, @for.Body, "value");
                        memoryWriter.WriteEndArray();
                    });
                }
                else
                {
                    // the value is not a for-expression - can emit normally
                    emitter.EmitProperty("value", propertySyntax.Value);
                }

                memoryWriter.WriteEndObject();
            }

            memoryWriter.WriteEndObject();
        }

        private void EmitModule(JsonTextWriter memoryWriter, ModuleSymbol moduleSymbol, ExpressionEmitter emitter)
        {
            memoryWriter.WriteStartObject();

            var body = moduleSymbol.DeclaringModule.Value;
            switch (body)
            {
                case IfConditionSyntax ifCondition:
                    body = ifCondition.Body;
                    emitter.EmitProperty("condition", ifCondition.ConditionExpression);
                    break;

                case ForSyntax @for:
                    body = @for.Body;
                    emitter.EmitProperty("copy", () => emitter.EmitCopyObject(moduleSymbol.Name, @for, input: null));
                    break;
            }

            emitter.EmitProperty("type", NestedDeploymentResourceType);
            emitter.EmitProperty("apiVersion", NestedDeploymentResourceApiVersion);

            // emit all properties apart from 'params'. In practice, this currrently only allows 'name', but we may choose to allow other top-level resource properties in future.
            // params requires special handling (see below).
            emitter.EmitObjectProperties((ObjectSyntax)body, ModulePropertiesToOmit);


            var scopeData = context.ModuleScopeData[moduleSymbol];
            ScopeHelper.EmitModuleScopeProperties(context.SemanticModel.TargetScope, scopeData, emitter);

            if (scopeData.RequestedScope != ResourceScope.ResourceGroup)
            {
                // if we're deploying to a scope other than resource group, we need to supply a location
                if (this.context.SemanticModel.TargetScope == ResourceScope.ResourceGroup)
                {
                    // the deployment() object at resource group scope does not contain a property named 'location', so we have to use resourceGroup().location
                    emitter.EmitProperty("location", new FunctionExpression(
                        "resourceGroup",
                        new LanguageExpression[] { },
                        new LanguageExpression[] { new JTokenExpression("location") }));
                }
                else
                {
                    // at all other scopes we can just use deployment().location
                    emitter.EmitProperty("location", new FunctionExpression(
                        "deployment",
                        new LanguageExpression[] { },
                        new LanguageExpression[] { new JTokenExpression("location") }));
                }
            }

            memoryWriter.WritePropertyName("properties");
            {
                memoryWriter.WriteStartObject();

                memoryWriter.WritePropertyName("expressionEvaluationOptions");
                {
                    memoryWriter.WriteStartObject();
                    emitter.EmitProperty("scope", "inner");
                    memoryWriter.WriteEndObject();
                }

                emitter.EmitProperty("mode", "Incremental");

                EmitModuleParameters(memoryWriter, moduleSymbol, emitter);

                memoryWriter.WritePropertyName("template");
                {
                    var moduleSemanticModel = GetModuleSemanticModel(moduleSymbol);
                    var moduleWriter = new TemplateWriter(memoryWriter, moduleSemanticModel, this.assemblyFileVersion);
                    moduleWriter.WriteModule();
                }

                memoryWriter.WriteEndObject();
            }

            this.EmitDependsOn(memoryWriter, moduleSymbol, emitter, body);

            memoryWriter.WriteEndObject();
        }

        private void EmitDependsOn(JsonTextWriter memoryWriter, DeclaredSymbol declaredSymbol, ExpressionEmitter emitter, SyntaxBase newContext)
        {
            var dependencies = context.ResourceDependencies[declaredSymbol];
            if (!dependencies.Any())
            {
                return;
            }

            memoryWriter.WritePropertyName("dependsOn");
            memoryWriter.WriteStartArray();
            // need to deduplicate and put dependencies in a deterministic order to generate a deterministic template
            foreach (var dependency in dependencies.Distinct(ResourceDependency.EqualityComparer).OrderBy(x => x.Resource.Name).OrderBy(x => (x.IndexExpression as IntegerLiteralSyntax)?.Value))
            {
                switch (dependency.Resource)
                {
                    case ResourceSymbol resourceDependency:
                        if (resourceDependency.IsCollection && dependency.IndexExpression == null)
                        {
                            // dependency is on the entire resource collection
                            // write the name of the resource collection as the dependency
                            memoryWriter.WriteValue(resourceDependency.DeclaringResource.Name.IdentifierName);

                            break;
                        }

                        if (!resourceDependency.DeclaringResource.IsExistingResource())
                        {
                            emitter.EmitResourceIdReference(resourceDependency, dependency.IndexExpression, newContext);
                        }

                        break;
                    case ModuleSymbol moduleDependency:
                        if (moduleDependency.IsCollection && dependency.IndexExpression == null)
                        {
                            // dependency is on the entire module collection
                            // write the name of the module collection as the dependency
                            memoryWriter.WriteValue(moduleDependency.DeclaringModule.Name.IdentifierName);

                            break;
                        }
                        
                        emitter.EmitResourceIdReference(moduleDependency, dependency.IndexExpression, newContext);
                        
                        break;
                    default:
                        throw new InvalidOperationException($"Found dependency '{dependency.Resource.Name}' of unexpected type {dependency.GetType()}");
                }
            }
            memoryWriter.WriteEndArray();
        }

        private void EmitOutputsIfPresent(JsonTextWriter memoryWriter, ExpressionEmitter emitter)
        {
            if (this.context.SemanticModel.Root.OutputDeclarations.Length == 0)
            {
                return;
            }

            memoryWriter.WritePropertyName("outputs");
            memoryWriter.WriteStartObject();

            foreach (var outputSymbol in this.context.SemanticModel.Root.OutputDeclarations)
            {
                memoryWriter.WritePropertyName(outputSymbol.Name);
                this.EmitOutput(memoryWriter, outputSymbol, emitter);
            }

            memoryWriter.WriteEndObject();
        }

        private void EmitOutput(JsonTextWriter memoryWriter, OutputSymbol outputSymbol, ExpressionEmitter emitter)
        {
            memoryWriter.WriteStartObject();

            emitter.EmitProperty("type", outputSymbol.Type.Name);
            emitter.EmitProperty("value", outputSymbol.Value);

            memoryWriter.WriteEndObject();
        }

        public void EmitMetadata(JsonTextWriter memoryWriter, ExpressionEmitter emitter)
        {
            memoryWriter.WritePropertyName("metadata");
            memoryWriter.WriteStartObject();
            memoryWriter.WritePropertyName("_generator");
            memoryWriter.WriteStartObject();

            emitter.EmitProperty("name", LanguageConstants.LanguageId);
            emitter.EmitProperty("version", this.assemblyFileVersion);
            memoryWriter.WriteEndObject();
            memoryWriter.WriteEndObject();
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

