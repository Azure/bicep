// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.SemanticModel.Namespaces
{
    [Flags]
    public enum AzResourceScope
    {
        None = 0,

        Tenant = 1 << 0,

        ManagementGroup = 1 << 1,

        Subscription = 1 << 2,

        ResourceGroup = 1 << 3,
    }

    public class AzNamespaceSymbol : NamespaceSymbol
    {
        private static ObjectType GetRestrictedResourceGroupReturnValue()
        {
            return new NamedObjectType("resourceGroup", TypeSymbolValidationFlags.DeclaresResourceScope, Enumerable.Empty<TypeProperty>(), null);
        }

        private static ObjectType GetRestrictedSubscriptionReturnValue()
        {
            return new NamedObjectType("subscription", TypeSymbolValidationFlags.DeclaresResourceScope, Enumerable.Empty<TypeProperty>(), null);
        }

        private static ObjectType GetRestrictedManagementGroupReturnValue()
        {
            return new NamedObjectType("managementGroup", TypeSymbolValidationFlags.DeclaresResourceScope, Enumerable.Empty<TypeProperty>(), null);
        }

        private static ObjectType GetRestrictedTenantReturnValue()
        {
            return new NamedObjectType("tenant", TypeSymbolValidationFlags.DeclaresResourceScope, Enumerable.Empty<TypeProperty>(), null);
        }

        private static ObjectType GetResourceGroupReturnValue()
        {
            var properties = new NamedObjectType("properties", TypeSymbolValidationFlags.Default, new []
            {
                new TypeProperty("provisioningState", LanguageConstants.String),
            }, null);

            return new NamedObjectType("resourceGroup", TypeSymbolValidationFlags.DeclaresResourceScope, new []
            {
                new TypeProperty("id", LanguageConstants.String),
                new TypeProperty("name", LanguageConstants.String),
                new TypeProperty("type", LanguageConstants.String),
                new TypeProperty("location", LanguageConstants.String),
                new TypeProperty("managedBy", LanguageConstants.String),
                new TypeProperty("tags", LanguageConstants.Tags),
                new TypeProperty("properties", properties),
            }, null);
        }

        private static ObjectType GetSubscriptionReturnValue()
        {
            return new NamedObjectType("subscription", TypeSymbolValidationFlags.DeclaresResourceScope, new []
            {
                new TypeProperty("id", LanguageConstants.String),
                new TypeProperty("subscriptionId", LanguageConstants.String),
                new TypeProperty("tenantId", LanguageConstants.String),
                new TypeProperty("displayName", LanguageConstants.String),
            }, null);
        }

        private static IEnumerable<(FunctionOverload functionOverload, AzResourceScope allowedScopes)> GetScopeFunctions()
        {
            var allScopes = AzResourceScope.Tenant | AzResourceScope.ManagementGroup | AzResourceScope.Subscription | AzResourceScope.ResourceGroup;
            yield return (FunctionOverload.CreateFixed("tenant", GetRestrictedTenantReturnValue()), allScopes);

            yield return (FunctionOverload.CreateFixed("managementGroup", GetRestrictedManagementGroupReturnValue()), AzResourceScope.ManagementGroup);
            yield return (FunctionOverload.CreateFixed("managementGroup", GetRestrictedManagementGroupReturnValue(), LanguageConstants.String), allScopes);

            yield return (FunctionOverload.CreateFixed("subscription", GetSubscriptionReturnValue()), AzResourceScope.Subscription | AzResourceScope.ResourceGroup);
            yield return (FunctionOverload.CreateFixed("subscription", GetRestrictedSubscriptionReturnValue(), LanguageConstants.String), allScopes);

            yield return (FunctionOverload.CreateFixed("resourceGroup", GetResourceGroupReturnValue()), AzResourceScope.ResourceGroup);
            yield return (FunctionOverload.CreateFixed("resourceGroup", GetRestrictedResourceGroupReturnValue(), LanguageConstants.String), AzResourceScope.Subscription | AzResourceScope.ResourceGroup);
            yield return (FunctionOverload.CreateFixed("resourceGroup", GetRestrictedResourceGroupReturnValue(), LanguageConstants.String, LanguageConstants.String), allScopes);
        }

        private static IEnumerable<FunctionOverload> GetAzOverloads(AzResourceScope scope)
        {
            foreach (var (functionOverload, allowedScopes) in GetScopeFunctions())
            {
                // we only include it if it's valid at all of the scopes that the template is valid at
                if (scope == (scope & allowedScopes))
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

        private static readonly ImmutableArray<FunctionOverload> AzOverloads = GetAzOverloads(AzResourceScope.ResourceGroup).ToImmutableArray();

        public AzNamespaceSymbol() : base("az", AzOverloads, ImmutableArray<BannedFunction>.Empty)
        {
        }
    }
}

