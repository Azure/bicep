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
        private static LanguageExpression GetManagementGroupScopeExpression(LanguageExpression managementGroupName)
            => new FunctionExpression(
                "format",
                new LanguageExpression[] { 
                    new JTokenExpression("Microsoft.Management/managementGroups/{0}"),
                    managementGroupName,
                },
                Array.Empty<LanguageExpression>());

        private static IReadOnlyDictionary<string, LanguageExpression> ToPropertyDictionary(LanguageExpression? scope = null, LanguageExpression? subscriptionId = null, LanguageExpression? resourceGroup = null)
        {
            var output = new Dictionary<string, LanguageExpression>();
            if (scope != null)
            {
                output["scope"] = scope;
            }
            if (subscriptionId != null)
            {
                output["subscriptionId"] = subscriptionId;
            }
            if (resourceGroup != null)
            {
                output["resourceGroup"] = resourceGroup;
            }

            return output;
        }

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
                    return ToPropertyDictionary();
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
                    return ToPropertyDictionary();
                case 1:
                    return ToPropertyDictionary(
                        scope: GetManagementGroupScopeExpression(expressionConverter.ConvertExpression(scopeType.Arguments[0].Expression)));
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
                    return ToPropertyDictionary();
                case 1:
                    return ToPropertyDictionary(
                        subscriptionId: expressionConverter.ConvertExpression(scopeType.Arguments[0].Expression));
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
                    return ToPropertyDictionary();
                case 1:
                    ValidateScope(templateScope, AzResourceScope.Subscription, AzResourceScope.ResourceGroup);
                    return ToPropertyDictionary(
                        resourceGroup: expressionConverter.ConvertExpression(scopeType.Arguments[0].Expression));
                case 2:
                    return ToPropertyDictionary(
                        subscriptionId: expressionConverter.ConvertExpression(scopeType.Arguments[0].Expression),
                        resourceGroup: expressionConverter.ConvertExpression(scopeType.Arguments[0].Expression));
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