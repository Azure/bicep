// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;

namespace Bicep.Core.Semantics.Namespaces
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
            // Note: there are other properties which could be included here, but they allow you to break out of the bicep world.
            // We're going to omit them and only include what is truly necessary. If we get feature requests to expose more properties, we should discuss this further.
            // Properties such as 'template', 'templateHash', 'parameters' depend on the codegen, and feel like they could be fragile.
            IEnumerable<TypeProperty> properties = new []
            {
                new TypeProperty("name", LanguageConstants.String),
                new TypeProperty("properties", new NamedObjectType("properties", TypeSymbolValidationFlags.Default, new []
                {
                    new TypeProperty("templateLink", new NamedObjectType("properties", TypeSymbolValidationFlags.Default, new []
                    {
                        new TypeProperty("uri", LanguageConstants.String)
                    }, null))
                }, null)),
            };

            if (!targetScope.HasFlag(ResourceScopeType.ResourceGroupScope))
            {
                // deployments in the 'resourcegroup' scope do not have the 'location' property. All other scopes do.
                var locationProperty = new TypeProperty("location", LanguageConstants.String);
                properties = properties.Concat(locationProperty.AsEnumerable());
            }

            return new NamedObjectType("deployment", TypeSymbolValidationFlags.Default, properties, null);
        }

        private static IEnumerable<(FunctionOverload functionOverload, ResourceScopeType allowedScopes)> GetScopeFunctions()
        {
            // Depending on the scope of the Bicep file, different sets of function overloads are invalid - for example, you can't use 'resourceGroup()' inside a tenant-level deployment

            // Also note that some of these functions and overloads ("GetRestrictedXYZ") have not yet been implemented in full in the ARM JSON. For these, we simply
            // return an empty object type (so that dot property access doesn't work), and generate as an ARM expression "json({})" if anyone tries to access the object value.
            // This list should be kept in-sync with ScopeHelper.CanConvertToArmJson().

            var allScopes = ResourceScopeType.TenantScope | ResourceScopeType.ManagementGroupScope | ResourceScopeType.SubscriptionScope | ResourceScopeType.ResourceGroupScope;
            
            yield return (new FunctionOverloadBuilder("tenant").WithDynamicReturnType(GetRestrictedTenantReturnValue).WithFixedParameters().Build(), allScopes);

            yield return (new FunctionOverloadBuilder("managementGroup").WithDynamicReturnType(GetRestrictedManagementGroupReturnValue).WithFixedParameters().Build(), ResourceScopeType.ManagementGroupScope);
            yield return (new FunctionOverloadBuilder("managementGroup").WithDynamicReturnType(GetRestrictedManagementGroupReturnValue).WithFixedParameters(LanguageConstants.String).Build(), ResourceScopeType.TenantScope | ResourceScopeType.ManagementGroupScope);

            yield return (new FunctionOverloadBuilder("subscription").WithDynamicReturnType(GetSubscriptionReturnValue).WithFixedParameters().Build(), ResourceScopeType.SubscriptionScope | ResourceScopeType.ResourceGroupScope);
            yield return (new FunctionOverloadBuilder("subscription").WithDynamicReturnType(GetRestrictedSubscriptionReturnValue).WithFixedParameters(LanguageConstants.String).Build(), allScopes);

            yield return (new FunctionOverloadBuilder("resourceGroup").WithDynamicReturnType(GetResourceGroupReturnValue).WithFixedParameters().Build(), ResourceScopeType.ResourceGroupScope);
            yield return (new FunctionOverloadBuilder("resourceGroup").WithDynamicReturnType(GetRestrictedResourceGroupReturnValue).WithFixedParameters(LanguageConstants.String).Build(), ResourceScopeType.SubscriptionScope | ResourceScopeType.ResourceGroupScope);
            yield return (new FunctionOverloadBuilder("resourceGroup").WithDynamicReturnType(GetRestrictedResourceGroupReturnValue).WithFixedParameters(LanguageConstants.String, LanguageConstants.String).Build(), ResourceScopeType.SubscriptionScope | ResourceScopeType.ResourceGroupScope);
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
            yield return new FunctionOverloadBuilder("deployment")
                .WithReturnType(GetDeploymentReturnType(resourceScope))
                .WithFixedParameters()
                .Build();

            yield return new FunctionOverloadBuilder("environment")
                .WithReturnType(GetEnvironmentReturnType())
                .WithFixedParameters()
                .Build();

            // TODO: This is based on docs. Verify
            yield return new FunctionOverloadBuilder("resourceId")
                .WithReturnType(LanguageConstants.String)
                .WithVariableParameters(2, LanguageConstants.String)
                .Build();

            yield return new FunctionOverloadBuilder("subscriptionResourceId")
                .WithReturnType(LanguageConstants.String)
                .WithVariableParameters(2, LanguageConstants.String)
                .Build();

            yield return new FunctionOverloadBuilder("tenantResourceId")
                .WithReturnType(LanguageConstants.String)
                .WithVariableParameters(2, LanguageConstants.String)
                .Build();

            yield return new FunctionOverloadBuilder("extensionResourceId")
                .WithReturnType(LanguageConstants.String)
                .WithVariableParameters(3, LanguageConstants.String)
                .Build();

            // TODO: Not sure about return type
            yield return new FunctionOverloadBuilder("providers")
                .WithReturnType(LanguageConstants.Array)
                .WithOptionalFixedParameters(1, LanguageConstants.String, LanguageConstants.String)
                .Build();

            // TODO: return type is string[]
            yield return new FunctionOverloadBuilder("pickZones")
                .WithReturnType(LanguageConstants.Array)
                .WithOptionalFixedParameters(3, LanguageConstants.String, LanguageConstants.String, LanguageConstants.String, LanguageConstants.Int, LanguageConstants.Int)
                .Build();

            yield return new FunctionOverloadBuilder("reference")
                .WithReturnType(LanguageConstants.Object)
                .WithOptionalFixedParameters(1, LanguageConstants.String, LanguageConstants.String, LanguageConstants.String)
                .WithFlags(FunctionFlags.RequiresInlining)
                .Build();

            yield return new FunctionWildcardOverload("list*", LanguageConstants.Any, 2, 3, new[] {LanguageConstants.String, LanguageConstants.String, LanguageConstants.Object}, null, new Regex("^list[a-zA-Z]*"), FunctionFlags.RequiresInlining);
        }

        public AzNamespaceSymbol(ResourceScopeType resourceScope)
            : base("az", GetAzOverloads(resourceScope), ImmutableArray<BannedFunction>.Empty)
        {
        }
    }
}