// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using Arm.Expression.Expressions;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;

namespace Bicep.Core.Emit
{
    public static class ScopeHelper
    {
        private static LanguageExpression GetDeploymentScopeExpression()
            => new FunctionExpression(
                "deployment",
                Array.Empty<LanguageExpression>(),
                new LanguageExpression[]
                {
                    new JTokenExpression("scope"),
                });

        private static LanguageExpression GetSubscriptionIdExpression()
            => new FunctionExpression(
                "subscription",
                Array.Empty<LanguageExpression>(),
                new LanguageExpression[] {
                    new JTokenExpression("subscriptionId"),
                });

        private static LanguageExpression GetResourceGroupNameExpression()
            => new FunctionExpression(
                "resourceGroup",
                Array.Empty<LanguageExpression>(),
                new LanguageExpression[] {
                    new JTokenExpression("name"),
                });

        private static LanguageExpression GetManagementGroupScopeExpression(LanguageExpression managementGroupName)
            => new FunctionExpression(
                "format",
                new LanguageExpression[] { 
                    new JTokenExpression("Microsoft.Management/managementGroups/{0}"),
                    managementGroupName,
                },
                Array.Empty<LanguageExpression>());

        private static IReadOnlyDictionary<string, LanguageExpression> ToScopePropertyDictionary(LanguageExpression scope)
            => new Dictionary<string, LanguageExpression>
            {
                ["scope"] = scope,
            };

        private static IReadOnlyDictionary<string, LanguageExpression> ToSubscriptionResourceGroupPropertyDictionary(LanguageExpression subscriptionId, LanguageExpression resourceGroupName)
            => new Dictionary<string, LanguageExpression>
            {
                ["subscriptionId"] = subscriptionId,
                ["resourceGroup"] = resourceGroupName,
            };

        private static IReadOnlyDictionary<string, LanguageExpression> ToSubscriptionPropertyDictionary(LanguageExpression subscriptionId)
            => new Dictionary<string, LanguageExpression>
            {
                ["subscriptionId"] = subscriptionId,
            };

        public static IReadOnlyDictionary<string, LanguageExpression> GetScopeProperties(AzResourceScope templateScope, ExpressionConverter expressionConverter, TypeSymbol scopeType)
            => scopeType switch {
                TenantScopeType tenantScopeType => ScopeHelper.GetTenantScopeProperties(templateScope, tenantScopeType),
                ManagementGroupScopeType managementGroupScopeType => ScopeHelper.GetManagementGroupScopeProperties(templateScope, expressionConverter, managementGroupScopeType),
                SubscriptionScopeType subscriptionScopeType => ScopeHelper.GetSubscriptionScopeProperties(templateScope, expressionConverter, subscriptionScopeType),
                ResourceGroupScopeType resourceGroupScopeType => ScopeHelper.GetResourceGroupScopeProperties(templateScope, expressionConverter, resourceGroupScopeType),
                // TODO: type checker for this as part of semantic model
                _ => throw new ArgumentException($"Unexpected scope type {scopeType.GetType()}"),
            };

        private static IReadOnlyDictionary<string, LanguageExpression> GetTenantScopeProperties(AzResourceScope templateScope, TenantScopeType scopeType)
        {
            switch (scopeType.Arguments.Length)
            {
                case 0:
                    ValidateScope(templateScope, AzResourceScope.Tenant);
                    return ToScopePropertyDictionary(new JTokenExpression("/"));
                default:
                    // TODO: type checker for this as part of semantic model
                    throw new ArgumentException($"Unexpected number of arguments for {scopeType.Name}");
            }
        }

        private static IReadOnlyDictionary<string, LanguageExpression> GetManagementGroupScopeProperties(AzResourceScope templateScope, ExpressionConverter expressionConverter, ManagementGroupScopeType scopeType)
        {
            switch (scopeType.Arguments.Length)
            {
                case 0:
                    ValidateScope(templateScope, AzResourceScope.ManagementGroup);
                    return ToScopePropertyDictionary(GetDeploymentScopeExpression());
                case 1:
                    return ToScopePropertyDictionary(GetManagementGroupScopeExpression(
                        expressionConverter.ConvertExpression(scopeType.Arguments[0].Expression)));
                default:
                    // TODO: type checker for this as part of semantic model
                    throw new ArgumentException($"Unexpected number of arguments for {scopeType.Name}");
            }
        }

        private static IReadOnlyDictionary<string, LanguageExpression> GetSubscriptionScopeProperties(AzResourceScope templateScope, ExpressionConverter expressionConverter, SubscriptionScopeType scopeType)
        {
            switch (scopeType.Arguments.Length)
            {
                case 0:
                    ValidateScope(templateScope, AzResourceScope.Subscription);
                    return ToSubscriptionPropertyDictionary(
                        GetSubscriptionIdExpression());
                case 1:
                    return ToSubscriptionPropertyDictionary(
                        expressionConverter.ConvertExpression(scopeType.Arguments[0].Expression));
                default:
                    // TODO: type checker for this as part of semantic model
                    throw new ArgumentException($"Unexpected number of arguments for {scopeType.Name}");
            }
        }

        private static IReadOnlyDictionary<string, LanguageExpression> GetResourceGroupScopeProperties(AzResourceScope templateScope, ExpressionConverter expressionConverter, ResourceGroupScopeType scopeType)
        {
            switch (scopeType.Arguments.Length)
            {
                case 0:
                    ValidateScope(templateScope, AzResourceScope.ResourceGroup);
                    return ToSubscriptionResourceGroupPropertyDictionary(
                        subscriptionId: GetSubscriptionIdExpression(),
                        resourceGroupName: GetResourceGroupNameExpression());
                case 1:
                    return ToSubscriptionResourceGroupPropertyDictionary(
                        subscriptionId: GetSubscriptionIdExpression(),
                        resourceGroupName: expressionConverter.ConvertExpression(scopeType.Arguments[0].Expression));
                case 2:
                    return ToSubscriptionResourceGroupPropertyDictionary(
                        subscriptionId: expressionConverter.ConvertExpression(scopeType.Arguments[0].Expression),
                        resourceGroupName: expressionConverter.ConvertExpression(scopeType.Arguments[1].Expression));
                default:
                    // TODO: type checker for this as part of semantic model
                    throw new ArgumentException($"Unexpected number of arguments for {scopeType.Name}");
            }
        }

        private static void ValidateScope(AzResourceScope templateScope, params AzResourceScope[] expectedScopes)
        {
            if (!expectedScopes.Contains(templateScope))
            {
                throw new ArgumentException($"Unexpected template scope");
            }
        }
    }
}