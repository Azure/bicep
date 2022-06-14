// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Azure.Deployments.Core.Helpers;
using Azure.Deployments.Expression.Expressions;
using Azure.Deployments.Core.Definitions.Schema;
using Bicep.Core.Intermediate;
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
using Microsoft.WindowsAzure.ResourceStack.Common.Json;

namespace Bicep.Core.Emit
{
    // TODO: Are there discrepancies between parameter, variable, and output names between bicep and ARM?
    public class TemplateWriter : ITemplateWriter
    {
        public const string GeneratorMetadataPath = "metadata._generator";
        public const string NestedDeploymentResourceType = AzResourceTypeProvider.ResourceTypeDeployments;

        // IMPORTANT: Do not update this API version until the new one is confirmed to be deployed and available in ALL the clouds.
        public const string NestedDeploymentResourceApiVersion = "2020-10-01";

        private static readonly ImmutableHashSet<string> ResourcePropertiesToOmit = new[] {
            LanguageConstants.ResourceScopePropertyName,
            LanguageConstants.ResourceParentPropertyName,
            LanguageConstants.ResourceDependsOnPropertyName,
        }.ToImmutableHashSet();

        private static readonly ImmutableHashSet<string> ModulePropertiesToOmit = new[] {
            LanguageConstants.ModuleParamsPropertyName,
            LanguageConstants.ResourceScopePropertyName,
            LanguageConstants.ResourceDependsOnPropertyName,
        }.ToImmutableHashSet();

        public static readonly ImmutableHashSet<string> DecoratorsToEmitAsResourceProperties = new[] {
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

            var program = emitter.ConvertProgram(context.SemanticModel.Root);

            jsonWriter.WriteStartObject();

            emitter.EmitProperty("$schema", GetSchema(context.SemanticModel.TargetScope));

            if (context.Settings.EnableSymbolicNames)
            {
                emitter.EmitProperty("languageVersion", "1.9-experimental");
            }

            emitter.EmitProperty("contentVersion", "1.0.0.0");

            this.EmitMetadata(emitter);

            this.EmitParametersIfPresent(program, emitter);

            this.EmitVariablesIfPresent(program, emitter);

            this.EmitImports(program, emitter);

            this.EmitResources(jsonWriter, emitter);

            this.EmitOutputsIfPresent(program, emitter);

            jsonWriter.WriteEndObject();

            var content = stringWriter.ToString();
            return (Template.FromJson<Template>(content), content.FromJson<JToken>());
        }

        private void EmitParametersIfPresent(ProgramOperation program, ExpressionEmitter emitter)
        {
            if (!program.Parameters.Any())
            {
                return;
            }

            emitter.EmitObjectProperty("parameters", () => {
                foreach (var parameter in program.Parameters)
                {
                    emitter.EmitObjectProperty(parameter.Name, () => {
                        foreach (var property in parameter.AdditionalProperties)
                        {
                            emitter.EmitOperation(property);
                        }
                    });
                }
            });
        }

        private void EmitVariablesIfPresent(ProgramOperation program, ExpressionEmitter emitter)
        {
            if (!program.Variables.Any())
            {
                return;
            }

            emitter.EmitObjectProperty("variables", () => {
                var loopVariables = program.Variables.Where(x => x.Value is ForLoopOperation);
                var nonLoopVariables = program.Variables.Where(x => x.Value is not ForLoopOperation);

                if (loopVariables.Any())
                {
                    // we have variables whose values are loops
                    emitter.EmitArrayProperty("copy", () => {
                        foreach (var loopVariable in loopVariables)
                        {
                            var forLoopVariable = ((ForLoopOperation)loopVariable.Value);
                            emitter.EmitCopyObject(loopVariable.Name, forLoopVariable.Expression, forLoopVariable.Body);
                        }
                    });
                }

                foreach (var variable in nonLoopVariables)
                {
                    emitter.EmitProperty(variable.Name, variable.Value);
                }
            });
        }

        private void EmitImports(ProgramOperation program, ExpressionEmitter emitter)
        {
            if (!program.Imports.Any())
            {
                return;
            }

            emitter.EmitObjectProperty("imports", () => {
                foreach (var import in program.Imports)
                {
                    emitter.EmitObjectProperty(import.AliasName, () => {
                        emitter.EmitProperty("provider", import.NamespaceType.Settings.ArmTemplateProviderName);
                        emitter.EmitProperty("version", import.NamespaceType.Settings.ArmTemplateProviderVersion);
                        if (import.Config is { } config)
                        {
                            emitter.EmitProperty("config", config);
                        }
                    });
                }
            });
        }

        private void EmitResources(JsonTextWriter jsonWriter, ExpressionEmitter emitter)
        {
            if (context.Settings.EnableSymbolicNames)
            {
                emitter.EmitObjectProperty("resources", () => {
                    foreach (var resource in this.context.SemanticModel.DeclaredResources)
                    {
                        emitter.EmitProperty(resource.Symbol.Name, () => EmitResource(jsonWriter, resource, emitter));
                    }

                    foreach (var moduleSymbol in this.context.SemanticModel.Root.ModuleDeclarations)
                    {
                        emitter.EmitProperty(moduleSymbol.Name, () => EmitModule(jsonWriter, moduleSymbol, emitter));
                    }
                });
            }
            else
            {
                emitter.EmitArrayProperty("resources", () => {
                    foreach (var resource in this.context.SemanticModel.DeclaredResources)
                    {
                        if (resource.IsExistingResource)
                        {
                            continue;
                        }

                        EmitResource(jsonWriter, resource, emitter);
                    }

                    foreach (var moduleSymbol in this.context.SemanticModel.Root.ModuleDeclarations)
                    {
                        EmitModule(jsonWriter, moduleSymbol, emitter);
                    }
                });
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
                var longValue = integerLiteral.Value switch {
                    <= long.MaxValue => (long)integerLiteral.Value,
                    _ => throw new InvalidOperationException($"Integer syntax hs value {integerLiteral.Value} which will overflow"),
                };

                return longValue;
            }
            return null;
        }

        private void EmitResource(JsonTextWriter jsonWriter, DeclaredResourceMetadata resource, ExpressionEmitter emitter)
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
                var loop = loops.Single();
                var batchSize = GetBatchSize(resource.Symbol.DeclaringResource);

                emitter.EmitProperty("copy", () => emitter.EmitCopyObject(
                    loop.name,
                    emitter.GetExpressionOperation(loop.@for.Expression),
                    null,
                    batchSize: batchSize));
            }
            else if (loops.Count > 1)
            {
                throw new InvalidOperationException("nested loops are not supported");
            }

            if (context.Settings.EnableSymbolicNames && resource.IsExistingResource)
            {
                emitter.EmitProperty("existing", new ConstantValueOperation(true));
            }

            var importSymbol = context.SemanticModel.Root.ImportDeclarations.FirstOrDefault(i => resource.Type.DeclaringNamespace.AliasNameEquals(i.Name));

            if (importSymbol is not null)
            {
                emitter.EmitProperty("import", importSymbol.Name);
            }

            if (resource.IsAzResource)
            {
                emitter.EmitProperty("type", resource.TypeReference.FormatType());
                if (resource.TypeReference.ApiVersion is not null)
                {
                    emitter.EmitProperty("apiVersion", resource.TypeReference.ApiVersion);
                }
            }
            else
            {
                emitter.EmitProperty("type", resource.TypeReference.FormatName());
            }

            if (context.SemanticModel.EmitLimitationInfo.ResourceScopeData.TryGetValue(resource, out var scopeData))
            {
                ScopeHelper.EmitResourceScopeProperties(context.SemanticModel, scopeData, emitter, body);
            }

            if (resource.IsAzResource)
            {
                emitter.EmitProperty(AzResourceTypeProvider.ResourceNamePropertyName, emitter.GetFullyQualifiedResourceName(resource));
                emitter.EmitObjectProperties((ObjectSyntax)body, ResourcePropertiesToOmit.Add(AzResourceTypeProvider.ResourceNamePropertyName));
            }
            else
            {
                emitter.EmitObjectProperty("properties", () => {
                    emitter.EmitObjectProperties((ObjectSyntax)body, ResourcePropertiesToOmit);
                });
            }

            this.EmitDependsOn(jsonWriter, resource.Symbol, emitter, body);

            foreach (var property in emitter.GetDecorators(resource.Symbol.DeclaringResource, resource.Symbol.Type))
            {
                emitter.EmitProperty(property.Key, property.Value);
            }

            jsonWriter.WriteEndObject();
        }

        private void EmitModuleParameters(ModuleSymbol moduleSymbol, ExpressionEmitter emitter)
        {
            var paramsValue = moduleSymbol.TryGetBodyPropertyValue(LanguageConstants.ModuleParamsPropertyName);
            if (paramsValue is not ObjectSyntax paramsObjectSyntax)
            {
                // 'params' is optional if the module has no required params
                return;
            }

            emitter.EmitObjectProperty("parameters", () => {
                foreach (var propertySyntax in paramsObjectSyntax.Properties)
                {
                    if (!(propertySyntax.TryGetKeyText() is string keyName))
                    {
                        // should have been caught by earlier validation
                        throw new ArgumentException("Disallowed interpolation in module parameter");
                    }

                    // we can't just call EmitObjectProperties here because the ObjectSyntax is flatter than the structure we're generating
                    // because nested deployment parameters are objects with a single value property
                    emitter.EmitObjectProperty(keyName, () => {
                        var value = emitter.GetExpressionOperation(propertySyntax.Value);
                        if (value is ForLoopOperation @for)
                        {
                            // the value is a for-expression
                            // write a single property copy loop
                            emitter.EmitArrayProperty("copy", () =>
                            {
                                emitter.EmitCopyObject("value", @for.Expression, @for.Body, "value");
                            });
                        }
                        else if (
                            this.context.SemanticModel.ResourceMetadata.TryLookup(propertySyntax.Value) is {} resourceMetadata &&
                            moduleSymbol.TryGetModuleType() is ModuleType moduleType &&
                            moduleType.TryGetParameterType(keyName) is ResourceParameterType parameterType)
                        {
                            // This is a resource being passed into a module, we actually want to pass in its id
                            // rather than the whole resource.
                            emitter.EmitProperty("value", new ResourceIdOperation(resourceMetadata, null));
                        }
                        else
                        {
                            // the value is not a for-expression - can emit normally
                            emitter.EmitModuleParameterValue(value);
                        }
                    });
                }
            });
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
                    if (@for.Body is IfConditionSyntax loopFilter)
                    {
                        body = loopFilter.Body;
                        emitter.EmitProperty("condition", loopFilter.ConditionExpression);
                    }
                    else
                    {
                        body = @for.Body;
                    }

                    var batchSize = GetBatchSize(moduleSymbol.DeclaringModule);

                    emitter.EmitProperty("copy", () => emitter.EmitCopyObject(
                        moduleSymbol.Name,
                        emitter.GetExpressionOperation(@for.Expression),
                        null,
                        batchSize: batchSize));
                    break;
            }

            emitter.EmitProperty("type", NestedDeploymentResourceType);
            emitter.EmitProperty("apiVersion", NestedDeploymentResourceApiVersion);

            // emit all properties apart from 'params'. In practice, this currrently only allows 'name', but we may choose to allow other top-level resource properties in future.
            // params requires special handling (see below).
            emitter.EmitObjectProperties((ObjectSyntax)body, ModulePropertiesToOmit);

            var scopeData = context.ModuleScopeData[moduleSymbol];
            ScopeHelper.EmitModuleScopeProperties(context.SemanticModel, scopeData, emitter, body);

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

            emitter.EmitObjectProperty("properties", () => {
                emitter.EmitObjectProperty("expressionEvaluationOptions", () => {
                    emitter.EmitProperty("scope", "inner");
                });

                emitter.EmitProperty("mode", "Incremental");

                EmitModuleParameters(moduleSymbol, emitter);

                var moduleSemanticModel = GetModuleSemanticModel(moduleSymbol);

                // If it is a template spec module, emit templateLink instead of template contents.
                emitter.EmitProperty(moduleSemanticModel is TemplateSpecSemanticModel ? "templateLink" : "template", () => {
                    TemplateWriterFactory.CreateTemplateWriter(moduleSemanticModel, this.settings).Write(jsonWriter);
                });
            });

            this.EmitDependsOn(jsonWriter, moduleSymbol, emitter, body);

            foreach (var property in emitter.GetDecorators(moduleSymbol.DeclaringModule, moduleSymbol.Type))
            {
                emitter.EmitProperty(property.Key, property.Value);
            }

            jsonWriter.WriteEndObject();
        }
        private static bool ShouldGenerateDependsOn(ResourceDependency dependency)
        {
            if(dependency.Kind == ResourceDependencyKind.Transitive)
            {
                // transitive dependencies do not have to be emitted
                return false;
            }

            return dependency.Resource switch
            {   // We only want to add a 'dependsOn' for resources being deployed in this file.
                ResourceSymbol resource => !resource.DeclaringResource.IsExistingResource(),
                ModuleSymbol => true,
                _ => throw new InvalidOperationException($"Found dependency '{dependency.Resource.Name}' of unexpected type {dependency.GetType()}"),
            };
        }

        private void EmitSymbolicNameDependsOnEntry(JsonTextWriter jsonWriter, ExpressionEmitter emitter, SyntaxBase newContext, ResourceDependency dependency)
        {
            switch (dependency.Resource)
            {
                case ResourceSymbol resourceDependency:
                    var resource = context.SemanticModel.ResourceMetadata.TryLookup(resourceDependency.DeclaringSyntax) as DeclaredResourceMetadata ??
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
                        case (true, { } indexExpression):
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
                        case (true, { } indexExpression):
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

                    var resource = context.SemanticModel.ResourceMetadata.TryLookup(resourceDependency.DeclaringSyntax) as DeclaredResourceMetadata ??
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

            emitter.EmitArrayProperty("dependsOn", () => {
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
            });
        }

        private void EmitOutputsIfPresent(ProgramOperation program, ExpressionEmitter emitter)
        {
            if (!program.Outputs.Any())
            {
                return;
            }

            emitter.EmitObjectProperty("outputs", () => {
                foreach (var output in program.Outputs)
                {
                    emitter.EmitObjectProperty(output.Name, () => {
                        emitter.EmitProperty("type", output.Type);
                        if (output.Value is ForLoopOperation forLoop)
                        {
                            emitter.EmitProperty("copy", () => emitter.EmitCopyObject(name: null, forLoop.Expression, forLoop.Body));
                        }
                        else
                        {
                            emitter.EmitProperty("value", output.Value);
                        }

                        foreach (var property in output.AdditionalProperties)
                        {
                            emitter.EmitOperation(property);
                        }
                    });
                }
            });
        }

        public void EmitMetadata(ExpressionEmitter emitter)
        {
            emitter.EmitObjectProperty("metadata", () => {
                if (context.Settings.EnableSymbolicNames)
                {
                    emitter.EmitProperty("EXPERIMENTAL_WARNING", "Symbolic name support in ARM is experimental, and should be enabled for testing purposes only. Do not enable this setting for any production usage, or you may be unexpectedly broken at any time!");
                }

                emitter.EmitObjectProperty("_generator", () => {
                    emitter.EmitProperty("name", LanguageConstants.LanguageId);
                    emitter.EmitProperty("version", this.context.Settings.AssemblyFileVersion);
                });
            });
        }
    }
}

