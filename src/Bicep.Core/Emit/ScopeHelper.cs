// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;

namespace Bicep.Core.Emit
{
    public static class ScopeHelper
    {
        public class ScopeData
        {
            /// <summary>
            /// Type of scope requested by the resource.
            /// </summary>
            public ResourceScope RequestedScope { get; set; }

            /// <summary>
            /// Expression for the name of the Management Group or null.
            /// </summary>
            public SyntaxBase? ManagementGroupNameProperty { get; set; }

            /// <summary>
            /// Expression for the subscription ID or null.
            /// </summary>
            public SyntaxBase? SubscriptionIdProperty { get; set; }

            /// <summary>
            /// Expression for the resource group name or null.
            /// </summary>
            public SyntaxBase? ResourceGroupProperty { get; set; }

            /// <summary>
            /// The symbol of the resource being extended or null.
            /// </summary>
            public DeclaredResourceMetadata? ResourceScope { get; set; }

            /// <summary>
            /// The expression for the loop index. This is used with loops when indexing into resource collections.
            /// </summary>
            public SyntaxBase? IndexExpression { get; set; }
        }

        public delegate void LogInvalidScopeDiagnostic(IPositionable positionable, ResourceScope suppliedScope, ResourceScope supportedScopes);

        private static ScopeData? ValidateScope(SemanticModel semanticModel, LogInvalidScopeDiagnostic logInvalidScopeFunc, ResourceScope supportedScopes, SyntaxBase bodySyntax, SyntaxBase? scopeValue)
        {
            if (scopeValue is null)
            {
                // no scope provided - use the target scope for the file
                if (!supportedScopes.HasFlag(semanticModel.TargetScope))
                {
                    logInvalidScopeFunc(bodySyntax, semanticModel.TargetScope, supportedScopes);
                    return null;
                }

                return null;
            }

            var (baseSyntax, indexExpression) = SyntaxHelper.UnwrapArrayAccessSyntax(scopeValue);
            var scopeSymbol = semanticModel.GetSymbolInfo(baseSyntax);
            var scopeType = semanticModel.GetTypeInfo(scopeValue);

            switch (scopeType)
            {
                case TenantScopeType type:
                    if (!supportedScopes.HasFlag(ResourceScope.Tenant))
                    {
                        logInvalidScopeFunc(scopeValue, ResourceScope.Tenant, supportedScopes);
                        return null;
                    }

                    return new ScopeData { RequestedScope = ResourceScope.Tenant, IndexExpression = indexExpression };

                case ManagementGroupScopeType type:
                    if (!supportedScopes.HasFlag(ResourceScope.ManagementGroup))
                    {
                        logInvalidScopeFunc(scopeValue, ResourceScope.ManagementGroup, supportedScopes);
                        return null;
                    }

                    return type.Arguments.Length switch
                    {
                        0 => new ScopeData { RequestedScope = ResourceScope.ManagementGroup, IndexExpression = indexExpression },
                        _ => new ScopeData { RequestedScope = ResourceScope.ManagementGroup, ManagementGroupNameProperty = type.Arguments[0].Expression, IndexExpression = indexExpression },
                    };

                case SubscriptionScopeType type:
                    if (!supportedScopes.HasFlag(ResourceScope.Subscription))
                    {
                        logInvalidScopeFunc(scopeValue, ResourceScope.Subscription, supportedScopes);
                        return null;
                    }

                    return type.Arguments.Length switch
                    {
                        0 => new ScopeData { RequestedScope = ResourceScope.Subscription, IndexExpression = indexExpression },
                        _ => new ScopeData { RequestedScope = ResourceScope.Subscription, SubscriptionIdProperty = type.Arguments[0].Expression, IndexExpression = indexExpression },
                    };

                case ResourceGroupScopeType type:
                    if (!supportedScopes.HasFlag(ResourceScope.ResourceGroup))
                    {
                        logInvalidScopeFunc(scopeValue, ResourceScope.ResourceGroup, supportedScopes);
                        return null;
                    }

                    return type.Arguments.Length switch
                    {
                        0 => new ScopeData { RequestedScope = ResourceScope.ResourceGroup, IndexExpression = indexExpression },
                        1 => new ScopeData { RequestedScope = ResourceScope.ResourceGroup, ResourceGroupProperty = type.Arguments[0].Expression, IndexExpression = indexExpression },
                        _ => new ScopeData { RequestedScope = ResourceScope.ResourceGroup, SubscriptionIdProperty = type.Arguments[0].Expression, ResourceGroupProperty = type.Arguments[1].Expression, IndexExpression = indexExpression },
                    };
                case { } when scopeSymbol is ResourceSymbol targetResourceSymbol:
                    if (semanticModel.ResourceMetadata.TryLookup(targetResourceSymbol.DeclaringSyntax) is not DeclaredResourceMetadata targetResource)
                    {
                        return null;
                    }

                    if (targetResource.Symbol.IsCollection && indexExpression is null)
                    {
                        // the target is a resource collection, but the user didn't apply an array indexer to it
                        // the type check will produce a good error
                        return null;
                    }

                    if (StringComparer.OrdinalIgnoreCase.Equals(targetResource.TypeReference.FormatType(), AzResourceTypeProvider.ResourceTypeResourceGroup))
                    {
                        // special-case 'Microsoft.Resources/resourceGroups' in order to allow it to create a resourceGroup-scope resource
                        // ignore diagnostics - these will be collected separately in the pass over resources
                        var hasErrors = false;
                        var rgScopeData = ScopeHelper.ValidateScope(semanticModel, (_, _, _) => { hasErrors = true; }, targetResource.Type.ValidParentScopes, targetResource.Symbol.DeclaringResource.Value, targetResource.TryGetScopeSyntax());
                        if (!hasErrors)
                        {
                            if (!supportedScopes.HasFlag(ResourceScope.ResourceGroup))
                            {
                                logInvalidScopeFunc(scopeValue, ResourceScope.ResourceGroup, supportedScopes);
                                return null;
                            }

                            return new ScopeData { RequestedScope = ResourceScope.ResourceGroup, SubscriptionIdProperty = rgScopeData?.SubscriptionIdProperty, ResourceGroupProperty = targetResource.TryGetNameSyntax(), IndexExpression = indexExpression };
                        }
                    }

                    if (StringComparer.OrdinalIgnoreCase.Equals(targetResource.TypeReference.FormatType(), AzResourceTypeProvider.ResourceTypeManagementGroup))
                    {
                        // special-case 'Microsoft.Management/managementGroups' in order to allow it to create a managementGroup-scope resource
                        // ignore diagnostics - these will be collected separately in the pass over resources
                        var hasErrors = false;
                        var mgScopeData = ScopeHelper.ValidateScope(semanticModel, (_, _, _) => { hasErrors = true; }, targetResource.Type.ValidParentScopes, targetResource.Symbol.DeclaringResource.Value, targetResource.TryGetScopeSyntax());
                        if (!hasErrors)
                        {
                            if (!supportedScopes.HasFlag(ResourceScope.ManagementGroup))
                            {
                                logInvalidScopeFunc(scopeValue, ResourceScope.ManagementGroup, supportedScopes);
                                return null;
                            }

                            return new ScopeData { RequestedScope = ResourceScope.ManagementGroup, ManagementGroupNameProperty = targetResource.TryGetNameSyntax(), IndexExpression = indexExpression };
                        }
                    }

                    if (!supportedScopes.HasFlag(ResourceScope.Resource))
                    {
                        logInvalidScopeFunc(scopeValue, ResourceScope.Resource, supportedScopes);
                        return null;
                    }

                    return new ScopeData { RequestedScope = ResourceScope.Resource, ResourceScope = targetResource, IndexExpression = indexExpression };

                case { } when scopeSymbol is ModuleSymbol targetModuleSymbol:
                    if (targetModuleSymbol.IsCollection == (indexExpression is not null))
                    {
                        // using a single module as a scope of another module is not allowed
                        // we log this error only when we have single module without an index expression or
                        // a module collection with an index expression
                        // otherwise, the errors produced by the type check are sufficient
                        logInvalidScopeFunc(scopeValue, ResourceScope.Module, supportedScopes);
                    }

                    return null;
            }

            // type validation should have already caught this
            return null;
        }

        public static LanguageExpression FormatFullyQualifiedResourceId(EmitterContext context, ExpressionConverter converter, ScopeData scopeData, string fullyQualifiedType, IEnumerable<LanguageExpression> nameSegments)
        {
            switch (scopeData.RequestedScope)
            {
                case ResourceScope.Tenant:
                    return ExpressionConverter.GenerateTenantResourceId(fullyQualifiedType, nameSegments);
                case ResourceScope.Subscription:
                    var arguments = new List<LanguageExpression>();
                    if (scopeData.SubscriptionIdProperty != null)
                    {
                        arguments.Add(converter.ConvertExpression(scopeData.SubscriptionIdProperty));
                    }
                    arguments.Add(new JTokenExpression(fullyQualifiedType));
                    arguments.AddRange(nameSegments);

                    return new FunctionExpression("subscriptionResourceId", arguments.ToArray(), Array.Empty<LanguageExpression>());
                case ResourceScope.ResourceGroup:
                    // We avoid using the 'resourceId' function at all here, because its behavior differs depending on the scope that it is called FROM.
                    LanguageExpression scope;
                    if (scopeData.SubscriptionIdProperty == null)
                    {
                        if (scopeData.ResourceGroupProperty == null)
                        {
                            return ExpressionConverter.GenerateResourceGroupResourceId(fullyQualifiedType, nameSegments);
                        }
                        else
                        {
                            var subscriptionId = new FunctionExpression("subscription", Array.Empty<LanguageExpression>(), new LanguageExpression[] { new JTokenExpression("subscriptionId") });
                            var resourceGroup = converter.ConvertExpression(scopeData.ResourceGroupProperty);
                            scope = ExpressionConverter.GenerateResourceGroupScope(subscriptionId, resourceGroup);
                        }
                    }
                    else
                    {
                        if (scopeData.ResourceGroupProperty == null)
                        {
                            throw new NotImplementedException($"Cannot format resourceId with non-null subscription and null resourceGroup");
                        }

                        var subscriptionId = converter.ConvertExpression(scopeData.SubscriptionIdProperty);
                        var resourceGroup = converter.ConvertExpression(scopeData.ResourceGroupProperty);
                        scope = ExpressionConverter.GenerateResourceGroupScope(subscriptionId, resourceGroup);
                    }

                    // We've got to DIY it, unfortunately. The resourceId() function behaves differently when used at different scopes, so is unsuitable here.
                    return ExpressionConverter.GenerateExtensionResourceId(scope, fullyQualifiedType, nameSegments);
                case ResourceScope.ManagementGroup:
                    LanguageExpression mgScope;
                    if (scopeData.ManagementGroupNameProperty != null)
                    {
                        mgScope = converter.GenerateManagementGroupResourceId(scopeData.ManagementGroupNameProperty, true);
                    }
                    else
                    {
                        // use managementGroup().id to format the scope. This will only work at management group scope,
                        // but we only permit referencing a parameter-less management group function at this scope.
                        mgScope = ExpressionConverter.GenerateCurrentManagementGroupId();
                    }

                    return ExpressionConverter.GenerateExtensionResourceId(mgScope, fullyQualifiedType, nameSegments);
                case ResourceScope.Resource:
                    if (scopeData.ResourceScope is not { } resource)
                    {
                        throw new InvalidOperationException("Cannot format resourceId with non-null resource scope symbol");
                    }

                    var parentResourceId = FormatFullyQualifiedResourceId(
                        context,
                        converter,
                        context.ResourceScopeData[resource],
                        resource.TypeReference.FormatType(),
                        converter.GetResourceNameSegments(resource));

                    return ExpressionConverter.GenerateExtensionResourceId(
                        parentResourceId,
                        fullyQualifiedType,
                        nameSegments);
                default:
                    throw new InvalidOperationException($"Cannot format resourceId for scope {scopeData.RequestedScope}");
            }
        }

        public static LanguageExpression FormatUnqualifiedResourceId(EmitterContext context, ExpressionConverter converter, ScopeData scopeData, string fullyQualifiedType, IEnumerable<LanguageExpression> nameSegments)
        {
            switch (scopeData.RequestedScope)
            {
                case ResourceScope.Tenant:
                case ResourceScope.Subscription:
                case ResourceScope.ResourceGroup:
                case ResourceScope.ManagementGroup:
                    return ExpressionConverter.GenerateUnqualifiedResourceId(fullyQualifiedType, nameSegments);
                case ResourceScope.Resource:
                    if (scopeData.ResourceScope is not { } resource)
                    {
                        throw new InvalidOperationException("Cannot format resourceId with non-null resource scope symbol");
                    }

                    var parentResourceId = FormatUnqualifiedResourceId(
                        context,
                        converter,
                        context.ResourceScopeData[resource],
                        resource.TypeReference.FormatType(),
                        converter.GetResourceNameSegments(resource));

                    return ExpressionConverter.GenerateExtensionResourceId(
                        parentResourceId,
                        fullyQualifiedType,
                        nameSegments);
                default:
                    throw new InvalidOperationException($"Cannot format resourceId for scope {scopeData.RequestedScope}");
            }
        }

        public static void EmitResourceScopeProperties(SemanticModel semanticModel, ScopeData scopeData, ExpressionEmitter expressionEmitter, SyntaxBase newContext)
        {
            if (scopeData.ResourceScope is DeclaredResourceMetadata scopeResource)
            {
                // emit the resource id of the resource being extended
                expressionEmitter.EmitProperty("scope", () => expressionEmitter.EmitUnqualifiedResourceId(scopeResource, scopeData.IndexExpression, newContext));
                return;
            }

            EmitResourceOrModuleScopeProperties(semanticModel, scopeData, expressionEmitter, newContext);
        }

        public static void EmitModuleScopeProperties(SemanticModel semanticModel, ScopeData scopeData, ExpressionEmitter expressionEmitter, SyntaxBase newContext) =>
            EmitResourceOrModuleScopeProperties(semanticModel, scopeData, expressionEmitter, newContext);

        private static void EmitResourceOrModuleScopeProperties(SemanticModel semanticModel, ScopeData scopeData, ExpressionEmitter expressionEmitter, SyntaxBase newContext)
        {
            switch (scopeData.RequestedScope)
            {
                case ResourceScope.Tenant:
                    if (semanticModel.TargetScope != ResourceScope.Tenant)
                    {
                        // emit the "/" to allow cross-scope deployment of a Tenant resource from another deployment scope
                        expressionEmitter.EmitProperty("scope", new JTokenExpression("/"));
                    }
                    return;
                case ResourceScope.ManagementGroup:
                    if (scopeData.ManagementGroupNameProperty is not null)
                    {
                        // The template engine expects an unqualified resourceId for the management group scope if deploying at tenant or management group scope
                        var useFullyQualifiedResourceId = semanticModel.TargetScope != ResourceScope.Tenant && semanticModel.TargetScope != ResourceScope.ManagementGroup;
                        expressionEmitter.EmitProperty("scope", expressionEmitter.GetManagementGroupResourceId(scopeData.ManagementGroupNameProperty, scopeData.IndexExpression, newContext, useFullyQualifiedResourceId));
                    }
                    return;
                case ResourceScope.Subscription:
                    if (scopeData.SubscriptionIdProperty is not null)
                    {
                        // TODO: It's very suspicious that this doesn't reference scopeData.IndexExpression
                        expressionEmitter.EmitProperty("subscriptionId", scopeData.SubscriptionIdProperty);
                    }
                    else if (semanticModel.TargetScope == ResourceScope.ResourceGroup)
                    {
                        // TODO: It's very suspicious that this doesn't reference scopeData.IndexExpression
                        expressionEmitter.EmitProperty("subscriptionId", new FunctionExpression("subscription", Array.Empty<LanguageExpression>(), new LanguageExpression[] { new JTokenExpression("subscriptionId") }));
                    }
                    return;
                case ResourceScope.ResourceGroup:
                    if (scopeData.SubscriptionIdProperty is not null)
                    {
                        expressionEmitter.EmitProperty("subscriptionId", () => expressionEmitter.EmitExpression(scopeData.SubscriptionIdProperty, scopeData.IndexExpression, newContext));
                    }
                    if (scopeData.ResourceGroupProperty is not null)
                    {
                        expressionEmitter.EmitProperty("resourceGroup", () => expressionEmitter.EmitExpression(scopeData.ResourceGroupProperty, scopeData.IndexExpression, newContext));
                    }
                    return;
                default:
                    throw new InvalidOperationException($"Cannot format resourceId for scope {scopeData.RequestedScope}");
            }
        }

        public static TypeProperty CreateExistingResourceScopeProperty(ResourceScope validScopes, TypePropertyFlags propertyFlags) =>
            CreateResourceScopePropertyInternal(validScopes, propertyFlags);

        public static TypeProperty? TryCreateNonExistingResourceScopeProperty(ResourceScope validScopes, TypePropertyFlags propertyFlags)
        {
            // we only support scope in these cases:
            // 1. extension resources (or resources where the scope is unknown and thus may be an extension resource)
            // 2. Tenant resources
            ResourceScope effectiveScopes = validScopes & (ResourceScope.Resource | ResourceScope.Tenant);
            return effectiveScopes != 0
                ? CreateResourceScopePropertyInternal(effectiveScopes, propertyFlags)
                : null;
        }

        private static TypeProperty CreateResourceScopePropertyInternal(ResourceScope validScopes, TypePropertyFlags scopePropertyFlags)
        {
            var scopeReference = LanguageConstants.CreateResourceScopeReference(validScopes);
            return new(LanguageConstants.ResourceScopePropertyName, scopeReference, scopePropertyFlags);
        }

        private static DeclaredResourceMetadata? GetRootResource(IReadOnlyDictionary<DeclaredResourceMetadata, ScopeData> scopeInfo, DeclaredResourceMetadata resource)
        {
            if (!scopeInfo.TryGetValue(resource, out var scopeData))
            {
                return null;
            }

            if (scopeData.ResourceScope is not null)
            {
                return GetRootResource(scopeInfo, scopeData.ResourceScope);
            }

            return resource;
        }

        private static void ValidateResourceScopeRestrictions(SemanticModel semanticModel, IReadOnlyDictionary<DeclaredResourceMetadata, ScopeData> scopeInfo, DeclaredResourceMetadata resource, Action<DiagnosticBuilder.DiagnosticBuilderDelegate> writeScopeDiagnostic)
        {
            if (resource.IsExistingResource)
            {
                // we don't have any cross-scope restrictions on 'existing' resource declarations
                return;
            }

            if (semanticModel.Binder.TryGetCycle(resource.Symbol) is not null)
            {
                return;
            }

            var rootResource = GetRootResource(scopeInfo, resource);
            if (rootResource is null ||
                !scopeInfo.TryGetValue(rootResource, out var scopeData))
            {
                // invalid scope should have already generated errors
                return;
            }

            if (!IsDeployableResourceScope(semanticModel, scopeData))
            {
                writeScopeDiagnostic(x => x.InvalidCrossResourceScope());
            }
        }

        private static bool IsDeployableResourceScope(SemanticModel semanticModel, ScopeData scopeData)
        {
            if (scopeData.RequestedScope == ResourceScope.Tenant)
            {
                // tenant resources can be deployed cross-scope
                return true;
            }

            // we only allow resources to be deployed at the target scope
            var matchesTargetScope = (scopeData.RequestedScope == semanticModel.TargetScope &&
                scopeData.ManagementGroupNameProperty is null &&
                scopeData.SubscriptionIdProperty is null &&
                scopeData.ResourceGroupProperty is null &&
                scopeData.ResourceScope is null);

            return matchesTargetScope;
        }

        public static ImmutableDictionary<DeclaredResourceMetadata, ScopeData> GetResourceScopeInfo(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
        {
            void logInvalidScopeDiagnostic(IPositionable positionable, ResourceScope suppliedScope, ResourceScope supportedScopes)
                => diagnosticWriter.Write(positionable, x => x.UnsupportedResourceScope(suppliedScope, supportedScopes));

            var scopeInfo = new Dictionary<DeclaredResourceMetadata, ScopeData>();
            var ancestorsLookup = semanticModel.DeclaredResources
                .ToDictionary(
                    x => x,
                    x => semanticModel.ResourceAncestors.GetAncestors(x));

            var defaultScopeData = new ScopeData { RequestedScope = semanticModel.TargetScope };

            // process symbols in order of ancestor depth.
            // this is because we want to avoid recomputing the scope for child resources which inherit it from their parents.
            foreach (var (resource, ancestors) in ancestorsLookup.OrderBy(kvp => kvp.Value.Length))
            {
                if (ancestors.Any())
                {
                    if (resource.TryGetScopeSyntax() is { } scopeSyntax)
                    {
                        // it doesn't make sense to have scope on a descendent resource; it should be inherited from the oldest ancestor.
                        diagnosticWriter.Write(scopeSyntax, x => x.ScopeUnsupportedOnChildResource(ancestors.Last().Resource.Symbol.Name));
                        // TODO: format the ancestor name using the resource accessor (::) for nested resources
                        scopeInfo[resource] = defaultScopeData;
                        continue;
                    }

                    var firstAncestor = ancestors.First();
                    if (!resource.IsExistingResource &&
                        firstAncestor.Resource.IsExistingResource &&
                        !IsDeployableResourceScope(semanticModel, scopeInfo[firstAncestor.Resource]))
                    {
                        // Setting 'scope' is blocked for child resources, so we just need to check whether the root ancestor has 'scope' set.
                        // If it does, it could be an 'existing' resource - which can be assigned any scope - so we need to ensure the assigned scope can be used to deploy.
                        diagnosticWriter.Write(resource.Symbol.DeclaringResource.Value, x => x.ScopeDisallowedForAncestorResource(firstAncestor.Resource.Symbol.Name));
                        // TODO: format the ancestor name using the resource accessor (::) for nested resources
                        scopeInfo[resource] = defaultScopeData;
                        continue;
                    }

                    if (semanticModel.Binder.TryGetCycle(resource.Symbol) is not null)
                    {
                        scopeInfo[resource] = defaultScopeData;
                        continue;
                    }

                    // we really just want the scope allocated to the oldest ancestor.
                    // since we are looping in order of depth, we can just read back the value from a previous iteration.
                    scopeInfo[resource] = scopeInfo[firstAncestor.Resource];
                    continue;
                }

                // way to validate scope-escaping.
                if (resource.TryGetScopeSyntax() is SyntaxBase scope &&
                    semanticModel.ResourceMetadata.TryLookup(scope) is ParameterResourceMetadata scopeMetadata)
                {
                    diagnosticWriter.Write(DiagnosticBuilder.ForPosition(scope).InvalidResourceScopeCannotBeResourceTypeParameter(scopeMetadata.Symbol.Name));
                    scopeInfo[resource] = defaultScopeData;
                    continue;
                }
                // This check has to live here because if here because this case is not handled when building the resource ancestor graph.
                else if (resource.Symbol.TryGetBodyPropertyValue(LanguageConstants.ResourceParentPropertyName) is {} referenceParentSyntax &&
                    semanticModel.ResourceMetadata.TryLookup(referenceParentSyntax) is ParameterResourceMetadata parentMetadata)
                {
                    diagnosticWriter.Write(DiagnosticBuilder.ForPosition(referenceParentSyntax).InvalidResourceScopeCannotBeResourceTypeParameter(parentMetadata.Symbol.Name));
                    scopeInfo[resource] = defaultScopeData;
                    continue;
                }

                var validatedScopeData = ScopeHelper.ValidateScope(semanticModel, logInvalidScopeDiagnostic, resource.Type.ValidParentScopes, resource.Symbol.DeclaringResource.Value, resource.TryGetScopeSyntax());
                scopeInfo[resource] = validatedScopeData ?? defaultScopeData;
            }

            foreach (var resourceToValidate in semanticModel.DeclaredResources)
            {
                ValidateResourceScopeRestrictions(
                    semanticModel,
                    scopeInfo,
                    resourceToValidate,
                    buildDiagnostic => diagnosticWriter.Write(resourceToValidate.TryGetScopeSyntax() ?? resourceToValidate.Symbol.DeclaringResource.Value, buildDiagnostic));
            }

            return scopeInfo.ToImmutableDictionary();
        }

        private static void ValidateNestedTemplateScopeRestrictions(SemanticModel semanticModel, ScopeData scopeData, Action<DiagnosticBuilder.DiagnosticBuilderDelegate> writeScopeDiagnostic)
        {
            bool checkScopes(params ResourceScope[] scopes)
                => scopes.Contains(semanticModel.TargetScope);

            var isValid = scopeData.RequestedScope switch
            {
                // If you update this switch block to add new supported nested template scope combinations,
                // please ensure you update the wording of error messages BCP113, BCP114, BCP115 & BCP116 to reflect this!
                ResourceScope.Tenant
                    => checkScopes(ResourceScope.Tenant, ResourceScope.ManagementGroup, ResourceScope.Subscription, ResourceScope.ResourceGroup),
                ResourceScope.ManagementGroup when scopeData.ManagementGroupNameProperty is not null
                    => checkScopes(ResourceScope.Tenant, ResourceScope.ManagementGroup),
                ResourceScope.ManagementGroup
                    => checkScopes(ResourceScope.Tenant, ResourceScope.ManagementGroup),
                ResourceScope.Subscription when scopeData.SubscriptionIdProperty is not null
                    => checkScopes(ResourceScope.Tenant, ResourceScope.ManagementGroup, ResourceScope.Subscription, ResourceScope.ResourceGroup),
                ResourceScope.Subscription
                    => checkScopes(ResourceScope.Subscription, ResourceScope.ResourceGroup),
                ResourceScope.ResourceGroup when scopeData.SubscriptionIdProperty is not null && scopeData.ResourceGroupProperty is not null
                    => checkScopes(ResourceScope.Tenant, ResourceScope.ManagementGroup, ResourceScope.Subscription, ResourceScope.ResourceGroup),
                ResourceScope.ResourceGroup when scopeData.ResourceGroupProperty is not null
                    => checkScopes(ResourceScope.Subscription, ResourceScope.ResourceGroup),
                ResourceScope.ResourceGroup
                    => checkScopes(ResourceScope.ResourceGroup),
                _ => false,
            };

            if (isValid)
            {
                return;
            }

            switch (semanticModel.TargetScope)
            {
                case ResourceScope.Tenant:
                    writeScopeDiagnostic(x => x.InvalidModuleScopeForTenantScope());
                    break;
                case ResourceScope.ManagementGroup:
                    writeScopeDiagnostic(x => x.InvalidModuleScopeForManagementScope());
                    break;
                case ResourceScope.Subscription:
                    writeScopeDiagnostic(x => x.InvalidModuleScopeForSubscriptionScope());
                    break;
                case ResourceScope.ResourceGroup:
                    writeScopeDiagnostic(x => x.InvalidModuleScopeForResourceGroup());
                    break;
            }
        }

        public static ImmutableDictionary<ModuleSymbol, ScopeData> GetModuleScopeInfo(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
        {
            void LogInvalidScopeDiagnostic(IPositionable positionable, ResourceScope suppliedScope, ResourceScope supportedScopes)
                => diagnosticWriter.Write(positionable, x => x.UnsupportedModuleScope(suppliedScope, supportedScopes));

            var scopeInfo = new Dictionary<ModuleSymbol, ScopeData>();

            foreach (var moduleSymbol in semanticModel.Root.ModuleDeclarations)
            {
                if (moduleSymbol.TryGetModuleType() is not { } moduleType)
                {
                    // missing type should be caught during type validation
                    continue;
                }

                var scopeValue = moduleSymbol.TryGetBodyPropertyValue(LanguageConstants.ResourceScopePropertyName);
                var scopeData = ScopeHelper.ValidateScope(semanticModel, LogInvalidScopeDiagnostic, moduleType.ValidParentScopes, moduleSymbol.DeclaringModule.Value, scopeValue);

                if (scopeData is null)
                {
                    scopeData = new ScopeData { RequestedScope = semanticModel.TargetScope };
                }

                ValidateNestedTemplateScopeRestrictions(
                    semanticModel,
                    scopeData,
                    buildDiagnostic => diagnosticWriter.Write(scopeValue ?? moduleSymbol.DeclaringModule.Value, buildDiagnostic));

                scopeInfo[moduleSymbol] = scopeData;
            }

            return scopeInfo.ToImmutableDictionary();
        }
    }
}
