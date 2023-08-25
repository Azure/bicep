// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Azure.Deployments.Core.Definitions.Identifiers;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Intermediate;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics.Namespaces
{
    public static class AzNamespaceType
    {
        public const string BuiltInName = "az";
        public const string GetSecretFunctionName = "getSecret";

        public static NamespaceSettings Settings { get; } = new(
            IsSingleton: true,
            BicepProviderName: BuiltInName,
            ConfigurationType: null,
            ArmTemplateProviderName: "AzureResourceManager",
            ArmTemplateProviderVersion: "1.0.0");

        private static FunctionOverload.ResultBuilderDelegate AddDiagnosticsAndReturnResult(TypeSymbol returnType, DiagnosticBuilder.DiagnosticBuilderDelegate writeDiagnostic)
        {
            return (binder, fileResolver, diagnostics, functionCall, argumentTypes) =>
            {
                diagnostics.Write(functionCall.Name, writeDiagnostic);
                return new(returnType);
            };
        }

        private static FunctionResult GetRestrictedResourceGroupReturnResult(IBinder binder, IFileResolver fileResolver, IDiagnosticWriter diagnostics, FunctionCallSyntaxBase functionCall, ImmutableArray<TypeSymbol> argumentTypes)
            => new(
                new ResourceGroupScopeType(functionCall.Arguments.ToImmutableArray(), Enumerable.Empty<TypeProperty>()),
                new ObjectExpression(functionCall, ImmutableArray<ObjectPropertyExpression>.Empty));

        private static FunctionResult GetRestrictedSubscriptionReturnResult(IBinder binder, IFileResolver fileResolver, IDiagnosticWriter diagnostics, FunctionCallSyntaxBase functionCall, ImmutableArray<TypeSymbol> argumentTypes)
            => new(
                new SubscriptionScopeType(functionCall.Arguments.ToImmutableArray(), Enumerable.Empty<TypeProperty>()),
                new ObjectExpression(functionCall, ImmutableArray<ObjectPropertyExpression>.Empty));

        private static FunctionResult GetRestrictedManagementGroupReturnResult(IBinder binder, IFileResolver fileResolver, IDiagnosticWriter diagnostics, FunctionCallSyntaxBase functionCall, ImmutableArray<TypeSymbol> argumentTypes)
            => new(
                new ManagementGroupScopeType(functionCall.Arguments.ToImmutableArray(), Enumerable.Empty<TypeProperty>()),
                new ObjectExpression(functionCall, ImmutableArray<ObjectPropertyExpression>.Empty));

        private static FunctionResult GetTenantReturnResult(IBinder binder, IFileResolver fileResolver, IDiagnosticWriter diagnostics, FunctionCallSyntaxBase functionCall, ImmutableArray<TypeSymbol> argumentTypes)
            => new(new TenantScopeType(functionCall.Arguments.ToImmutableArray(), new[]
            {
                new TypeProperty("tenantId", LanguageConstants.String),
                new TypeProperty("country", LanguageConstants.String),
                new TypeProperty("countryCode", LanguageConstants.String),
                new TypeProperty("displayName", LanguageConstants.String),
            }));

        private static FunctionResult GetManagementGroupReturnResult(IBinder binder, IFileResolver fileResolver, IDiagnosticWriter diagnostics, FunctionCallSyntaxBase functionCall, ImmutableArray<TypeSymbol> argumentTypes)
        {
            var summary = new ObjectType("summaryProperties", TypeSymbolValidationFlags.Default, new[]
            {
                new TypeProperty("id", LanguageConstants.String),
                new TypeProperty("name", LanguageConstants.String),
                new TypeProperty("type", LanguageConstants.String),
            }, null);

            var details = new ObjectType("detailsProperties", TypeSymbolValidationFlags.Default, new[]
            {
                new TypeProperty("version", LanguageConstants.String),
                new TypeProperty("updatedTime", LanguageConstants.String),
                new TypeProperty("updatedBy", LanguageConstants.String),
                new TypeProperty("parent", summary)
            }, null);

            var properties = new ObjectType("properties", TypeSymbolValidationFlags.Default, new[]
            {
                new TypeProperty("tenantId", LanguageConstants.String),
                new TypeProperty("displayName", LanguageConstants.String),
                new TypeProperty("details", details)
            }, null);

            return new(new ManagementGroupScopeType(functionCall.Arguments.ToImmutableArray(), new[]
            {
                new TypeProperty("id", LanguageConstants.String),
                new TypeProperty("name", LanguageConstants.String),
                new TypeProperty("type", LanguageConstants.String),
                new TypeProperty("properties", properties),
            }));
        }

        private static FunctionResult GetResourceGroupReturnResult(IBinder binder, IFileResolver fileResolver, IDiagnosticWriter diagnostics, FunctionCallSyntaxBase functionCall, ImmutableArray<TypeSymbol> argumentTypes)
        {
            var properties = new ObjectType("properties", TypeSymbolValidationFlags.Default, new[]
            {
                new TypeProperty("provisioningState", LanguageConstants.String),
            }, null);

            return new(new ResourceGroupScopeType(functionCall.Arguments.ToImmutableArray(), new[]
            {
                new TypeProperty("id", LanguageConstants.String),
                new TypeProperty("name", LanguageConstants.String),
                new TypeProperty("type", LanguageConstants.String),
                new TypeProperty("location", LanguageConstants.String),
                new TypeProperty("managedBy", LanguageConstants.String),
                new TypeProperty("tags", AzResourceTypeProvider.Tags),
                new TypeProperty("properties", properties),
            }));
        }

        private static FunctionResult GetSubscriptionReturnResult(IBinder binder, IFileResolver fileResolver, IDiagnosticWriter diagnostics, FunctionCallSyntaxBase functionCall, ImmutableArray<TypeSymbol> argumentTypes)
        {
            return new(new SubscriptionScopeType(functionCall.Arguments.ToImmutableArray(), new[]
            {
                new TypeProperty("id", LanguageConstants.String),
                new TypeProperty("subscriptionId", LanguageConstants.String),
                new TypeProperty("tenantId", LanguageConstants.String),
                new TypeProperty("displayName", LanguageConstants.String),
            }));
        }

        private static ObjectType GetProvidersSingleResourceReturnType()
        {
            // from https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/template-functions-resource?tabs=json#providers
            return new ObjectType("ProviderResource", TypeSymbolValidationFlags.Default, new[]
            {
                new TypeProperty("resourceType", LanguageConstants.String),
                new TypeProperty("locations", new TypedArrayType(LanguageConstants.String, TypeSymbolValidationFlags.Default)),
                new TypeProperty("apiVersions", new TypedArrayType(LanguageConstants.String, TypeSymbolValidationFlags.Default)),
            }, null);
        }

        private static ObjectType GetProvidersSingleProviderReturnType()
        {
            // from https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/template-functions-resource?tabs=json#providers
            return new ObjectType("Provider", TypeSymbolValidationFlags.Default, new[]
            {
                new TypeProperty("namespace", LanguageConstants.String),
                new TypeProperty("resourceTypes", new TypedArrayType(GetProvidersSingleResourceReturnType(), TypeSymbolValidationFlags.Default)),
                }, null);
        }

        private static ObjectType GetEnvironmentReturnType()
        {
            return new ObjectType("environment", TypeSymbolValidationFlags.Default, new[]
            {
                new TypeProperty("activeDirectoryDataLake", LanguageConstants.String),
                new TypeProperty("authentication", new ObjectType("authenticationProperties", TypeSymbolValidationFlags.Default, new []
                {
                    new TypeProperty("audiences", new TypedArrayType(LanguageConstants.String, TypeSymbolValidationFlags.Default)),
                    new TypeProperty("identityProvider", LanguageConstants.String),
                    new TypeProperty("loginEndpoint", LanguageConstants.String),
                    new TypeProperty("tenant", LanguageConstants.String),
                }, null)),
                new TypeProperty("batch", LanguageConstants.String),
                new TypeProperty("gallery", LanguageConstants.String),
                new TypeProperty("graph", LanguageConstants.String),
                new TypeProperty("graphAudience", LanguageConstants.String),
                new TypeProperty("media", LanguageConstants.String),
                new TypeProperty("name", LanguageConstants.String),
                new TypeProperty("portal", LanguageConstants.String),
                new TypeProperty("resourceManager", LanguageConstants.String),
                new TypeProperty("sqlManagement", LanguageConstants.String),
                new TypeProperty("suffixes", new ObjectType("suffixesProperties", TypeSymbolValidationFlags.Default, new []
                {
                    new TypeProperty("acrLoginServer", LanguageConstants.String),
                    new TypeProperty("azureDatalakeAnalyticsCatalogAndJob", LanguageConstants.String),
                    new TypeProperty("azureDatalakeStoreFileSystem", LanguageConstants.String),
                    new TypeProperty("azureFrontDoorEndpointSuffix", LanguageConstants.String),
                    new TypeProperty("keyvaultDns", LanguageConstants.String),
                    new TypeProperty("sqlServerHostname", LanguageConstants.String),
                    new TypeProperty("storage", LanguageConstants.String),
                }, null)),
                new TypeProperty("vmImageAliasDoc", LanguageConstants.String),
            }, null);
        }

        private static ObjectType GetDeploymentReturnType(ResourceScope targetScope)
        {
            // Note: there are other properties which could be included here, but they allow you to break out of the bicep world.
            // We're going to omit them and only include what is truly necessary. If we get feature requests to expose more properties, we should discuss this further.
            // Properties such as 'template', 'templateHash', 'parameters' depend on the codegen, and feel like they could be fragile.
            // template.contentVersion was requested in issue #3114
            IEnumerable<TypeProperty> properties = new[]
            {
                new TypeProperty("name", LanguageConstants.String),
                new TypeProperty("properties", new ObjectType("properties", TypeSymbolValidationFlags.Default, new []
                {
                    new TypeProperty("template", new ObjectType("templateProperties", TypeSymbolValidationFlags.Default, new []
                    {
                        new TypeProperty("contentVersion", LanguageConstants.String)
                    }, null)),
                    new TypeProperty("templateLink", new ObjectType("templateLinkProperties", TypeSymbolValidationFlags.Default, new []
                    {
                        new TypeProperty("id", LanguageConstants.String),
                        new TypeProperty("uri", LanguageConstants.String),
                    }, null))
                }, null)),
            };

            if (!targetScope.HasFlag(ResourceScope.ResourceGroup))
            {
                // deployments in the 'resourcegroup' scope do not have the 'location' property. All other scopes do.
                var locationProperty = new TypeProperty("location", LanguageConstants.String);
                properties = properties.Concat(locationProperty.AsEnumerable());
            }

            return new ObjectType("deployment", TypeSymbolValidationFlags.Default, properties, null);
        }

        private static IEnumerable<(FunctionOverload functionOverload, ResourceScope allowedScopes)> GetScopeFunctions()
        {
            // Depending on the scope of the Bicep file, different sets of function overloads are invalid - for example, you can't use 'resourceGroup()' inside a tenant-level deployment

            // Also note that some of these functions and overloads ("GetRestrictedXYZ") have not yet been implemented in full in the ARM JSON. For these, we simply
            // return an empty object type (so that dot property access doesn't work), and generate as an ARM expression "createObject()" if anyone tries to access the object value.
            // This list should be kept in-sync with ScopeHelper.CanConvertToArmJson().

            yield return (
                new FunctionOverloadBuilder("tenant")
                    .WithReturnResultBuilder(GetTenantReturnResult, new TenantScopeType(Enumerable.Empty<FunctionArgumentSyntax>(), Enumerable.Empty<TypeProperty>()))
                    .WithGenericDescription("Returns the current tenant scope.")
                    .Build(),
                ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup);

            const string managementGroupGenericDescription = "Returns a management group scope.";
            yield return (
                new FunctionOverloadBuilder("managementGroup")
                    .WithReturnResultBuilder(GetManagementGroupReturnResult, new ManagementGroupScopeType(Enumerable.Empty<FunctionArgumentSyntax>(), Enumerable.Empty<TypeProperty>()))
                    .WithGenericDescription(managementGroupGenericDescription)
                    .WithDescription("Returns the current management group scope.")
                    .Build(),
                ResourceScope.ManagementGroup);
            yield return (
                new FunctionOverloadBuilder("managementGroup")
                    .WithReturnResultBuilder(GetRestrictedManagementGroupReturnResult, new ManagementGroupScopeType(Enumerable.Empty<FunctionArgumentSyntax>(), Enumerable.Empty<TypeProperty>()))
                    .WithGenericDescription(managementGroupGenericDescription)
                    .WithDescription("Returns the scope for a named management group.")
                    .WithRequiredParameter("name", LanguageConstants.String, "The unique identifier of the management group (not the display name).")
                    .Build(),
                ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup);

            const string subscriptionGenericDescription = "Returns a subscription scope.";
            yield return (
                new FunctionOverloadBuilder("subscription")
                    .WithReturnResultBuilder(GetSubscriptionReturnResult, new SubscriptionScopeType(Enumerable.Empty<FunctionArgumentSyntax>(), Enumerable.Empty<TypeProperty>()))
                    .WithGenericDescription(subscriptionGenericDescription)
                    .WithDescription("Returns the subscription scope for the current deployment.")
                    .Build(),
                ResourceScope.Subscription | ResourceScope.ResourceGroup);
            yield return (
                new FunctionOverloadBuilder("subscription")
                    .WithReturnResultBuilder(GetRestrictedSubscriptionReturnResult, new SubscriptionScopeType(Enumerable.Empty<FunctionArgumentSyntax>(), Enumerable.Empty<TypeProperty>()))
                    .WithGenericDescription(subscriptionGenericDescription)
                    .WithDescription("Returns a named subscription scope.")
                    .WithRequiredParameter("subscriptionId", LanguageConstants.String, "The subscription ID")
                    .Build(),
                ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup);

            const string resourceGroupGenericDescription = "Returns a resource group scope.";
            yield return (
                new FunctionOverloadBuilder("resourceGroup")
                    .WithReturnResultBuilder(GetResourceGroupReturnResult, new ResourceGroupScopeType(Enumerable.Empty<FunctionArgumentSyntax>(), Enumerable.Empty<TypeProperty>()))
                    .WithGenericDescription(resourceGroupGenericDescription)
                    .WithDescription("Returns the current resource group scope.")
                    .Build(),
                ResourceScope.ResourceGroup);
            yield return (
                new FunctionOverloadBuilder("resourceGroup")
                    .WithReturnResultBuilder(GetRestrictedResourceGroupReturnResult, new ResourceGroupScopeType(Enumerable.Empty<FunctionArgumentSyntax>(), Enumerable.Empty<TypeProperty>()))
                    .WithGenericDescription(resourceGroupGenericDescription)
                    .WithDescription("Returns a named resource group scope")
                    .WithRequiredParameter("resourceGroupName", LanguageConstants.String, "The resource group name")
                    .Build(),
                ResourceScope.Subscription | ResourceScope.ResourceGroup);
            yield return (
                new FunctionOverloadBuilder("resourceGroup")
                    .WithReturnResultBuilder(GetRestrictedResourceGroupReturnResult, new ResourceGroupScopeType(Enumerable.Empty<FunctionArgumentSyntax>(), Enumerable.Empty<TypeProperty>()))
                    .WithGenericDescription(resourceGroupGenericDescription)
                    .WithDescription("Returns a named resource group scope.")
                    .WithRequiredParameter("subscriptionId", LanguageConstants.String, "The subscription ID")
                    .WithRequiredParameter("resourceGroupName", LanguageConstants.String, "The resource group name")
                    .Build(),
                ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup);
        }

        private static IEnumerable<FunctionOverload> GetAzOverloads(ResourceScope resourceScope, BicepSourceFileKind sourceFileKind)
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

            if (sourceFileKind == BicepSourceFileKind.ParamsFile)
            {
                yield return new FunctionOverloadBuilder(GetSecretFunctionName)
                    .WithReturnType(LanguageConstants.SecureString)
                    .WithGenericDescription("Retrieve a value from an Azure Key Vault at the start of a deployment. All arguments must be compile-time constants.")
                    .WithReturnResultBuilder((_, _, _, func, argumentTypes) =>
                    {
                        if ((argumentTypes[0] as StringLiteralType)?.RawStringValue is not { } subscriptionId)
                        {
                            return new(ErrorType.Create(DiagnosticBuilder.ForPosition(func.Arguments[0]).CompileTimeConstantRequired()));
                        }
                        if ((argumentTypes[1] as StringLiteralType)?.RawStringValue is not { } resourceGroupName)
                        {
                            return new(ErrorType.Create(DiagnosticBuilder.ForPosition(func.Arguments[1]).CompileTimeConstantRequired()));
                        }
                        if ((argumentTypes[2] as StringLiteralType)?.RawStringValue is not { } keyVaultName)
                        {
                            return new(ErrorType.Create(DiagnosticBuilder.ForPosition(func.Arguments[2]).CompileTimeConstantRequired()));
                        }
                        if ((argumentTypes[3] as StringLiteralType)?.RawStringValue is not { } secretName)
                        {
                            return new(ErrorType.Create(DiagnosticBuilder.ForPosition(func.Arguments[3]).CompileTimeConstantRequired()));
                        }

                        string? secretVersion = null;
                        if (func.Arguments.Length > 4)
                        {
                            if ((argumentTypes[4] as StringLiteralType)?.RawStringValue is not { } sv)
                            {
                                return new(ErrorType.Create(DiagnosticBuilder.ForPosition(func.Arguments[4]).CompileTimeConstantRequired()));
                            }
                            secretVersion = sv;
                        }

                        var kvResourceId = ResourceGroupLevelResourceId.Create(subscriptionId, resourceGroupName, "Microsoft.KeyVault", new[] { "vaults" }, new[] { keyVaultName });
                        return new(LanguageConstants.SecureString, new ParameterKeyVaultReferenceExpression(func, kvResourceId.FullyQualifiedId, secretName, secretVersion));
                    }, LanguageConstants.SecureString)
                    .WithRequiredParameter("subscriptionId", LanguageConstants.String, "Id of the Subscription that has the target KeyVault")
                    .WithRequiredParameter("resourceGroupName", LanguageConstants.String, "Name of the Resource Group that has the target KeyVault")
                    .WithRequiredParameter("keyVaultName", LanguageConstants.String, "Name of the target KeyVault")
                    .WithRequiredParameter("secretName", LanguageConstants.String, "Name of the Secret")
                    .WithOptionalParameter("secretVersion", LanguageConstants.String, "Version of the Secret")
                    .WithFlags(FunctionFlags.DirectAssignment)
                    .Build();
            }
            else
            {
                // TODO: Add schema for return type
                yield return new FunctionOverloadBuilder("deployment")
                    .WithReturnType(GetDeploymentReturnType(resourceScope))
                    .WithGenericDescription("Returns information about the current deployment operation.")
                    .Build();

                yield return new FunctionOverloadBuilder("environment")
                    .WithReturnType(GetEnvironmentReturnType())
                    .WithGenericDescription("Returns information about the Azure environment used for deployment.")
                    .Build();

                // TODO: This is based on docs. Verify
                // the resourceId function relies on leading optional parameters that are disambiguated at runtime
                // modeling this as multiple overload with all possible permutations of the leading parameters
                const string resourceIdDescription = "Returns the unique identifier of a resource. You use this function when the resource name is ambiguous or not provisioned within the same template. The format of the returned identifier varies based on whether the deployment happens at the scope of a resource group, subscription, management group, or tenant.";
                yield return new FunctionOverloadBuilder("resourceId")
                    .WithReturnType(LanguageConstants.String)
                    .WithGenericDescription(resourceIdDescription)
                    .WithRequiredParameter("resourceType", LanguageConstants.String, "Type of resource including resource provider namespace")
                    .WithVariableParameter("resourceName", LanguageConstants.String, minimumCount: 1, "The resource name segment")
                    .Build();

                yield return new FunctionOverloadBuilder("resourceId")
                    .WithReturnType(LanguageConstants.String)
                    .WithGenericDescription(resourceIdDescription)
                    .WithRequiredParameter("subscriptionId", LanguageConstants.String, "The subscription ID")
                    .WithRequiredParameter("resourceType", LanguageConstants.String, "Type of resource including resource provider namespace")
                    .WithVariableParameter("resourceName", LanguageConstants.String, minimumCount: 1, "The resource name segment")
                    .Build();

                yield return new FunctionOverloadBuilder("resourceId")
                    .WithReturnType(LanguageConstants.String)
                    .WithGenericDescription(resourceIdDescription)
                    .WithRequiredParameter("resourceGroupName", LanguageConstants.String, "The resource group name")
                    .WithRequiredParameter("resourceType", LanguageConstants.String, "Type of resource including resource provider namespace")
                    .WithVariableParameter("resourceName", LanguageConstants.String, minimumCount: 1, "The resource name segment")
                    .Build();

                yield return new FunctionOverloadBuilder("resourceId")
                    .WithReturnType(LanguageConstants.String)
                    .WithGenericDescription(resourceIdDescription)
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
                    .WithGenericDescription(subscriptionResourceIdDescription)
                    .WithRequiredParameter("resourceType", LanguageConstants.String, "Type of resource including resource provider namespace")
                    .WithVariableParameter("resourceName", LanguageConstants.String, minimumCount: 1, "The resource name segment")
                    .Build();

                yield return new FunctionOverloadBuilder("subscriptionResourceId")
                    .WithReturnType(LanguageConstants.String)
                    .WithGenericDescription(subscriptionResourceIdDescription)
                    .WithRequiredParameter("subscriptionId", LanguageConstants.String, "The subscription ID")
                    .WithRequiredParameter("resourceType", LanguageConstants.String, "Type of resource including resource provider namespace")
                    .WithVariableParameter("resourceName", LanguageConstants.String, minimumCount: 1, "The resource name segment")
                    .Build();

                yield return new FunctionOverloadBuilder("tenantResourceId")
                    .WithReturnType(LanguageConstants.String)
                    .WithGenericDescription("Returns the unique identifier for a resource deployed at the tenant level.")
                    .WithRequiredParameter("resourceType", LanguageConstants.String, "Type of resource including resource provider namespace")
                    .WithVariableParameter("resourceName", LanguageConstants.String, minimumCount: 1, "The resource name segment")
                    .Build();

                yield return new FunctionOverloadBuilder("extensionResourceId")
                    .WithReturnType(LanguageConstants.String)
                    // .WithGenericDescription("Returns the resource ID for an extension resource, which is a resource type that is applied to another resource to add to its capabilities.")
                    .WithGenericDescription("Returns the resource ID for an [extension](https://docs.microsoft.com/en-us/azure/azure-resource-manager/management/extension-resource-types) resource, which is a resource type that is applied to another resource to add to its capabilities.")
                    .WithRequiredParameter("resourceId", LanguageConstants.String, "The resource ID for the resource that the extension resource is applied to")
                    .WithRequiredParameter("resourceType", LanguageConstants.String, "Type of the extension resource including resource provider namespace")
                    .WithVariableParameter("resourceName", LanguageConstants.String, minimumCount: 1, "The extension resource name segment")
                    .Build();

                const string managementGroupResourceIdDescription = "Returns the unique identifier for a resource deployed at the management group level.";
                yield return new FunctionOverloadBuilder("managementGroupResourceId")
                    .WithReturnType(LanguageConstants.String)
                    .WithGenericDescription(managementGroupResourceIdDescription)
                    .WithRequiredParameter("resourceType", LanguageConstants.String, "Type of resource including resource provider namespace")
                    .WithVariableParameter("resourceName", LanguageConstants.String, minimumCount: 1, "The resource name segment")
                    .Build();

                yield return new FunctionOverloadBuilder("managementGroupResourceId")
                    .WithReturnType(LanguageConstants.String)
                    .WithGenericDescription(managementGroupResourceIdDescription)
                    .WithRequiredParameter("managementGroupId", LanguageConstants.String, "The management group ID")
                    .WithRequiredParameter("resourceType", LanguageConstants.String, "Type of resource including resource provider namespace")
                    .WithVariableParameter("resourceName", LanguageConstants.String, minimumCount: 1, "The resource name segment")
                    .Build();

                const string providersDescription = "Returns information about a resource provider and its supported resource types. If you don't provide a resource type, the function returns all the supported types for the resource provider.";
                yield return new FunctionOverloadBuilder("providers")
                    .WithReturnResultBuilder(AddDiagnosticsAndReturnResult(GetProvidersSingleProviderReturnType(), x => x.DeprecatedProvidersFunction("providers")), GetProvidersSingleProviderReturnType())
                    .WithGenericDescription(providersDescription)
                    .WithRequiredParameter("providerNamespace", LanguageConstants.String, "the namespace of the provider")
                    .Build();

                yield return new FunctionOverloadBuilder("providers")
                    .WithReturnResultBuilder(AddDiagnosticsAndReturnResult(GetProvidersSingleResourceReturnType(), x => x.DeprecatedProvidersFunction("providers")), GetProvidersSingleResourceReturnType())
                    .WithGenericDescription(providersDescription)
                    .WithRequiredParameter("providerNamespace", LanguageConstants.String, "the namespace of the provider")
                    .WithRequiredParameter("resourceType", LanguageConstants.String, "The type of resource within the specified namespace")
                    .Build();

                // TODO: return type is string[]
                // TODO: Location param should be of location type if we ever add it
                yield return new FunctionOverloadBuilder("pickZones")
                    .WithReturnType(LanguageConstants.Array)
                    .WithGenericDescription("Determines whether a resource type supports zones for a region.")
                    .WithRequiredParameter("providerNamespace", LanguageConstants.String, "The resource provider namespace for the resource type to check for zone support")
                    .WithRequiredParameter("resourceType", LanguageConstants.String, "The resource type to check for zone support")
                    .WithRequiredParameter("location", LanguageConstants.String, "The region to check for zone support")
                    .WithOptionalParameter("numberOfZones", LanguageConstants.Int, "The number of logical zones to return. The default is 1. The number must a positive integer from 1 to 3. Use 1 for single-zoned resources. For multi-zoned resources, the value must be less than or equal to the number of supported zones.")
                    .WithOptionalParameter("offset", LanguageConstants.Int, "The offset from the starting logical zone. The function returns an error if offset plus numberOfZones exceeds the number of supported zones.")
                    .Build();

                // TODO: Change 'Full' to literal type after verifying in the runtime source
                yield return new FunctionOverloadBuilder("reference")
                    .WithReturnType(LanguageConstants.Object)
                    .WithGenericDescription("Returns an object representing a resource's runtime state.")
                    .WithRequiredParameter("resourceNameOrIdentifier", LanguageConstants.String, "Name or unique identifier of a resource. When referencing a resource in the current template, provide only the resource name as a parameter. When referencing a previously deployed resource or when the name of the resource is ambiguous, provide the resource ID.")
                    .WithOptionalParameter("apiVersion", LanguageConstants.String, "API version of the specified resource. This parameter is required when the resource isn't provisioned within same template.")
                    .WithOptionalParameter("full", LanguageConstants.String, "Value that specifies whether to return the full resource object. If you don't specify 'Full', only the properties object of the resource is returned. The full object includes values such as the resource ID and location.")
                    .WithFlags(FunctionFlags.RequiresInlining)
                    .Build();

                // TODO: Doc parameters need an update
                yield return new FunctionWildcardOverloadBuilder("list*", AzConstants.ListWildcardFunctionRegex)
                    .WithReturnType(LanguageConstants.Any)
                    .WithGenericDescription("The syntax for this function varies by name of the list operations. Each implementation returns values for the resource type that supports a list operation. The operation name must start with list. Some common usages are `listKeys`, `listKeyValue`, and `listSecrets`.")
                    .WithRequiredParameter("resourceNameOrIdentifier", LanguageConstants.String, "Name or unique identifier of a resource. When referencing a resource in the current template, provide only the resource name as a parameter. When referencing a previously deployed resource or when the name of the resource is ambiguous, provide the resource ID.")
                    .WithRequiredParameter("apiVersion", LanguageConstants.String, "API version of resource runtime state. Typically, in the format, yyyy-mm-dd.")
                    .WithOptionalParameter("functionValues", LanguageConstants.Object, "An object that has values for the function. Only provide this object for functions that support receiving an object with parameter values, such as listAccountSas on a storage account. An example of passing function values is shown in this article.")
                    .WithFlags(FunctionFlags.RequiresInlining)
                    .Build();
            }
        }

        public static NamespaceType Create(string aliasName, ResourceScope resourceScope, AzResourceTypeProvider resourceTypeProvider, BicepSourceFileKind bicepSourceFileKind)
        {
            return new NamespaceType(
                aliasName,
                new NamespaceSettings(
                    IsSingleton: true,
                    BicepProviderName: BuiltInName,
                    ConfigurationType: null,
                    ArmTemplateProviderName: "AzureResourceManager",
                    ArmTemplateProviderVersion: resourceTypeProvider.Version),
                ImmutableArray<TypeProperty>.Empty,
                GetAzOverloads(resourceScope, bicepSourceFileKind),
                ImmutableArray<BannedFunction>.Empty,
                ImmutableArray<Decorator>.Empty,
                resourceTypeProvider);
        }
    }
}
