// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.DataFlow;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Intermediate;
using Bicep.Core.Navigation;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Types;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit
{
    public static class EmitLimitationCalculator
    {
        public static EmitLimitationInfo Calculate(SemanticModel model)
        {
            var diagnostics = ToListDiagnosticWriter.Create();

            var moduleScopeData = ScopeHelper.GetModuleScopeInfo(model, diagnostics);
            var resourceScopeData = ScopeHelper.GetResourceScopeInfo(model, diagnostics);
            var resourceTypeResolver = ResourceTypeResolver.Create(model);

            NestedRuntimeMemberAccessValidator.Validate(model, resourceTypeResolver, diagnostics);
            DeployTimeConstantValidator.Validate(model, resourceTypeResolver, diagnostics);
            ForSyntaxValidatorVisitor.Validate(model, diagnostics);
            FunctionPlacementValidatorVisitor.Validate(model, diagnostics);
            IntegerValidatorVisitor.Validate(model, diagnostics);
            ExtensionReferenceValidatorVisitor.Validate(model, diagnostics);

            DetectDuplicateNames(model, diagnostics, resourceScopeData, moduleScopeData);
            DetectIncorrectlyFormattedNames(model, diagnostics);
            DetectUnexpectedResourceLoopInvariantProperties(model, diagnostics);
            DetectUnexpectedModuleLoopInvariantProperties(model, diagnostics);
            DetectUnsupportedModulePropertyAssignments(model, diagnostics);
            DetectCopyVariableName(model, diagnostics);
            DetectInvalidValueForParentProperty(model, diagnostics);
            BlockLambdasOutsideFunctionArguments(model, diagnostics);
            BlockUnsupportedLambdaVariableUsage(model, diagnostics);
            BlockModuleOutputResourcePropertyAccess(model, diagnostics);
            BlockSafeDereferenceOfModuleOrResourceCollectionMember(model, diagnostics);
            BlockTestFrameworkWithoutExperimentalFeaure(model, diagnostics);
            BlockAssertsWithoutExperimentalFeatures(model, diagnostics);
            BlockNamesDistinguishedOnlyByCase(model, diagnostics);
            BlockResourceDerivedTypesThatDoNotDereferenceProperties(model, diagnostics);
            BlockSpreadInUnsupportedLocations(model, diagnostics);
            BlockSecureOutputsWithLocalDeploy(model, diagnostics);
            BlockSecureOutputAccessOnIndirectReference(model, diagnostics);
            BlockExtendsWithoutFeatureFlagEnabled(model, diagnostics);
            BlockExplicitDependenciesInOrOnInlinedExistingResources(model, resourceTypeResolver, diagnostics);
            ValidateUsingWithClauseMatchesExperimentalFeatureEnablement(model, diagnostics);
            BlockMultilineStringInterpolationWithoutFeatureFlagEnabled(model, diagnostics);

            var paramAssignmentEvaluator = new ParameterAssignmentEvaluator(model);
            var (paramAssignments, usingConfig) = CalculateParameterAssignments(model, paramAssignmentEvaluator, diagnostics);
            var extConfigAssignments = CalculateExtensionConfigAssignments(model, paramAssignmentEvaluator, diagnostics);

            return new(diagnostics.GetDiagnostics(), paramAssignments, extConfigAssignments, usingConfig);
        }

        private static void DetectDuplicateNames(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter, ImmutableDictionary<DeclaredResourceMetadata, ScopeHelper.ScopeData> resourceScopeData, ImmutableDictionary<ModuleSymbol, ScopeHelper.ScopeData> moduleScopeData)
        {
            // TODO generalize or move into Az extension

            // This method only checks, if in one deployment we do not have 2 or more resources with this same name in one deployment to avoid template validation error
            // This will not check resource constraints such as necessity of having unique virtual network names within resource group
            var duplicateResources = semanticModel.DeclaredResources
                .GroupBy(x => x, new DeclaredResourceIdComparer(semanticModel, resourceScopeData))
                .Where(group => group.Count() > 1);

            foreach (var duplicatedResourceGroup in duplicateResources)
            {
                foreach (var duplicatedResource in duplicatedResourceGroup)
                {
                    diagnosticWriter.Write(duplicatedResource.NameSyntax, x => x.ResourceMultipleDeclarations(duplicatedResourceGroup.Select(r => r.Symbol.Name)));
                }
            }

            var duplicateModules = semanticModel.Root.ModuleDeclarations
                .GroupBy(x => x, new ModuleIdComparer(semanticModel, moduleScopeData))
                .Where(group => group.Count() > 1);

            foreach (var duplicatedModuleGroup in duplicateModules)
            {
                foreach (var duplicatedModule in duplicatedModuleGroup)
                {
                    diagnosticWriter.Write(
                        duplicatedModule.TryGetBodyPropertyValue(LanguageConstants.ModuleNamePropertyName) ?? duplicatedModule.DeclaringModule.Name,
                        x => x.ModuleMultipleDeclarations(duplicatedModuleGroup.Select(m => m.Name)));
                }
            }
        }

        private class DeclaredResourceIdComparer : IEqualityComparer<DeclaredResourceMetadata>
        {
            private readonly SemanticModel model;
            private readonly ImmutableDictionary<DeclaredResourceMetadata, ScopeHelper.ScopeData> resourceScopeData;
            private readonly Dictionary<DeclaredResourceMetadata, ImmutableArray<IArmIdSegment>> resourceNameSegments = new();

            internal DeclaredResourceIdComparer(
                SemanticModel model,
                ImmutableDictionary<DeclaredResourceMetadata, ScopeHelper.ScopeData> resourceScopeData)
            {
                this.model = model;
                this.resourceScopeData = resourceScopeData;
            }

            public bool Equals(DeclaredResourceMetadata? x, DeclaredResourceMetadata? y)
            {
                if (x is null && y is null)
                {
                    return true;
                }

                return
                    // extension resources do not have an ARM ID
                    x?.IsAzResource is true &&
                    y?.IsAzResource is true &&
                    // ARM resource ID uniqueness is only enforced on resources with a `true` condition
                    (x.Symbol.DeclaringResource.TryGetCondition() is not { } xCondition ||
                        model.GetTypeInfo(xCondition) is BooleanLiteralType { Value: true }) &&
                    (y.Symbol.DeclaringResource.TryGetCondition() is not { } yCondition ||
                        model.GetTypeInfo(yCondition) is BooleanLiteralType { Value: true }) &&
                    // To have the same ID, resource must have:
                    // the same type
                    LanguageConstants.ResourceTypeComparer.Equals(x.TypeReference.FormatType(), y.TypeReference.FormatType()) &&
                    // the same scope
                    resourceScopeData.TryGetValue(x, out var xScopeData) &&
                    resourceScopeData.TryGetValue(y, out var yScopeData) &&
                    xScopeData.Equals(yScopeData) &&
                    // and the same name
                    resourceNameSegments.GetOrAdd(x, NameSegmentsFor)
                        .SequenceEqual(resourceNameSegments.GetOrAdd(y, NameSegmentsFor));
            }

            public int GetHashCode([DisallowNull] DeclaredResourceMetadata obj)
            {
                var baseHash = obj.TypeReference.FormatType().GetHashCode();

                foreach (var nameSegment in resourceNameSegments.GetOrAdd(obj, NameSegmentsFor))
                {
                    baseHash = HashCode.Combine(baseHash, nameSegment);
                }

                return baseHash;
            }

            private ImmutableArray<IArmIdSegment> NameSegmentsFor(DeclaredResourceMetadata resource)
                =>
                [
                    .. model.ResourceAncestors.GetAncestors(resource)
                            .Reverse()
                            .SelectMany(r => r.IndexExpression switch
                            {
                                SyntaxBase idx when model.GetTypeInfo(idx) is IntegerLiteralType literalIndex
                                    => new[] { NameSegmentFor(r.Resource), new LiteralIdSegment(literalIndex.Value.ToString()) },
                                SyntaxBase idx => new[] { NameSegmentFor(r.Resource), new NonLiteralIdSegment(idx) },
                                _ => NameSegmentFor(r.Resource).AsEnumerable(),
                            }),
                    NameSegmentFor(resource),
                ];

            private IArmIdSegment NameSegmentFor(DeclaredResourceMetadata resource)
                => IArmIdSegment.For(resource.TryGetNameSyntax(), model) switch
                {
                    IArmIdSegment nonNull => nonNull,
                    _ => new NonLiteralIdSegment(resource.Symbol.DeclaringResource.Name),
                };
        }

        private interface IArmIdSegment
        {
            [return: NotNullIfNotNull(nameof(syntax))]
            static IArmIdSegment? For(SyntaxBase? syntax, SemanticModel model) => syntax switch
            {
                SyntaxBase nonNull when model.GetTypeInfo(nonNull) is StringLiteralType literalSegment
                    => new LiteralIdSegment(literalSegment.RawStringValue),
                SyntaxBase nonNull => new NonLiteralIdSegment(nonNull),
                _ => null,
            };
        }

        private record LiteralIdSegment(string Name) : IArmIdSegment;

        private record NonLiteralIdSegment(SyntaxBase NameSyntax) : IArmIdSegment;

        // Using a class instead of a record here so that reference equality will be used for missing segments
        private class MissingIdSegment() : IArmIdSegment { }

        private class ModuleIdComparer : IEqualityComparer<ModuleSymbol>
        {
            private readonly SemanticModel model;
            private readonly ImmutableDictionary<ModuleSymbol, ScopeHelper.ScopeData> moduleScopeData;
            private readonly Dictionary<ModuleSymbol, ModuleScopingSegments> moduleScopingSegments = new();

            public ModuleIdComparer(
                SemanticModel model,
                ImmutableDictionary<ModuleSymbol, ScopeHelper.ScopeData> moduleScopeData)
            {
                this.model = model;
                this.moduleScopeData = moduleScopeData;
            }

            public bool Equals(ModuleSymbol? x, ModuleSymbol? y)
            {
                if (x is null && y is null)
                {
                    return true;
                }

                // if a module declaration omits a name, it is either using the optional module names feature
                // (in which case its name will be unique) or already has an error-level diagnostic
                return x?.TryGetBodyPropertyValue(LanguageConstants.ModuleNamePropertyName) is { } xName &&
                    y?.TryGetBodyPropertyValue(LanguageConstants.ModuleNamePropertyName) is { } yName &&
                    // ARM resource ID uniqueness is only enforced on resources with a `true` condition
                    (x.DeclaringModule.TryGetCondition() is not { } xCondition ||
                        model.GetTypeInfo(xCondition) is BooleanLiteralType { Value: true }) &&
                    (y.DeclaringModule.TryGetCondition() is not { } yCondition ||
                        model.GetTypeInfo(yCondition) is BooleanLiteralType { Value: true }) &&
                    // To have the same ID, modules must have the same scope
                    moduleScopingSegments.GetOrAdd(x, ScopingSegmentsFor)
                        .Equals(moduleScopingSegments.GetOrAdd(y, ScopingSegmentsFor)) &&
                    // and the same name
                    IArmIdSegment.For(xName, model).Equals(IArmIdSegment.For(yName, model));
            }

            public int GetHashCode([DisallowNull] ModuleSymbol obj)
                => IArmIdSegment.For(obj.TryGetBodyPropertyValue(LanguageConstants.ModuleNamePropertyName), model)?.GetHashCode() ?? 0;

            private ModuleScopingSegments ScopingSegmentsFor(ModuleSymbol m)
            {
                if (!moduleScopeData.TryGetValue(m, out var scopeData))
                {
                    return new(new MissingIdSegment(), new MissingIdSegment(), new MissingIdSegment());
                }

                return new(
                    IArmIdSegment.For(scopeData.ManagementGroupNameProperty, model),
                    IArmIdSegment.For(scopeData.SubscriptionIdProperty, model),
                    IArmIdSegment.For(scopeData.ResourceGroupProperty, model));
            }

            private record ModuleScopingSegments(
                IArmIdSegment? ManagementGroupName,
                IArmIdSegment? SubscriptionId,
                IArmIdSegment? ResourceGroupName);
        }

        private static void DetectIncorrectlyFormattedNames(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
        {
            // TODO move into Az extension
            foreach (var resource in semanticModel.DeclaredResources)
            {
                if (!resource.IsAzResource)
                {
                    continue;
                }

                if (resource.TryGetNameSyntax() is not { } resourceName ||
                    resourceName is not StringSyntax resourceNameString)
                {
                    // the resource doesn't have a name set, or it's not a string and thus difficult to analyze
                    continue;
                }

                var ancestors = semanticModel.ResourceAncestors.GetAncestors(resource);
                if (ancestors.Any())
                {
                    // try to detect cases where someone has applied top-level resource declaration naming to a nested/parent resource
                    // e.g. '{parent.name}/child' or 'parent/child'
                    if (resourceNameString.SegmentValues.Any(v => v.Contains('/')))
                    {
                        diagnosticWriter.Write(resourceNameString, x => x.ChildResourceNameContainsQualifiers());
                    }
                }
                else
                {
                    var slashCount = resourceNameString.SegmentValues.Sum(x => x.Count(y => y == '/'));

                    // The number of name segments should be (number of type segments) - 1, because type segments includes the provider name.
                    // The number of name slashes should be (number of name segments) - 1, because  the slash is used to separate segments (e.g. "nameA/nameB/nameC")
                    // This is how we get to (number of type segments) - 2.
                    var expectedSlashCount = resource.TypeReference.TypeSegments.Length - 2;

                    // Try to detect cases where someone has applied nested/parent resource declaration naming to a top-level resource - e.g. 'child'.
                    if (resourceNameString.IsInterpolated())
                    {
                        // This is best-effort for interpolated strings, as variables may pull in additional '/' characters.
                        // So we can only accurately show a diagnostic if there are TOO MANY '/' characters.
                        if (slashCount > expectedSlashCount)
                        {
                            diagnosticWriter.Write(resourceNameString, x => x.TopLevelChildResourceNameIncorrectQualifierCount(expectedSlashCount));
                        }
                    }
                    else
                    {
                        // We know exactly how many '/' characters must be present, because we have a string literal. So expect an exact match.
                        if (slashCount != expectedSlashCount)
                        {
                            diagnosticWriter.Write(resourceNameString, x => x.TopLevelChildResourceNameIncorrectQualifierCount(expectedSlashCount));
                        }
                    }
                }
            }
        }

        private static void DetectUnexpectedResourceLoopInvariantProperties(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
        {
            foreach (var resource in semanticModel.DeclaredResources)
            {
                if (resource.IsExistingResource)
                {
                    // existing resource syntax doesn't result in deployment but instead is
                    // used as a convenient way of constructing a symbolic name
                    // as such, invariant names aren't really a concern here
                    // (and may even be desirable)
                    continue;
                }

                if (resource.Symbol.DeclaringResource.Value is not ForSyntax @for || @for.ItemVariable is not { } itemVariable)
                {
                    // invariant identifiers are only a concern for resource loops
                    // this is not a resource loop OR the item variable is malformed
                    continue;
                }

                if (resource.Symbol.TryGetBodyObjectType() is not { } bodyType)
                {
                    // unable to get the object type
                    continue;
                }

                // collect the values of the expected variant properties
                // provided that they exist on the type
                var expectedVariantPropertiesForType = bodyType.Properties.Values
                    .Where(property => property.Flags.HasFlag(TypePropertyFlags.LoopVariant))
                    .OrderBy(property => property.Name, LanguageConstants.IdentifierComparer);

                var propertyMap = expectedVariantPropertiesForType
                    .Select(property => (property, value: resource.Symbol.TryGetBodyPropertyValue(property.Name)))
                    // exclude missing or malformed property values
                    .Where(pair => pair.value is not null and not SkippedTriviaSyntax)
                    .ToImmutableDictionary(pair => pair.property, pair => pair.value!);

                if (!propertyMap.Any(pair => pair.Key.Flags.HasFlag(TypePropertyFlags.Required)))
                {
                    // required loop-variant properties have not been set yet
                    // do not overwarn the user because they have other errors to deal with
                    continue;
                }

                var indexVariable = @for.IndexVariable;
                if (propertyMap.All(pair => IsInvariant(semanticModel, itemVariable, indexVariable, pair.Value)))
                {
                    diagnosticWriter.Write(DiagnosticBuilder.ForPosition(resource.Symbol.NameSource).ForExpressionContainsLoopInvariants(itemVariable.Name.IdentifierName, indexVariable?.Name.IdentifierName, expectedVariantPropertiesForType.Select(p => p.Name)));
                }
            }
        }

        private static void DetectUnexpectedModuleLoopInvariantProperties(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
        {
            foreach (var module in semanticModel.Root.ModuleDeclarations)
            {
                if (module.DeclaringModule.Value is not ForSyntax @for || @for.ItemVariable is not { } itemVariable)
                {
                    // invariant identifiers are only a concern for module loops
                    // this is not a module loop OR the item variable is malformed
                    continue;
                }

                if (module.TryGetBodyObjectType() is not { } bodyType)
                {
                    // unable to get the object type
                    continue;
                }

                // collect the values of the expected variant properties
                // provided that they exist on the type
                var expectedVariantPropertiesForType = bodyType.Properties.Values
                    .Where(property => property.Flags.HasFlag(TypePropertyFlags.LoopVariant))
                    .OrderBy(property => property.Name, LanguageConstants.IdentifierComparer);

                var propertyMap = expectedVariantPropertiesForType
                    .Select(property => (property, value: module.TryGetBodyPropertyValue(property.Name)))
                    // exclude missing or malformed property values
                    .Where(pair => pair.value is not null && pair.value is not SkippedTriviaSyntax)
                    .ToImmutableDictionary(pair => pair.property, pair => pair.value!);

                if (propertyMap.Keys.FirstOrDefault(x => x.Name.Equals(LanguageConstants.ModuleNamePropertyName)) is not { } moduleNameProperty)
                {
                    // The module name is generated and implictly uses the loop variable (copyIndex()).
                    continue;
                }

                if (!propertyMap.Any(pair => pair.Key.Flags.HasFlag(TypePropertyFlags.Required)) && moduleNameProperty is null)
                {
                    // required loop-variant properties have not been set yet
                    // do not overwarn the user because they have other errors to deal with
                    continue;
                }

                var indexVariable = @for.IndexVariable;
                if (propertyMap.All(pair => IsInvariant(semanticModel, itemVariable, indexVariable, pair.Value)))
                {
                    // all the expected variant properties are loop invariant
                    diagnosticWriter.Write(DiagnosticBuilder.ForPosition(module.NameSource).ForExpressionContainsLoopInvariants(itemVariable.Name.IdentifierName, indexVariable?.Name.IdentifierName, expectedVariantPropertiesForType.Select(p => p.Name)));
                }
            }
        }

        private static void DetectUnsupportedModulePropertyAssignments(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
        {
            foreach (var moduleSymbol in semanticModel.Root.ModuleDeclarations)
            {
                if (moduleSymbol.DeclaringModule.TryGetBody() is not ObjectSyntax body)
                {
                    // skip modules with malformed bodies
                    continue;
                }

                var paramsValue = body.TryGetPropertyByName(LanguageConstants.ModuleParamsPropertyName)?.Value;

                if (paramsValue is not (null or ObjectSyntax or SkippedTriviaSyntax)) // we have params, it's not an object literal and not bad syntax...
                {
                    // unexpected type is assigned as the value of the "params" property
                    // we can't emit that directly because the parameters have to be converted into an object whose property values are objects with a "value" property
                    // ideally we would add a runtime function to take care of the conversion in these cases, but it doesn't exist yet
                    diagnosticWriter.Write(DiagnosticBuilder.ForPosition(paramsValue).PropertyRequiresObjectLiteral(LanguageConstants.ModuleParamsPropertyName));
                }

                ModuleExtensionConfigsLimitations.Validate(body, semanticModel, diagnosticWriter);
            }
        }

        private static void DetectCopyVariableName(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
        {
            var copyVariableSymbol = semanticModel.Root.VariableDeclarations.FirstOrDefault(x => x.Name.Equals(LanguageConstants.CopyLoopIdentifier, StringComparison.OrdinalIgnoreCase));
            if (copyVariableSymbol is not null)
            {
                diagnosticWriter.Write(DiagnosticBuilder.ForPosition(copyVariableSymbol.NameSource).ReservedIdentifier(LanguageConstants.CopyLoopIdentifier));
            }
        }

        public static void DetectInvalidValueForParentProperty(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
        {
            foreach (var resourceDeclarationSymbol in semanticModel.Root.ResourceDeclarations)
            {
                if (resourceDeclarationSymbol.TryGetBodyPropertyValue(LanguageConstants.ResourceParentPropertyName) is { } referenceParentSyntax)
                {
                    var (baseSyntax, _) = SyntaxHelper.UnwrapArrayAccessSyntax(referenceParentSyntax);

                    if (semanticModel.ResourceMetadata.TryLookup(baseSyntax) is not { } && !semanticModel.GetTypeInfo(baseSyntax).IsError())
                    {
                        // we throw an error diagnostic when the parent property contains a value that cannot be computed or does not directly reference another resource.
                        // this includes ternary operator expressions, which Bicep does not support
                        diagnosticWriter.Write(referenceParentSyntax, x => x.InvalidValueForParentProperty());
                    }
                }
            }
        }

        private static void BlockLambdasOutsideFunctionArguments(SemanticModel model, IDiagnosticWriter diagnostics)
        {
            foreach (var lambda in SyntaxAggregator.AggregateByType<LambdaSyntax>(model.Root.Syntax))
            {
                foreach (var ancestor in model.Binder.EnumerateAncestorsUpwards(lambda))
                {
                    if (ancestor is FunctionArgumentSyntax)
                    {
                        // we're inside a function argument - all good
                        break;
                    }

                    if (ancestor is ParenthesizedExpressionSyntax)
                    {
                        // we've got a parenthesized expression - keep searching upwards
                        continue;
                    }

                    // lambdas are not valid inside any other syntax - raise an error and exit
                    diagnostics.Write(lambda, x => x.LambdaFunctionsOnlyValidInFunctionArguments());
                    break;
                }
            }
        }

        private static void BlockUnsupportedLambdaVariableUsage(SemanticModel model, IDiagnosticWriter diagnostics)
        {
            IEnumerable<LocalVariableSymbol> CollectLambdaVariables(SyntaxBase syntax)
            {
                return SyntaxAggregator.AggregateByType<VariableAccessSyntax>(syntax)
                    .Select(v => model.Binder.GetSymbolInfo(v))
                    .OfType<LocalVariableSymbol>()
                    .Distinct()
                    .Where(symbol => symbol.LocalKind == LocalKind.LambdaItemVariable);
            }

            var visited = new HashSet<SyntaxBase>();
            foreach (var lambda in SyntaxAggregator.AggregateByType<LambdaSyntax>(model.Root.Syntax))
            {
                foreach (var functionCall in SyntaxAggregator.AggregateByType<FunctionCallSyntaxBase>(lambda.Body))
                {
                    // Block the usage of lambdas inside reference() or list*() functions.
                    // The Deployment Engine needs to be able to process these upfront to build the deployment graph, so they cannot contain unevaluated lambda variables.
                    if (!visited.Contains(functionCall) &&
                        model.GetSymbolInfo(functionCall) is FunctionSymbol functionSymbol &&
                        functionSymbol.FunctionFlags.HasFlag(FunctionFlags.RequiresInlining))
                    {
                        var blockedSymbols = functionCall.Arguments.SelectMany(x => CollectLambdaVariables(x.Expression)).Distinct()
                            .Select(s => s.Name).ToImmutableArray();

                        if (blockedSymbols.Any())
                        {
                            diagnostics.Write(functionCall, x => x.LambdaVariablesInInlineFunctionUnsupported(functionCall.Name.IdentifierName, blockedSymbols));
                        }
                    }

                    visited.Add(functionCall);
                }

                foreach (var arrayAccess in SyntaxAggregator.AggregateByType<ArrayAccessSyntax>(lambda.Body))
                {
                    // Block the usage of lambdas to index into arrays of resources, as this may result in the generation of a reference() or list*() function call.
                    // The Deployment Engine needs to be able to process these upfront to build the deployment graph, so they cannot contain unevaluated lambda variables.
                    if (!visited.Contains(arrayAccess) &&
                        model.GetSymbolInfo(arrayAccess.BaseExpression) is ModuleSymbol or ResourceSymbol)
                    {
                        var blockedSymbols = CollectLambdaVariables(arrayAccess.IndexExpression)
                            .Select(s => s.Name).ToImmutableArray();

                        if (blockedSymbols.Any())
                        {
                            diagnostics.Write(arrayAccess.IndexExpression, x => x.LambdaVariablesInResourceOrModuleArrayAccessUnsupported(blockedSymbols));
                        }
                    }

                    visited.Add(arrayAccess);
                }
            }
        }

        private static bool IsInvariant(SemanticModel semanticModel, LocalVariableSyntax itemVariable, LocalVariableSyntax? indexVariable, SyntaxBase expression)
        {
            var referencedLocals = LocalSymbolDependencyVisitor.GetLocalSymbolDependencies(semanticModel, expression);

            bool IsLocalInvariant(LocalVariableSyntax? local) =>
                local is { } &&
                semanticModel.GetSymbolInfo(local) is LocalVariableSymbol localSymbol &&
                !referencedLocals.Contains(localSymbol);

            return indexVariable is null
                ? IsLocalInvariant(itemVariable)
                : IsLocalInvariant(itemVariable) && IsLocalInvariant(indexVariable);
        }

        private static void BlockModuleOutputResourcePropertyAccess(SemanticModel model, IDiagnosticWriter diagnostics) =>
            diagnostics.WriteMultiple(
                SyntaxAggregator.Aggregate(model.Root.Syntax, syntax => IsModuleOutputResourceRuntimePropertyAccess(model, syntax) || IsModuleOutputResourceListFunction(model, syntax))
                    .Select(syntaxToBlock => DiagnosticBuilder.ForPosition(syntaxToBlock).ModuleOutputResourcePropertyAccessDetected()));

        private static bool IsModuleOutputResourceRuntimePropertyAccess(SemanticModel model, SyntaxBase syntax)
        {
            if (syntax is PropertyAccessSyntax propertyAccess &&
                model.ResourceMetadata.TryLookup(propertyAccess.BaseExpression) is ModuleOutputResourceMetadata &&
                !AzResourceTypeProvider.ReadWriteDeployTimeConstantPropertyNames.Contains(propertyAccess.PropertyName.IdentifierName))
            {
                return true;
            }

            return false;
        }

        private static bool IsModuleOutputResourceListFunction(SemanticModel model, SyntaxBase syntax)
            => syntax is InstanceFunctionCallSyntax instanceFunctionCall &&
                model.ResourceMetadata.TryLookup(instanceFunctionCall.BaseExpression) is ModuleOutputResourceMetadata &&
                !LanguageConstants.IdentifierComparer.Equals(instanceFunctionCall.Name.IdentifierName, AzResourceTypeProvider.GetSecretFunctionName);

        private static void BlockSafeDereferenceOfModuleOrResourceCollectionMember(SemanticModel model, IDiagnosticWriter diagnostics) =>
            diagnostics.WriteMultiple(SyntaxAggregator.AggregateByType<ArrayAccessSyntax>(model.Root.Syntax)
                .Select(arrayAccess => arrayAccess.IsSafeAccess
                    ? model.GetSymbolInfo(arrayAccess.BaseExpression) switch
                    {
                        ModuleSymbol module when module.IsCollection => arrayAccess.SafeAccessMarker,
                        ResourceSymbol resource when resource.IsCollection => arrayAccess.SafeAccessMarker,
                        _ => null,
                    }
                    : null)
                .WhereNotNull()
                .Select(forbiddenSafeAccessMarker => DiagnosticBuilder.ForPosition(forbiddenSafeAccessMarker).SafeDereferenceNotPermittedOnResourceCollections()));

        private static (ImmutableDictionary<ParameterAssignmentSymbol, ParameterAssignmentValue> paramAssignments, ParameterAssignmentValue? usingConfig) CalculateParameterAssignments(
            SemanticModel model,
            ParameterAssignmentEvaluator evaluator,
            IDiagnosticWriter diagnostics)
        {
            if (model.HasParsingErrors())
            {
                return ([], null);
            }

            var referencesInValues = model.Binder.Bindings.Values.OfType<DeclaredSymbol>().Distinct()
                .ToImmutableDictionary(p => p, p => SymbolicReferenceCollector.CollectSymbolsReferenced(model.Binder, p.DeclaringSyntax));
            var generated = ImmutableDictionary.CreateBuilder<ParameterAssignmentSymbol, ParameterAssignmentValue>();

            var extendsDeclarations = model.SourceFile.ProgramSyntax.Declarations.OfType<ExtendsDeclarationSyntax>();

            foreach (var extendsDeclaration in extendsDeclarations)
            {
                var result = extendsDeclaration.TryGetReferencedModel(model.SourceFileGrouping, model.ModelLookup, b => b.ExtendsPathHasNotBeenSpecified());

                if (result.IsSuccess(out var extendedModel, out var failure))
                {
                    if (extendedModel is not SemanticModel extendedSemanticModel)
                    {
                        throw new UnreachableException("We have already verified this is a .bicepparam file");
                    }
                    generated.AddRange(extendedSemanticModel.EmitLimitationInfo.ParameterAssignments);
                }
                else
                {
                    diagnostics.Write(failure);
                }
            }

            HashSet<Symbol> erroredSymbols = new();

            foreach (var symbol in GetTopologicallySortedSymbols(referencesInValues))
            {
                if (symbol.Type is ErrorType)
                {
                    // no point evaluating if we're already reporting an error
                    erroredSymbols.Add(symbol);
                    continue;
                }

                var referencedValueHasError = false;
                foreach (var referenced in referencesInValues[symbol])
                {
                    if (erroredSymbols.Contains(referenced.Key))
                    {
                        referencedValueHasError = true;
                    }
                    else if (referenced.Key is ParameterAssignmentSymbol parameterAssignment)
                    {
                        if (generated[parameterAssignment].KeyVaultReferenceExpression is not null)
                        {
                            diagnostics.WriteMultiple(referenced.Value.Select(syntax => DiagnosticBuilder.ForPosition(syntax).ParameterReferencesKeyVaultSuppliedParameter(parameterAssignment.Name)));
                            referencedValueHasError = true;
                        }

                        if (generated[parameterAssignment].Value is JToken evaluated && evaluated.Type == JTokenType.Null)
                        {
                            diagnostics.WriteMultiple(referenced.Value.Select(syntax => DiagnosticBuilder.ForPosition(syntax).ParameterReferencesDefaultedParameter(parameterAssignment.Name)));
                            referencedValueHasError = true;
                        }
                    }
                }

                if (referencedValueHasError)
                {
                    erroredSymbols.Add(symbol);
                    continue;
                }

                if (symbol is not ParameterAssignmentSymbol parameter)
                {
                    continue;
                }
                // We may emit duplicate errors here - type checking will also execute some ARM functions and generate errors
                // This is something we should improve before the first release.
                var result = evaluator.EvaluateParameter(parameter);
                if (result.Diagnostic is { })
                {
                    diagnostics.Write(result.Diagnostic);
                }
                if (result.Value is not null || result.Expression is not null || result.KeyVaultReference is not null)
                {
                    generated[parameter] = new(result.Value, result.Expression, result.KeyVaultReference);
                }
            }

            ParameterAssignmentValue? usingConfig = null;
            if (evaluator.EvaluateUsingConfig(model.Root) is { } usingConfigResult)
            {
                usingConfig = new(usingConfigResult.Value, usingConfigResult.Expression, usingConfigResult.KeyVaultReference);
            }

            return (generated.ToImmutableDictionary(), usingConfig);
        }

        private static ImmutableDictionary<ExtensionConfigAssignmentSymbol, ImmutableDictionary<string, ExtensionConfigAssignmentValue>> CalculateExtensionConfigAssignments(
            SemanticModel model,
            ParameterAssignmentEvaluator evaluator,
            IDiagnosticWriter diagnostics)
        {
            if (model.Root.ExtensionConfigAssignments.IsEmpty)
            {
                return [];
            }

            var generated = ImmutableDictionary.CreateBuilder<ExtensionConfigAssignmentSymbol, ImmutableDictionary<string, ExtensionConfigAssignmentValue>>();

            var extendsDeclarations = model.SourceFile.ProgramSyntax.Declarations.OfType<ExtendsDeclarationSyntax>();

            foreach (var extendsDeclaration in extendsDeclarations)
            {
                var result = extendsDeclaration.TryGetReferencedModel(model.SourceFileGrouping, model.ModelLookup, b => b.ExtendsPathHasNotBeenSpecified());

                if (result.IsSuccess(out var extendedModel, out var failure))
                {
                    if (extendedModel is not SemanticModel extendedSemanticModel)
                    {
                        throw new UnreachableException("We have already verified this is a .bicepparam file");
                    }

                    generated.AddRange(extendedSemanticModel.EmitLimitationInfo.ExtensionConfigAssignments);
                }
                else
                {
                    diagnostics.Write(failure);
                }
            }

            var extensionConfigAssignmentSymbols = model.Binder.Bindings.Values.OfType<ExtensionConfigAssignmentSymbol>();

            foreach (var extConfigAssignment in extensionConfigAssignmentSymbols)
            {
                var assignmentProperties = ImmutableDictionary.CreateBuilder<string, ExtensionConfigAssignmentValue>();

                foreach (var (propertyName, result) in evaluator.EvaluateExtensionConfigAssignment(extConfigAssignment))
                {
                    if (result.Diagnostic is { })
                    {
                        diagnostics.Write(result.Diagnostic);
                    }

                    if (result.Value is not null || result.KeyVaultReference is not null)
                    {
                        assignmentProperties.Add(propertyName, new(result.Value, result.KeyVaultReference));
                    }
                }

                generated[extConfigAssignment] = assignmentProperties.ToImmutableDictionary();
            }

            return generated.ToImmutableDictionary();
        }

        private static IEnumerable<DeclaredSymbol> GetTopologicallySortedSymbols(ImmutableDictionary<DeclaredSymbol, ImmutableDictionary<DeclaredSymbol, ImmutableSortedSet<SyntaxBase>>> referencesInValues)
        {
            HashSet<DeclaredSymbol> processed = new();
            IEnumerable<DeclaredSymbol> YieldSymbolAndUnprocessedPredecessors(DeclaredSymbol n)
            {
                if (processed.Contains(n))
                {
                    yield break;
                }
                processed.Add(n);

                foreach (var predecessor in referencesInValues[n].Keys.SelectMany(YieldSymbolAndUnprocessedPredecessors))
                {
                    yield return predecessor;
                }

                yield return n;
            }

            return referencesInValues.Keys.SelectMany(YieldSymbolAndUnprocessedPredecessors);
        }

        private static void BlockTestFrameworkWithoutExperimentalFeaure(SemanticModel model, IDiagnosticWriter diagnostics)
        {
            foreach (var test in model.Root.TestDeclarations)
            {
                if (!model.Features.TestFrameworkEnabled)
                {
                    diagnostics.Write(test.DeclaringTest, x => x.TestDeclarationStatementsUnsupported());
                }
            }
        }

        private static void BlockExtendsWithoutFeatureFlagEnabled(SemanticModel model, IDiagnosticWriter diagnostics)
        {
            foreach (var extendsDeclaration in model.SourceFile.ProgramSyntax.Declarations.OfType<ExtendsDeclarationSyntax>())
            {
                if (!model.Features.ExtendableParamFilesEnabled)
                {
                    diagnostics.Write(extendsDeclaration, x => x.ExtendsNotSupported());
                }
            }
        }

        private static void ValidateUsingWithClauseMatchesExperimentalFeatureEnablement(SemanticModel model, IDiagnosticWriter diagnostics)
        {
            foreach (var syntax in model.SourceFile.ProgramSyntax.Declarations.OfType<UsingDeclarationSyntax>())
            {
                if (syntax.WithClause is not SkippedTriviaSyntax && !model.Features.DeployCommandsEnabled)
                {
                    diagnostics.Write(syntax.WithClause, x => x.UsingWithClauseRequiresExperimentalFeature());
                }

                if (syntax.WithClause is SkippedTriviaSyntax && model.Features.DeployCommandsEnabled)
                {
                    diagnostics.Write(syntax, x => x.UsingWithClauseRequiredIfExperimentalFeatureEnabled());
                }
            }
        }

        private static void BlockAssertsWithoutExperimentalFeatures(SemanticModel model, IDiagnosticWriter diagnostics)
        {
            foreach (var assert in model.Root.AssertDeclarations)
            {
                if (!model.Features.AssertsEnabled)
                {
                    diagnostics.Write(assert.DeclaringAssert, x => x.AssertsUnsupported());
                }
            }
            foreach (var resourceDeclarationSymbol in model.Root.ResourceDeclarations)
            {

                if (resourceDeclarationSymbol.TryGetBodyProperty(LanguageConstants.ResourceAssertPropertyName)?.Value is SyntaxBase value && !model.Features.AssertsEnabled)
                {
                    diagnostics.Write(value, x => x.AssertsUnsupported());
                }
            }
        }

        private static void BlockNamesDistinguishedOnlyByCase(SemanticModel model, IDiagnosticWriter diagnostics)
        {
            foreach (var (symbolTypePluralName, symbolsOfType) in new (string, IEnumerable<DeclaredSymbol>)[]
            {
                ("parameters", model.Root.ParameterDeclarations),
                ("variables", model.Root.VariableDeclarations.Concat<DeclaredSymbol>(model.Root.ImportedVariables)),
                ("outputs", model.Root.OutputDeclarations),
                ("types", model.Root.TypeDeclarations.Concat<DeclaredSymbol>(model.Root.ImportedTypes)),
                ("asserts", model.Root.AssertDeclarations),
                ("functions", model.Root.FunctionDeclarations.Concat<DeclaredSymbol>(model.Root.ImportedFunctions))
            })
            {
                BlockCaseInsensitiveNameClashes(symbolTypePluralName, symbolsOfType, s => s.Name, s => s.NameSource, diagnostics);
            }

            foreach (var objectTypeDeclaration in SyntaxAggregator.AggregateByType<ObjectTypeSyntax>(model.SourceFile.ProgramSyntax))
            {
                BlockCaseInsensitiveNameClashes("type properties",
                    objectTypeDeclaration.Properties.SelectMany(p => p.TryGetKeyText() is string key ? (key, p.Key).AsEnumerable() : Enumerable.Empty<(string, SyntaxBase)>()),
                    t => t.Item1,
                    t => t.Item2,
                    diagnostics);
            }
        }

        private static void BlockCaseInsensitiveNameClashes<T>(string itemTypePluralName, IEnumerable<T> itemsOfType, Func<T, string> nameExtractor, Func<T, IPositionable> nameSyntaxExtractor, IDiagnosticWriter diagnostics)
        {
            foreach (var grouping in itemsOfType.ToLookup(nameExtractor, StringComparer.OrdinalIgnoreCase).Where(g => g.Count() > 1))
            {
                var clashingNames = grouping.Select(nameExtractor).ToArray();

                // if any symbols are exact matches, a different diagnostic about multiple declarations will have already been raised
                if (clashingNames.Distinct().Count() != clashingNames.Length)
                {
                    continue;
                }

                diagnostics.WriteMultiple(grouping.Select(
                    item => DiagnosticBuilder.ForPosition(nameSyntaxExtractor(item)).ItemsMustBeCaseInsensitivelyUnique(itemTypePluralName, clashingNames)));
            }
        }

        private static void BlockResourceDerivedTypesThatDoNotDereferenceProperties(SemanticModel model, IDiagnosticWriter diagnostics)
        {
            static bool IsPermittedResourceDerivedTypeParent(IBinder binder, SyntaxBase? syntax) => syntax switch
            {
                ParenthesizedExpressionSyntax or
                NonNullAssertionSyntax or
                NullableTypeSyntax => IsPermittedResourceDerivedTypeParent(binder, binder.GetParent(syntax)),
                TypePropertyAccessSyntax or
                TypeItemsAccessSyntax or
                TypeAdditionalPropertiesAccessSyntax or
                TypeArrayAccessSyntax => true,
                _ => false,
            };

            diagnostics.WriteMultiple(SyntaxAggregator.AggregateByType<ParameterizedTypeInstantiationSyntaxBase>(model.Root.Syntax)
                .Where(typeInstantiation => model.TypeManager.TryGetReifiedType(typeInstantiation) is ResourceDerivedTypeExpression &&
                    !IsPermittedResourceDerivedTypeParent(model.Binder, model.Binder.GetParent(typeInstantiation)))
                .Select(typeInstantiaion => DiagnosticBuilder.ForPosition(typeInstantiaion).CannotUseEntireResourceBodyAsType()));
        }

        private static void BlockSpreadInUnsupportedLocations(SemanticModel model, IDiagnosticWriter diagnostics)
        {
            foreach (var body in GetObjectSyntaxesToBlockSpreadsIn(model))
            {
                foreach (var spread in body.Children.OfType<SpreadExpressionSyntax>())
                {
                    diagnostics.Write(spread, x => x.SpreadOperatorUnsupportedInLocation(spread));
                }
            }

            foreach (var spread in SyntaxAggregator.AggregateByType<SpreadExpressionSyntax>(model.Root.Syntax))
            {
                if (model.Binder.GetParent(spread) is not ObjectSyntax parentObject)
                {
                    continue;
                }

                if (parentObject.Properties.Any(x => x.Value is ForSyntax))
                {
                    diagnostics.Write(spread, x => x.SpreadOperatorCannotBeUsedWithForLoop(spread));
                }

                if (model.Binder.GetParent(parentObject) is ExtensionWithClauseSyntax)
                {
                    diagnostics.Write(spread, x => x.SpreadOperatorUnsupportedInLocation(spread));
                }
            }
        }

        private static void BlockSecureOutputsWithLocalDeploy(SemanticModel model, IDiagnosticWriter diagnostics)
        {
            if (model.TargetScope != ResourceScope.Local)
            {
                return;
            }

            foreach (var module in model.Root.ModuleDeclarations)
            {
                if (module.TryGetSemanticModel().TryUnwrap() is { } moduleModel &&
                    moduleModel.Outputs.Any(output => output.IsSecure))
                {
                    diagnostics.Write(DiagnosticBuilder.ForPosition(module.NameSource).SecureOutputsNotSupportedWithLocalDeploy(module.Name));
                }
            }
        }

        private static void BlockSecureOutputAccessOnIndirectReference(
            SemanticModel model,
            IDiagnosticWriter diagnostics)
        {
            // we're looking for access expressions...
            foreach (var accessExpr in SyntaxAggregator.AggregateByType<AccessExpressionSyntax>(model.Root.Syntax))
            {
                // ... whose base expression is a match for `<something>.outputs`
                if (accessExpr.BaseExpression is AccessExpressionSyntax baseAccessExpr &&
                    baseAccessExpr.AccessExpressionMatches(
                        SyntaxFactory.CreateStringLiteral(LanguageConstants.ModuleOutputsPropertyName)) &&
                    // ... and whose base expression is a module...
                    TypeHelper.SatisfiesCondition(model.GetTypeInfo(baseAccessExpr.BaseExpression), t => t is ModuleType) &&
                    // ... when the type of the output dereferenced would trigger the use of `listOutputsWithSecureValues`
                    TypeHelper.IsOrContainsSecureType(model.GetTypeInfo(accessExpr)))
                {
                    if (model.GetSymbolInfo(baseAccessExpr.BaseExpression) is ModuleSymbol ||
                        (SyntaxHelper.UnwrapNonNullAssertion(baseAccessExpr.BaseExpression) is ArrayAccessSyntax grandBaseArrayAccess &&
                            model.GetSymbolInfo(grandBaseArrayAccess.BaseExpression) is ModuleSymbol greatGrandBaseModule &&
                            greatGrandBaseModule.IsCollection))
                    {
                        // if the module reference is **direct** (e.g., `mod.outputs.sensitive` or
                        // `mod[0].outputs.sensitive`, don't raise a diagnostic
                        continue;
                    }

                    diagnostics.Write(DiagnosticBuilder.ForPosition(accessExpr.IndexExpression)
                        .SecureOutputsOnlyAllowedOnDirectModuleReference());
                }
            }
        }

        private static void BlockExplicitDependenciesInOrOnInlinedExistingResources(
            SemanticModel model,
            ResourceTypeResolver resolver,
            IDiagnosticWriter diagnostics)
        {
            static IEnumerable<string> GetRuntimeIdentifierProperties(ResourceTypeResolver resolver, ResourceSymbol resource)
                => resolver.TryGetBodyObjectType(resource)?.Properties
                    .Where(kvp => AzResourceTypeProvider.UniqueIdentifierProperties.Contains(kvp.Key) &&
                        !kvp.Value.Flags.HasFlag(TypePropertyFlags.ReadableAtDeployTime))
                    .Select(kvp => kvp.Key) ?? [];

            foreach (var resourceDeclaration in model.Root.ResourceDeclarations)
            {
                if (resourceDeclaration.TryGetBodyProperty(LanguageConstants.ResourceDependsOnPropertyName)
                    is { } explicitDependencies)
                {
                    if (model.SymbolsToInline.ExistingResourcesToInline.Contains(resourceDeclaration))
                    {
                        // inlined resources can't have explicit dependencies
                        diagnostics.Write(DiagnosticBuilder.ForPosition(explicitDependencies)
                            .InlinedResourcesCannotHaveExplicitDependencies(
                                resourceDeclaration.Name,
                                GetRuntimeIdentifierProperties(resolver, resourceDeclaration)));
                    }
                    else
                    {
                        foreach (var (inlinedResource, explicitDependency) in SymbolicReferenceCollector
                            .CollectSymbolsReferenced(model.Binder, explicitDependencies.Value)
                            .SelectMany(kvp => kvp.Key is ResourceSymbol r && model.SymbolsToInline.ExistingResourcesToInline.Contains(r)
                                ? kvp.Value.Select(syntax => (r, syntax))
                                : []))
                        {
                            diagnostics.Write(DiagnosticBuilder.ForPosition(explicitDependency)
                                .CannotExplicitlyDependOnInlinedResource(
                                    resourceDeclaration.Name,
                                    inlinedResource.Name,
                                    GetRuntimeIdentifierProperties(resolver, inlinedResource)));
                        }
                    }
                }
            }
        }

        private static IEnumerable<ObjectSyntax> GetObjectSyntaxesToBlockSpreadsIn(SemanticModel model)
        {
            foreach (var module in model.Root.ModuleDeclarations)
            {
                if (module.DeclaringModule.TryGetBody() is not { } body)
                {
                    continue;
                }

                yield return body;

                if (body.TryGetPropertyByName(LanguageConstants.ModuleParamsPropertyName)?.Value is ObjectSyntax paramsBody)
                {
                    yield return paramsBody;
                }

                if (model.Features.ModuleExtensionConfigsEnabled && body.TryGetPropertyByName(LanguageConstants.ModuleExtensionConfigsPropertyName)?.Value is ObjectSyntax extensionsBody)
                {
                    yield return extensionsBody;

                    // Contract is Dictionary<string, Dictionary<string, DeploymentExtensionConfigItem>>
                    foreach (var extConfigObjProp in extensionsBody.Properties)
                    {
                        if (extConfigObjProp.Value is ObjectSyntax extConfigObj)
                        {
                            yield return extConfigObj;
                        }
                    }
                }
            }

            foreach (var resource in model.Root.ResourceDeclarations)
            {
                if (resource.DeclaringResource.TryGetBody() is { } body)
                {
                    yield return body;
                }
            }
        }

        private static void BlockMultilineStringInterpolationWithoutFeatureFlagEnabled(SemanticModel model, IDiagnosticWriter diagnostics)
        {
            if (model.Features.MultilineStringInterpolationEnabled)
            {
                return;
            }

            foreach (var @string in SyntaxAggregator.AggregateByType<StringSyntax>(model.Root.Syntax)
                .Where(x => x.IsMultiLineString() && !x.IsVerbatimString()))
            {
                diagnostics.Write(@string, x => x.MultilineStringRequiresExperimentalFeature());
            }
        }
    }
}
