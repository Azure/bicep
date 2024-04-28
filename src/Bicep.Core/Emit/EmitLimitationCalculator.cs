// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.DataFlow;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Intermediate;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.Utils;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
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

            DetectDuplicateNames(model, diagnostics, resourceScopeData, moduleScopeData);
            DetectIncorrectlyFormattedNames(model, diagnostics);
            DetectUnexpectedResourceLoopInvariantProperties(model, diagnostics);
            DetectUnexpectedModuleLoopInvariantProperties(model, diagnostics);
            DetectUnsupportedModuleParameterAssignments(model, diagnostics);
            DetectCopyVariableName(model, diagnostics);
            DetectInvalidValueForParentProperty(model, diagnostics);
            BlockLambdasOutsideFunctionArguments(model, diagnostics);
            BlockUnsupportedLambdaVariableUsage(model, diagnostics);
            BlockModuleOutputResourcePropertyAccess(model, diagnostics);
            BlockSafeDereferenceOfModuleOrResourceCollectionMember(model, diagnostics);
            BlockCyclicAggregateTypeReferences(model, diagnostics);
            BlockTestFrameworkWithoutExperimentalFeaure(model, diagnostics);
            BlockAssertsWithoutExperimentalFeatures(model, diagnostics);
            BlockNamesDistinguishedOnlyByCase(model, diagnostics);
            BlockResourceDerivedTypesThatDoNotDereferenceProperties(model, diagnostics);
            BlockSpreadInUnsupportedLocations(model, diagnostics);

            var paramAssignments = CalculateParameterAssignments(model, diagnostics);

            return new(diagnostics.GetDiagnostics(), moduleScopeData, resourceScopeData, paramAssignments);
        }

        private static void DetectDuplicateNames(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter, ImmutableDictionary<DeclaredResourceMetadata, ScopeHelper.ScopeData> resourceScopeData, ImmutableDictionary<ModuleSymbol, ScopeHelper.ScopeData> moduleScopeData)
        {
            // TODO generalize or move into Az extension

            // This method only checks, if in one deployment we do not have 2 or more resources with this same name in one deployment to avoid template validation error
            // This will not check resource constraints such as necessity of having unique virtual network names within resource group

            var duplicateResources = GetResourceDefinitions(semanticModel, resourceScopeData)
                .GroupBy(x => x, ResourceDefinition.EqualityComparer)
                .Where(group => group.Count() > 1);

            foreach (var duplicatedResourceGroup in duplicateResources)
            {
                var duplicatedResourceNames = duplicatedResourceGroup.Select(x => x.ResourceName).ToArray();
                foreach (var duplicatedResource in duplicatedResourceGroup)
                {
                    diagnosticWriter.Write(duplicatedResource.ResourceNamePropertyValue, x => x.ResourceMultipleDeclarations(duplicatedResourceNames));
                }
            }

            var duplicateModules = GetModuleDefinitions(semanticModel, moduleScopeData)
                .GroupBy(x => x, ModuleDefinition.EqualityComparer)
                .Where(group => group.Count() > 1);

            foreach (var duplicatedModuleGroup in duplicateModules)
            {
                var duplicatedModuleNames = duplicatedModuleGroup.Select(x => x.ModuleName).ToArray();
                foreach (var duplicatedModule in duplicatedModuleGroup)
                {
                    diagnosticWriter.Write(duplicatedModule.ModulePropertyNameValue, x => x.ModuleMultipleDeclarations(duplicatedModuleNames));
                }
            }
        }

        private static IEnumerable<ModuleDefinition> GetModuleDefinitions(SemanticModel semanticModel, ImmutableDictionary<ModuleSymbol, ScopeHelper.ScopeData> moduleScopeData)
        {
            foreach (var module in semanticModel.Root.ModuleDeclarations)
            {
                if (!moduleScopeData.TryGetValue(module, out var scopeData))
                {
                    //module has invalid scope provided, ignoring from duplicate check
                    continue;
                }
                if (module.TryGetBodyPropertyValue(LanguageConstants.ModuleNamePropertyName) is not StringSyntax propertyNameValue)
                {
                    //currently limiting check to 'name' property values that are strings, although it can be references or other syntaxes
                    continue;
                }

                var propertyScopeValue = (module.TryGetBodyPropertyValue(LanguageConstants.ResourceScopePropertyName) as FunctionCallSyntax)?.Arguments.Select(x => x.Expression as StringSyntax).ToImmutableArray();

                yield return new ModuleDefinition(module.Name, scopeData.RequestedScope, propertyScopeValue, propertyNameValue);
            }
        }

        private static IEnumerable<ResourceDefinition> GetResourceDefinitions(SemanticModel semanticModel, ImmutableDictionary<DeclaredResourceMetadata, ScopeHelper.ScopeData> resourceScopeData)
        {
            foreach (var resource in semanticModel.DeclaredResources)
            {
                if (resource.IsExistingResource)
                {
                    // 'existing' resources are not being deployed so duplicates are allowed
                    continue;
                }

                if (!resource.IsAzResource)
                {
                    // comparison checks currently blocked for non-ARM resources
                    continue;
                }

                if (resource.TryGetNameSyntax() is not { } resourceName ||
                    resourceName is not StringSyntax resourceNameString)
                {
                    // the resource doesn't have a name set, or it's not a string and thus difficult to analyze
                    continue;
                }

                // Determine the scope - this is either something like a resource group/subscription or another resource
                ResourceMetadata? resourceScope;
                if (resourceScopeData.TryGetValue(resource, out var scopeData) && scopeData.ResourceScope is { } scopeMetadata)
                {
                    resourceScope = scopeMetadata;
                }
                else
                {
                    resourceScope = semanticModel.ResourceAncestors.GetAncestors(resource).LastOrDefault()?.Resource;
                }

                yield return new ResourceDefinition(resource.Symbol.Name, resourceScope, resource.TypeReference.FormatType(), resourceNameString);
            }
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

                if (!propertyMap.Any(pair => pair.Key.Flags.HasFlag(TypePropertyFlags.Required)))
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

        private static void DetectUnsupportedModuleParameterAssignments(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
        {
            foreach (var moduleSymbol in semanticModel.Root.ModuleDeclarations)
            {
                if (moduleSymbol.DeclaringModule.TryGetBody() is not ObjectSyntax body)
                {
                    // skip modules with malformed bodies
                    continue;
                }

                var paramsValue = body.TryGetPropertyByName(LanguageConstants.ModuleParamsPropertyName)?.Value;
                switch (paramsValue)
                {
                    case null:
                    case ObjectSyntax:
                    case SkippedTriviaSyntax:
                        // no params, the value is an object literal, or we have parse errors
                        // skip the module
                        continue;

                    default:
                        // unexpected type is assigned as the value of the "params" property
                        // we can't emit that directly because the parameters have to be converted into an object whose property values are objects with a "value" property
                        // ideally we would add a runtime function to take care of the conversion in these cases, but it doesn't exist yet
                        diagnosticWriter.Write(DiagnosticBuilder.ForPosition(paramsValue).ModuleParametersPropertyRequiresObjectLiteral());
                        break;
                }
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
            => syntax is PropertyAccessSyntax propertyAccess &&
                model.ResourceMetadata.TryLookup(propertyAccess.BaseExpression) is ModuleOutputResourceMetadata &&
                !AzResourceTypeProvider.ReadWriteDeployTimeConstantPropertyNames.Contains(propertyAccess.PropertyName.IdentifierName);

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

        private static void BlockCyclicAggregateTypeReferences(SemanticModel model, IDiagnosticWriter diagnostics)
        {
            var cycles = CyclicTypeCheckVisitor.FindCycles(model);
            diagnostics.WriteMultiple(cycles.Select(kvp => kvp.Value.Length switch
            {
                1 => DiagnosticBuilder.ForPosition(kvp.Key.DeclaringType.Name).CyclicTypeSelfReference(),
                _ => DiagnosticBuilder.ForPosition(kvp.Key.DeclaringType.Name).CyclicType(kvp.Value.Select(s => s.Name)),
            }));
        }

        private static ImmutableDictionary<ParameterAssignmentSymbol, ParameterAssignmentValue> CalculateParameterAssignments(SemanticModel model, IDiagnosticWriter diagnostics)
        {
            if (model.Root.ParameterAssignments.IsEmpty ||
                model.HasParsingErrors())
            {
                return ImmutableDictionary<ParameterAssignmentSymbol, ParameterAssignmentValue>.Empty;
            }

            var referencesInValues = model.Binder.Bindings.Values.OfType<DeclaredSymbol>().Distinct()
                .ToImmutableDictionary(p => p, p => SymbolicReferenceCollector.CollectSymbolsReferenced(model.Binder, p.DeclaringSyntax));
            var generated = ImmutableDictionary.CreateBuilder<ParameterAssignmentSymbol, ParameterAssignmentValue>();
            var evaluator = new ParameterAssignmentEvaluator(model);
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
                if (result.Value is not null || result.KeyVaultReference is not null)
                {
                    generated[parameter] = new(result.Value, result.KeyVaultReference);
                }
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
                ("variables", model.Root.VariableDeclarations),
                ("outputs", model.Root.OutputDeclarations),
                ("types", model.Root.TypeDeclarations),
                ("asserts", model.Root.AssertDeclarations),
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
            IEnumerable<ObjectSyntax> getObjectSyntaxesToBlock()
            {
                foreach (var module in model.Root.ModuleDeclarations)
                {
                    if (module.DeclaringModule.TryGetBody() is { } body)
                    {
                        yield return body;

                        if (body.TryGetPropertyByName(LanguageConstants.ModuleParamsPropertyName)?.Value is ObjectSyntax paramsBody)
                        {
                            yield return paramsBody;
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

            foreach (var body in getObjectSyntaxesToBlock())
            {
                foreach (var spread in body.Children.OfType<SpreadExpressionSyntax>())
                {
                    diagnostics.Write(spread, x => x.SpreadOperatorUnsupportedInLocation(spread));
                }
            }
        }
    }
}
