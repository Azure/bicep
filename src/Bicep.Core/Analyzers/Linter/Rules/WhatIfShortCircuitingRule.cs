// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using Azure.Deployments.Core.Configuration;
using Azure.Deployments.Core.Definitions.Extensibility;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Expression.Intermediate;
using Azure.Deployments.Expression.Intermediate.Extensions;
using Azure.Deployments.Templates.Engines;
using Azure.Deployments.Templates.ParsedEntities;
using Bicep.Core.ArmHelpers;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Types;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class WhatIfShortCircuitingRule : LinterRuleBase
    {
        public new const string Code = "what-if-short-circuiting";

        private const string SentinelValueFunctionName = "sentinel-placeholder";

        public WhatIfShortCircuitingRule() : base(
            code: Code,
            description: CoreResources.WhatIfShortCircuitingRuleDescription,
            LinterRuleCategory.PotentialCodeIssues,
            overrideCategoryDefaultDiagnosticLevel: DiagnosticLevel.Off) // Disabled by default while still experimental
        { }

        public override string FormatMessage(params object[] values)
        {
            return string.Format(CoreResources.WhatIfShortCircuitingRuleMessageFormat, values);
        }

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
        {
            HashSet<SemanticModel> moduleModelsInProcess = new();
            Dictionary<SemanticModel, IReadOnlyDictionary<string, ParameterUsageFlags>> resultsCache = new();
            foreach (var module in model.Root.ModuleDeclarations)
            {
                if (module.DeclaringModule.TryGetBody()?.TryGetPropertyByName(LanguageConstants.ModuleParamsPropertyName)
                    is not { } moduleParamsProperty)
                {
                    // if the module isn't passed any parameters, there's no analysis to perform. Skip this module.
                    continue;
                }

                if (module.TryGetSemanticModel().TryUnwrap() is not { } moduleSemanticModel)
                {
                    // if the module's semantic model can't be loaded due to an error, we can't do any analysis of the
                    // values passed to it. Skip this module.
                    continue;
                }

                Dictionary<string, ObjectPropertySyntax> parametersByName = new(StringComparer.OrdinalIgnoreCase);
                HashSet<ObjectPropertySyntax> parametersWithUnknownNames = new(ReferenceEqualityComparer.Instance);

                if (moduleParamsProperty.Value is ObjectSyntax paramsObject)
                {
                    foreach (var param in paramsObject.Properties)
                    {
                        if (TryGetKeyText(param, model) is string paramName)
                        {
                            parametersByName[paramName] = param;
                        }
                        else
                        {
                            parametersWithUnknownNames.Add(param);
                        }
                    }
                }
                else
                {
                    parametersWithUnknownNames.Add(moduleParamsProperty);
                }

                var moduleModelFlags = GetModuleParameterUsageFlags(
                    moduleModelsInProcess,
                    resultsCache,
                    moduleSemanticModel,
                    parametersWithUnknownNames.Count > 0
                        ? moduleSemanticModel.Parameters.Keys.ToHashSet()
                        : parametersByName.Keys.ToHashSet());

                var resourceTypeResolver = ResourceTypeResolver.Create(model);
                foreach (var kvp in parametersByName)
                {
                    if (moduleModelFlags.TryGetValue(kvp.Key, out var flags) &&
                        flags != ParameterUsageFlags.None &&
                        !IsDeployTimeConstant(kvp.Value, model, resourceTypeResolver))
                    {
                        var target = kvp.Value.Value;
                        yield return CreateDiagnosticForSpan(diagnosticLevel, kvp.Value.Value.Span, kvp.Key, module.Name);
                    }
                }

                var unknownParameterFlags = moduleModelFlags
                    .Where(kvp => !parametersByName.ContainsKey(kvp.Key))
                    .Aggregate(ParameterUsageFlags.None, (acc, cur) => acc | cur.Value);

                if (unknownParameterFlags != ParameterUsageFlags.None)
                {
                    foreach (var parameterWithUnknownName in parametersWithUnknownNames)
                    {
                        if (!IsDeployTimeConstant(parameterWithUnknownName, model, resourceTypeResolver))
                        {
                            var target = parameterWithUnknownName.Value;
                            yield return CreateDiagnosticForSpan(diagnosticLevel, parameterWithUnknownName.Value.Span, "<unknown>", module.Name);
                        }
                    }
                }
            }
        }

        private static string? TryGetKeyText(ObjectPropertySyntax propertySyntax, SemanticModel model)
        {
            if (propertySyntax.TryGetKeyText() is string uninterpolatedKeyText)
            {
                return uninterpolatedKeyText;
            }

            if (model.GetTypeInfo(propertySyntax.Key) is StringLiteralType { RawStringValue: string interpolatedConstantKey })
            {
                return interpolatedConstantKey;
            }

            return null;
        }

        [Flags]
        private enum ParameterUsageFlags
        {
            None = 0,
            UsedInResourceIdentifier = 1 << 0,
            UsedInResourceCondition = 1 << 1,
            UsedInResourceApiVersion = 1 << 2,
            UsedInLoopInput = 1 << 3,
        }

        private static bool IsDeployTimeConstant(ObjectPropertySyntax syntax, SemanticModel model, ResourceTypeResolver resolver)
        {
            var diagWriter = ToListDiagnosticWriter.Create();
            DeployTimeConstantValidator.CheckDeployTimeConstantViolations(syntax, syntax.Value, model, diagWriter, resolver);

            return diagWriter.GetDiagnostics().Count == 0;
        }

        private static IReadOnlyDictionary<string, ParameterUsageFlags> GetModuleParameterUsageFlags(
            HashSet<SemanticModel> modelsInProcess,
            Dictionary<SemanticModel, IReadOnlyDictionary<string, ParameterUsageFlags>> staticResults,
            ISemanticModel moduleModel,
            IReadOnlySet<string> moduleParametersSupplied) => moduleModel switch
            {
                SemanticModel bicepModel => GetBicepModuleParameterUsageFlags(
                    modelsInProcess,
                    staticResults,
                    bicepModel,
                    moduleParametersSupplied),
                ArmTemplateSemanticModel armModel => GetArmTemplateModuleParameterUsageFlags(
                    armModel.TargetScope,
                    armModel.SourceFile,
                    moduleParametersSupplied),
                TemplateSpecSemanticModel templateSpecModel => GetArmTemplateModuleParameterUsageFlags(
                    templateSpecModel.TargetScope,
                    templateSpecModel.SourceFile.MainTemplateFile,
                    moduleParametersSupplied),
                _ => throw new UnreachableException(
                    $"Unable to handle model of type {moduleModel.GetType().FullName}"),
            };

        private static IReadOnlyDictionary<string, ParameterUsageFlags> GetBicepModuleParameterUsageFlags(
            HashSet<SemanticModel> modelsInProcess,
            Dictionary<SemanticModel, IReadOnlyDictionary<string, ParameterUsageFlags>> staticResults,
            SemanticModel moduleModel,
            IReadOnlySet<string> moduleParametersSupplied)
        {
            var moduleModelFlags = GatherStaticParameterUsageFlags(moduleModel, modelsInProcess, staticResults);
            Dictionary<string, ParameterUsageFlags> usageFlags = new(StringComparer.OrdinalIgnoreCase);

            foreach (var parameterName in moduleParametersSupplied)
            {
                if (!moduleModelFlags.TryGetValue(parameterName, out var flagsToTaint))
                {
                    // If the supplied parameter isn't defined on the module's model, an error will be
                    // raised elsewhere
                    flagsToTaint = ParameterUsageFlags.None;
                }

                usageFlags.Add(parameterName, flagsToTaint);
            }

            if (moduleModelFlags.Keys.Any(k => !moduleParametersSupplied.Contains(k)))
            {
                // there are some parameters that are not supplied. Their flags should taint any parameters that
                // they use in their default value expressions
                var unsuppliedParameterFlags = moduleModelFlags
                    .Where(kvp => !moduleParametersSupplied.Contains(kvp.Key))
                    .ToDictionary(StringComparer.OrdinalIgnoreCase);

                // first, find which (if any) parameters are referenced in the default value expression for each
                // parameter that was not supplied
                IReadOnlyDictionary<string, IReadOnlySet<string>> edges = unsuppliedParameterFlags.Keys
                    .Select(parameterName =>
                    {
                        if (moduleModel.Root.ParameterDeclarations
                            .Where(p => p.Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase))
                            .FirstOrDefault()
                            ?.DeclaringParameter
                            .Modifier is ParameterDefaultValueSyntax defaultValueSyntax)
                        {
                            return new KeyValuePair<string, IReadOnlySet<string>>(
                                parameterName,
                                SymbolicReferenceCollector
                                    .CollectSymbolsReferenced(moduleModel.Binder, defaultValueSyntax)
                                    .Keys
                                    .SelectMany(s => GetParametersUsedInClosureOf(moduleModel.Binder, s))
                                    .Select(p => p.Name)
                                    .ToHashSet());
                        }

                        return new(parameterName, ImmutableHashSet<string>.Empty);
                    })
                    .ToDictionary(StringComparer.OrdinalIgnoreCase);

                // Next, go over the unsupplied parameters in topological order (since parameters can refer to each
                // other (directly or indirectly) in their default value expressions)
                foreach (var unsuppliedParameter in TopoSort(edges))
                {
                    var flags = unsuppliedParameterFlags[unsuppliedParameter];
                    // ...find the name of each parameter that is used in the default value expression
                    foreach (var input in edges[unsuppliedParameter])
                    {
                        // ... if the parameter was supplied explicitly to the model, taint `unsuppliedParameter`'s
                        // flags onto the supplied parameter
                        if (usageFlags.TryGetValue(input, out var flagsOnSuppliedParam))
                        {
                            usageFlags[input] = flagsOnSuppliedParam | flags;
                        }
                        // ... if the parameter wasn't supplied, taint `unsuppliedParameter`'s flags onto that parameter
                        else if (unsuppliedParameterFlags.TryGetValue(input, out var flagsOnUnsuppliedParam))
                        {
                            unsuppliedParameterFlags[input] = flagsOnUnsuppliedParam | flags;
                        }
                    }
                }
            }

            return usageFlags;
        }

        private static IReadOnlyDictionary<string, ParameterUsageFlags> GatherStaticParameterUsageFlags(
            SemanticModel model,
            HashSet<SemanticModel> modelsInProcess,
            Dictionary<SemanticModel, IReadOnlyDictionary<string, ParameterUsageFlags>> resultsCache)
        {
            if (!modelsInProcess.Add(model))
            {
                return ImmutableDictionary<string, ParameterUsageFlags>.Empty;
            }

            if (resultsCache.TryGetValue(model, out var cached))
            {
                return cached;
            }

            var gatheredFlags = ParameterUsageFlagCollector.GatherParameterUsageFlags(model, modelsInProcess, resultsCache);
            resultsCache[model] = gatheredFlags;
            modelsInProcess.Remove(model);

            return gatheredFlags;
        }

        private class ParameterUsageFlagCollector : AstVisitor
        {
            private readonly SemanticModel model;
            private readonly HashSet<SemanticModel> modelsInProcess;
            private readonly Dictionary<SemanticModel, IReadOnlyDictionary<string, ParameterUsageFlags>> resultsCache;

            private readonly Dictionary<ParameterSymbol, ParameterUsageFlags> parameterFlags;

            private ParameterUsageFlags flagsToApply;
            private bool processingResourceTopLevelProperties = false;
            private ISemanticModel? moduleModel;

            private ParameterUsageFlagCollector(
                SemanticModel model,
                HashSet<SemanticModel> modelsInProcess,
                Dictionary<SemanticModel, IReadOnlyDictionary<string, ParameterUsageFlags>> resultsCache)
            {
                this.model = model;
                this.modelsInProcess = modelsInProcess;
                this.resultsCache = resultsCache;

                flagsToApply = ParameterUsageFlags.None;
                parameterFlags = model.Root.ParameterDeclarations.ToDictionary(p => p, _ => flagsToApply);
            }

            public static IReadOnlyDictionary<string, ParameterUsageFlags> GatherParameterUsageFlags(
                SemanticModel model,
                HashSet<SemanticModel> modelsInProcess,
                Dictionary<SemanticModel, IReadOnlyDictionary<string, ParameterUsageFlags>> resultsCache)
            {
                ParameterUsageFlagCollector visitor = new(model, modelsInProcess, resultsCache);
                visitor.Visit(model.Root.Syntax);

                return visitor.parameterFlags.ToDictionary(
                    kvp => kvp.Key.Name,
                    kvp => kvp.Value,
                    StringComparer.OrdinalIgnoreCase);
            }

            public override void VisitIfConditionSyntax(IfConditionSyntax syntax)
            {
                var prevFlagsToApply = flagsToApply;
                flagsToApply |= ParameterUsageFlags.UsedInResourceCondition;
                Visit(syntax.ConditionExpression);
                flagsToApply = prevFlagsToApply;

                Visit(syntax.Body);
            }

            public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax)
            {
                VisitNodes(syntax.LeadingNodes);
                Visit(syntax.Name);
                Visit(syntax.Path);

                var prevModuleModel = this.moduleModel;
                if (model.GetSymbolInfo(syntax) is ModuleSymbol module &&
                    module.TryGetSemanticModel().TryUnwrap() is ISemanticModel moduleSemanticModel)
                {
                    // if this block was not entered, there's a compilation-blocking error with the module statement.
                    // otherwise, set up the visitor to check for transitive parameter usage (e.g., if a parameter is
                    // passed to a module, which then uses it as a resource name).
                    this.moduleModel = moduleSemanticModel;
                }
                this.Visit(syntax.Value);
                this.moduleModel = prevModuleModel;
            }

            public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
            {
                VisitNodes(syntax.LeadingNodes);
                Visit(syntax.Name);
                Visit(syntax.Type);

                var prevProcessingResourceTopLevelProperties = processingResourceTopLevelProperties;
                processingResourceTopLevelProperties = true;
                Visit(syntax.Value);
                processingResourceTopLevelProperties = prevProcessingResourceTopLevelProperties;
            }

            public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
            {
                if (processingResourceTopLevelProperties)
                {
                    Visit(syntax.Key);

                    switch (TryGetKeyText(syntax, model)?.ToLowerInvariant())
                    {
                        case "name":
                        case LanguageConstants.ResourceScopePropertyName:
                            VisitResourceIdentifier(syntax.Value);
                            break;
                        default:
                            processingResourceTopLevelProperties = false;
                            Visit(syntax.Value);
                            processingResourceTopLevelProperties = true;
                            break;
                    }
                }
                else if (moduleModel is not null)
                {
                    Visit(syntax.Key);

                    switch (TryGetKeyText(syntax, model)?.ToLowerInvariant())
                    {
                        case LanguageConstants.ResourceScopePropertyName:
                            VisitResourceIdentifier(syntax.Value);
                            break;
                        case LanguageConstants.ModuleParamsPropertyName:
                            VisitModuleParameters(syntax.Value, moduleModel);
                            break;
                        default:
                            var prevModuleModel = moduleModel;
                            moduleModel = null;
                            Visit(syntax.Value);
                            moduleModel = prevModuleModel;
                            break;
                    }
                }
                else
                {
                    base.VisitObjectPropertySyntax(syntax);
                }
            }

            private void VisitResourceIdentifier(SyntaxBase syntax)
            {
                var prevFlagsToApply = flagsToApply;
                flagsToApply |= ParameterUsageFlags.UsedInResourceIdentifier;
                Visit(syntax);
                flagsToApply = prevFlagsToApply;
            }

            private void VisitModuleParameters(SyntaxBase syntax, ISemanticModel moduleModel)
            {
                Dictionary<string, IReadOnlySet<ParameterSymbol>> parametersPassedAsModuleParameters
                    = new(StringComparer.OrdinalIgnoreCase);
                HashSet<ParameterSymbol> usedInUnknownParameters = new();
                bool anyUnknownParameters = false;
                if (syntax is ObjectSyntax moduleParams)
                {
                    foreach (var suppliedParameter in moduleParams.Properties)
                    {
                        var parametersReferencedInValue = SymbolicReferenceCollector
                            .CollectSymbolsReferenced(model.Binder, suppliedParameter.Value)
                            .Keys
                            .SelectMany(GetParametersUsedInClosureOf)
                            .ToHashSet();

                        if (TryGetKeyText(suppliedParameter, model) is string parameterName)
                        {

                            parametersPassedAsModuleParameters[parameterName] = parametersReferencedInValue;
                        }
                        else
                        {
                            anyUnknownParameters = true;
                            usedInUnknownParameters.UnionWith(parametersReferencedInValue);
                        }
                    }
                }
                else
                {
                    anyUnknownParameters = true;
                    usedInUnknownParameters = [.. SymbolicReferenceCollector
                        .CollectSymbolsReferenced(model.Binder, syntax)
                        .Keys
                        .SelectMany(GetParametersUsedInClosureOf)];
                }

                // if there are some parameters whose names we don't know (which should only happen rarely), we have
                // to taint any parameters that are passed within **any** unknown parameters with the flags that
                // apply to **all** unsupplied parameters. Accordingly, get the flags for all known parameters (not
                // just the ones we know are being passed to the module
                HashSet<string> moduleParametersSupplied = [.. anyUnknownParameters
                    ? moduleModel.Parameters.Keys
                    : parametersPassedAsModuleParameters.Keys];

                var moduleFlags = GetModuleParameterUsageFlags(
                    modelsInProcess,
                    resultsCache,
                    moduleModel,
                    moduleParametersSupplied);

                foreach (var flagsForParam in moduleFlags)
                {
                    if (!parametersPassedAsModuleParameters.TryGetValue(flagsForParam.Key, out var parametersToTaint))
                    {
                        parametersToTaint = usedInUnknownParameters;
                    }

                    foreach (var parameter in parametersToTaint)
                    {
                        parameterFlags[parameter] |= flagsForParam.Value;
                    }
                }
            }

            public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
            {
                UpdateParameterFlagsForClosureOf(model.GetSymbolInfo(syntax));
            }

            private void UpdateParameterFlagsForClosureOf(Symbol? symbol)
            {
                foreach (var parameter in GetParametersUsedInClosureOf(symbol))
                {
                    parameterFlags[parameter] |= flagsToApply;
                }
            }

            private IEnumerable<ParameterSymbol> GetParametersUsedInClosureOf(Symbol? symbol)
                => WhatIfShortCircuitingRule.GetParametersUsedInClosureOf(model.Binder, symbol);
        }

        private static IEnumerable<string> TopoSort(IReadOnlyDictionary<string, IReadOnlySet<string>> edges)
        {
            HashSet<string> processed = new();
            IEnumerable<string> YieldNameAndUnprocessedPredecessors(string n)
            {
                if (!processed.Add(n))
                {
                    yield break;
                }

                if (edges.TryGetValue(n, out var transitiveEdges))
                {
                    foreach (var predecessor in transitiveEdges.SelectMany(YieldNameAndUnprocessedPredecessors))
                    {
                        yield return predecessor;
                    }

                    yield return n;
                }
            }

            return edges.Keys.SelectMany(YieldNameAndUnprocessedPredecessors);
        }

        private static IReadOnlyDictionary<string, ParameterUsageFlags> GetArmTemplateModuleParameterUsageFlags(
            ResourceScope targetScope,
            ArmTemplateFile moduleModel,
            IReadOnlyCollection<string> moduleParametersSupplied)
        {
            if (moduleModel.Template is null)
            {
                // If the module's template failed to parse or load, there's no further analysis we can do here.
                // A compilation-blocking error will be reported elsewhere. 
                return ImmutableDictionary<string, ParameterUsageFlags>.Empty;
            }

            Dictionary<string, ITemplateLanguageExpression> parameterPlaceholders = new(StringComparer.OrdinalIgnoreCase);
            Dictionary<string, ParameterUsageFlags> moduleModelFlags = new(StringComparer.OrdinalIgnoreCase);
            foreach (var parameterName in moduleParametersSupplied)
            {
                parameterPlaceholders[parameterName] = new FunctionExpression(
                    SentinelValueFunctionName,
                    [parameterName.AsExpression()],
                    position: null,
                    irreducible: true);
                moduleModelFlags[parameterName] = ParameterUsageFlags.None;
            }

            var (armResources, extensibilityResources) = GetModuleTemplateAndNestedResources(
                moduleModel.Template,
                targetScope,
                parameterPlaceholders);

            foreach (var armResource in armResources)
            {
                if (armResource.Condition is { } unresolvedCondition)
                {
                    foreach (var parameterUsedInCondition in unresolvedCondition
                        .Apply(ParameterReferenceCollector.Instance, null))
                    {
                        moduleModelFlags[parameterUsedInCondition] |= ParameterUsageFlags.UsedInResourceCondition;
                    }
                }

                foreach (var parameterUsedInIdentifier in armResource.FullyQualifiedResourceId
                    .Apply(ParameterReferenceCollector.Instance, null))
                {
                    moduleModelFlags[parameterUsedInIdentifier] |= ParameterUsageFlags.UsedInResourceIdentifier;
                }

                foreach (var parameterUsedInApiVersion in armResource.ApiVersion
                    .Apply(ParameterReferenceCollector.Instance, null))
                {
                    moduleModelFlags[parameterUsedInApiVersion] |= ParameterUsageFlags.UsedInResourceApiVersion;
                }
            }

            foreach (var extensibilityResource in extensibilityResources)
            {
                // Extensibility resources have already been converted back to JTokens, so we need to reparse
                // in order to perform further analysis.
                if (extensibilityResource.Context?.UnresolvedCondition is { } unresolvedCondition)
                {
                    foreach (var parameterUsedInCondition in ExpressionParser.ParseLanguageExpression(unresolvedCondition)
                        .Apply(ParameterReferenceCollector.Instance, null))
                    {
                        moduleModelFlags[parameterUsedInCondition] = ParameterUsageFlags.UsedInResourceCondition;
                    }
                }

                if (extensibilityResource.ApiVersion is { } apiVersion)
                {
                    foreach (var parameterUsedInApiVersion in ExpressionParser.ParseLanguageExpression(apiVersion)
                        .Apply(ParameterReferenceCollector.Instance, null))
                    {
                        moduleModelFlags[parameterUsedInApiVersion] = ParameterUsageFlags.UsedInResourceCondition;
                    }
                }

                // Because extensibility resource identifiers are discovered during what-if by calling the
                // provider, skip analysis of identifiers here.
            }

            return moduleModelFlags;
        }

        private static (
            ImmutableArray<DeploymentResourceWithParsedExpressions> armResources,
            ImmutableArray<ExtensibleResource> extensibilityResources
        ) GetModuleTemplateAndNestedResources(
            Template template,
            ResourceScope targetScope,
            IReadOnlyDictionary<string, ITemplateLanguageExpression> parameters)
        {
            try
            {
                var (_, preflightResources, extensibleResources) = TemplateEngine.ExpandNestedDeploymentsSync(
                    deploymentApiVersion: EmitConstants.NestedDeploymentResourceApiVersion,
                    deploymentScope: EnumConverter.ToTemplateDeploymentScope(targetScope)
                        ?? TemplateDeploymentScope.NotSpecified,
                    template: template,
                    parameters: parameters);

                return (preflightResources, extensibleResources);
            }
            catch (Exception ex)
            {
                // TODO: Raise diagnostic when the template evaluation fails if the diagnostic will provide extra context to the failure
                // Adding a diagnostic here without checking for other errors would currently result in duplicate errors for any error in module params
                Trace.WriteLine($"Exception occurred while reducing template language expressions: {ex}");
                return (ImmutableArray<DeploymentResourceWithParsedExpressions>.Empty, ImmutableArray<ExtensibleResource>.Empty);
            }
        }

        private static IEnumerable<ParameterSymbol> GetParametersUsedInClosureOf(IBinder binder, Symbol? symbol)
            => symbol switch
            {
                ParameterSymbol parameter => [parameter],
                // We're using a recursive call to this function instead of just calling
                // IBinder.GetReferencedSymbolClosureFor because we don't want to pick up any symbols referenced
                // through a parameter's default value declaration
                VariableSymbol variable => binder.GetSymbolsReferencedInDeclarationOf(variable)
                    .SelectMany(s => GetParametersUsedInClosureOf(binder, s)),
                _ => [],
            };

        private class ParameterReferenceCollector : IExpressionMappingVisitor<IEnumerable<string>>
        {
            internal static readonly ParameterReferenceCollector Instance = new();

            public IEnumerable<string> MapArrayExpression(
                Azure.Deployments.Expression.Intermediate.ArrayExpression expression,
                ISet<ITemplateLanguageExpression>? expressionsToSkip)
            {
                if (expressionsToSkip?.Contains(expression) is true)
                {
                    return [];
                }

                return expression.Items.SelectMany(e => e.Apply(this, expressionsToSkip));
            }

            public IEnumerable<string> MapBooleanExpression(
                BooleanExpression expression,
                ISet<ITemplateLanguageExpression>? expressionsToSkip) => [];

            public IEnumerable<string> MapFloatExpression(
                FloatExpression expression,
                ISet<ITemplateLanguageExpression>? expressionsToSkip) => [];

            public IEnumerable<string> MapFunctionExpression(
                FunctionExpression expression,
                ISet<ITemplateLanguageExpression>? expressionsToSkip)
            {
                if (expressionsToSkip?.Contains(expression) is true)
                {
                    return [];
                }

                if (expression.Name.Equals(SentinelValueFunctionName, StringComparison.OrdinalIgnoreCase))
                {
                    return [EvaluationHelpers.GetSingleStringArgument(expression.Name, expression.Arguments, expression)];
                }

                return expression.Arguments.SelectMany(arg => arg.Apply(this, expressionsToSkip))
                    .Concat(expression.Properties.SelectMany(prop => prop.Apply(this, expressionsToSkip)));
            }

            public IEnumerable<string> MapIntegerExpression(
                IntegerExpression expression,
                ISet<ITemplateLanguageExpression>? expressionsToSkip) => [];

            public IEnumerable<string> MapInvalidLanguageExpression(
                InvalidLanguageExpression expression,
                ISet<ITemplateLanguageExpression>? expressionsToSkip) => [];

            public IEnumerable<string> MapNullExpression(
                NullExpression expression,
                ISet<ITemplateLanguageExpression>? expressionsToSkip) => [];

            public IEnumerable<string> MapObjectExpression(
                Azure.Deployments.Expression.Intermediate.ObjectExpression expression,
                ISet<ITemplateLanguageExpression>? expressionsToSkip)
            {
                if (expressionsToSkip?.Contains(expression) is true)
                {
                    return [];
                }

                return expression.Properties.SelectMany(
                    kvp => kvp.Key.Apply(this, expressionsToSkip).Concat(kvp.Value.Apply(this, expressionsToSkip)));
            }

            public IEnumerable<string> MapStringExpression(
                StringExpression expression,
                ISet<ITemplateLanguageExpression>? expressionsToSkip) => [];

            public IEnumerable<string> MapUnevaluableExpression(
                UnevaluableExpression expression,
                ISet<ITemplateLanguageExpression>? expressionsToSkip) => [];
        }
    }
}
