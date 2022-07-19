// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.DataFlow;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.Utils;
using Bicep.Core.Extensions;
using Bicep.Core.Syntax.Visitors;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.Emit
{
    public static class EmitLimitationCalculator
    {
        public static EmitLimitationInfo Calculate(SemanticModel model)
        {
            var diagnostics = ToListDiagnosticWriter.Create();

            var moduleScopeData = ScopeHelper.GetModuleScopeInfo(model, diagnostics);
            var resourceScopeData = ScopeHelper.GetResourceScopeInfo(model, diagnostics);

            DeployTimeConstantValidator.Validate(model, diagnostics);
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

            return new(diagnostics.GetDiagnostics(), moduleScopeData, resourceScopeData);
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
                    diagnosticWriter.Write(DiagnosticBuilder.ForPosition(resource.Symbol.NameSyntax).ForExpressionContainsLoopInvariants(itemVariable.Name.IdentifierName, indexVariable?.Name.IdentifierName, expectedVariantPropertiesForType.Select(p => p.Name)));
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
                    diagnosticWriter.Write(DiagnosticBuilder.ForPosition(module.NameSyntax).ForExpressionContainsLoopInvariants(itemVariable.Name.IdentifierName, indexVariable?.Name.IdentifierName, expectedVariantPropertiesForType.Select(p => p.Name)));
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
                diagnosticWriter.Write(DiagnosticBuilder.ForPosition(copyVariableSymbol.NameSyntax).ReservedIdentifier(LanguageConstants.CopyLoopIdentifier));
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
    }
}
