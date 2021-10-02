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
using Azure.Deployments.Core.Definitions.Schema;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Bicep.Core.Semantics.Metadata;
using System.Diagnostics;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Emit
{
    // TODO: Are there discrepancies between parameter, variable, and output names between bicep and ARM?
    public class TemplateWriter : ITemplateWriter
    {
        public const string GeneratorMetadataPath = "metadata._generator";
        public const string NestedDeploymentResourceType = AzResourceTypeProvider.ResourceTypeDeployments;
        
        // IMPORTANT: Do not update this API version until the new one is confirmed to be deployed and available in ALL the clouds.
        public const string NestedDeploymentResourceApiVersion = "2020-06-01";

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

        private static readonly ImmutableHashSet<string> DecoratorsToEmitAsResourceProperties = new[] {
            LanguageConstants.ParameterSecurePropertyName,
            LanguageConstants.ParameterAllowedPropertyName,
            LanguageConstants.ParameterMinValuePropertyName,
            LanguageConstants.ParameterMaxValuePropertyName,
            LanguageConstants.ParameterMinLengthPropertyName,
            LanguageConstants.ParameterMaxLengthPropertyName,
            LanguageConstants.ParameterMetadataPropertyName,
            LanguageConstants.MetadataDescriptionPropertyName,
        }.ToImmutableHashSet();

        private static ISemanticModel GetModuleSemanticModel(ModuleSymbol moduleSymbol)
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
        private readonly EmitterSettings settings;

        public TemplateWriter(SemanticModel semanticModel, EmitterSettings settings)
        {
            this.context = new EmitterContext(semanticModel, settings);
            this.settings = settings;
        }

        public void Write(JsonTextWriter writer)
        {
            // Template is used for calcualting template hash, template jtoken is used for writing to file.
            var (template, templateJToken) = GenerateTemplateWithoutHash();
            var templateHash = TemplateHelpers.ComputeTemplateHash(template.ToJToken());
            if (templateJToken.SelectToken(GeneratorMetadataPath) is not JObject generatorObject)
            {
                throw new InvalidOperationException($"generated template doesn't contain a generator object at the path {GeneratorMetadataPath}");
            }
            generatorObject.Add("templateHash", templateHash);
            templateJToken.WriteTo(writer);
        }

        private (Template, JToken) GenerateTemplateWithoutHash()
        {
            // TODO: since we merely return a JToken, refactor the emitter logic to add properties to a JObject
            // instead of writing to a JsonWriter and converting it to JToken at the end
            using var stringWriter = new StringWriter();
            using var jsonWriter = new JsonTextWriter(stringWriter);
            var emitter = new ExpressionEmitter(jsonWriter, this.context);

            jsonWriter.WriteStartObject();

            emitter.EmitProperty("$schema", GetSchema(context.SemanticModel.TargetScope));

            if (context.Settings.EnableSymbolicNames)
            {
                emitter.EmitProperty("languageVersion", "1.9-experimental");
            }

            emitter.EmitProperty("contentVersion", "1.0.0.0");
            
            this.EmitMetadata(jsonWriter, emitter);

            this.EmitParametersIfPresent(jsonWriter, emitter);

            jsonWriter.WritePropertyName("functions");
            jsonWriter.WriteStartArray();
            jsonWriter.WriteEndArray();

            this.EmitVariablesIfPresent(jsonWriter, emitter);

            this.EmitImports(jsonWriter, emitter);

            this.EmitResources(jsonWriter, emitter);

            this.EmitOutputsIfPresent(jsonWriter, emitter);

            jsonWriter.WriteEndObject();

            var content = stringWriter.ToString();
            return (Template.FromJson<Template>(content), content.FromJson<JToken>());
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

        private ObjectSyntax AddDecoratorsToBody(StatementSyntax statement, ObjectSyntax input, TypeSymbol targetType)
        {
            var result = input;
            foreach (var decoratorSyntax in statement.Decorators.Reverse())
            {
                var symbol = this.context.SemanticModel.GetSymbolInfo(decoratorSyntax.Expression);

                if (symbol is FunctionSymbol decoratorSymbol &&
                    decoratorSymbol.DeclaringObject is NamespaceType namespaceType &&
                    DecoratorsToEmitAsResourceProperties.Contains(decoratorSymbol.Name))
                {
                    var argumentTypes = decoratorSyntax.Arguments
                        .Select(argument => this.context.SemanticModel.TypeManager.GetTypeInfo(argument))
                        .ToArray();

                    // There should be exact one matching decorator since there's no errors.
                    var decorator = namespaceType.DecoratorResolver.GetMatches(decoratorSymbol, argumentTypes).Single();

                    var evaluated = decorator.Evaluate(decoratorSyntax, targetType, result);
                    if (evaluated is not null)
                    {
                        result = evaluated;
                    }
                }
            }

            return result;
        }

        private void EmitParameter(JsonTextWriter jsonWriter, ParameterSymbol parameterSymbol, ExpressionEmitter emitter)
        {
            var declaringParameter = parameterSymbol.DeclaringParameter;
            if (SyntaxHelper.TryGetPrimitiveType(declaringParameter) is not TypeSymbol primitiveType)
            {
                // this should have been caught by the type checker long ago
                throw new ArgumentException($"Unable to find primitive type for parameter {parameterSymbol.Name}");
            }

            jsonWriter.WriteStartObject();

            var parameterType = SyntaxFactory.CreateStringLiteral(primitiveType.Name);
            var parameterObject = SyntaxFactory.CreateObject(SyntaxFactory.CreateObjectProperty("type", parameterType).AsEnumerable());

            if (declaringParameter.Modifier is ParameterDefaultValueSyntax defaultValueSyntax)
            {
                parameterObject = parameterObject.MergeProperty("defaultValue", defaultValueSyntax.DefaultValue);
            }

            parameterObject = AddDecoratorsToBody(declaringParameter, parameterObject, primitiveType);

            foreach (var property in parameterObject.Properties)
            {
                if (property.TryGetKeyText() is string propertyName)
                {
                    emitter.EmitProperty(propertyName, property.Value);
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

        private void EmitImports(JsonTextWriter jsonWriter, ExpressionEmitter emitter)
        {
            if (!context.SemanticModel.Root.ImportDeclarations.Any())
            {
                return;
            }

            jsonWriter.WritePropertyName("imports");
            jsonWriter.WriteStartObject();

            foreach (var import in this.context.SemanticModel.Root.ImportDeclarations)
            {
                var namespaceType = context.SemanticModel.GetTypeInfo(import.DeclaringSyntax) as NamespaceType  
                    ?? throw new ArgumentException("Imported namespace does not have namespace type");

                jsonWriter.WritePropertyName(import.DeclaringImport.AliasName.IdentifierName);
                jsonWriter.WriteStartObject();

                emitter.EmitProperty("provider", namespaceType.Settings.ArmTemplateProviderName);
                emitter.EmitProperty("version", namespaceType.Settings.ArmTemplateProviderVersion);
                if (import.DeclaringImport.Config is {} config)
                {
                    emitter.EmitProperty("config", config);
                }

                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndObject();
        }

        private void EmitResources(JsonTextWriter jsonWriter, ExpressionEmitter emitter)
        {
            jsonWriter.WritePropertyName("resources");
            if (context.Settings.EnableSymbolicNames)
            {
                jsonWriter.WriteStartObject();
            }
            else
            {
                jsonWriter.WriteStartArray();
            }

            foreach (var resource in this.context.SemanticModel.AllResources)
            {
                if (resource.IsExistingResource && !context.Settings.EnableSymbolicNames)
                {
                    continue;
                }

                if (context.Settings.EnableSymbolicNames)
                {
                    jsonWriter.WritePropertyName(resource.Symbol.Name);
                }

                this.EmitResource(jsonWriter, resource, emitter);
            }

            foreach (var moduleSymbol in this.context.SemanticModel.Root.ModuleDeclarations)
            {
                if (context.Settings.EnableSymbolicNames)
                {
                    jsonWriter.WritePropertyName(moduleSymbol.Name);
                }

                this.EmitModule(jsonWriter, moduleSymbol, emitter);
            }

            if (context.Settings.EnableSymbolicNames)
            {
                jsonWriter.WriteEndObject();
            }
            else
            {
                jsonWriter.WriteEndArray();
            }
        }

        private long? GetBatchSize(StatementSyntax statement)
        {
            var decorator = SemanticModelHelper.TryGetDecoratorInNamespace(
                context.SemanticModel,
                statement,
                SystemNamespaceType.BuiltInName,
                LanguageConstants.BatchSizePropertyName);

            if (decorator?.Arguments is { } arguments
                && arguments.Count() == 1
                && arguments.ToList()[0].Expression is IntegerLiteralSyntax integerLiteral)
            {
                return integerLiteral.Value;
            }
            return null;
        }

        private void EmitResource(JsonTextWriter jsonWriter, ResourceMetadata resource, ExpressionEmitter emitter)
        {
            jsonWriter.WriteStartObject();

            // Note: conditions STACK with nesting.
            //
            // Children inherit the conditions of their parents, etc. This avoids a problem
            // where we emit a dependsOn to something that's not in the template, or not
            // being evaulated i the template. 
            var conditions = new List<SyntaxBase>();
            var loops = new List<(string name, ForSyntax @for, SyntaxBase? input)>();

            var ancestors = this.context.SemanticModel.ResourceAncestors.GetAncestors(resource);
            foreach (var ancestor in ancestors)
            {
                if (ancestor.AncestorType == ResourceAncestorGraph.ResourceAncestorType.Nested &&
                    ancestor.Resource.Symbol.DeclaringResource.Value is IfConditionSyntax ifCondition)
                {
                    conditions.Add(ifCondition.ConditionExpression);
                }

                if (ancestor.AncestorType == ResourceAncestorGraph.ResourceAncestorType.Nested &&
                    ancestor.Resource.Symbol.DeclaringResource.Value is ForSyntax @for)
                {
                    loops.Add((ancestor.Resource.Symbol.Name, @for, null));
                }
            }

            // Unwrap the 'real' resource body if there's a condition
            var body = resource.Symbol.DeclaringResource.Value;
            switch (body)
            {
                case IfConditionSyntax ifCondition:
                    body = ifCondition.Body;
                    conditions.Add(ifCondition.ConditionExpression);
                    break;

                case ForSyntax @for:
                    loops.Add((resource.Symbol.Name, @for, null));
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
                var batchSize = GetBatchSize(resource.Symbol.DeclaringResource);
                emitter.EmitProperty("copy", () => emitter.EmitCopyObject(loops[0].name, loops[0].@for, loops[0].input, batchSize: batchSize));
            }
            else if (loops.Count > 1)
            {
                throw new InvalidOperationException("nested loops are not supported");
            }

            emitter.EmitProperty("type", resource.TypeReference.FullyQualifiedType);
            emitter.EmitProperty("apiVersion", resource.TypeReference.ApiVersion);
            if (context.SemanticModel.EmitLimitationInfo.ResourceScopeData.TryGetValue(resource, out var scopeData))
            {
                ScopeHelper.EmitResourceScopeProperties(context.SemanticModel, scopeData, emitter, body);
            }

            emitter.EmitProperty("name", emitter.GetFullyQualifiedResourceName(resource));

            if (context.Settings.EnableSymbolicNames && resource.IsExistingResource)
            {
                jsonWriter.WritePropertyName("existing");
                jsonWriter.WriteValue(true);
            }
            
            body = AddDecoratorsToBody(resource.Symbol.DeclaringResource, (ObjectSyntax)body, resource.Type);
            emitter.EmitObjectProperties((ObjectSyntax)body, ResourcePropertiesToOmit);

            this.EmitDependsOn(jsonWriter, resource.Symbol, emitter, body);

            jsonWriter.WriteEndObject();
        }

        private static void EmitModuleParameters(JsonTextWriter jsonWriter, ModuleSymbol moduleSymbol, ExpressionEmitter emitter)
        {
            var paramsValue = moduleSymbol.SafeGetBodyPropertyValue(LanguageConstants.ModuleParamsPropertyName);
            if(paramsValue is not ObjectSyntax paramsObjectSyntax)
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

            body = AddDecoratorsToBody(moduleSymbol.DeclaringModule, (ObjectSyntax)body, moduleSymbol.Type);
            // emit all properties apart from 'params'. In practice, this currrently only allows 'name', but we may choose to allow other top-level resource properties in future.
            // params requires special handling (see below).
            emitter.EmitObjectProperties((ObjectSyntax)body, ModulePropertiesToOmit);

            var scopeData = context.ModuleScopeData[moduleSymbol];
            ScopeHelper.EmitModuleScopeProperties(context.SemanticModel.TargetScope, scopeData, emitter, body);

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

                var moduleSemanticModel = GetModuleSemanticModel(moduleSymbol);

                // If it is a template spec module, emit templateLink instead of template contents.
                jsonWriter.WritePropertyName(moduleSemanticModel is TemplateSpecSemanticModel ? "templateLink" : "template");
                {
                    TemplateWriterFactory.CreateTemplateWriter(moduleSemanticModel, this.settings).Write(jsonWriter);
                }

                jsonWriter.WriteEndObject();
            }

            this.EmitDependsOn(jsonWriter, moduleSymbol, emitter, body);

            jsonWriter.WriteEndObject();
        }
        private static bool ShouldGenerateDependsOn(ResourceDependency dependency) => dependency.Resource switch
        {   // We only want to add a 'dependsOn' for resources being deployed in this file.
            ResourceSymbol resource => !resource.DeclaringResource.IsExistingResource(),
            ModuleSymbol => true,
            _ => throw new InvalidOperationException($"Found dependency '{dependency.Resource.Name}' of unexpected type {dependency.GetType()}"),
        };

        private void EmitSymbolicNameDependsOnEntry(JsonTextWriter jsonWriter, ExpressionEmitter emitter, SyntaxBase newContext, ResourceDependency dependency)
        {
            switch (dependency.Resource)
            {
                case ResourceSymbol resourceDependency:
                    var resource = context.SemanticModel.ResourceMetadata.TryLookup(resourceDependency.DeclaringSyntax) ??
                        throw new ArgumentException($"Unable to find resource metadata for dependency '{dependency.Resource.Name}'");

                    switch ((resourceDependency.IsCollection, dependency.IndexExpression))
                    {
                        case (false, _):
                            jsonWriter.WriteValue(resourceDependency.Name);
                            Debug.Assert(dependency.IndexExpression is null);
                            break;
                        // dependency is on the entire resource collection
                        // write the name of the resource collection as the dependency
                        case (true, null):
                            jsonWriter.WriteValue(resourceDependency.Name);
                            break;
                        case (true, {} indexExpression):
                            emitter.EmitIndexedSymbolReference(resource, indexExpression, newContext);
                            break;
                    }
                    break;
                case ModuleSymbol moduleDependency:
                    switch ((moduleDependency.IsCollection, dependency.IndexExpression))
                    {
                        case (false, _):
                            jsonWriter.WriteValue(moduleDependency.Name);
                            Debug.Assert(dependency.IndexExpression is null);
                            break;
                        // dependency is on the entire resource collection
                        // write the name of the resource collection as the dependency
                        case (true, null):
                            jsonWriter.WriteValue(moduleDependency.Name);
                            break;
                        case (true, {} indexExpression):
                            emitter.EmitIndexedSymbolReference(moduleDependency, indexExpression, newContext);
                            break;
                    }
                    break;
                default:
                    throw new InvalidOperationException($"Found dependency '{dependency.Resource.Name}' of unexpected type {dependency.GetType()}");
            }            
        }

        private void EmitClassicDependsOnEntry(JsonTextWriter jsonWriter, ExpressionEmitter emitter, SyntaxBase newContext, ResourceDependency dependency)
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

                    var resource = context.SemanticModel.ResourceMetadata.TryLookup(resourceDependency.DeclaringSyntax) ??
                        throw new ArgumentException($"Unable to find resource metadata for dependency '{dependency.Resource.Name}'");

                    emitter.EmitResourceIdReference(resource, dependency.IndexExpression, newContext);
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
                if (context.Settings.EnableSymbolicNames)
                {
                    EmitSymbolicNameDependsOnEntry(jsonWriter, emitter, newContext, dependency);
                }
                else
                {
                    EmitClassicDependsOnEntry(jsonWriter, emitter, newContext, dependency);
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

        private void EmitOutput(JsonTextWriter jsonWriter, OutputSymbol outputSymbol, ExpressionEmitter emitter)
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
                // emit any decorators on this output
                var body = AddDecoratorsToBody(
                outputSymbol.DeclaringOutput, 
                SyntaxFactory.CreateObject(Enumerable.Empty<ObjectPropertySyntax>()), 
                outputSymbol.Type);
                foreach (var (property, val) in body.ToNamedPropertyValueDictionary())
                {
                    emitter.EmitProperty(property, val);
                }
            }
            jsonWriter.WriteEndObject();
        }

        public void EmitMetadata(JsonTextWriter jsonWriter, ExpressionEmitter emitter)
        {
            jsonWriter.WritePropertyName("metadata");
            jsonWriter.WriteStartObject();
            {
                if (context.Settings.EnableSymbolicNames)
                {
                    emitter.EmitProperty("EXPERIMENTAL_WARNING", "Symbolic name support in ARM is experimental, and should be enabled for testing purposes only. Do not enable this setting for any production usage, or you may be unexpectedly broken at any time!");
                }

                jsonWriter.WritePropertyName("_generator");
                jsonWriter.WriteStartObject();
                {
                    emitter.EmitProperty("name", LanguageConstants.LanguageId);
                    emitter.EmitProperty("version", this.context.Settings.AssemblyFileVersion);
                }
                jsonWriter.WriteEndObject();
            }
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

