// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
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

namespace Bicep.Core.Emit
{
    public static class EmitLimitationCalculator
    {
        public static EmitLimitationInfo Calculate(SemanticModel model)
        {
            var diagnosticWriter = ToListDiagnosticWriter.Create();

            var moduleScopeData = ScopeHelper.GetModuleScopeInfo(model, diagnosticWriter);
            var resourceScopeData = ScopeHelper.GetResourceScopeInfo(model, diagnosticWriter);

            DeployTimeConstantValidator.Validate(model, diagnosticWriter);
            ForSyntaxValidatorVisitor.Validate(model, diagnosticWriter);
            FunctionPlacementValidatorVisitor.Validate(model, diagnosticWriter);

            DetectDuplicateNames(model, diagnosticWriter, resourceScopeData, moduleScopeData);
            DetectIncorrectlyFormattedNames(model, diagnosticWriter);
            DetectUnexpectedResourceLoopInvariantProperties(model, diagnosticWriter);
            DetectUnexpectedModuleLoopInvariantProperties(model, diagnosticWriter);
            DetectUnsupportedModuleParameterAssignments(model, diagnosticWriter);

            return new EmitLimitationInfo(diagnosticWriter.GetDiagnostics(), moduleScopeData, resourceScopeData);
        }

        private static void DetectDuplicateNames(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter, ImmutableDictionary<ResourceMetadata, ScopeHelper.ScopeData> resourceScopeData, ImmutableDictionary<ModuleSymbol, ScopeHelper.ScopeData> moduleScopeData)
        {
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
                if (module.SafeGetBodyPropertyValue(LanguageConstants.ResourceNamePropertyName) is not StringSyntax propertyNameValue)
                {
                    //currently limiting check to 'name' property values that are strings, although it can be references or other syntaxes
                    continue;
                }

                var propertyScopeValue = (module.SafeGetBodyPropertyValue(LanguageConstants.ResourceScopePropertyName) as FunctionCallSyntax)?.Arguments.Select(x => x.Expression as StringSyntax).ToImmutableArray();

                yield return new ModuleDefinition(module.Name, scopeData.RequestedScope, propertyScopeValue, propertyNameValue);
            }
        }

        private static IEnumerable<ResourceDefinition> GetResourceDefinitions(SemanticModel semanticModel, ImmutableDictionary<ResourceMetadata, ScopeHelper.ScopeData> resourceScopeData)
        {
            foreach (var resource in semanticModel.AllResources)
            {
                if (resource.IsExistingResource)
                {
                    // 'existing' resources are not being deployed so duplicates are allowed
                    continue;
                }

                // Determine the scope - this is either something like a resource group/subscription or another resource
                ResourceMetadata? resourceScope;
                if (resourceScopeData.TryGetValue(resource, out var scopeData) && scopeData.ResourceScope is {} scopeMetadata)
                {
                    resourceScope = scopeMetadata;
                }
                else
                {
                    resourceScope = semanticModel.ResourceAncestors.GetAncestors(resource).LastOrDefault()?.Resource;
                }

                if (resource.NameSyntax is not StringSyntax namePropertyValue)
                {
                    //currently limiting check to 'name' property values that are strings, although it can be references or other syntaxes
                    continue;
                }

                yield return new ResourceDefinition(resource.Symbol.Name, resourceScope, resource.TypeReference.FullyQualifiedType, namePropertyValue);
            }
        }

        public static void DetectIncorrectlyFormattedNames(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
        {
            foreach (var resource in semanticModel.AllResources)
            {
                if (resource.NameSyntax is not StringSyntax resourceNameString)
                {
                    // not easy to do analysis if it's not a string!
                    continue;
                }

                var ancestors = semanticModel.ResourceAncestors.GetAncestors(resource);
                if (ancestors.Any())
                {
                    // try to detect cases where someone has applied top-level resource declaration naming to a nested/parent resource
                    // e.g. '{parent.name}/child' or 'parent/child'
                    if (resourceNameString.SegmentValues.Any(v => v.Contains('/')))
                    {
                        diagnosticWriter.Write(resource.NameSyntax, x => x.ChildResourceNameContainsQualifiers());
                    }
                }
                else
                {
                    var slashCount = resourceNameString.SegmentValues.Sum(x => x.Count(y => y == '/'));
                    var expectedSlashCount = resource.TypeReference.Types.Length - 1;

                    // Try to detect cases where someone has applied nested/parent resource declaration naming to a top-level resource - e.g. 'child'.
                    if (resourceNameString.IsInterpolated())
                    {
                        // This is best-effort for interpolated strings, as variables may pull in additional '/' characters.
                        // So we can only accurately show a diagnostic if there are TOO MANY '/' characters.
                        if (slashCount > expectedSlashCount)
                        {   
                            diagnosticWriter.Write(resource.NameSyntax, x => x.TopLevelChildResourceNameIncorrectQualifierCount(expectedSlashCount));
                        }
                    }
                    else
                    {
                        // We know exactly how many '/' characters must be present, because we have a string literal. So expect an exact match.
                        if (slashCount != expectedSlashCount)
                        {
                            diagnosticWriter.Write(resource.NameSyntax, x => x.TopLevelChildResourceNameIncorrectQualifierCount(expectedSlashCount));
                        }
                    }
                }
            }
        }

        public static void DetectUnexpectedResourceLoopInvariantProperties(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
        {
            foreach (var resource in semanticModel.AllResources)
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
                    .Select(property => (property, value: resource.Symbol.SafeGetBodyPropertyValue(property.Name)))
                    // exclude missing or malformed property values
                    .Where(pair => pair.value is not null and not SkippedTriviaSyntax)
                    .ToImmutableDictionary(pair => pair.property, pair => pair.value!);

                if (!propertyMap.Any(pair=>pair.Key.Flags.HasFlag(TypePropertyFlags.Required)))
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

        public static void DetectUnexpectedModuleLoopInvariantProperties(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
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
                    .Select(property => (property, value: module.SafeGetBodyPropertyValue(property.Name)))
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

        public static void DetectUnsupportedModuleParameterAssignments(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
        {
            foreach(var moduleSymbol in semanticModel.Root.ModuleDeclarations)
            {
                if(moduleSymbol.DeclaringModule.TryGetBody() is not ObjectSyntax body)
                {
                    // skip modules with malformed bodies
                    continue;
                }

                var paramsValue = body.SafeGetPropertyByName(LanguageConstants.ModuleParamsPropertyName)?.Value;
                switch(paramsValue)
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
