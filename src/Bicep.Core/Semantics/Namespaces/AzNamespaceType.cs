// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using Azure.Bicep.Types.Az;
using Azure.Deployments.Core.Definitions.Identifiers;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Intermediate;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Semantics.Namespaces
{
    public static class AzNamespaceType
    {
        public const string BuiltInName = "az";
        public const string GetSecretFunctionName = "getSecret";
        private static readonly string EmbeddedAzExtensionVersion = typeof(AzTypeLoader).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version
            ?? throw new UnreachableException("The 'Azure.Bicep.Types.Az' assembly should always have a file version attribute.");

        private static readonly Lazy<IResourceTypeProvider> TypeProviderLazy
            = new(() => new AzResourceTypeProvider(new AzResourceTypeLoader(new AzTypeLoader())));

        public static IResourceTypeProvider BuiltInTypeProvider => TypeProviderLazy.Value;

        public static NamespaceSettings Settings { get; } = new(
            IsSingleton: true,
            BicepExtensionName: BuiltInName,
            ConfigurationType: null,
            TemplateExtensionName: "AzureResourceManager",
            TemplateExtensionVersion: new Version(EmbeddedAzExtensionVersion).ToString(3));

        private delegate bool VisibilityDelegate(ResourceScope scope, BicepSourceFileKind sourceFileKind);
        private record NamespaceValue<T>(T Value, VisibilityDelegate IsVisible);

        private static readonly ImmutableArray<NamespaceValue<FunctionOverload>> Overloads = [.. GetAzOverloads()];

        private static FunctionOverload.ResultBuilderDelegate AddDiagnosticsAndReturnResult(TypeSymbol returnType, DiagnosticBuilder.DiagnosticBuilderDelegate writeDiagnostic)
        {
            return (_, diagnostics, functionCall, argumentTypes) =>
            {
                diagnostics.Write(functionCall.Name, writeDiagnostic);
                return new(returnType);
            };
        }

        private static FunctionResult GetRestrictedResourceGroupReturnResult(SemanticModel model, IDiagnosticWriter diagnostics, FunctionCallSyntaxBase functionCall, ImmutableArray<TypeSymbol> argumentTypes)
            => new(
                new ResourceGroupScopeType(functionCall.Arguments, []),
                new ObjectExpression(functionCall, []));

        private static FunctionResult GetRestrictedSubscriptionReturnResult(SemanticModel model, IDiagnosticWriter diagnostics, FunctionCallSyntaxBase functionCall, ImmutableArray<TypeSymbol> argumentTypes)
            => new(
                new SubscriptionScopeType(functionCall.Arguments, []),
                new ObjectExpression(functionCall, []));

        private static FunctionResult GetRestrictedManagementGroupReturnResult(SemanticModel model, IDiagnosticWriter diagnostics, FunctionCallSyntaxBase functionCall, ImmutableArray<TypeSymbol> argumentTypes)
            => new(
                new ManagementGroupScopeType(functionCall.Arguments, []),
                new ObjectExpression(functionCall, []));

        private static FunctionResult GetTenantReturnResult(SemanticModel model, IDiagnosticWriter diagnostics, FunctionCallSyntaxBase functionCall, ImmutableArray<TypeSymbol> argumentTypes)
            => new(new TenantScopeType(functionCall.Arguments, new[]
            {
                new NamedTypeProperty("tenantId", LanguageConstants.String),
                new NamedTypeProperty("country", LanguageConstants.String),
                new NamedTypeProperty("countryCode", LanguageConstants.String),
                new NamedTypeProperty("displayName", LanguageConstants.String),
            }));

        private static FunctionResult GetManagementGroupReturnResult(SemanticModel model, IDiagnosticWriter diagnostics, FunctionCallSyntaxBase functionCall, ImmutableArray<TypeSymbol> argumentTypes)
        {
            var summary = new ObjectType("summaryProperties", TypeSymbolValidationFlags.Default, new[]
            {
                new NamedTypeProperty("id", LanguageConstants.String),
                new NamedTypeProperty("name", LanguageConstants.String),
                new NamedTypeProperty("type", LanguageConstants.String),
            }, null);

            var details = new ObjectType("detailsProperties", TypeSymbolValidationFlags.Default, new[]
            {
                new NamedTypeProperty("version", LanguageConstants.String),
                new NamedTypeProperty("updatedTime", LanguageConstants.String),
                new NamedTypeProperty("updatedBy", LanguageConstants.String),
                new NamedTypeProperty("parent", summary)
            }, null);

            var properties = new ObjectType("properties", TypeSymbolValidationFlags.Default, new[]
            {
                new NamedTypeProperty("tenantId", LanguageConstants.String),
                new NamedTypeProperty("displayName", LanguageConstants.String),
                new NamedTypeProperty("details", details)
            }, null);

            return new(new ManagementGroupScopeType(functionCall.Arguments, new[]
            {
                new NamedTypeProperty("id", LanguageConstants.String),
                new NamedTypeProperty("name", LanguageConstants.String),
                new NamedTypeProperty("type", LanguageConstants.String),
                new NamedTypeProperty("properties", properties),
            }));
        }

        private static FunctionResult GetResourceGroupReturnResult(SemanticModel model, IDiagnosticWriter diagnostics, FunctionCallSyntaxBase functionCall, ImmutableArray<TypeSymbol> argumentTypes)
        {
            var properties = new ObjectType("properties", TypeSymbolValidationFlags.Default, new[]
            {
                new NamedTypeProperty("provisioningState", LanguageConstants.String),
            }, null);

            return new(new ResourceGroupScopeType(functionCall.Arguments, new[]
            {
                new NamedTypeProperty("id", LanguageConstants.String),
                new NamedTypeProperty("name", LanguageConstants.String),
                new NamedTypeProperty("type", LanguageConstants.String),
                new NamedTypeProperty("location", LanguageConstants.String),
                new NamedTypeProperty("managedBy", LanguageConstants.String),
                new NamedTypeProperty("tags", AzResourceTypeProvider.Tags),
                new NamedTypeProperty("properties", properties),
            }));
        }

        private static FunctionResult GetSubscriptionReturnResult(SemanticModel model, IDiagnosticWriter diagnostics, FunctionCallSyntaxBase functionCall, ImmutableArray<TypeSymbol> argumentTypes)
        {
            return new(new SubscriptionScopeType(functionCall.Arguments, new[]
            {
                new NamedTypeProperty("id", LanguageConstants.String),
                new NamedTypeProperty("subscriptionId", LanguageConstants.String),
                new NamedTypeProperty("tenantId", LanguageConstants.String),
                new NamedTypeProperty("displayName", LanguageConstants.String),
            }));
        }

        private static ObjectType GetProvidersSingleResourceReturnType()
        {
            // from https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/template-functions-resource?tabs=json#providers
            return new ObjectType("ProviderResource", TypeSymbolValidationFlags.Default, new[]
            {
                new NamedTypeProperty("resourceType", LanguageConstants.String),
                new NamedTypeProperty("locations", new TypedArrayType(LanguageConstants.String, TypeSymbolValidationFlags.Default)),
                new NamedTypeProperty("apiVersions", new TypedArrayType(LanguageConstants.String, TypeSymbolValidationFlags.Default)),
            }, null);
        }

        private static ObjectType GetProvidersSingleProviderReturnType()
        {
            // from https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/template-functions-resource?tabs=json#providers
            return new ObjectType("Provider", TypeSymbolValidationFlags.Default, new[]
            {
                new NamedTypeProperty("namespace", LanguageConstants.String),
                new NamedTypeProperty("resourceTypes", new TypedArrayType(GetProvidersSingleResourceReturnType(), TypeSymbolValidationFlags.Default)),
            }, null);
        }

        private static ObjectType GetEnvironmentReturnType()
        {
            return new ObjectType("environment", TypeSymbolValidationFlags.Default, new[]
            {
                new NamedTypeProperty("activeDirectoryDataLake", LanguageConstants.String),
                new NamedTypeProperty("authentication", new ObjectType("authenticationProperties", TypeSymbolValidationFlags.Default, new []
                {
                    new NamedTypeProperty("audiences", new TypedArrayType(LanguageConstants.String, TypeSymbolValidationFlags.Default)),
                    new NamedTypeProperty("identityProvider", LanguageConstants.String),
                    new NamedTypeProperty("loginEndpoint", LanguageConstants.String),
                    new NamedTypeProperty("tenant", LanguageConstants.String),
                }, null)),
                new NamedTypeProperty("batch", LanguageConstants.String),
                new NamedTypeProperty("gallery", LanguageConstants.String),
                new NamedTypeProperty("graph", LanguageConstants.String),
                new NamedTypeProperty("graphAudience", LanguageConstants.String),
                new NamedTypeProperty("media", LanguageConstants.String),
                new NamedTypeProperty("name", LanguageConstants.String),
                new NamedTypeProperty("portal", LanguageConstants.String),
                new NamedTypeProperty("resourceManager", LanguageConstants.String),
                new NamedTypeProperty("sqlManagement", LanguageConstants.String),
                new NamedTypeProperty("suffixes", new ObjectType("suffixesProperties", TypeSymbolValidationFlags.Default, new []
                {
                    new NamedTypeProperty("acrLoginServer", LanguageConstants.String),
                    new NamedTypeProperty("azureDatalakeAnalyticsCatalogAndJob", LanguageConstants.String),
                    new NamedTypeProperty("azureDatalakeStoreFileSystem", LanguageConstants.String),
                    new NamedTypeProperty("azureFrontDoorEndpointSuffix", LanguageConstants.String),
                    new NamedTypeProperty("keyvaultDns", LanguageConstants.String),
                    new NamedTypeProperty("sqlServerHostname", LanguageConstants.String),
                    new NamedTypeProperty("storage", LanguageConstants.String),
                }, null)),
                new NamedTypeProperty("vmImageAliasDoc", LanguageConstants.String),
            }, null);
        }

        private static FunctionResult GetDeploymentReturnResult(SemanticModel model, IDiagnosticWriter diagnostics, FunctionCallSyntaxBase functionCall, ImmutableArray<TypeSymbol> argumentTypes)
        {
            List<NamedTypeProperty> templateProperties = [new("contentVersion", LanguageConstants.String)];
            if (model.Root.MetadataDeclarations.Length > 0)
            {
                templateProperties.Add(new("metadata", new ObjectType(
                    "metadataProperties",
                    TypeSymbolValidationFlags.Default,
                    model.Root.MetadataDeclarations.Select(md => new NamedTypeProperty(
                        md.Name,
                        md.Type,
                        Description: md.TryGetDescriptionFromDecorator(model))))));
            }

            // Note: there are other properties which could be included here, but they allow you to break out of the bicep world.
            // We're going to omit them and only include what is truly necessary. If we get feature requests to expose more properties, we should discuss this further.
            // Properties such as 'template', 'templateHash', 'parameters' depend on the codegen, and feel like they could be fragile.
            // template.contentVersion was requested in issue #3114
            IEnumerable<NamedTypeProperty> properties = new[]
            {
                new NamedTypeProperty("name", LanguageConstants.String),
                new NamedTypeProperty("properties", new ObjectType("properties", TypeSymbolValidationFlags.Default, new []
                {
                    new NamedTypeProperty("template", new ObjectType("templateProperties", TypeSymbolValidationFlags.Default, templateProperties, null)),
                    new NamedTypeProperty("templateLink", new ObjectType("templateLinkProperties", TypeSymbolValidationFlags.Default, new []
                    {
                        new NamedTypeProperty("id", LanguageConstants.String),
                        new NamedTypeProperty("uri", LanguageConstants.String),
                    }, null))
                }, null)),
            };

            if (model.TargetScope != ResourceScope.ResourceGroup)
            {
                // deployments in the 'resourcegroup' scope do not have the 'location' property. All other scopes do.
                var locationProperty = new NamedTypeProperty("location", LanguageConstants.String);
                properties = properties.Concat(locationProperty.AsEnumerable());
            }

            return new(new ObjectType("deployment", TypeSymbolValidationFlags.Default, properties, null));
        }

        private static ObjectType GetDeployerReturnType()
        {
            IEnumerable<NamedTypeProperty> properties = new[]
            {
                new NamedTypeProperty("objectId", LanguageConstants.String),
                new NamedTypeProperty("tenantId", LanguageConstants.String),
                new NamedTypeProperty("userPrincipalName", LanguageConstants.String),
            };

            return new ObjectType("deployer", TypeSymbolValidationFlags.Default, properties, null);
        }

        private static IEnumerable<(FunctionOverload functionOverload, ResourceScope allowedScopes)> GetScopeFunctions()
        {
            // Depending on the scope of the Bicep file, different sets of function overloads are invalid - for example, you can't use 'resourceGroup()' inside a tenant-level deployment

            // Also note that some of these functions and overloads ("GetRestrictedXYZ") have not yet been implemented in full in the ARM JSON. For these, we simply
            // return an empty object type (so that dot property access doesn't work), and generate as an ARM expression "createObject()" if anyone tries to access the object value.
            // This list should be kept in-sync with ScopeHelper.CanConvertToArmJson().

            yield return (
                new FunctionOverloadBuilder("tenant")
                    .WithReturnResultBuilder(GetTenantReturnResult, new TenantScopeType([], []))
                    .WithGenericDescription("Returns the current tenant scope.")
                    .Build(),
                ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup);

            const string managementGroupGenericDescription = "Returns a management group scope.";
            yield return (
                new FunctionOverloadBuilder("managementGroup")
                    .WithReturnResultBuilder(GetManagementGroupReturnResult, new ManagementGroupScopeType([], []))
                    .WithGenericDescription(managementGroupGenericDescription)
                    .WithDescription("Returns the current management group scope.")
                    .Build(),
                ResourceScope.ManagementGroup);
            yield return (
                new FunctionOverloadBuilder("managementGroup")
                    .WithReturnResultBuilder(GetRestrictedManagementGroupReturnResult, new ManagementGroupScopeType([], []))
                    .WithGenericDescription(managementGroupGenericDescription)
                    .WithDescription("Returns the scope for a named management group.")
                    .WithRequiredParameter("name", LanguageConstants.String, "The unique identifier of the management group (not the display name).")
                    .Build(),
                ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup);

            const string subscriptionGenericDescription = "Returns a subscription scope.";
            yield return (
                new FunctionOverloadBuilder("subscription")
                    .WithReturnResultBuilder(GetSubscriptionReturnResult, new SubscriptionScopeType([], []))
                    .WithGenericDescription(subscriptionGenericDescription)
                    .WithDescription("Returns the subscription scope for the current deployment.")
                    .Build(),
                ResourceScope.Subscription | ResourceScope.ResourceGroup);
            yield return (
                new FunctionOverloadBuilder("subscription")
                    .WithReturnResultBuilder(GetRestrictedSubscriptionReturnResult, new SubscriptionScopeType([], []))
                    .WithGenericDescription(subscriptionGenericDescription)
                    .WithDescription("Returns a named subscription scope.")
                    .WithRequiredParameter("subscriptionId", LanguageConstants.String, "The subscription ID")
                    .Build(),
                ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup | ResourceScope.Local);

            const string resourceGroupGenericDescription = "Returns a resource group scope.";
            yield return (
                new FunctionOverloadBuilder("resourceGroup")
                    .WithReturnResultBuilder(GetResourceGroupReturnResult, new ResourceGroupScopeType([], []))
                    .WithGenericDescription(resourceGroupGenericDescription)
                    .WithDescription("Returns the current resource group scope.")
                    .Build(),
                ResourceScope.ResourceGroup);
            yield return (
                new FunctionOverloadBuilder("resourceGroup")
                    .WithReturnResultBuilder(GetRestrictedResourceGroupReturnResult, new ResourceGroupScopeType([], []))
                    .WithGenericDescription(resourceGroupGenericDescription)
                    .WithDescription("Returns a named resource group scope")
                    .WithRequiredParameter("resourceGroupName", LanguageConstants.String, "The resource group name")
                    .Build(),
                ResourceScope.Subscription | ResourceScope.ResourceGroup);
            yield return (
                new FunctionOverloadBuilder("resourceGroup")
                    .WithReturnResultBuilder(GetRestrictedResourceGroupReturnResult, new ResourceGroupScopeType([], []))
                    .WithGenericDescription(resourceGroupGenericDescription)
                    .WithDescription("Returns a named resource group scope.")
                    .WithRequiredParameter("subscriptionId", LanguageConstants.String, "The subscription ID")
                    .WithRequiredParameter("resourceGroupName", LanguageConstants.String, "The resource group name")
                    .Build(),
                ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup | ResourceScope.Local);

            yield return (
                new FunctionOverloadBuilder("deployment")
                    .WithReturnResultBuilder(
                        GetDeploymentReturnResult,
                        new ObjectType("deployment", TypeSymbolValidationFlags.Default, [], new TypeProperty(LanguageConstants.Any)))
                    .WithGenericDescription("Returns information about the current deployment operation.")
                    .Build(),
                ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup);

            yield return (
                new FunctionOverloadBuilder("deployer")
                    .WithReturnType(GetDeployerReturnType())
                    .WithGenericDescription("Returns information about the current deployment principal.")
                    .Build(),
                ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup);
        }

        private static IEnumerable<NamespaceValue<FunctionOverload>> GetAzOverloads()
        {
            static IEnumerable<FunctionOverload> GetParamsFilePermittedOverloads()
            {
                yield return new FunctionOverloadBuilder(GetSecretFunctionName)
                    .WithReturnType(LanguageConstants.SecureString)
                    .WithGenericDescription("Retrieve a value from an Azure Key Vault at the start of a deployment. All arguments must be compile-time constants.")
                    .WithReturnResultBuilder((_, _, func, argumentTypes) =>
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

            static IEnumerable<FunctionOverload> GetBicepFilePermittedOverloads()
            {
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

                yield return new FunctionOverloadBuilder("toLogicalZone")
                    .WithReturnType(LanguageConstants.String)
                    .WithGenericDescription("Returns the logical zone corresponding to the given physical zone.")
                    .WithRequiredParameter("subscriptionId", LanguageConstants.String, "The subscription ID of the deployed availability zones")
                    .WithRequiredParameter("location", LanguageConstants.String, "The location of the availability zone mappings")
                    .WithRequiredParameter("physicalZone", LanguageConstants.String, "The physical zone to convert")
                    .WithFlags(FunctionFlags.RequiresInlining)
                    .Build();

                yield return new FunctionOverloadBuilder("toLogicalZones")
                    .WithReturnType(new TypedArrayType(LanguageConstants.String, TypeSymbolValidationFlags.Default))
                    .WithGenericDescription("Returns the logical zone array corresponding to the given array of physical zones.")
                    .WithRequiredParameter("subscriptionId", LanguageConstants.String, "The subscription ID of the deployed availability zones")
                    .WithRequiredParameter("location", LanguageConstants.String, "The location of the availability zone mappings")
                    .WithRequiredParameter("physicalZones", LanguageConstants.Array, "An array of physical zones to convert")
                    .WithFlags(FunctionFlags.RequiresInlining)
                    .Build();

                yield return new FunctionOverloadBuilder("toPhysicalZone")
                    .WithReturnType(LanguageConstants.String)
                    .WithGenericDescription("Returns the physical zone corresponding to the given logical zone.")
                    .WithRequiredParameter("subscriptionId", LanguageConstants.String, "The subscription ID of the deployed availability zones")
                    .WithRequiredParameter("location", LanguageConstants.String, "The location of the availability zone mappings")
                    .WithRequiredParameter("logicalZone", LanguageConstants.String, "The logical zone to convert")
                    .WithFlags(FunctionFlags.RequiresInlining)
                    .Build();

                yield return new FunctionOverloadBuilder("toPhysicalZones")
                    .WithReturnType(new TypedArrayType(LanguageConstants.String, TypeSymbolValidationFlags.Default))
                    .WithGenericDescription("Returns the physical zone array corresponding to the given array of logical zones.")
                    .WithRequiredParameter("subscriptionId", LanguageConstants.String, "The subscription ID of the deployed availability zones")
                    .WithRequiredParameter("location", LanguageConstants.String, "The location of the availability zone mappings")
                    .WithRequiredParameter("logicalZones", LanguageConstants.Array, "An array of logical zones to convert")
                    .WithFlags(FunctionFlags.RequiresInlining)
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

            foreach (var overload in GetBicepFilePermittedOverloads())
            {
                yield return new(overload, (targetScope, sfk) => sfk == BicepSourceFileKind.BicepFile && targetScope != ResourceScope.Local);
            }

            foreach (var overload in GetParamsFilePermittedOverloads())
            {
                yield return new(overload, (targetScope, sfk) => sfk == BicepSourceFileKind.ParamsFile && targetScope != ResourceScope.Local);
            }

            foreach (var (overload, allowedScopes) in GetScopeFunctions())
            {
                // we only include it if it's valid at all of the scopes that the template is valid at
                yield return new(overload, (scope, sfk) => sfk == BicepSourceFileKind.BicepFile && scope == (scope & allowedScopes));
            }
        }

        public static NamespaceType Create(string? aliasName, ResourceScope scope, IResourceTypeProvider resourceTypeProvider, BicepSourceFileKind sourceFileKind)
        {
            return new NamespaceType(
                aliasName ?? BuiltInName,
                Settings,
                ImmutableArray<NamedTypeProperty>.Empty,
                Overloads.Where(x => x.IsVisible(scope, sourceFileKind)).Select(x => x.Value),
                ImmutableArray<BannedFunction>.Empty,
                ImmutableArray<Decorator>.Empty,
                resourceTypeProvider);
        }
    }
}
