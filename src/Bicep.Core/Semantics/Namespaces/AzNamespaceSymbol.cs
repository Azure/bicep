// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Bicep.Core.Diagnostics;
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
                        new TypeProperty("id", LanguageConstants.String),
                        new TypeProperty("uri", LanguageConstants.String),
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
            // return an empty object type (so that dot property access doesn't work), and generate as an ARM expression "createObject()" if anyone tries to access the object value.
            // This list should be kept in-sync with ScopeHelper.CanConvertToArmJson().

            var allScopes = ResourceScopeType.TenantScope | ResourceScopeType.ManagementGroupScope | ResourceScopeType.SubscriptionScope | ResourceScopeType.ResourceGroupScope;

            yield return (
                new FunctionOverloadBuilder("tenant")
                    .WithDynamicReturnType(GetRestrictedTenantReturnValue)
                    .WithDescription("Returns the current tenant scope.")
                    .Build(),
                allScopes);

            yield return (
                new FunctionOverloadBuilder("managementGroup")
                    .WithDynamicReturnType(GetRestrictedManagementGroupReturnValue)
                    .WithDescription("Returns the current management group scope. **This function can only be used in managementGroup deployments.**")
                    .Build(),
                ResourceScopeType.ManagementGroupScope);
            yield return (
                new FunctionOverloadBuilder("managementGroup")
                    .WithDynamicReturnType(GetRestrictedManagementGroupReturnValue)
                    .WithDescription("Returns the scope for a named management group.")
                    .WithRequiredParameter("name", LanguageConstants.String, "The unique identifier of the management group (not the display name).")
                    .Build(),
                ResourceScopeType.TenantScope | ResourceScopeType.ManagementGroupScope);

            yield return (
                new FunctionOverloadBuilder("subscription")
                    .WithDynamicReturnType(GetSubscriptionReturnValue)
                    .WithDescription("Returns the subscription scope for the current deployment. **This function can only be used in subscription and resourceGroup deployments.**")
                    .Build(),
                ResourceScopeType.SubscriptionScope | ResourceScopeType.ResourceGroupScope);
            yield return (
                new FunctionOverloadBuilder("subscription")
                    .WithDynamicReturnType(GetRestrictedSubscriptionReturnValue)
                    .WithDescription("Returns a named subscription scope. **This function can only be used in subscription and resourceGroup deployments.**")
                    .WithRequiredParameter("subscriptionId", LanguageConstants.String, "The subscription ID")
                    .Build(),
                allScopes);

            yield return (
                new FunctionOverloadBuilder("resourceGroup")
                    .WithDynamicReturnType(GetResourceGroupReturnValue)
                    .WithDescription("Returns the current resource group scope. **This function can only be used in resourceGroup deployments.**")
                    .Build(),
                ResourceScopeType.ResourceGroupScope);
            yield return (
                new FunctionOverloadBuilder("resourceGroup")
                    .WithDynamicReturnType(GetRestrictedResourceGroupReturnValue)
                    .WithDescription("Returns a named resource group scope. **This function can only be used in subscription and resourceGroup deployments.**")
                    .WithRequiredParameter("resourceGroupName", LanguageConstants.String, "The resource group name")
                    .Build(),
                ResourceScopeType.SubscriptionScope | ResourceScopeType.ResourceGroupScope);
            yield return (
                new FunctionOverloadBuilder("resourceGroup")
                    .WithDynamicReturnType(GetRestrictedResourceGroupReturnValue)
                    .WithDescription("Returns a named resource group scope. **This function can only be used in subscription and resourceGroup deployments.**")
                    .WithRequiredParameter("subscriptionId", LanguageConstants.String, "The subscription ID")
                    .WithRequiredParameter("resourceGroupName", LanguageConstants.String, "The resource group name")
                    .Build(),
                ResourceScopeType.TenantScope | ResourceScopeType.ManagementGroupScope | ResourceScopeType.SubscriptionScope | ResourceScopeType.ResourceGroupScope);
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
                .WithDescription("Returns information about the current deployment operation.")
                .Build();

            yield return new FunctionOverloadBuilder("environment")
                .WithReturnType(GetEnvironmentReturnType())
                .WithDescription("Returns information about the Azure environment used for deployment.")
                .Build();

            // TODO: This is based on docs. Verify
            // the resourceId function relies on leading optional parameters that are disambiguated at runtime
            // modeling this as multiple overload with all possible permutations of the leading parameters
            const string resourceIdDescription = "Returns the unique identifier of a resource. You use this function when the resource name is ambiguous or not provisioned within the same template. The format of the returned identifier varies based on whether the deployment happens at the scope of a resource group, subscription, management group, or tenant.";
            yield return new FunctionOverloadBuilder("resourceId")
                .WithReturnType(LanguageConstants.String)
                .WithDescription(resourceIdDescription)
                .WithRequiredParameter("resourceType", LanguageConstants.String, "Type of resource including resource provider namespace")
            //base.VisitInstanceFunctionCallSyntax(syntax);
                .WithVariableParameter("resourceName", LanguageConstants.String, minimumCount: 1, "The resource name segment")
                .Build();

            yield return new FunctionOverloadBuilder("resourceId")
                .WithReturnType(LanguageConstants.String)
                .WithDescription(resourceIdDescription)
                .WithRequiredParameter("subscriptionId", LanguageConstants.String, "The subscription ID")
                .WithRequiredParameter("resourceType", LanguageConstants.String, "Type of resource including resource provider namespace")
                .WithVariableParameter("resourceName", LanguageConstants.String, minimumCount: 1, "The resource name segment")
                .Build(); 
            
            yield return new FunctionOverloadBuilder("resourceId")
                .WithReturnType(LanguageConstants.String)
                .WithDescription(resourceIdDescription)
                .WithRequiredParameter("resourceGroupName", LanguageConstants.String, "The resource group name")
                .WithRequiredParameter("resourceType", LanguageConstants.String, "Type of resource including resource provider namespace")
                .WithVariableParameter("resourceName", LanguageConstants.String, minimumCount: 1, "The resource name segment")
                .Build();

            yield return new FunctionOverloadBuilder("resourceId")
                .WithReturnType(LanguageConstants.String)
                .WithDescription(resourceIdDescription)
                .WithRequiredParameter("subscriptionId", LanguageConstants.String, "The subscription ID")
                .WithRequiredParameter("resourceGroupName", LanguageConstants.String, "The resource group name")
                .WithRequiredParameter("resourceType", LanguageConstants.String, "Type of resource including resource provider namespace")
                .WithVariableParameter("resourceName", LanguageConstants.String, minimumCount: 1, "The resource name segment")
                .Build();

            // the subscriptionResourceId function relies on leading optional parameters that are disambiguated at runtime
            // modeling this as multiple overload with all possible permutations of the leading parameters
            const string subscriptionResourceIdDescription = "Returns the unique identifier for a resource deployed at the subscription level.";
            yield return new FunctionOverloadBuilder("subscriptionResourceId")
                .WithReturnType(LanguageConstants.String)
                .WithDescription(subscriptionResourceIdDescription)
                .WithRequiredParameter("resourceType", LanguageConstants.String, "Type of resource including resource provider namespace")
                .WithVariableParameter("resourceName", LanguageConstants.String, minimumCount: 1, "The resource name segment")
                .Build();

            yield return new FunctionOverloadBuilder("subscriptionResourceId")
                .WithReturnType(LanguageConstants.String)
                .WithDescription(subscriptionResourceIdDescription)
                .WithRequiredParameter("subscriptionId", LanguageConstants.String, "The subscription ID")
                .WithRequiredParameter("resourceType", LanguageConstants.String, "Type of resource including resource provider namespace")
                .WithVariableParameter("resourceName", LanguageConstants.String, minimumCount: 1, "The resource name segment")
                .Build();

            yield return new FunctionOverloadBuilder("tenantResourceId")
                .WithReturnType(LanguageConstants.String)
                .WithDescription("Returns the unique identifier for a resource deployed at the tenant level.")
                .WithRequiredParameter("resourceType", LanguageConstants.String, "Type of resource including resource provider namespace")
                .WithVariableParameter("resourceName", LanguageConstants.String, minimumCount: 1, "The resource name segment")
                .Build();

            yield return new FunctionOverloadBuilder("extensionResourceId")
                .WithReturnType(LanguageConstants.String)
                .WithDescription("Returns the resource ID for an [extension](https://docs.microsoft.com/en-us/azure/azure-resource-manager/management/extension-resource-types) resource, which is a resource type that is applied to another resource to add to its capabilities.")
                .WithRequiredParameter("resourceId",LanguageConstants.String, "The resource ID for the resource that the extension resource is applied to")
                .WithRequiredParameter("resourceType",LanguageConstants.String, "Type of the extension resource including resource provider namespace")
                .WithVariableParameter("resourceName",LanguageConstants.String, minimumCount: 1, "The extension resource name segment")
                .Build();

            // TODO: Not sure about return type
            yield return new FunctionOverloadBuilder("providers")
                .WithReturnType(LanguageConstants.Array)
                .WithDescription("Returns information about a resource provider and its supported resource types. If you don't provide a resource type, the function returns all the supported types for the resource provider.")
                .WithRequiredParameter("providerNamespace",LanguageConstants.String, "the namespace of the provider")
                .WithOptionalParameter("resourceType",LanguageConstants.String, "The type of resource within the specified namespace")
                .Build();

            // TODO: return type is string[]
            // TODO: Location param should be of location type if we ever add it
            yield return new FunctionOverloadBuilder("pickZones")
                .WithReturnType(LanguageConstants.Array)
                .WithDescription("Determines whether a resource type supports zones for a region.")
                .WithRequiredParameter("providerNamespace", LanguageConstants.String, "The resource provider namespace for the resource type to check for zone support")
                .WithRequiredParameter("resourceType", LanguageConstants.String, "The resource type to check for zone support")
                .WithRequiredParameter("location", LanguageConstants.String, "The region to check for zone support")
                .WithOptionalParameter("numberOfZones", LanguageConstants.Int, "The number of logical zones to return. The default is 1. The number must a positive integer from 1 to 3. Use 1 for single-zoned resources. For multi-zoned resources, the value must be less than or equal to the number of supported zones.")
                .WithOptionalParameter("offset", LanguageConstants.Int, "The offset from the starting logical zone. The function returns an error if offset plus numberOfZones exceeds the number of supported zones.")
                .Build();

            // TODO: Change 'Full' to literal type after verifying in the runtime source
            yield return new FunctionOverloadBuilder("reference")
                .WithReturnType(LanguageConstants.Object)
                .WithDescription("Returns an object representing a resource's runtime state.")
                .WithRequiredParameter("resourceNameOrIdentifier", LanguageConstants.String, "Name or unique identifier of a resource. When referencing a resource in the current template, provide only the resource name as a parameter. When referencing a previously deployed resource or when the name of the resource is ambiguous, provide the resource ID.")
                .WithOptionalParameter("apiVersion", LanguageConstants.String, "API version of the specified resource. This parameter is required when the resource isn't provisioned within same template.")
                .WithOptionalParameter("full", LanguageConstants.String, "Value that specifies whether to return the full resource object. If you don't specify 'Full', only the properties object of the resource is returned. The full object includes values such as the resource ID and location.")
                .WithFlags(FunctionFlags.RequiresInlining)
                .Build();

            // TODO: Doc parameters need an update
            yield return new FunctionWildcardOverloadBuilder("list*", new Regex("^list[a-zA-Z]*"))
                .WithReturnType(LanguageConstants.Any)
                .WithDescription("The syntax for this function varies by name of the list operations. Each implementation returns values for the resource type that supports a list operation. The operation name must start with list. Some common usages are `listKeys`, `listKeyValue`, and `listSecrets`.")
                .WithRequiredParameter("resourceNameOrIdentifier", LanguageConstants.String, "Name or unique identifier of a resource. When referencing a resource in the current template, provide only the resource name as a parameter. When referencing a previously deployed resource or when the name of the resource is ambiguous, provide the resource ID.")
                .WithRequiredParameter("apiVersion",LanguageConstants.String, "API version of resource runtime state. Typically, in the format, yyyy-mm-dd.")
                .WithOptionalParameter("functionValues", LanguageConstants.Object, "An object that has values for the function. Only provide this object for functions that support receiving an object with parameter values, such as listAccountSas on a storage account. An example of passing function values is shown in this article.")
                .WithFlags(FunctionFlags.RequiresInlining)
                .Build();
        }

        private static IEnumerable<Decorator> GetAzDecorators()
        {
            static DecoratorValidator ValidateTargetType(TypeSymbol attachableType) =>
                (decoratorName, decoratorSyntax, targetType, _, diagnosticWriter) =>
                {
                    if (!TypeValidator.AreTypesAssignable(targetType, attachableType))
                    {
                        diagnosticWriter.Write(DiagnosticBuilder.ForPosition(decoratorSyntax).CannotAttacheDecoratorToTarget(decoratorName, attachableType, targetType));
                    }
                };

            static DecoratorEvaluator MergeToTargetObject(string propertyName, Func<DecoratorSyntax, SyntaxBase> propertyValueSelector) =>
                (decoratorSyntax, _, targetObject) =>
                    targetObject.MergeProperty(propertyName, propertyValueSelector(decoratorSyntax));

            static SyntaxBase SingleArgumentSelector(DecoratorSyntax decoratorSyntax) => decoratorSyntax.Arguments.Single().Expression;

            yield return new DecoratorBuilder("secure")
                .WithDescription("Makes the parameter a secure parameter.")
                .WithFlags(FunctionFlags.ParameterDecorator)
                .WithEvaluator((_, targetType, targetObject) =>
                {
                    if (ReferenceEquals(targetType, LanguageConstants.String))
                    {
                        return targetObject.MergeProperty("type", "secureString");
                    }

                    if (ReferenceEquals(targetType, LanguageConstants.Object))
                    {
                        return targetObject.MergeProperty("type", "secureObject");
                    }

                    return targetObject;
                })
                .Build();

            yield return new DecoratorBuilder("allowed")
                .WithDescription("Defines the allowed values of the parameter.")
                .WithRequiredParameter("values", LanguageConstants.Array, "The allowed values.")
                .WithFlags(FunctionFlags.ParameterDecorator)
                .WithValidator((_, decoratorSyntax, targetType, typeManager, diagnosticWriter) =>
                    TypeValidator.NarrowTypeAndCollectDiagnostics(
                        typeManager,
                        decoratorSyntax.Arguments.Single().Expression,
                        new TypedArrayType(targetType, TypeSymbolValidationFlags.Default),
                        diagnosticWriter))
                .WithEvaluator(MergeToTargetObject("allowedValues", SingleArgumentSelector))
                .Build();

            yield return new DecoratorBuilder("minValue")
                .WithDescription("Defines the minimum value of the parameter.")
                .WithRequiredParameter("value", LanguageConstants.Int, "The minimum value.")
                .WithFlags(FunctionFlags.ParameterDecorator)
                .WithValidator(ValidateTargetType(LanguageConstants.Int))
                .WithEvaluator(MergeToTargetObject("minValue", SingleArgumentSelector))
                .Build();

            yield return new DecoratorBuilder("maxValue")
                .WithDescription("Defines the maximum value of the parameter.")
                .WithRequiredParameter("value", LanguageConstants.Int, "The maximum value.")
                .WithFlags(FunctionFlags.ParameterDecorator)
                .WithValidator(ValidateTargetType(LanguageConstants.Int))
                .WithEvaluator(MergeToTargetObject("maxValue", SingleArgumentSelector))
                .Build();

            yield return new DecoratorBuilder("minLength")
                .WithDescription("Defines the minimum length of the parameter.")
                .WithRequiredParameter("length", LanguageConstants.Int, "The minimum length.")
                .WithFlags(FunctionFlags.ParameterDecorator)
                .WithValidator(ValidateTargetType(UnionType.Create(LanguageConstants.String, LanguageConstants.Array)))
                .WithEvaluator(MergeToTargetObject("minLength", SingleArgumentSelector))
                .Build();

            yield return new DecoratorBuilder("maxLength")
                .WithDescription("Defines the maximum length of the parameter.")
                .WithRequiredParameter("length", LanguageConstants.Int, "The maximum length.")
                .WithFlags(FunctionFlags.ParameterDecorator)
                .WithValidator(ValidateTargetType(UnionType.Create(LanguageConstants.String, LanguageConstants.Array)))
                .WithEvaluator(MergeToTargetObject("maxLength", SingleArgumentSelector))
                .Build();

            yield return new DecoratorBuilder("metadata")
                .WithDescription("Defines metadata of the parameter.")
                .WithRequiredParameter("object", LanguageConstants.Object, "The metadata object.")
                .WithFlags(FunctionFlags.ParameterDecorator)
                .WithEvaluator(MergeToTargetObject("metadata", SingleArgumentSelector))
                .Build();

            yield return new DecoratorBuilder("description")
                .WithDescription("Describes the parameter.")
                .WithRequiredParameter("text", LanguageConstants.String, "The description.")
                .WithFlags(FunctionFlags.ParameterDecorator)
                .WithEvaluator(MergeToTargetObject("metadata", decoratorSyntax => SyntaxFactory.CreateObject(
                    SyntaxFactory.CreateObjectProperty("description", SingleArgumentSelector(decoratorSyntax)).AsEnumerable())))
                .Build();
        }

        public AzNamespaceSymbol(ResourceScopeType resourceScope)
            : base("az", GetAzOverloads(resourceScope), ImmutableArray<BannedFunction>.Empty, GetAzDecorators())
        {
        }
    }
}