// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;

namespace Bicep.Core.Emit
{
    public static class ScopeHelper
    {
        public class ScopeData
        {
            public ResourceScope RequestedScope { get; set; }

            public SyntaxBase? ManagementGroupNameProperty { get; set; }

            public SyntaxBase? SubscriptionIdProperty { get; set; }

            public SyntaxBase? ResourceGroupProperty { get; set; }

            public ResourceSymbol? ResourceScopeSymbol { get; set; }
        }

        public delegate void LogInvalidScopeDiagnostic(IPositionable positionable, ResourceScope suppliedScope, ResourceScope supportedScopes);

        private static ScopeData? ValidateScope(SemanticModel semanticModel, LogInvalidScopeDiagnostic logInvalidScopeFunc, ResourceScope supportedScopes, SyntaxBase bodySyntax, ObjectPropertySyntax? scopeProperty)
        {
            if (scopeProperty is null)
            {
                // no scope provided - use the target scope for the file
                if (!supportedScopes.HasFlag(semanticModel.TargetScope))
                {
                    logInvalidScopeFunc(bodySyntax, semanticModel.TargetScope, supportedScopes);
                    return null;
                }

                return null;
            }

            var scopeSymbol = semanticModel.GetSymbolInfo(scopeProperty.Value);
            var scopeType = semanticModel.GetTypeInfo(scopeProperty.Value);

            switch (scopeType)
            {
                case TenantScopeType type:
                    if (!supportedScopes.HasFlag(ResourceScope.Tenant))
                    {
                        logInvalidScopeFunc(scopeProperty.Value, ResourceScope.Tenant, supportedScopes);
                        return null;
                    }

                    return new ScopeData { RequestedScope = ResourceScope.Tenant };
                case ManagementGroupScopeType type:
                    if (!supportedScopes.HasFlag(ResourceScope.ManagementGroup))
                    {
                        logInvalidScopeFunc(scopeProperty.Value, ResourceScope.ManagementGroup, supportedScopes);
                        return null;
                    }

                    return type.Arguments.Length switch {
                        0 => new ScopeData { RequestedScope = ResourceScope.ManagementGroup },
                        _ => new ScopeData { RequestedScope = ResourceScope.ManagementGroup, ManagementGroupNameProperty = type.Arguments[0].Expression },
                    };
                case SubscriptionScopeType type:
                    if (!supportedScopes.HasFlag(ResourceScope.Subscription))
                    {
                        logInvalidScopeFunc(scopeProperty.Value, ResourceScope.Subscription, supportedScopes);
                        return null;
                    }

                    return type.Arguments.Length switch {
                        0 => new ScopeData { RequestedScope = ResourceScope.Subscription },
                        _ => new ScopeData { RequestedScope = ResourceScope.Subscription, SubscriptionIdProperty = type.Arguments[0].Expression },
                    };
                case ResourceGroupScopeType type:
                    if (!supportedScopes.HasFlag(ResourceScope.ResourceGroup))
                    {
                        logInvalidScopeFunc(scopeProperty.Value, ResourceScope.ResourceGroup, supportedScopes);
                        return null;
                    }

                    return type.Arguments.Length switch {
                        0 => new ScopeData { RequestedScope = ResourceScope.ResourceGroup },
                        1 => new ScopeData { RequestedScope = ResourceScope.ResourceGroup, ResourceGroupProperty = type.Arguments[0].Expression },
                        _ => new ScopeData { RequestedScope = ResourceScope.ResourceGroup, SubscriptionIdProperty = type.Arguments[0].Expression, ResourceGroupProperty = type.Arguments[1].Expression },
                    };
                case {} when scopeSymbol is ResourceSymbol targetResourceSymbol:
                    if (!supportedScopes.HasFlag(ResourceScope.Resource))
                    {
                        logInvalidScopeFunc(scopeProperty.Value, ResourceScope.Resource, supportedScopes);
                        return null;
                    }

                    return new ScopeData { RequestedScope = ResourceScope.Resource, ResourceScopeSymbol = targetResourceSymbol };
            }

            // type validation should have already caught this
            return null;
        }

        public static LanguageExpression FormatCrossScopeResourceId(ExpressionConverter expressionConverter, ScopeData scopeData, string fullyQualifiedType, IEnumerable<LanguageExpression> nameSegments)
        {
            var arguments = new List<LanguageExpression>();

            switch (scopeData.RequestedScope)
            {
                case ResourceScope.Tenant:
                    arguments.Add(new JTokenExpression(fullyQualifiedType));
                    arguments.AddRange(nameSegments);

                    return new FunctionExpression("tenantResourceId", arguments.ToArray(), Array.Empty<LanguageExpression>());
                case ResourceScope.Subscription:
                    if (scopeData.SubscriptionIdProperty != null)
                    {
                        arguments.Add(expressionConverter.ConvertExpression(scopeData.SubscriptionIdProperty));
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
                            scope = new FunctionExpression("resourceGroup", Array.Empty<LanguageExpression>(), new LanguageExpression[] { new JTokenExpression("id") });
                        }
                        else
                        {
                            var subscriptionId = new FunctionExpression("subscription", Array.Empty<LanguageExpression>(), new LanguageExpression[] { new JTokenExpression("subscriptionId") });
                            var resourceGroup = expressionConverter.ConvertExpression(scopeData.ResourceGroupProperty);
                            scope = ExpressionConverter.GenerateResourceGroupScope(subscriptionId, resourceGroup);
                        }
                    }
                    else
                    {
                        if (scopeData.ResourceGroupProperty == null)
                        {
                            throw new NotImplementedException($"Cannot format resourceId with non-null subscription and null resourceGroup");
                        }

                        var subscriptionId = expressionConverter.ConvertExpression(scopeData.SubscriptionIdProperty);
                        var resourceGroup = expressionConverter.ConvertExpression(scopeData.ResourceGroupProperty);
                        scope = ExpressionConverter.GenerateResourceGroupScope(subscriptionId, resourceGroup);
                    }

                    // We've got to DIY it, unfortunately. The resourceId() function behaves differently when used at different scopes, so is unsuitable here.
                    return ExpressionConverter.GenerateScopedResourceId(scope, fullyQualifiedType, nameSegments);
                case ResourceScope.ManagementGroup:
                    if (scopeData.ManagementGroupNameProperty != null)
                    {
                        var managementGroupScope = expressionConverter.GenerateManagementGroupResourceId(scopeData.ManagementGroupNameProperty, true);

                        return ExpressionConverter.GenerateScopedResourceId(managementGroupScope, fullyQualifiedType, nameSegments);
                    }

                    // We need to do things slightly differently for Management Groups, because there is no IL to output for "Give me a fully-qualified resource id at the current scope",
                    // and we don't even have a mechanism for reliably getting the current scope (e.g. something like 'deployment().scope'). There are plans to add a managementGroupResourceId function,
                    // but until we have it, we should generate unqualified resource Ids. There should not be a risk of collision, because we do not allow mixing of resource scopes in a single bicep file.
                    return ExpressionConverter.GenerateUnqualifiedResourceId(fullyQualifiedType, nameSegments);
                case ResourceScope.Resource:
                    if (scopeData.ResourceScopeSymbol is null)
                    {
                        throw new InvalidOperationException($"Cannot format resourceId with non-null resource scope symbol");
                    }

                    return ExpressionConverter.GenerateScopedResourceId(
                        expressionConverter.GetLocallyScopedResourceId(scopeData.ResourceScopeSymbol),
                        fullyQualifiedType,
                        nameSegments);
                default:
                    throw new InvalidOperationException($"Cannot format resourceId for scope {scopeData.RequestedScope}");
            }
        }

        public static LanguageExpression FormatLocallyScopedResourceId(ResourceScope? targetScope, string fullyQualifiedType, IEnumerable<LanguageExpression> nameSegments)
        {
            var initialArgs = new JTokenExpression(fullyQualifiedType).AsEnumerable();
            switch (targetScope)
            {
                case ResourceScope.Tenant:
                    var tenantArgs = initialArgs.Concat(nameSegments);
                    return new FunctionExpression("tenantResourceId", tenantArgs.ToArray(), Array.Empty<LanguageExpression>());
                case ResourceScope.Subscription:
                    var subscriptionArgs = initialArgs.Concat(nameSegments);
                    return new FunctionExpression("subscriptionResourceId", subscriptionArgs.ToArray(), Array.Empty<LanguageExpression>());
                case ResourceScope.ResourceGroup:
                    var resourceGroupArgs = initialArgs.Concat(nameSegments);
                    return new FunctionExpression("resourceId", resourceGroupArgs.ToArray(), Array.Empty<LanguageExpression>());
                case ResourceScope.ManagementGroup:
                    // We need to do things slightly differently for Management Groups, because there is no IL to output for "Give me a fully-qualified resource id at the current scope",
                    // and we don't even have a mechanism for reliably getting the current scope (e.g. something like 'deployment().scope'). There are plans to add a managementGroupResourceId function,
                    // but until we have it, we should generate unqualified resource Ids. There should not be a risk of collision, because we do not allow mixing of resource scopes in a single bicep file.
                    return ExpressionConverter.GenerateUnqualifiedResourceId(fullyQualifiedType, nameSegments);
                case null:
                    return ExpressionConverter.GenerateUnqualifiedResourceId(fullyQualifiedType, nameSegments);
                default:
                    // this should have already been caught during compilation
                    throw new InvalidOperationException($"Invalid target scope {targetScope} for module");
            }
        }

        public static void EmitModuleScopeProperties(ResourceScope targetScope, ScopeData scopeData, ExpressionEmitter expressionEmitter)
        {
            switch (scopeData.RequestedScope)
            {
                case ResourceScope.Tenant:
                    expressionEmitter.EmitProperty("scope", new JTokenExpression("/"));
                    return;
                case ResourceScope.ManagementGroup:
                    if (scopeData.ManagementGroupNameProperty != null)
                    {
                        // The template engine expects an unqualified resourceId for the management group scope if deploying at tenant scope
                        var useFullyQualifiedResourceId = targetScope != ResourceScope.Tenant;
                        expressionEmitter.EmitProperty("scope", expressionEmitter.GetManagementGroupResourceId(scopeData.ManagementGroupNameProperty, useFullyQualifiedResourceId));
                    }
                    return;
                case ResourceScope.Subscription:
                    if (scopeData.SubscriptionIdProperty != null)
                    {
                        expressionEmitter.EmitProperty("subscriptionId", scopeData.SubscriptionIdProperty);
                    }
                    else if (targetScope == ResourceScope.ResourceGroup)
                    {
                        expressionEmitter.EmitProperty("subscriptionId", new FunctionExpression("subscription", Array.Empty<LanguageExpression>(), new LanguageExpression[] { new JTokenExpression("subscriptionId") }));
                    }
                    return;
                case ResourceScope.ResourceGroup:
                    if (scopeData.SubscriptionIdProperty != null)
                    {
                        expressionEmitter.EmitProperty("subscriptionId", scopeData.SubscriptionIdProperty);
                    }
                    if (scopeData.ResourceGroupProperty != null)
                    {
                        expressionEmitter.EmitProperty("resourceGroup", scopeData.ResourceGroupProperty);
                    }
                    return;
                default:
                    throw new InvalidOperationException($"Cannot format resourceId for scope {scopeData.RequestedScope}");
            }
        }

        public static ImmutableDictionary<ResourceSymbol, ScopeData> GetResoureScopeInfo(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
        {
            void logInvalidScopeDiagnostic(IPositionable positionable, ResourceScope suppliedScope, ResourceScope supportedScopes)
                => diagnosticWriter.Write(positionable, x => x.UnsupportedResourceScope(suppliedScope, supportedScopes));

            var scopeInfo = new Dictionary<ResourceSymbol, ScopeData>();

            foreach (var resourceSymbol in semanticModel.Root.ResourceDeclarations)
            {
                if (resourceSymbol.Type is not ResourceType resourceType)
                {
                    // missing type should be caught during type validation
                    continue;
                }

                var scopeProperty = resourceSymbol.SafeGetBodyProperty(LanguageConstants.ResourceScopePropertyName);
                var scopeData = ScopeHelper.ValidateScope(semanticModel, logInvalidScopeDiagnostic, resourceType.ValidParentScopes, resourceSymbol.DeclaringResource.Body, scopeProperty);

                if (scopeData is null)
                {
                    continue;
                }

                scopeInfo[resourceSymbol] = scopeData;
            }

            return scopeInfo.ToImmutableDictionary();
        }

        private static bool ValidateNestedTemplateScopeRestrictions(SemanticModel semanticModel, ScopeData scopeData)
        {
            bool checkScopes(params ResourceScope[] scopes)
                => scopes.Contains(semanticModel.TargetScope);

            switch (scopeData.RequestedScope)
            {
                // If you update this switch block to add new supported nested template scope combinations,
                // please ensure you update the wording of error messages BCP113, BCP114, BCP115 & BCP116 to reflect this!
                case ResourceScope.Tenant:
                    return checkScopes(ResourceScope.Tenant, ResourceScope.ManagementGroup, ResourceScope.Subscription, ResourceScope.ResourceGroup);
                case ResourceScope.ManagementGroup when scopeData.ManagementGroupNameProperty is not null:
                    return checkScopes(ResourceScope.Tenant, ResourceScope.ManagementGroup);
                case ResourceScope.ManagementGroup:
                    return checkScopes(ResourceScope.Tenant, ResourceScope.ManagementGroup);
                case ResourceScope.Subscription when scopeData.SubscriptionIdProperty is not null:
                    return checkScopes(ResourceScope.Tenant, ResourceScope.ManagementGroup, ResourceScope.Subscription, ResourceScope.ResourceGroup);
                case ResourceScope.Subscription:
                    return checkScopes(ResourceScope.Subscription, ResourceScope.ResourceGroup);
                case ResourceScope.ResourceGroup when scopeData.SubscriptionIdProperty is not null && scopeData.ResourceGroupProperty is not null:
                    return checkScopes(ResourceScope.Tenant, ResourceScope.ManagementGroup, ResourceScope.Subscription, ResourceScope.ResourceGroup);
                case ResourceScope.ResourceGroup when scopeData.ResourceGroupProperty is not null:
                    return checkScopes(ResourceScope.Subscription, ResourceScope.ResourceGroup);
                case ResourceScope.ResourceGroup:
                    return checkScopes(ResourceScope.ResourceGroup);
            }

            return true;
        }

        public static ImmutableDictionary<ModuleSymbol, ScopeData> GetModuleScopeInfo(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
        {
            void logInvalidScopeDiagnostic(IPositionable positionable, ResourceScope suppliedScope, ResourceScope supportedScopes)
                => diagnosticWriter.Write(positionable, x => x.UnsupportedModuleScope(suppliedScope, supportedScopes));

            var scopeInfo = new Dictionary<ModuleSymbol, ScopeData>();

            foreach (var moduleSymbol in semanticModel.Root.ModuleDeclarations)
            {
                if (moduleSymbol.Type is not ModuleType moduleType)
                {
                    // missing type should be caught during type validation
                    continue;
                }

                var scopeProperty = moduleSymbol.SafeGetBodyProperty(LanguageConstants.ResourceScopePropertyName);
                var scopeData = ScopeHelper.ValidateScope(semanticModel, logInvalidScopeDiagnostic, moduleType.ValidParentScopes, moduleSymbol.DeclaringModule.Body, scopeProperty);

                if (scopeData is null)
                {
                    scopeData = new ScopeData { RequestedScope = semanticModel.TargetScope };
                }

                if (!ScopeHelper.ValidateNestedTemplateScopeRestrictions(semanticModel, scopeData))
                {
                    if (scopeProperty is null)
                    {
                        // if there's a scope mismatch, the scope property will be required.
                        // this means a missing scope property will have already been flagged as an error by type validation.
                        continue;
                    }

                    switch (semanticModel.TargetScope)
                    {
                        case ResourceScope.Tenant:
                            diagnosticWriter.Write(scopeProperty.Value, x => x.InvalidModuleScopeForTenantScope());
                            break;
                        case ResourceScope.ManagementGroup:
                            diagnosticWriter.Write(scopeProperty.Value, x => x.InvalidModuleScopeForManagementScope());
                            break;
                        case ResourceScope.Subscription:
                            diagnosticWriter.Write(scopeProperty.Value, x => x.InvalidModuleScopeForSubscriptionScope());
                            break;
                        case ResourceScope.ResourceGroup:
                            diagnosticWriter.Write(scopeProperty.Value, x => x.InvalidModuleScopeForResourceGroup());
                            break;
                        default:
                            throw new InvalidOperationException($"Unrecognized target scope {semanticModel.TargetScope}");
                    }
                    continue;
                }

                scopeInfo[moduleSymbol] = scopeData;
            }

            return scopeInfo.ToImmutableDictionary();
        }
    }
}