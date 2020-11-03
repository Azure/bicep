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

        public static IReadOnlyDictionary<string, LanguageExpression>? GetScopeProperties(ExpressionConverter expressionConverter, TypeSymbol scopeType)
            => scopeType switch {
                TenantScopeType tenantScopeType => ScopeHelper.GetTenantScopeProperties(tenantScopeType),
                ManagementGroupScopeType managementGroupScopeType => ScopeHelper.GetManagementGroupScopeProperties(expressionConverter, managementGroupScopeType),
                SubscriptionScopeType subscriptionScopeType => ScopeHelper.GetSubscriptionScopeProperties(expressionConverter, subscriptionScopeType),
                ResourceGroupScopeType resourceGroupScopeType => ScopeHelper.GetResourceGroupScopeProperties(expressionConverter, resourceGroupScopeType),
                _ => null,
            };

        private static IReadOnlyDictionary<string, LanguageExpression>? GetTenantScopeProperties(TenantScopeType scopeType)
            => scopeType.Arguments.Length switch {
                0 => ToPropertyDictionary(),
                _ => null,
            };

        private static IReadOnlyDictionary<string, LanguageExpression>? GetManagementGroupScopeProperties(ExpressionConverter expressionConverter, ManagementGroupScopeType scopeType)
            => scopeType.Arguments.Length switch {
                0 => ToPropertyDictionary(),
                1 => ToPropertyDictionary(
                    scope: GetManagementGroupScopeExpression(expressionConverter.ConvertExpression(scopeType.Arguments[0].Expression))),
                _ => null,
            };

        private static IReadOnlyDictionary<string, LanguageExpression>? GetSubscriptionScopeProperties(ExpressionConverter expressionConverter, SubscriptionScopeType scopeType)
            => scopeType.Arguments.Length switch {
                0 => ToPropertyDictionary(),
                1 => ToPropertyDictionary(
                    subscriptionId: expressionConverter.ConvertExpression(scopeType.Arguments[0].Expression)),
                _ => null,
            };

        private static IReadOnlyDictionary<string, LanguageExpression>? GetResourceGroupScopeProperties(ExpressionConverter expressionConverter, ResourceGroupScopeType scopeType)
            => scopeType.Arguments.Length switch {
                0 => ToPropertyDictionary(),
                1 => ToPropertyDictionary(
                    resourceGroup: expressionConverter.ConvertExpression(scopeType.Arguments[0].Expression)),
                2 => ToPropertyDictionary(
                    subscriptionId: expressionConverter.ConvertExpression(scopeType.Arguments[0].Expression),
                    resourceGroup: expressionConverter.ConvertExpression(scopeType.Arguments[1].Expression)),
                _ => null,
            };

        public static bool CanConvertToArmJson(IResourceScopeType resourceScopeType)
            => resourceScopeType switch {
                TenantScopeType _ => false,
                ManagementGroupScopeType _ => false,
                SubscriptionScopeType subscriptionScopeType => subscriptionScopeType.Arguments.Length == 0,
                ResourceGroupScopeType resourceGroupScopeType => resourceGroupScopeType.Arguments.Length == 0,
                _ => true,
            };
    }
}