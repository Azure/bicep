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
        
        private static NamedObjectType GetEnvironmentReturnType()
        {
            return new NamedObjectType("environment", TypeSymbolValidationFlags.Default, new []
            {
                new TypeProperty("name", LanguageConstants.String),
                new TypeProperty("gallery", LanguageConstants.String),
                new TypeProperty("graph", LanguageConstants.String),
                new TypeProperty("portal", LanguageConstants.String),
                new TypeProperty("graphAudience", LanguageConstants.String),
                new TypeProperty("activeDirectoryDataLake", LanguageConstants.String),
                new TypeProperty("batch", LanguageConstants.String),
                new TypeProperty("media", LanguageConstants.String),
                new TypeProperty("sqlManagement", LanguageConstants.String),
                new TypeProperty("vmImageAliasDoc", LanguageConstants.String),
                new TypeProperty("resourceManager", LanguageConstants.String),
                new TypeProperty("authentication", new NamedObjectType("authentication", TypeSymbolValidationFlags.Default, new []
                {
                    new TypeProperty("loginEndpoint", LanguageConstants.String),
                    new TypeProperty("audiences", new TypedArrayType(LanguageConstants.String, TypeSymbolValidationFlags.Default)),
                    new TypeProperty("tenant", LanguageConstants.String),
                    new TypeProperty("identityProvider", LanguageConstants.String),
                }, null)),
                new TypeProperty("suffixes", new NamedObjectType("suffixes", TypeSymbolValidationFlags.Default, new []
                {
                    new TypeProperty("acrLoginServer", LanguageConstants.String),
                    new TypeProperty("azureDatalakeAnalyticsCatalogAndJob", LanguageConstants.String),
                    new TypeProperty("azureDatalakeStoreFileSystem", LanguageConstants.String),
                    new TypeProperty("keyvaultDns", LanguageConstants.String),
                    new TypeProperty("sqlServerHostname", LanguageConstants.String),
                    new TypeProperty("storage", LanguageConstants.String),
                }, null)),
                new TypeProperty("locations", new TypedArrayType(new NamedObjectType("locations", TypeSymbolValidationFlags.Default, new []
                {
                    new  TypeProperty("id", LanguageConstants.String),
                    new  TypeProperty("name", LanguageConstants.String),
                    new  TypeProperty("displayName", LanguageConstants.String),
                    new  TypeProperty("longitude", LanguageConstants.String),
                }, null), TypeSymbolValidationFlags.Default)),
            }, null);
        }

        private static NamedObjectType GetDeploymentReturnType(ResourceScopeType targetScope)
        {
            if (targetScope.HasFlag(ResourceScopeType.ResourceGroupScope))
            {
                // deployments in the 'resourcegroup' scope do not have the 'location' property. All other scopes do.
                return new NamedObjectType("environment", TypeSymbolValidationFlags.Default, new []
                {
                    new TypeProperty("name", LanguageConstants.String),
                }, null);
            }

            return new NamedObjectType("environment", TypeSymbolValidationFlags.Default, new []
            {
                new TypeProperty("name", LanguageConstants.String),
                new TypeProperty("location", LanguageConstants.String),
            }, null);
        }

        private static IEnumerable<(FunctionOverload functionOverload, ResourceScopeType allowedScopes)> GetScopeFunctions()
        {
            // Depending on the scope of the Bicep file, different sets of function overloads are invalid - for example, you can't use 'resourceGroup()' inside a tenant-level deployment

            // Also note that some of these functions and overloads ("GetRestrictedXYZ") have not yet been implemented in full in the ARM JSON. For these, we simply
            // return an empty object type (so that dot property access doesn't work), and generate as an ARM expression "json({})" if anyone tries to access the object value.
            // This list should be kept in-sync with ScopeHelper.CanConvertToArmJson().

            var allScopes = ResourceScopeType.TenantScope | ResourceScopeType.ManagementGroupScope | ResourceScopeType.SubscriptionScope | ResourceScopeType.ResourceGroupScope;
            yield return (FunctionOverload.CreateFixed("tenant", GetRestrictedTenantReturnValue), allScopes);

            yield return (FunctionOverload.CreateFixed("managementGroup", GetRestrictedManagementGroupReturnValue), ResourceScopeType.ManagementGroupScope);
            yield return (FunctionOverload.CreateFixed("managementGroup", GetRestrictedManagementGroupReturnValue, LanguageConstants.String), ResourceScopeType.TenantScope | ResourceScopeType.ManagementGroupScope);

            yield return (FunctionOverload.CreateFixed("subscription", GetSubscriptionReturnValue), ResourceScopeType.SubscriptionScope | ResourceScopeType.ResourceGroupScope);
            yield return (FunctionOverload.CreateFixed("subscription", GetRestrictedSubscriptionReturnValue, LanguageConstants.String), allScopes);

            yield return (FunctionOverload.CreateFixed("resourceGroup", GetResourceGroupReturnValue), ResourceScopeType.ResourceGroupScope);
            yield return (FunctionOverload.CreateFixed("resourceGroup", GetRestrictedResourceGroupReturnValue, LanguageConstants.String), ResourceScopeType.SubscriptionScope | ResourceScopeType.ResourceGroupScope);
            yield return (FunctionOverload.CreateFixed("resourceGroup", GetRestrictedResourceGroupReturnValue, LanguageConstants.String, LanguageConstants.String), ResourceScopeType.SubscriptionScope | ResourceScopeType.ResourceGroupScope);
        }

        private static IEnumerable<FunctionOverload> GetAzOverloads(ResourceScopeType resourceScope)
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
            yield return FunctionOverload.CreateFixed("deployment", GetDeploymentReturnType(resourceScope));

            yield return FunctionOverload.CreateFixed("environment", GetEnvironmentReturnType());

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

        public AzNamespaceSymbol(ResourceScopeType resourceScope)
            : base("az", GetAzOverloads(resourceScope), ImmutableArray<BannedFunction>.Empty)
        {
        }
    }
}