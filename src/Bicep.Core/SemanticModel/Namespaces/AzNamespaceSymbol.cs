// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;

namespace Bicep.Core.SemanticModel.Namespaces
{
    public class AzNamespaceSymbol : NamespaceSymbol
    {
        private static ObjectType GetRestrictedResourceGroupReturnValue(IEnumerable<FunctionArgumentSyntax> arguments)
            => new ResourceGroupScopeType(arguments, Enumerable.Empty<TypeProperty>());

        private static ObjectType GetRestrictedSubscriptionReturnValue(IEnumerable<FunctionArgumentSyntax> arguments)
            => new SubscriptionScopeType(arguments, Enumerable.Empty<TypeProperty>());

        private static ObjectType GetRestrictedManagementGroupReturnValue(IEnumerable<FunctionArgumentSyntax> arguments)
            => new ManagementGroupScopeType(arguments, Enumerable.Empty<TypeProperty>());

        private static ObjectType GetRestrictedTenantReturnValue(IEnumerable<FunctionArgumentSyntax> arguments)
            => new TenantScopeType(arguments, Enumerable.Empty<TypeProperty>());

        private static ObjectType GetResourceGroupReturnValue(IEnumerable<FunctionArgumentSyntax> arguments)
        {
            var properties = new NamedObjectType("properties", TypeSymbolValidationFlags.Default, new []
            {
                new TypeProperty("provisioningState", LanguageConstants.String),
            }, null);

            return new ResourceGroupScopeType(arguments, new []
            {
                new TypeProperty("id", LanguageConstants.String),
                new TypeProperty("name", LanguageConstants.String),
                new TypeProperty("type", LanguageConstants.String),
                new TypeProperty("location", LanguageConstants.String),
                new TypeProperty("managedBy", LanguageConstants.String),
                new TypeProperty("tags", LanguageConstants.Tags),
                new TypeProperty("properties", properties),
            });
        }

        private static ObjectType GetSubscriptionReturnValue(IEnumerable<FunctionArgumentSyntax> arguments)
        {
            return new SubscriptionScopeType(arguments, new []
            {
                new TypeProperty("id", LanguageConstants.String),
                new TypeProperty("subscriptionId", LanguageConstants.String),
                new TypeProperty("tenantId", LanguageConstants.String),
                new TypeProperty("displayName", LanguageConstants.String),
            });
        }

        private static IEnumerable<(FunctionOverload functionOverload, AzResourceScope allowedScopes)> GetScopeFunctions()
        {
            // Depending on the scope of the Bicep file, different sets of function overloads are invalid - for example, you can't use 'resourceGroup()' inside a tenant-level deployment

            // Also note that some of these functions and overloads ("GetRestrictedXYZ") have not yet been implemented in full in the ARM JSON. For these, we simply
            // return an empty object type (so that dot property access doesn't work), and generate as an ARM expression "json({})" if anyone tries to access the object value.
            // This list should be kept in-sync with ScopeHelper.CanConvertToArmJson().

            var allScopes = AzResourceScope.Tenant | AzResourceScope.ManagementGroup | AzResourceScope.Subscription | AzResourceScope.ResourceGroup;
            yield return (FunctionOverload.CreateFixed("tenant", GetRestrictedTenantReturnValue), allScopes);

            yield return (FunctionOverload.CreateFixed("managementGroup", GetRestrictedManagementGroupReturnValue), AzResourceScope.ManagementGroup);
            yield return (FunctionOverload.CreateFixed("managementGroup", GetRestrictedManagementGroupReturnValue, LanguageConstants.String), AzResourceScope.Tenant | AzResourceScope.ManagementGroup);

            yield return (FunctionOverload.CreateFixed("subscription", GetSubscriptionReturnValue), AzResourceScope.Subscription | AzResourceScope.ResourceGroup);
            yield return (FunctionOverload.CreateFixed("subscription", GetRestrictedSubscriptionReturnValue, LanguageConstants.String), allScopes);

            yield return (FunctionOverload.CreateFixed("resourceGroup", GetResourceGroupReturnValue), AzResourceScope.ResourceGroup);
            yield return (FunctionOverload.CreateFixed("resourceGroup", GetRestrictedResourceGroupReturnValue, LanguageConstants.String), AzResourceScope.Subscription | AzResourceScope.ResourceGroup);
            yield return (FunctionOverload.CreateFixed("resourceGroup", GetRestrictedResourceGroupReturnValue, LanguageConstants.String, LanguageConstants.String), AzResourceScope.Subscription | AzResourceScope.ResourceGroup);
        }

        private static IEnumerable<FunctionOverload> GetAzOverloads(AzResourceScope resourceScope)
        {
            foreach (var (functionOverload, allowedScopes) in GetScopeFunctions())
            {
                // we only include it if it's valid at all of the scopes that the template is valid at
                if (resourceScope == (resourceScope & allowedScopes))
                {
                    yield return functionOverload;
                }
                // TODO: add banned function to explain why a given function isn't available
            }

            // TODO: Add schema for return type
            yield return FunctionOverload.CreateFixed("deployment", LanguageConstants.Object);

            // TODO: Add schema for return type
            yield return FunctionOverload.CreateFixed("environment", LanguageConstants.Object);

            // TODO: This is based on docs. Verify
            yield return FunctionOverload.CreateWithVarArgs("resourceId", LanguageConstants.String, 2, LanguageConstants.String);
            yield return FunctionOverload.CreateWithVarArgs("subscriptionResourceId", LanguageConstants.String, 2, LanguageConstants.String);
            yield return FunctionOverload.CreateWithVarArgs("tenantResourceId", LanguageConstants.String, 2, LanguageConstants.String);
            yield return FunctionOverload.CreateWithVarArgs("extensionResourceId", LanguageConstants.String, 3, LanguageConstants.String);

            // TODO: Not sure about return type
            yield return new FunctionOverload("providers", LanguageConstants.Array, 1, 2, Enumerable.Repeat(LanguageConstants.String, 2), null);

            // TODO: return type is string[]
            yield return new FunctionOverload("pickZones", LanguageConstants.Array, 3, 5, new[] {LanguageConstants.String, LanguageConstants.String, LanguageConstants.String, LanguageConstants.Int, LanguageConstants.Int}, null);

            // the use of FunctionPlacementConstraints.Resources prevents use of these functions anywhere where they can't be directly inlined into a resource body
            yield return new FunctionOverload("reference", LanguageConstants.Object, 1, 3, Enumerable.Repeat(LanguageConstants.String, 3), null, FunctionFlags.RequiresInlining);
            yield return new FunctionWildcardOverload("list*", LanguageConstants.Any, 2, 3, new[] { LanguageConstants.String, LanguageConstants.String, LanguageConstants.Object }, null, new Regex("^list[a-zA-Z]+"), FunctionFlags.RequiresInlining);
        }

        public AzNamespaceSymbol(AzResourceScope resourceScope)
            : base("az", GetAzOverloads(resourceScope), ImmutableArray<BannedFunction>.Empty)
        {
        }
    }
}