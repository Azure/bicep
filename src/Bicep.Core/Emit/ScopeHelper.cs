﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.Extensions;
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
        }

        public static ScopeData? TryGetScopeData(ResourceScope currentScope, TypeSymbol scopeType)
        {
            switch (currentScope)
            {
                case ResourceScope.Tenant:
                    switch (scopeType)
                    {
                        case TenantScopeType:
                            return new ScopeData { 
                                RequestedScope = ResourceScope.Tenant };
                        case ManagementGroupScopeType managementGroupScopeType when managementGroupScopeType.Arguments.Length == 1:
                            return new ScopeData { 
                                RequestedScope = ResourceScope.ManagementGroup,
                                ManagementGroupNameProperty = managementGroupScopeType.Arguments[0].Expression };
                        case SubscriptionScopeType subscriptionScopeType when subscriptionScopeType.Arguments.Length == 1:
                            return new ScopeData { 
                                RequestedScope = ResourceScope.Subscription, 
                                SubscriptionIdProperty = subscriptionScopeType.Arguments[0].Expression };
                        case ResourceGroupScopeType resourceGroupScopeType when resourceGroupScopeType.Arguments.Length == 2:
                            return new ScopeData {
                                RequestedScope = ResourceScope.ResourceGroup,
                                SubscriptionIdProperty = resourceGroupScopeType.Arguments[0].Expression,
                                ResourceGroupProperty = resourceGroupScopeType.Arguments[1].Expression };
                    }
                    break;
                case ResourceScope.ManagementGroup:
                    switch (scopeType)
                    {
                        case TenantScopeType:
                            return new ScopeData { 
                                RequestedScope = ResourceScope.Tenant };
                        case ManagementGroupScopeType managementGroupScopeType when managementGroupScopeType.Arguments.Length == 0:
                            return new ScopeData { 
                                RequestedScope = ResourceScope.ManagementGroup };
                        case ManagementGroupScopeType managementGroupScopeType when managementGroupScopeType.Arguments.Length == 1:
                            return new ScopeData { 
                                RequestedScope = ResourceScope.ManagementGroup, 
                                ManagementGroupNameProperty = managementGroupScopeType.Arguments[0].Expression };
                        case SubscriptionScopeType subscriptionScopeType when subscriptionScopeType.Arguments.Length == 1:
                            return new ScopeData { 
                                RequestedScope = ResourceScope.Subscription, 
                                SubscriptionIdProperty = subscriptionScopeType.Arguments[0].Expression };
                        case ResourceGroupScopeType resourceGroupScopeType when resourceGroupScopeType.Arguments.Length == 2:
                            return new ScopeData {
                                RequestedScope = ResourceScope.ResourceGroup,
                                SubscriptionIdProperty = resourceGroupScopeType.Arguments[0].Expression,
                                ResourceGroupProperty = resourceGroupScopeType.Arguments[1].Expression };
                    }
                    break;
                case ResourceScope.Subscription:
                    switch (scopeType)
                    {
                        case TenantScopeType:
                            return new ScopeData { 
                                RequestedScope = ResourceScope.Tenant };
                        case SubscriptionScopeType subscriptionScopeType when subscriptionScopeType.Arguments.Length == 0:
                            return new ScopeData { 
                                RequestedScope = ResourceScope.Subscription };
                        case SubscriptionScopeType subscriptionScopeType when subscriptionScopeType.Arguments.Length == 1:
                            return new ScopeData { 
                                RequestedScope = ResourceScope.Subscription, 
                                SubscriptionIdProperty = subscriptionScopeType.Arguments[0].Expression };
                        case ResourceGroupScopeType resourceGroupScopeType when resourceGroupScopeType.Arguments.Length == 1:
                            return new ScopeData { 
                                RequestedScope = ResourceScope.ResourceGroup, 
                                ResourceGroupProperty = resourceGroupScopeType.Arguments[0].Expression };
                    }
                    break;
                case ResourceScope.ResourceGroup:
                    switch (scopeType)
                    {
                        case TenantScopeType:
                            return new ScopeData { 
                                RequestedScope = ResourceScope.Tenant };
                        case ResourceGroupScopeType resourceGroupScopeType when resourceGroupScopeType.Arguments.Length == 0:
                            return new ScopeData { 
                                RequestedScope = ResourceScope.ResourceGroup };
                        case ResourceGroupScopeType resourceGroupScopeType when resourceGroupScopeType.Arguments.Length == 1:
                            return new ScopeData { 
                                RequestedScope = ResourceScope.ResourceGroup,
                                ResourceGroupProperty = resourceGroupScopeType.Arguments[0].Expression };
                        case ResourceGroupScopeType resourceGroupScopeType when resourceGroupScopeType.Arguments.Length == 2:
                            return new ScopeData {
                                RequestedScope = ResourceScope.ResourceGroup,
                                SubscriptionIdProperty = resourceGroupScopeType.Arguments[0].Expression,
                                ResourceGroupProperty = resourceGroupScopeType.Arguments[1].Expression };
                    }
                    break;
            }

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
                default:
                    throw new NotImplementedException($"Cannot format resourceId for scope {scopeData.RequestedScope}");
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
                    throw new NotImplementedException($"Cannot format resourceId for scope {scopeData.RequestedScope}");
            }
        }
    }
}