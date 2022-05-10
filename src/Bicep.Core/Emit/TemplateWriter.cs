// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Core.Extensions;
using Azure.Deployments.Core.Helpers;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;

namespace Bicep.Core.Emit
{
    // TODO: Are there discrepancies between parameter, variable, and output names between bicep and ARM?
    public class TemplateWriter : ITemplateWriter
    {
        public const string GeneratorMetadataPath = "metadata._generator";
        public const string NestedDeploymentResourceType = AzResourceTypeProvider.ResourceTypeDeployments;
        public const string TemplateHashPropertyName = "templateHash";

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
        public readonly Dictionary<int, (int, int)> rawSourceMap;

        public TemplateWriter(SemanticModel semanticModel, EmitterSettings settings)
        {
            this.context = new EmitterContext(semanticModel, settings);
            this.settings = settings;
            this.rawSourceMap = new Dictionary<int, (int, int)>();
        }

        public void Write(JsonTextWriter writer)
        {
            // Template is used for calculating template hash, template JToken is used for writing to file.
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
            using var stringWriter = new StringWriter();
            using var jsonWriter = PositionTrackingJsonTextWriter.Create(stringWriter);
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

            this.EmitVariablesIfPresent(jsonWriter, emitter);

            this.EmitImports(jsonWriter, emitter);

            this.EmitResources(jsonWriter, emitter);

            this.EmitOutputsIfPresent(jsonWriter, emitter);

            jsonWriter.WriteEndObject();

            var content = stringWriter.ToString();
            return (Template.FromJson<Template>(content), content.FromJson<JToken>());
        }

        private void TransformSourceMap(JToken rawTemplate)
        {
            var unformattedTemplate = rawTemplate.ToString(Formatting.None); // DEBUG
            var formattedTemplate = rawTemplate.ToString(Formatting.Indented); // DEBUG

            // get line starts of unformatted JSON by stripping formatting from each line of formatted JSON
            var unformattedLineStarts = rawTemplate
                .ToString(Formatting.Indented)
                .Split(Environment.NewLine, StringSplitOptions.None)
                .Aggregate(
                    new List<int>() { 0 }, // first line starts at position 0
                    (lineStarts, line) =>
                    {
                        var unformattedLine = Regex.Replace(line, @"(""(?:[^""\\]|\\.)*"")|\s+", "$1");
                        lineStarts.Add(lineStarts.Last() + unformattedLine.Length);
                        return lineStarts;
                    });

            // transform offsets in rawSourceMap to line numbers for formatted JSON using unformattedLineStarts
            // add 1 to all line numbers to convert to 1-indexing
            var formattedSourceMap = this.rawSourceMap.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.ToDictionary(
                    kvp => kvp.Key + 1,
                    kvp => (
                        TextCoordinateConverter.GetPosition(unformattedLineStarts, kvp.Value.start).line + 1,
                        TextCoordinateConverter.GetPosition(unformattedLineStarts, kvp.Value.end).line + 1,
                        unformattedTemplate[kvp.Value.start..kvp.Value.end] // DEBUG
                    )));

            // unfold key-values in bicep-to-json map to reverse to json-to-bicep map
            this.sourceMap = new (string, int)[unformattedLineStarts.Count + 1];
            var weights = new int[unformattedLineStarts.Count + 1];
            Array.Clear(this.sourceMap);
            Array.Fill(weights, int.MaxValue);

            formattedSourceMap.ForEach(fileKvp =>
                fileKvp.Value.ForEach(lineKvp =>
                {
                    // write most specific mapping available for each json (less lines => higher weight)
                    int linesInMapping = lineKvp.Value.Item2 - lineKvp.Value.Item1;
                    for (int i = lineKvp.Value.Item1; i <= lineKvp.Value.Item2; i++)
                    {
                        // write new mapping if weight is stronger than existing mapping
                        if (linesInMapping < weights[i])
                        {
                            this.sourceMap[i] = (fileKvp.Key, lineKvp.Key);
                            weights[i] = linesInMapping;
                        }
                    }
                }));
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
                int startPos = jsonWriter.CurrentPos;

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

        private void EmitParameter(PositionTrackingJsonTextWriter jsonWriter, ParameterSymbol parameterSymbol, ExpressionEmitter emitter)
        {
            int startPos = jsonWriter.CurrentPos;

            var declaringParameter = parameterSymbol.DeclaringParameter;

            var properties = new List<ObjectPropertySyntax>();
            if (parameterSymbol.Type is ResourceType resourceType)
            {
                // Encode a resource type as a string parameter with a metadata for the resource type.
                properties.Add(SyntaxFactory.CreateObjectProperty("type", SyntaxFactory.CreateStringLiteral(LanguageConstants.String.Name)));
                properties.Add(SyntaxFactory.CreateObjectProperty(
                    LanguageConstants.ParameterMetadataPropertyName,
                    SyntaxFactory.CreateObject(new[]
                    {
                        SyntaxFactory.CreateObjectProperty(
                            LanguageConstants.MetadataResourceTypePropertyName,
                            SyntaxFactory.CreateStringLiteral(resourceType.TypeReference.FormatName())),
                    })));
            }
            else if (SyntaxHelper.TryGetPrimitiveType(declaringParameter) is TypeSymbol primitiveType)
            {
                properties.Add(SyntaxFactory.CreateObjectProperty("type", SyntaxFactory.CreateStringLiteral(primitiveType.Name)));
            }
            else
            {
                // this should have been caught by the type checker long ago
                throw new ArgumentException($"Unable to find primitive type for parameter {parameterSymbol.Name}");
            }

            jsonWriter.WriteStartObject();

            var parameterObject = SyntaxFactory.CreateObject(properties);

            if (declaringParameter.Modifier is ParameterDefaultValueSyntax defaultValueSyntax)
            {
                parameterObject = parameterObject.MergeProperty("defaultValue", defaultValueSyntax.DefaultValue);
            }

            parameterObject = AddDecoratorsToBody(declaringParameter, parameterObject, SyntaxHelper.TryGetPrimitiveType(declaringParameter) ?? parameterSymbol.Type);

            foreach (var property in parameterObject.Properties)
            {
                if (property.TryGetKeyText() is string propertyName)
                {
                    // TODO: investigate if ever viable position
                    //int start2 = jsonWriter.CurrentPos;
                    emitter.EmitProperty(propertyName, property.Value);
                    //AddSourceMapping(property, start2, jsonWriter);
                }
            }

            jsonWriter.WriteEndObject();
        }

        private void EmitVariablesIfPresent(PositionTrackingJsonTextWriter jsonWriter, ExpressionEmitter emitter)
        {
            if (!this.context.SemanticModel.Root.VariableDeclarations.Any(symbol => !this.context.VariablesToInline.Contains(symbol)) &&
                this.context.FunctionVariables.Count == 0)
            {
                return;
            }

            jsonWriter.WritePropertyName("variables");
            jsonWriter.WriteStartObject();

            //emit internal variables
            foreach (var functionVariable in this.context.FunctionVariables.Values.OrderBy(x => x.Name, LanguageConstants.IdentifierComparer))
            {
                jsonWriter.WritePropertyName(functionVariable.Name);
                emitter.EmitExpression(functionVariable.Value);
            }

            var variableLookup = this.context.SemanticModel.Root.VariableDeclarations.ToLookup(variableSymbol => variableSymbol.Value is ForSyntax);

            // local function
            IEnumerable<VariableSymbol> GetNonInlinedVariables(bool valueIsLoop) =>
                variableLookup[valueIsLoop].Where(symbol => !this.context.VariablesToInline.Contains(symbol));

            if (GetNonInlinedVariables(valueIsLoop: true).Any())
            {
                // we have variables whose values are loops
                emitter.EmitProperty("copy", () =>
                {
                    jsonWriter.WriteStartArray();

                    foreach (var variableSymbol in GetNonInlinedVariables(valueIsLoop: true))
                    {
                        int startPos = jsonWriter.CurrentPos;

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
                int startPos = jsonWriter.CurrentPos;

                jsonWriter.WritePropertyName(variableSymbol.Name);
                emitter.EmitExpression(variableSymbol.Value);
            }

            jsonWriter.WriteEndObject();
        }

        private void EmitImports(PositionTrackingJsonTextWriter jsonWriter, ExpressionEmitter emitter)
        {
            if (!context.SemanticModel.Root.ImportDeclarations.Any())
            {
                return;
            }

            jsonWriter.WritePropertyName("imports");
            jsonWriter.WriteStartObject();

            foreach (var import in this.context.SemanticModel.Root.ImportDeclarations)
            {
                int startPos = jsonWriter.CurrentPos;

                var namespaceType = context.SemanticModel.GetTypeInfo(import.DeclaringSyntax) as NamespaceType
                    ?? throw new ArgumentException("Imported namespace does not have namespace type");

                jsonWriter.WritePropertyName(import.DeclaringImport.AliasName.IdentifierName);
                jsonWriter.WriteStartObject();

                emitter.EmitProperty("provider", namespaceType.Settings.ArmTemplateProviderName);
                emitter.EmitProperty("version", namespaceType.Settings.ArmTemplateProviderVersion);
                if (import.DeclaringImport.Config is { } config)
                {
                    emitter.EmitProperty("config", config);
                }

                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndObject();
        }

        private void EmitResources(PositionTrackingJsonTextWriter jsonWriter, ExpressionEmitter emitter)
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

            foreach (var resource in this.context.SemanticModel.DeclaredResources)
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

        private ulong? GetBatchSize(StatementSyntax statement)
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

        private void EmitResource(PositionTrackingJsonTextWriter jsonWriter, DeclaredResourceMetadata resource, ExpressionEmitter emitter)
        {
            int startPos = jsonWriter.CurrentPos;

            jsonWriter.WriteStartObject();

            // Save current line (start of resource) for source map
            int startLine = jsonWriter.CurrentLine;

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

            if (context.Settings.EnableSymbolicNames && resource.IsExistingResource)
            {
                jsonWriter.WritePropertyName("existing");
                jsonWriter.WriteValue(true);
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
                jsonWriter.WritePropertyName("properties");
                jsonWriter.WriteStartObject();

                emitter.EmitObjectProperties((ObjectSyntax)body, ResourcePropertiesToOmit);

                jsonWriter.WriteEndObject();
            }

            this.EmitDependsOn(jsonWriter, resource.Symbol, emitter, body);

            // Since we don't want to be mutating the body of the original ObjectSyntax, we create an placeholder body in place
            // and emit its properties to merge decorator properties.
            foreach (var (property, val) in AddDecoratorsToBody(
                resource.Symbol.DeclaringResource,
                SyntaxFactory.CreateObject(Enumerable.Empty<ObjectPropertySyntax>()),
                resource.Symbol.Type).ToNamedPropertyValueDictionary())
            {
                emitter.EmitProperty(property, val);
            }

            jsonWriter.WriteEndObject();
        }

        private void EmitModuleParameters(JsonTextWriter jsonWriter, ModuleSymbol moduleSymbol, ExpressionEmitter emitter)
        {
            var paramsValue = moduleSymbol.TryGetBodyPropertyValue(LanguageConstants.ModuleParamsPropertyName);
            if (paramsValue is not ObjectSyntax paramsObjectSyntax)
            {
                // 'params' is optional if the module has no required params
                return;
            }

            jsonWriter.WritePropertyName("parameters");

            jsonWriter.WriteStartObject();

            foreach (var propertySyntax in paramsObjectSyntax.Properties)
            {
                if (propertySyntax.TryGetKeyText() is not string keyName)
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
                else if (
                    this.context.SemanticModel.ResourceMetadata.TryLookup(propertySyntax.Value) is { } resourceMetadata &&
                    moduleSymbol.TryGetModuleType() is ModuleType moduleType &&
                    moduleType.TryGetParameterType(keyName) is ResourceParameterType parameterType)
                {
                    // This is a resource being passed into a module, we actually want to pass in its id
                    // rather than the whole resource.
                    emitter.EmitProperty("value", new PropertyAccessSyntax(propertySyntax.Value, SyntaxFactory.DotToken, SyntaxFactory.CreateIdentifier("id")));
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

        private void EmitModule(PositionTrackingJsonTextWriter jsonWriter, ModuleSymbol moduleSymbol, ExpressionEmitter emitter)
        {
            int startPos = jsonWriter.CurrentPos;

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
                    emitter.EmitProperty("copy", () => emitter.EmitCopyObject(moduleSymbol.Name, @for, input: null, batchSize: batchSize));
                    break;
            }

            emitter.EmitProperty("type", NestedDeploymentResourceType);
            emitter.EmitProperty("apiVersion", NestedDeploymentResourceApiVersion);

            // emit all properties apart from 'params'. In practice, this currently only allows 'name', but we may choose to allow other top-level resource properties in future.
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

            // Since we don't want to be mutating the body of the original ObjectSyntax, we create an placeholder body in place
            // and emit its properties to merge decorator properties.
            foreach (var (property, val) in AddDecoratorsToBody(
                moduleSymbol.DeclaringModule,
                SyntaxFactory.CreateObject(Enumerable.Empty<ObjectPropertySyntax>()),
                moduleSymbol.Type).ToNamedPropertyValueDictionary())
            {
                emitter.EmitProperty(property, val);
            }

            jsonWriter.WriteEndObject();
        }

        private static bool ShouldGenerateDependsOn(ResourceDependency dependency)
        {
            if (dependency.Kind == ResourceDependencyKind.Transitive)
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

        private void EmitOutputsIfPresent(PositionTrackingJsonTextWriter jsonWriter, ExpressionEmitter emitter)
        {
            if (this.context.SemanticModel.Root.OutputDeclarations.Length == 0)
            {
                return;
            }

            jsonWriter.WritePropertyName("outputs");
            jsonWriter.WriteStartObject();

            foreach (var outputSymbol in this.context.SemanticModel.Root.OutputDeclarations)
            {
                int startPos = jsonWriter.CurrentPos;

                jsonWriter.WritePropertyName(outputSymbol.Name);
                EmitOutput(jsonWriter, outputSymbol, emitter);
            }

            jsonWriter.WriteEndObject();
        }

        private void EmitOutput(PositionTrackingJsonTextWriter jsonWriter, OutputSymbol outputSymbol, ExpressionEmitter emitter)
        {
            int startPos = jsonWriter.CurrentPos;

            jsonWriter.WriteStartObject();

            var properties = new List<ObjectPropertySyntax>();
            if (outputSymbol.Type is ResourceType resourceType)
            {
                // Resource-typed outputs are encoded as strings
                emitter.EmitProperty("type", LanguageConstants.String.Name);

                properties.Add(SyntaxFactory.CreateObjectProperty(
                    LanguageConstants.ParameterMetadataPropertyName,
                    SyntaxFactory.CreateObject(new[]
                    {
                        SyntaxFactory.CreateObjectProperty(
                            LanguageConstants.MetadataResourceTypePropertyName,
                            SyntaxFactory.CreateStringLiteral(resourceType.TypeReference.FormatName())),
                    })));
            }
            else
            {
                emitter.EmitProperty("type", outputSymbol.Type.Name);
            }


            if (outputSymbol.Value is ForSyntax @for)
            {
                emitter.EmitProperty("copy", () => emitter.EmitCopyObject(name: null, @for, @for.Body));
            }
            else
            {
                if (outputSymbol.Type is ResourceType)
                {
                    // Resource-typed outputs are serialized using the resource id.
                    var value = new PropertyAccessSyntax(outputSymbol.Value, SyntaxFactory.DotToken, SyntaxFactory.CreateIdentifier("id"));
                    emitter.EmitProperty("value", value);
                }
                else
                {
                    emitter.EmitProperty("value", outputSymbol.Value);
                }
            }

            // emit any decorators on this output
            foreach (var (property, val) in AddDecoratorsToBody(
                outputSymbol.DeclaringOutput,
                SyntaxFactory.CreateObject(properties),
                outputSymbol.Type).ToNamedPropertyValueDictionary())
            {
                emitter.EmitProperty(property, val);
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

        private void AddSourceMapping(SyntaxBase bicepSyntax, int startPosition, PositionTrackingJsonTextWriter jsonWriter)
        {
            (int bicepLine, _) = TextCoordinateConverter.GetPosition(this.context.SemanticModel.SourceFile.LineStarts, bicepSyntax.GetPosition());

            // account for leading nodes (decorators)
            if (bicepSyntax is StatementSyntax syntax)
            {
                bicepLine += syntax.LeadingNodes.Count(node => node is Token token && token.Type == TokenType.NewLine);
            }

            // increment start position if starting on a comma (happens when outputting successive items in objects and arrays)
            startPosition = jsonWriter.CommaPositions.Contains(startPosition)
                ? startPosition + 1
                : startPosition;

            rawSourceMap[bicepLine] = (startPosition, jsonWriter.CurrentPos - 1);
        }
    }
}

