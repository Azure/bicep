// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.Resources;

namespace Bicep.Core.TypeSystem.Az
{
    public class AzResourceTypeProvider : IResourceTypeProvider
    {
        private static (ResourceTypeReference, Func<ResourceType>) Get_Microsoft_Resources_resourceGroups_2020_06_01()
        {
            // hand crafted from https://github.com/Azure/azure-rest-api-specs/blob/405df4e/specification/resources/resource-manager/Microsoft.Resources/stable/2020-06-01/resources.json
            var reference = ResourceTypeReference.Parse("Microsoft.Resources/resourceGroups@2020-06-01");

            return (reference, () => new ResourceType(
                reference,
                new NamedObjectType(
                    reference.FormatName(),
                    TypeSymbolValidationFlags.WarnOnTypeMismatch,
                        LanguageConstants.GetCommonResourceProperties(reference).Concat(
                        new TypeProperty("location", LanguageConstants.String, TypePropertyFlags.Required),
                        new TypeProperty("tags", LanguageConstants.Tags, TypePropertyFlags.None)
                    ),
                    null),
                TypeSymbolValidationFlags.WarnOnTypeMismatch));
        }

        private static TypeSymbol CreateEnumType(params string[] values)
            => UnionType.Create(values.Select(x => new StringLiteralType(x)));

        private static (ResourceTypeReference, Func<ResourceType>) Get_Microsoft_Resources_deploymentScripts_2020_10_01()
        {
            // hand crafted from https://github.com/Azure/azure-rest-api-specs/blob/405df4e/specification/resources/resource-manager/Microsoft.Resources/preview/2019-10-01-preview/deploymentScripts.json
            var reference = ResourceTypeReference.Parse("Microsoft.Resources/deploymentScripts@2019-10-01-preview");

            var userAssignedIdentity = new NamedObjectType(
                "UserAssignedIdentity",
                TypeSymbolValidationFlags.WarnOnTypeMismatch,
                new [] { 
                    new TypeProperty("clientId", LanguageConstants.String),
                    new TypeProperty("principalId", LanguageConstants.String),
                },
                null);

            var identity = new NamedObjectType(
                "ManagedServiceIdentity",
                TypeSymbolValidationFlags.WarnOnTypeMismatch,
                new [] { 
                    new TypeProperty("type", CreateEnumType("UserAssigned"), TypePropertyFlags.None),
                    new TypeProperty("userAssignedIdentities", new NamedObjectType("UserAssignedIdentities", TypeSymbolValidationFlags.WarnOnTypeMismatch, Enumerable.Empty<TypeProperty>(), userAssignedIdentity), TypePropertyFlags.None),
                },
                null);

            var containerConfiguration = new NamedObjectType(
                "ContainerConfiguration",
                TypeSymbolValidationFlags.WarnOnTypeMismatch,
                new [] {
                    new TypeProperty("containerGroupName", LanguageConstants.String, TypePropertyFlags.None),
                },
                null);

            var environmentVariable = new NamedObjectType(
                "EnvironmentVariable",
                TypeSymbolValidationFlags.WarnOnTypeMismatch,
                new [] {
                    new TypeProperty("name", LanguageConstants.String, TypePropertyFlags.Required),
                    new TypeProperty("secureValue", LanguageConstants.String, TypePropertyFlags.None),
                    new TypeProperty("value", LanguageConstants.String, TypePropertyFlags.None),
                },
                null);

            var storageAccountConfiguration = new NamedObjectType(
                "StorageAccountConfiguration",
                TypeSymbolValidationFlags.WarnOnTypeMismatch,
                new [] {
                    new TypeProperty("storageAccountKey", LanguageConstants.String, TypePropertyFlags.None),
                    new TypeProperty("storageAccountName", LanguageConstants.String, TypePropertyFlags.None),
                },
                null);

            var scriptStatus = new NamedObjectType(
                "ScriptStatus",
                TypeSymbolValidationFlags.WarnOnTypeMismatch,
                new [] {
                    new TypeProperty("containerInstanceId", LanguageConstants.String, TypePropertyFlags.ReadOnly),
                    new TypeProperty("storageAccountId", LanguageConstants.String, TypePropertyFlags.ReadOnly),
                    new TypeProperty("startTime", LanguageConstants.String, TypePropertyFlags.ReadOnly),
                    new TypeProperty("endTime", LanguageConstants.String, TypePropertyFlags.ReadOnly),
                    new TypeProperty("expirationTime", LanguageConstants.String, TypePropertyFlags.ReadOnly),
                    new TypeProperty("error", LanguageConstants.Any, TypePropertyFlags.ReadOnly),
                },
                null);

            var baseResource = LanguageConstants.GetCommonResourceProperties(reference).Concat(
                new TypeProperty("location", LanguageConstants.String, TypePropertyFlags.Required),
                new TypeProperty("tags", LanguageConstants.Tags, TypePropertyFlags.None),
                new TypeProperty("identity", identity, TypePropertyFlags.Required));

            var deploymentScriptPropertiesBase = new [] {
                new TypeProperty("containerSettings", containerConfiguration, TypePropertyFlags.None),
                new TypeProperty("storageAccountSettings", storageAccountConfiguration, TypePropertyFlags.None),
                new TypeProperty("cleanupPreference", UnionType.Create(new StringLiteralType("Always"), new StringLiteralType("OnSuccess"), new StringLiteralType("OnExpiration")), TypePropertyFlags.None),
                new TypeProperty("provisioningState", CreateEnumType("Creating", "ProvisioningResources", "Running", "Succeeded", "Failed", "Canceled"), TypePropertyFlags.ReadOnly),
                new TypeProperty("status", scriptStatus, TypePropertyFlags.ReadOnly),
                new TypeProperty("outputs", LanguageConstants.Any, TypePropertyFlags.ReadOnly),
            };

            var scriptConfigurationBase = new [] {
                new TypeProperty("primaryScriptUri", LanguageConstants.String, TypePropertyFlags.None),
                new TypeProperty("supportingScriptUris", new TypedArrayType(LanguageConstants.String, TypeSymbolValidationFlags.WarnOnTypeMismatch), TypePropertyFlags.None),
                new TypeProperty("scriptContent", LanguageConstants.String, TypePropertyFlags.None),
                new TypeProperty("arguments", LanguageConstants.String, TypePropertyFlags.None),
                new TypeProperty("environmentVariables", new TypedArrayType(environmentVariable, TypeSymbolValidationFlags.WarnOnTypeMismatch), TypePropertyFlags.None),
                new TypeProperty("forceUpdateTag", LanguageConstants.String, TypePropertyFlags.None),
                new TypeProperty("retentionInterval", LanguageConstants.String, TypePropertyFlags.Required),
                new TypeProperty("timeout", LanguageConstants.String, TypePropertyFlags.None),
            };

            var powerShellResourceProperties = new NamedObjectType(
                "AzurePowerShellScriptProperties",
                TypeSymbolValidationFlags.WarnOnTypeMismatch,
                deploymentScriptPropertiesBase.Concat(scriptConfigurationBase).Concat(
                    new TypeProperty("azPowerShellVersion", LanguageConstants.String, TypePropertyFlags.Required)
                ),
                null);

            var powerShellResource = new NamedObjectType(
                "AzurePowerShellScript",
                TypeSymbolValidationFlags.WarnOnTypeMismatch,
                baseResource.Concat(
                    new TypeProperty("kind", new StringLiteralType("AzurePowerShell"), TypePropertyFlags.Required),
                    new TypeProperty("properties", powerShellResourceProperties, TypePropertyFlags.Required)),
                null);

            var azCliResourceProperties = new NamedObjectType(
                "AzureCliScriptProperties",
                TypeSymbolValidationFlags.WarnOnTypeMismatch,
                deploymentScriptPropertiesBase.Concat(scriptConfigurationBase).Concat(
                    new TypeProperty("azCliVersion", LanguageConstants.String, TypePropertyFlags.Required)
                ),
                null);

            var azCliResource = new NamedObjectType(
                "AzureCliScript",
                TypeSymbolValidationFlags.WarnOnTypeMismatch,
                baseResource.Concat(
                    new TypeProperty("kind", new StringLiteralType("AzureCLI"), TypePropertyFlags.Required),
                    new TypeProperty("properties", azCliResourceProperties, TypePropertyFlags.Required)),
                null);

            return (reference, () => new ResourceType(
                reference,
                new DiscriminatedObjectType(
                    reference.FormatName(),
                    TypeSymbolValidationFlags.WarnOnTypeMismatch,
                    "kind",
                    new [] {
                        powerShellResource,
                        azCliResource,
                    }),
                TypeSymbolValidationFlags.WarnOnTypeMismatch));
        }

        public ResourceType GetType(ResourceTypeReference typeReference)
        {
            var resourceType = TryGetResource(typeReference);

            if (resourceType != null)
            {
                return resourceType;
            }

            // TODO move default definition into types assembly
            return new ResourceType(typeReference, new NamedObjectType(typeReference.FormatName(), TypeSymbolValidationFlags.Default, LanguageConstants.CreateResourceProperties(typeReference), null), TypeSymbolValidationFlags.Default);
        }

        public bool HasType(ResourceTypeReference typeReference)
            => TryGetResource(typeReference) != null;

        private readonly IReadOnlyDictionary<ResourceTypeReference, Func<ResourceType>> manualTypeDefinitions = new []
        {
            Get_Microsoft_Resources_resourceGroups_2020_06_01(),
            Get_Microsoft_Resources_deploymentScripts_2020_10_01(),
        }.ToDictionary(kvp => kvp.Item1, kvp => kvp.Item2, ResourceTypeReferenceComparer.Instance);

        private ResourceType? TryGetResource(ResourceTypeReference typeReference)
            => manualTypeDefinitions.TryGetValue(typeReference, out var builderFunc) ? builderFunc() : null;
    }
}