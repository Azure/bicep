// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
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

        private static ImmutableHashSet<string> ResourcePropertiesToOmit = new[] {
            LanguageConstants.ResourceScopePropertyName,
            LanguageConstants.ResourceDependsOnPropertyName,
        }.ToImmutableHashSet();

        private static ImmutableHashSet<string> ModulePropertiesToOmit = new[] {
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
        /// Should be the only constructor used externally
        /// </summary>
        public void Write()
        {
            this.Write(moduleEmit: false);
        }

        /// <summary>
        /// Since emitting modules is a nested call to this, it behaves differently (we reuse the main memoryWriter)
        /// </summary>
        public void Write(bool moduleEmit)
        {
            string templateString;
            if (moduleEmit)
            {
                this.Write(this.writer, new ExpressionEmitter(this.writer, this.context), false);
                return;
            }
            else
            {
                using (StringWriter stringWriter = new())
                using (JsonTextWriter memoryWriter = new(stringWriter))
                {
                    var emitter = new ExpressionEmitter(memoryWriter, this.context);
                    // no templatehash, write to memory
                    this.Write(memoryWriter, emitter, true);
                    // TODO: avoid reading the whole template into a string
                    templateString = stringWriter.ToString();
                }
            }
            var templateHash = TemplateHashExtensions.ComputeTemplateHash(templateString);
            JObject template = (JObject)JObject.Parse(templateString);
            ((JObject) template["metadata"]!["_generator"]!).Add(new JProperty("templateHash", templateHash));
            template.WriteTo(this.writer);
        }


        private void Write(JsonTextWriter textWriter, ExpressionEmitter emitter, bool emitMetadata)
        {
            textWriter.WriteStartObject();

            emitter.EmitProperty("$schema", GetSchema(context.SemanticModel.TargetScope));

            emitter.EmitProperty("contentVersion", "1.0.0.0");

            this.EmitParametersIfPresent(textWriter, emitter);

            textWriter.WritePropertyName("functions");
            textWriter.WriteStartArray();
            textWriter.WriteEndArray();

            this.EmitVariablesIfPresent(textWriter, emitter);

            this.EmitResources(textWriter, emitter);

            this.EmitOutputsIfPresent(textWriter, emitter);

            // We skip emitting metadata when emitting modules (no metadata for nested deployments i.e. emitting modules)
            if (emitMetadata)
            {
                this.EmitMetadata(textWriter, emitter);
            }

            textWriter.WriteEndObject();
        }

        private void EmitParametersIfPresent(JsonTextWriter textWriter, ExpressionEmitter emitter)
        {
            if (this.context.SemanticModel.Root.ParameterDeclarations.Length == 0)
            {
                return;
            }

            textWriter.WritePropertyName("parameters");
            textWriter.WriteStartObject();

            foreach (var parameterSymbol in this.context.SemanticModel.Root.ParameterDeclarations)
            {
                textWriter.WritePropertyName(parameterSymbol.Name);
                this.EmitParameter(textWriter, parameterSymbol, emitter);
            }

            textWriter.WriteEndObject();
        }

        private void EmitParameter(JsonTextWriter textWriter, ParameterSymbol parameterSymbol, ExpressionEmitter emitter)
        {
            // local function
            bool IsSecure(SyntaxBase? value) => value is BooleanLiteralSyntax boolLiteral && boolLiteral.Value;

            if (!(SyntaxHelper.TryGetPrimitiveType(parameterSymbol.DeclaringParameter) is TypeSymbol primitiveType))
            {
                // this should have been caught by the type checker long ago
                throw new ArgumentException($"Unable to find primitive type for parameter {parameterSymbol.Name}");
            }

            textWriter.WriteStartObject();

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

            textWriter.WriteEndObject();
        }

        private void EmitVariablesIfPresent(JsonTextWriter textWriter, ExpressionEmitter emitter)
        {
            if (!this.context.SemanticModel.Root.VariableDeclarations.Any(symbol => !this.context.VariablesToInline.Contains(symbol)))
            {
                return;
            }

            textWriter.WritePropertyName("variables");
            textWriter.WriteStartObject();

            foreach (var variableSymbol in this.context.SemanticModel.Root.VariableDeclarations)
            {
                if (!this.context.VariablesToInline.Contains(variableSymbol))
                {
                    textWriter.WritePropertyName(variableSymbol.Name);
                    this.EmitVariable(variableSymbol, emitter);
                }
            }

            textWriter.WriteEndObject();
        }

        private void EmitVariable(VariableSymbol variableSymbol, ExpressionEmitter emitter)
        {
            // TODO: When we have expressions, only expressions without runtime functions can be emitted this way. Everything else will need to be inlined.
            emitter.EmitExpression(variableSymbol.Value);
        }

        private void EmitResources(JsonTextWriter textWriter, ExpressionEmitter emitter)
        {
            textWriter.WritePropertyName("resources");
            textWriter.WriteStartArray();

            foreach (var resourceSymbol in this.context.SemanticModel.Root.ResourceDeclarations)
            {
                if (resourceSymbol.DeclaringResource.IsExistingResource())
                {
                    continue;
                }

                this.EmitResource(textWriter, resourceSymbol, emitter);
            }

            foreach (var moduleSymbol in this.context.SemanticModel.Root.ModuleDeclarations)
            {
                this.EmitModule(textWriter, moduleSymbol, emitter);
            }

            textWriter.WriteEndArray();
        }

        private void EmitResource(JsonTextWriter textWriter, ResourceSymbol resourceSymbol, ExpressionEmitter emitter)
        {
            textWriter.WriteStartObject();

            var typeReference = EmitHelpers.GetTypeReference(resourceSymbol);
            var body = resourceSymbol.DeclaringResource.Value;
            if (body is IfConditionSyntax ifCondition)
            {
                body = ifCondition.Body;
                emitter.EmitProperty("condition", ifCondition.ConditionExpression);
            }

            emitter.EmitProperty("type", typeReference.FullyQualifiedType);
            emitter.EmitProperty("apiVersion", typeReference.ApiVersion);
            if (context.SemanticModel.EmitLimitationInfo.ResourceScopeData.TryGetValue(resourceSymbol, out var scopeData) && scopeData.ResourceScopeSymbol is { } scopeResource)
            {
                emitter.EmitProperty("scope", () => emitter.EmitUnqualifiedResourceId(scopeResource));
            }
            emitter.EmitObjectProperties((ObjectSyntax)body, ResourcePropertiesToOmit);

            // dependsOn is currently not allowed as a top-level resource property in bicep
            // we will need to revisit this and probably merge the two if we decide to allow it
            this.EmitDependsOn(textWriter, resourceSymbol, emitter);

            textWriter.WriteEndObject();
        }

        private void EmitModuleParameters(JsonTextWriter textWriter, ModuleSymbol moduleSymbol, ExpressionEmitter emitter)
        {
            var paramsValue = moduleSymbol.SafeGetBodyPropertyValue(LanguageConstants.ModuleParamsPropertyName);
            if (paramsValue is not ObjectSyntax paramsObjectSyntax)
            {
                // 'params' is optional if the module has no required params
                return;
            }

            textWriter.WritePropertyName("parameters");

            textWriter.WriteStartObject();

            foreach (var propertySyntax in paramsObjectSyntax.Properties)
            {
                if (!(propertySyntax.TryGetKeyText() is string keyName))
                {
                    // should have been caught by earlier validation
                    throw new ArgumentException("Disallowed interpolation in module parameter");
                }

                textWriter.WritePropertyName(keyName);
                {
                    textWriter.WriteStartObject();
                    emitter.EmitProperty("value", propertySyntax.Value);
                    textWriter.WriteEndObject();
                }
            }

            textWriter.WriteEndObject();
        }

        private void EmitModule(JsonTextWriter textWriter, ModuleSymbol moduleSymbol, ExpressionEmitter emitter)
        {
            textWriter.WriteStartObject();

            var body = moduleSymbol.DeclaringModule.Value;
            if (body is IfConditionSyntax ifCondition)
            {
                body = ifCondition.Body;
                emitter.EmitProperty("condition", ifCondition.ConditionExpression);
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

            textWriter.WritePropertyName("properties");
            {
                textWriter.WriteStartObject();

                textWriter.WritePropertyName("expressionEvaluationOptions");
                {
                    textWriter.WriteStartObject();
                    emitter.EmitProperty("scope", "inner");
                    textWriter.WriteEndObject();
                }

                emitter.EmitProperty("mode", "Incremental");

                EmitModuleParameters(textWriter, moduleSymbol, emitter);

                textWriter.WritePropertyName("template");
                {
                    var moduleSemanticModel = GetModuleSemanticModel(moduleSymbol);
                    var moduleWriter = new TemplateWriter(textWriter, moduleSemanticModel, this.assemblyFileVersion);
                    // we don't need the hash of a module, we use a global hash for the template
                    moduleWriter.Write(true);
                }

                textWriter.WriteEndObject();
            }

            this.EmitDependsOn(textWriter, moduleSymbol, emitter);

            textWriter.WriteEndObject();
        }

        private void EmitDependsOn(JsonTextWriter textWriter, DeclaredSymbol declaredSymbol, ExpressionEmitter emitter)
        {
            var dependencies = context.ResourceDependencies[declaredSymbol];
            if (!dependencies.Any())
            {
                return;
            }

            textWriter.WritePropertyName("dependsOn");
            textWriter.WriteStartArray();
            // need to put dependencies in a deterministic order to generate a deterministic template
            foreach (var dependency in dependencies.OrderBy(x => x.Name))
            {
                switch (dependency)
                {
                    case ResourceSymbol resourceDependency:
                        if (!resourceDependency.DeclaringResource.IsExistingResource())
                        {
                            emitter.EmitResourceIdReference(resourceDependency);
                        }
                        break;
                    case ModuleSymbol moduleDependency:
                        emitter.EmitResourceIdReference(moduleDependency);
                        break;
                    default:
                        throw new InvalidOperationException($"Found dependency '{dependency.Name}' of unexpected type {dependency.GetType()}");
                }
            }
            textWriter.WriteEndArray();
        }

        private void EmitOutputsIfPresent(JsonTextWriter textWriter, ExpressionEmitter emitter)
        {
            if (this.context.SemanticModel.Root.OutputDeclarations.Length == 0)
            {
                return;
            }

            textWriter.WritePropertyName("outputs");
            textWriter.WriteStartObject();

            foreach (var outputSymbol in this.context.SemanticModel.Root.OutputDeclarations)
            {
                textWriter.WritePropertyName(outputSymbol.Name);
                this.EmitOutput(textWriter, outputSymbol, emitter);
            }

            textWriter.WriteEndObject();
        }

        private void EmitOutput(JsonTextWriter textWriter, OutputSymbol outputSymbol, ExpressionEmitter emitter)
        {
            textWriter.WriteStartObject();

            emitter.EmitProperty("type", outputSymbol.Type.Name);
            emitter.EmitProperty("value", outputSymbol.Value);

            textWriter.WriteEndObject();
        }

        public void EmitMetadata(JsonTextWriter textWriter, ExpressionEmitter emitter)
        {
            textWriter.WritePropertyName("metadata");
            textWriter.WriteStartObject();
            textWriter.WritePropertyName("_generator");
            textWriter.WriteStartObject();

            emitter.EmitProperty("name", LanguageConstants.LanguageId);
            emitter.EmitProperty("version", this.assemblyFileVersion);
            textWriter.WriteEndObject();
            textWriter.WriteEndObject();
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

