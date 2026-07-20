// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem.Providers.Az
{
    /// <summary>
    /// REP 0015 ("Formalize handling of scope"). Models the duck-typed "@scope" discriminated union:
    ///
    /// <code>
    /// @discriminator('type')
    /// type ResourceScope =
    ///     { type: 'tenant' }
    ///   | { type: 'managementGroup', managementGroupId: string }
    ///   | { type: 'subscription', subscriptionId: string }
    ///   | { type: 'resourceGroup', subscriptionId: string, resourceGroup: string }
    ///   | { type: 'extension', resourceId: string }
    /// </code>
    ///
    /// The scope functions (resourceGroup(), subscription(), tenant(), managementGroup()) return the
    /// matching member type, and the "scope" property accepts the union. Because each member carries a
    /// string-literal "type" discriminator, the scope kind is known from the type alone (duck typing),
    /// which is what lets code-gen become a near pass-through into the "@scope" property.
    /// </summary>
    public static class AzResourceScopeType
    {
        public const string DiscriminatorKey = "type";

        public static readonly ObjectType Tenant = CreateMember("tenant");

        public static readonly ObjectType ManagementGroup = CreateMember(
            "managementGroup",
            new NamedTypeProperty("managementGroupId", LanguageConstants.String, TypePropertyFlags.Required));

        public static readonly ObjectType Subscription = CreateMember(
            "subscription",
            new NamedTypeProperty("subscriptionId", LanguageConstants.String, TypePropertyFlags.Required));

        public static readonly ObjectType ResourceGroup = CreateMember(
            "resourceGroup",
            new NamedTypeProperty("subscriptionId", LanguageConstants.String, TypePropertyFlags.Required),
            new NamedTypeProperty("resourceGroup", LanguageConstants.String, TypePropertyFlags.Required));

        public static readonly ObjectType Extension = CreateMember(
            "extension",
            new NamedTypeProperty("resourceId", LanguageConstants.String, TypePropertyFlags.Required));

        public static readonly DiscriminatedObjectType Union = new(
            "resourceScope",
            TypeSymbolValidationFlags.Default,
            DiscriminatorKey,
            new[] { Tenant, ManagementGroup, Subscription, ResourceGroup, Extension });

        private static ObjectType CreateMember(string discriminator, params NamedTypeProperty[] extraProperties)
        {
            var properties = new List<NamedTypeProperty>
            {
                new(DiscriminatorKey, TypeFactory.CreateStringLiteralType(discriminator), TypePropertyFlags.Required | TypePropertyFlags.Constant),
            };
            properties.AddRange(extraProperties);

            return new ObjectType(discriminator, TypeSymbolValidationFlags.Default, properties, additionalProperties: null);
        }
    }
}
