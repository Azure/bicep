// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Azure.Deployments.Core.Helpers;
using Azure.Deployments.Core.Json;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
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
        public const string GeneratorMetadataPath = "metadata._generator";
        public const string NestedDeploymentResourceType = AzResourceTypeProvider.ResourceTypeDeployments;
        
        // IMPORTANT: Do not update this API version until the new one is confirmed to be deployed and available in ALL the clouds.
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

        private static readonly ImmutableHashSet<string> ResourcePropertiesToOmit = new[] {
            LanguageConstants.ResourceScopePropertyName,
            LanguageConstants.ResourceParentPropertyName,
            LanguageConstants.ResourceDependsOnPropertyName,
            LanguageConstants.ResourceNamePropertyName,
        }.ToImmutableHashSet();

        private static readonly ImmutableHashSet<string> ModulePropertiesToOmit = new[] {
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
        private readonly EmitterContext context;
        private readonly string assemblyFileVersion;

        public TemplateWriter(SemanticModel semanticModel, string assemblyFileVersion)
        {
            this.context = new EmitterContext(semanticModel);
            this.assemblyFileVersion = assemblyFileVersion;
        }

        public void Write(JsonTextWriter writer)
        {
            JToken template = GenerateTemplateWithoutHash();
            var templateHash = TemplateHelpers.ComputeTemplateHash(template);
            if (template.SelectToken(GeneratorMetadataPath) is not JObject generatorObject)
            {
                throw new InvalidOperationException($"generated template doesn't contain a generator object at the path {GeneratorMetadataPath}");
            }
            generatorObject.Add("templateHash", templateHash);
            template.WriteTo(writer);
        }

        private JToken GenerateTemplateWithoutHash()
        {
            // TODO: since we merely return a JToken, refactor the emitter logic to add properties to a JObject
            // instead of writing to a JsonWriter and converting it to JToken at the end
            using var stringWriter = new StringWriter();
            using var jsonWriter = new JsonTextWriter(stringWriter);
            var emitter = new ExpressionEmitter(jsonWriter, this.context);

            jsonWriter.WriteStartObject();

            emitter.EmitProperty("$schema", GetSchema(context.SemanticModel.TargetScope));

            emitter.EmitProperty("contentVersion", "1.0.0.0");
            
            this.EmitMetadata(jsonWriter, emitter);

            this.EmitParametersIfPresent(jsonWriter, emitter);

            jsonWriter.WritePropertyName("functions");
            jsonWriter.WriteStartArray();
            jsonWriter.WriteEndArray();

            this.EmitVariablesIfPresent(jsonWriter, emitter);

            this.EmitResources(jsonWriter, emitter);

            this.EmitOutputsIfPresent(jsonWriter, emitter);

            jsonWriter.WriteEndObject();

            return stringWriter.ToString().FromJson<JToken>();
        }

        private void EmitParametersIfPresent(JsonTextWriter jsonWriter, ExpressionEmitter emitter)
        {
            if (this.context.SemanticModel.Root.ParameterDeclarations.Length == 0)
            {
                return;
            }

            jsonWriter.WritePropertyName("parameters");
            jsonWriter.WriteStartObject();

            foreach (var parameterSymbol in this.context.SemanticModel.Root.ParameterDeclarations)
            {
                jsonWriter.WritePropertyName(parameterSymbol.Name);
                this.EmitParameter(jsonWriter, parameterSymbol, emitter);
            }

            jsonWriter.WriteEndObject();
        }

        private ObjectSyntax EvaluateDecorators(StatementSyntax statement, ObjectSyntax input, TypeSymbol targetType)
        {
            var result = input;
            foreach (var decoratorSyntax in statement.Decorators.Reverse())
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

                    result = decorator.Evaluate(decoratorSyntax, targetType, result);
                }
            }

            return result;
        }

        private void EmitParameter(JsonTextWriter jsonWriter, ParameterSymbol parameterSymbol, ExpressionEmitter emitter)
        {
            // local function
            static bool IsSecure(SyntaxBase? value) => value is BooleanLiteralSyntax boolLiteral && boolLiteral.Value;

            if (!(SyntaxHelper.TryGetPrimitiveType(parameterSymbol.DeclaringParameter) is TypeSymbol primitiveType))
            {
                // this should have been caught by the type checker long ago
                throw new ArgumentException($"Unable to find primitive type for parameter {parameterSymbol.Name}");
            }

            jsonWriter.WriteStartObject();

            if (parameterSymbol.DeclaringParameter.Decorators.Any())
            {
                var parameterType = SyntaxFactory.CreateStringLiteral(primitiveType.Name);
                var parameterObject = SyntaxFactory.CreateObject(SyntaxFactory.CreateObjectProperty("type", parameterType).AsEnumerable());

                if (parameterSymbol.Modifier is ParameterDefaultValueSyntax defaultValueSyntax)
                {
                    parameterObject = parameterObject.MergeProperty("defaultValue", defaultValueSyntax.DefaultValue);
                }

                parameterObject = EvaluateDecorators(parameterSymbol.DeclaringParameter, parameterObject, primitiveType);

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
                        var properties = modifierSyntax.ToNamedPropertyValueDictionary();

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

            jsonWriter.WriteEndObject();
        }

        private void EmitVariablesIfPresent(JsonTextWriter jsonWriter, ExpressionEmitter emitter)
        {
            if (!this.context.SemanticModel.Root.VariableDeclarations.Any(symbol => !this.context.VariablesToInline.Contains(symbol)))
            {
                return;
            }

            jsonWriter.WritePropertyName("variables");
            jsonWriter.WriteStartObject();

            var variableLookup = this.context.SemanticModel.Root.VariableDeclarations.ToLookup(variableSymbol => variableSymbol.Value is ForSyntax);

            // local function
            IEnumerable<VariableSymbol> GetNonInlinedVariables(bool valueIsLoop) =>
                variableLookup[valueIsLoop].Where(symbol => !this.context.VariablesToInline.Contains(symbol));

            if(GetNonInlinedVariables(valueIsLoop: true).Any())
            {
                // we have variables whose values are loops
                emitter.EmitProperty("copy", () =>
                {
                    jsonWriter.WriteStartArray();

                    foreach(var variableSymbol in GetNonInlinedVariables(valueIsLoop: true))
                    {
                        // enforced by the lookup predicate above
                        var @for = (ForSyntax)variableSymbol.Value;

                        emitter.EmitCopyObject(variableSymbol.Name, @for, @for.Body);
                    }

                    jsonWriter.WriteEndArray();
                });
            }

            // emit non-loop variables
            foreach (var variableSymbol in GetNonInlinedVariables(valueIsLoop: false))
            {
                jsonWriter.WritePropertyName(variableSymbol.Name);
                emitter.EmitExpression(variableSymbol.Value);
            }

            jsonWriter.WriteEndObject();
        }

        private void EmitResources(JsonTextWriter jsonWriter, ExpressionEmitter emitter)
        {
            jsonWriter.WritePropertyName("resources");
            jsonWriter.WriteStartArray();

            foreach (var resourceSymbol in this.context.SemanticModel.Root.GetAllResourceDeclarations())
            {
                if (resourceSymbol.DeclaringResource.IsExistingResource())
                {
                    continue;
                }

                this.EmitResource(jsonWriter, resourceSymbol, emitter);
            }

            foreach (var moduleSymbol in this.context.SemanticModel.Root.ModuleDeclarations)
            {
                this.EmitModule(jsonWriter, moduleSymbol, emitter);
            }

            jsonWriter.WriteEndArray();
        }

        private long? GetBatchSize(StatementSyntax decoratedSyntax)
        {
            var evaluated = this.EvaluateDecorators(decoratedSyntax, SyntaxFactory.CreateObject(Enumerable.Empty<ObjectPropertySyntax>()), LanguageConstants.Array);
            var batchSizeProperty = evaluated.SafeGetPropertyByName("batchSize");

            return batchSizeProperty switch
            {
                ObjectPropertySyntax { Value: IntegerLiteralSyntax integerLiteral } => integerLiteral.Value,
                _ => null
            };
        }

        private void EmitResource(JsonTextWriter jsonWriter, ResourceSymbol resourceSymbol, ExpressionEmitter emitter)
        {
            jsonWriter.WriteStartObject();

            var typeReference = resourceSymbol.GetResourceTypeReference();

            // Note: conditions STACK with nesting.
            //
            // Children inherit the conditions of their parents, etc. This avoids a problem
            // where we emit a dependsOn to something that's not in the template, or not
            // being evaulated i the template. 
            var conditions = new List<SyntaxBase>();
            var loops = new List<(string name, ForSyntax @for, SyntaxBase? input)>();

            var ancestors = this.context.SemanticModel.ResourceAncestors.GetAncestors(resourceSymbol);
            foreach (var ancestor in ancestors)
            {
                if (ancestor.AncestorType == ResourceAncestorGraph.ResourceAncestorType.Nested &&
                    ancestor.Resource.DeclaringResource.Value is IfConditionSyntax ifCondition)
                {
                    conditions.Add(ifCondition.ConditionExpression);
                }

                if (ancestor.AncestorType == ResourceAncestorGraph.ResourceAncestorType.Nested &&
                    ancestor.Resource.DeclaringResource.Value is ForSyntax @for)
                {
                    loops.Add((ancestor.Resource.Name, @for, null));
                }
            }

            // Unwrap the 'real' resource body if there's a condition
            var body = resourceSymbol.DeclaringResource.Value;
            switch (body)
            {
                case IfConditionSyntax ifCondition:
                    body = ifCondition.Body;
                    conditions.Add(ifCondition.ConditionExpression);
                    break;

                case ForSyntax @for:
                    loops.Add((resourceSymbol.Name, @for, null));
                    if (@for.Body is IfConditionSyntax loopFilter)
                    {
                        body = loopFilter.Body;
                        conditions.Add(loopFilter.ConditionExpression);
                    }
                    else
                    {
                        body = @for.Body;
                    }

                    break;
            }

            if (conditions.Count == 1)
            {
                emitter.EmitProperty("condition", conditions[0]);
            }
            else if (conditions.Count > 1)
            {
                var @operator = new BinaryOperationSyntax(
                    conditions[0],
                    SyntaxFactory.CreateToken(TokenType.LogicalAnd),
                    conditions[1]);
                for (var i = 2; i < conditions.Count; i++)
                {
                    @operator = new BinaryOperationSyntax(
                        @operator,
                        SyntaxFactory.CreateToken(TokenType.LogicalAnd),
                        conditions[i]);
                }

                emitter.EmitProperty("condition", @operator);
            }

            if (loops.Count == 1)
            {
                var batchSize = GetBatchSize(resourceSymbol.DeclaringResource);
                emitter.EmitProperty("copy", () => emitter.EmitCopyObject(loops[0].name, loops[0].@for, loops[0].input, batchSize: batchSize));
            }
            else if (loops.Count > 1)
            {
                throw new InvalidOperationException("nested loops are not supported");
            }

            emitter.EmitProperty("type", typeReference.FullyQualifiedType);
            emitter.EmitProperty("apiVersion", typeReference.ApiVersion);
            if (context.SemanticModel.EmitLimitationInfo.ResourceScopeData.TryGetValue(resourceSymbol, out var scopeData))
            {
                ScopeHelper.EmitResourceScopeProperties(context.SemanticModel.TargetScope, scopeData, emitter, body);
            }

            emitter.EmitProperty("name", emitter.GetFullyQualifiedResourceName(resourceSymbol));

            emitter.EmitObjectProperties((ObjectSyntax)body, ResourcePropertiesToOmit);

            this.EmitDependsOn(jsonWriter, resourceSymbol, emitter, body);

            jsonWriter.WriteEndObject();
        }

        private static void EmitModuleParameters(JsonTextWriter jsonWriter, ModuleSymbol moduleSymbol, ExpressionEmitter emitter)
        {
            var paramsValue = moduleSymbol.SafeGetBodyPropertyValue(LanguageConstants.ModuleParamsPropertyName);
            if (paramsValue is not ObjectSyntax paramsObjectSyntax)
            {
                // 'params' is optional if the module has no required params
                return;
            }

            jsonWriter.WritePropertyName("parameters");

            jsonWriter.WriteStartObject();

            foreach (var propertySyntax in paramsObjectSyntax.Properties)
            {
                if (!(propertySyntax.TryGetKeyText() is string keyName))
                {
                    // should have been caught by earlier validation
                    throw new ArgumentException("Disallowed interpolation in module parameter");
                }

                // we can't just call EmitObjectProperties here because the ObjectSyntax is flatter than the structure we're generating
                // because nested deployment parameters are objects with a single value property
                jsonWriter.WritePropertyName(keyName);
                jsonWriter.WriteStartObject();
                if (propertySyntax.Value is ForSyntax @for)
                {
                    // the value is a for-expression
                    // write a single property copy loop
                    emitter.EmitProperty("copy", () =>
                    {
                        jsonWriter.WriteStartArray();
                        emitter.EmitCopyObject("value", @for, @for.Body, "value");
                        jsonWriter.WriteEndArray();
                    });
                }
                else
                {
                    // the value is not a for-expression - can emit normally
                    emitter.EmitModuleParameterValue(propertySyntax.Value);
                }

                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndObject();
        }

        private void EmitModule(JsonTextWriter jsonWriter, ModuleSymbol moduleSymbol, ExpressionEmitter emitter)
        {
            jsonWriter.WriteStartObject();

            var body = moduleSymbol.DeclaringModule.Value;
            switch (body)
            {
                case IfConditionSyntax ifCondition:
                    body = ifCondition.Body;
                    emitter.EmitProperty("condition", ifCondition.ConditionExpression);
                    break;

                case ForSyntax @for:
                    if(@for.Body is IfConditionSyntax loopFilter)
                    {
                        body = loopFilter.Body;
                        emitter.EmitProperty("condition", loopFilter.ConditionExpression);
                    }
                    else
                    {
                        body = @for.Body;
                    }
                    
                    var batchSize = GetBatchSize(moduleSymbol.DeclaringModule);
                    emitter.EmitProperty("copy", () => emitter.EmitCopyObject(moduleSymbol.Name, @for, input: null, batchSize: batchSize));
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
                        Array.Empty<LanguageExpression>(),
                        new LanguageExpression[] { new JTokenExpression("location") }));
                }
                else
                {
                    // at all other scopes we can just use deployment().location
                    emitter.EmitProperty("location", new FunctionExpression(
                        "deployment",
                        Array.Empty<LanguageExpression>(),
                        new LanguageExpression[] { new JTokenExpression("location") }));
                }
            }

            jsonWriter.WritePropertyName("properties");
            {
                jsonWriter.WriteStartObject();

                jsonWriter.WritePropertyName("expressionEvaluationOptions");
                {
                    jsonWriter.WriteStartObject();
                    emitter.EmitProperty("scope", "inner");
                    jsonWriter.WriteEndObject();
                }

                emitter.EmitProperty("mode", "Incremental");

                EmitModuleParameters(jsonWriter, moduleSymbol, emitter);

                jsonWriter.WritePropertyName("template");
                {
                    var moduleSemanticModel = GetModuleSemanticModel(moduleSymbol);
                    var moduleWriter = new TemplateWriter(moduleSemanticModel, this.assemblyFileVersion);
                    moduleWriter.Write(jsonWriter);
                }

                jsonWriter.WriteEndObject();
            }

            this.EmitDependsOn(jsonWriter, moduleSymbol, emitter, body);

            jsonWriter.WriteEndObject();
        }
        private static bool ShouldGenerateDependsOn(ResourceDependency dependency)
        {
            switch (dependency.Resource)
            {
                case ResourceSymbol resource:
                    // We only want to add a 'dependsOn' for resources being deployed in this file.
                    return !resource.DeclaringResource.IsExistingResource();
                case ModuleSymbol:
                    return true;
                default:
                    throw new InvalidOperationException($"Found dependency '{dependency.Resource.Name}' of unexpected type {dependency.GetType()}");
            }
        }

        private void EmitDependsOn(JsonTextWriter jsonWriter, DeclaredSymbol declaredSymbol, ExpressionEmitter emitter, SyntaxBase newContext)
        {
            var dependencies = context.ResourceDependencies[declaredSymbol]
                .Where(dep => ShouldGenerateDependsOn(dep));

            if (!dependencies.Any())
            {
                return;
            }

            jsonWriter.WritePropertyName("dependsOn");
            jsonWriter.WriteStartArray();
            // need to put dependencies in a deterministic order to generate a deterministic template
            foreach (var dependency in dependencies.OrderBy(x => x.Resource.Name))
            {
                switch (dependency.Resource)
                {
                    case ResourceSymbol resourceDependency:
                        if (resourceDependency.IsCollection && dependency.IndexExpression == null)
                        {
                            // dependency is on the entire resource collection
                            // write the name of the resource collection as the dependency
                            jsonWriter.WriteValue(resourceDependency.DeclaringResource.Name.IdentifierName);

                            break;
                        }

                        emitter.EmitResourceIdReference(resourceDependency, dependency.IndexExpression, newContext);
                        break;
                    case ModuleSymbol moduleDependency:
                        if (moduleDependency.IsCollection && dependency.IndexExpression == null)
                        {
                            // dependency is on the entire module collection
                            // write the name of the module collection as the dependency
                            jsonWriter.WriteValue(moduleDependency.DeclaringModule.Name.IdentifierName);

                            break;
                        }

                        emitter.EmitResourceIdReference(moduleDependency, dependency.IndexExpression, newContext);

                        break;
                    default:
                        throw new InvalidOperationException($"Found dependency '{dependency.Resource.Name}' of unexpected type {dependency.GetType()}");
                }
            }
            jsonWriter.WriteEndArray();
        }

        private void EmitOutputsIfPresent(JsonTextWriter jsonWriter, ExpressionEmitter emitter)
        {
            if (this.context.SemanticModel.Root.OutputDeclarations.Length == 0)
            {
                return;
            }

            jsonWriter.WritePropertyName("outputs");
            jsonWriter.WriteStartObject();

            foreach (var outputSymbol in this.context.SemanticModel.Root.OutputDeclarations)
            {
                jsonWriter.WritePropertyName(outputSymbol.Name);
                EmitOutput(jsonWriter, outputSymbol, emitter);
            }

            jsonWriter.WriteEndObject();
        }

        private static void EmitOutput(JsonTextWriter jsonWriter, OutputSymbol outputSymbol, ExpressionEmitter emitter)
        {
            jsonWriter.WriteStartObject();

            emitter.EmitProperty("type", outputSymbol.Type.Name);
            if (outputSymbol.Value is ForSyntax @for)
            {
                emitter.EmitProperty("copy", () => emitter.EmitCopyObject(name: null, @for, @for.Body));
            }
            else
            {
                emitter.EmitProperty("value", outputSymbol.Value);
            }

            jsonWriter.WriteEndObject();
        }

        public void EmitMetadata(JsonTextWriter jsonWriter, ExpressionEmitter emitter)
        {
            jsonWriter.WritePropertyName("metadata");
            jsonWriter.WriteStartObject();
            jsonWriter.WritePropertyName("_generator");
            jsonWriter.WriteStartObject();

            emitter.EmitProperty("name", LanguageConstants.LanguageId);
            emitter.EmitProperty("version", this.assemblyFileVersion);
            jsonWriter.WriteEndObject();
            jsonWriter.WriteEndObject();
        }

        private static string GetTemplateTypeName(TypeSymbol type, bool secure)
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

