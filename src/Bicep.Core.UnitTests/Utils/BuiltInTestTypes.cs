// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Resources;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;

namespace Bicep.Core.UnitTests.Utils
{
    /// <summary>
    /// A set of reusable resource types to validate various pieces of functionality.
    /// </summary>
    public class BuiltInTestTypes
    {
        private static ResourceTypeComponents BasicTestsType()
        {
            var resourceType = ResourceTypeReference.Parse("Test.Rp/basicTests@2020-01-01");

            return new ResourceTypeComponents(resourceType, ResourceScope.ResourceGroup, new ObjectType(resourceType.FormatName(), TypeSymbolValidationFlags.Default,
                AzResourceTypeProvider.GetCommonResourceProperties(resourceType).Concat(new[] {
                    new TypeProperty("kind", LanguageConstants.String, TypePropertyFlags.ReadOnly, "kind property"),
                }), null));
        }

        private static ResourceTypeComponents ReadWriteTestsType()
        {
            var resourceType = ResourceTypeReference.Parse("Test.Rp/readWriteTests@2020-01-01");

            var propertiesType = new ObjectType("Properties", TypeSymbolValidationFlags.WarnOnTypeMismatch, new[] {
                new TypeProperty("readwrite", LanguageConstants.String, TypePropertyFlags.None, "This is a property which supports reading AND writing!"),
                new TypeProperty("readonly", LanguageConstants.String, TypePropertyFlags.ReadOnly, "This is a property which only supports reading."),
                new TypeProperty("writeonly", LanguageConstants.String, TypePropertyFlags.WriteOnly, "This is a property which only supports writing."),
                new TypeProperty("required", LanguageConstants.String, TypePropertyFlags.Required, "This is a property which is required."),
            }, null);

            return new ResourceTypeComponents(resourceType, ResourceScope.ResourceGroup, new ObjectType(resourceType.FormatName(), TypeSymbolValidationFlags.Default,
                AzResourceTypeProvider.GetCommonResourceProperties(resourceType).Concat(new[] {
                    new TypeProperty("properties", propertiesType, TypePropertyFlags.Required, "properties property"),
                }), null));
        }

        private static ResourceTypeComponents DiscriminatorTestsType()
        {
            var resourceType = ResourceTypeReference.Parse("Test.Rp/discriminatorTests@2020-01-01");

            var bodyAProps = new ObjectType(
                "BodyAProperties",
                TypeSymbolValidationFlags.WarnOnTypeMismatch,
                new[] {
                    new TypeProperty("propA", LanguageConstants.String, TypePropertyFlags.None, "This is the description for propA!"),
                },
                null);

            var bodyBProps = new ObjectType(
                "BodyBProperties",
                TypeSymbolValidationFlags.WarnOnTypeMismatch,
                new[] {
                    new TypeProperty("propB", LanguageConstants.String, TypePropertyFlags.None, "This is the description for propB!"),
                },
                null);

            var bodyType = new DiscriminatedObjectType(
                resourceType.FormatName(),
                TypeSymbolValidationFlags.Default,
                "kind",
                new[] {
                    new ObjectType("BodyA", TypeSymbolValidationFlags.Default, AzResourceTypeProvider.GetCommonResourceProperties(resourceType).Concat(new [] {
                        new TypeProperty("kind", new StringLiteralType("BodyA"), TypePropertyFlags.None, "This is the kind of body A"),
                        new TypeProperty("properties", bodyAProps, TypePropertyFlags.None, "These are the properties for body A"),
                    }), null),
                    new ObjectType("BodyB", TypeSymbolValidationFlags.Default, AzResourceTypeProvider.GetCommonResourceProperties(resourceType).Concat(new [] {
                        new TypeProperty("kind", new StringLiteralType("BodyB"), TypePropertyFlags.None, "This is the kind of body B"),
                        new TypeProperty("properties", bodyBProps, TypePropertyFlags.None, "These are the properties for body B"),
                    }), null),
                });

            return new ResourceTypeComponents(resourceType, ResourceScope.ResourceGroup, bodyType);
        }

        private static ResourceTypeComponents DiscriminatedPropertiesTestsType()
        {
            var resourceType = ResourceTypeReference.Parse("Test.Rp/discriminatedPropertiesTests@2020-01-01");

            var propsA = new ObjectType(
                "PropertiesA",
                TypeSymbolValidationFlags.WarnOnTypeMismatch,
                new[] {
                    new TypeProperty("propType", new StringLiteralType("PropertiesA"), TypePropertyFlags.None, "..."),
                    new TypeProperty("propA", LanguageConstants.String, TypePropertyFlags.None, "This is the description for propA!"),
                },
                null);

            var propsB = new ObjectType(
                "PropertiesB",
                TypeSymbolValidationFlags.WarnOnTypeMismatch,
                new[] {
                    new TypeProperty("propType", new StringLiteralType("PropertiesB"), TypePropertyFlags.None, "..."),
                    new TypeProperty("propB", LanguageConstants.String, TypePropertyFlags.None, "This is the description for propB!"),
                },
                null);

            var propertiesType = new DiscriminatedObjectType(
                "properties",
                TypeSymbolValidationFlags.Default,
                "propType",
                new[] { propsA, propsB });

            return new ResourceTypeComponents(resourceType, ResourceScope.ResourceGroup, new ObjectType(resourceType.FormatName(), TypeSymbolValidationFlags.Default,
                AzResourceTypeProvider.GetCommonResourceProperties(resourceType).Concat(new[] {
                    new TypeProperty("properties", propertiesType, TypePropertyFlags.Required, "properties property"),
                }), null));
        }

        private static ResourceTypeComponents DiscriminatedPropertiesTestsType2()
        {
            var resourceType = ResourceTypeReference.Parse("Test.Rp/discriminatedPropertiesTests2@2020-01-01");

            var bodyAProps = new ObjectType(
                "BodyAProperties",
                TypeSymbolValidationFlags.WarnOnTypeMismatch,
                new[] {
                    new TypeProperty("propA", LanguageConstants.String, TypePropertyFlags.None, "This is the description for propA!"),
                },
                null);

            var bodyBProps = new ObjectType(
                "BodyBProperties",
                TypeSymbolValidationFlags.WarnOnTypeMismatch,
                new[] {
                    new TypeProperty("propB", LanguageConstants.String, TypePropertyFlags.None, "This is the description for propB!"),
                },
                null);

            var propertiesType = new DiscriminatedObjectType(
                "properties",
                TypeSymbolValidationFlags.Default,
                "propType",
                new[] {
                    new ObjectType("BodyA", TypeSymbolValidationFlags.Default, AzResourceTypeProvider.GetCommonResourceProperties(resourceType).Concat(new [] {
                        new TypeProperty("propType", new StringLiteralType("PropertiesA"), TypePropertyFlags.None, "This is the propType of body A"),
                        new TypeProperty("values", bodyAProps, TypePropertyFlags.None, "These are the properties for body A"),
                    }), null),
                    new ObjectType("BodyB", TypeSymbolValidationFlags.Default, AzResourceTypeProvider.GetCommonResourceProperties(resourceType).Concat(new [] {
                        new TypeProperty("propType", new StringLiteralType("PropertiesB"), TypePropertyFlags.None, "This is the propType of body B"),
                        new TypeProperty("values", bodyBProps, TypePropertyFlags.None, "These are the properties for body B"),
                    }), null),
                });

            return new ResourceTypeComponents(resourceType, ResourceScope.ResourceGroup, new ObjectType(resourceType.FormatName(), TypeSymbolValidationFlags.Default,
                AzResourceTypeProvider.GetCommonResourceProperties(resourceType).Concat(new[] {
                    new TypeProperty("properties", propertiesType, TypePropertyFlags.Required, "properties property"),
                }), null));
        }

        private static ResourceTypeComponents FallbackPropertyTestsType()
        {
            var resourceType = ResourceTypeReference.Parse("Test.Rp/fallbackProperties@2020-01-01");

            var propertiesType = new ObjectType("Properties", TypeSymbolValidationFlags.WarnOnTypeMismatch, new[] {
                new TypeProperty("required", LanguageConstants.String, TypePropertyFlags.Required, "This is a property which is required."),
            }, null);

            return new ResourceTypeComponents(resourceType, ResourceScope.ResourceGroup, new ObjectType(resourceType.FormatName(), TypeSymbolValidationFlags.Default,
                AzResourceTypeProvider.GetCommonResourceProperties(resourceType).Concat(new[] {
                    new TypeProperty("properties", propertiesType, TypePropertyFlags.Required, "properties property"),
                }).Concat(
                    AzResourceTypeProvider.KnownTopLevelResourceProperties().Where(p => !string.Equals(p.Name, "properties", LanguageConstants.IdentifierComparison))
                                     .Select(p => new TypeProperty(p.Name, p.TypeReference, TypePropertyFlags.None, "Property that does something important"))
                ), null));
        }

        public static INamespaceProvider Create()
            => TestTypeHelper.CreateProviderWithTypes(new[] {
                BasicTestsType(),
                ReadWriteTestsType(),
                DiscriminatorTestsType(),
                DiscriminatedPropertiesTestsType(),
                DiscriminatedPropertiesTestsType2(),
                FallbackPropertyTestsType(),
            });
    }
}
