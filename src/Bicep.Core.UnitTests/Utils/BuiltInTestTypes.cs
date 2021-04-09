// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Azure.Bicep.Types.Concrete;
using Bicep.Core.Resources;

namespace Bicep.Core.UnitTests.Utils
{
    /// <summary>
    /// A set of reusable resource types to validate various pieces of functionality.
    /// </summary>
    public class BuiltInTestTypes
    {
        private readonly TypeFactory factory;
        private readonly IReadOnlyDictionary<BuiltInTypeKind, ITypeReference> builtInTypes;

        private BuiltInTestTypes(TypeFactory factory)
        {
            this.factory = factory;
            this.builtInTypes = new Dictionary<BuiltInTypeKind, ITypeReference>
            {
                [BuiltInTypeKind.Any] = AddType(factory, new BuiltInType(BuiltInTypeKind.Any)),
                [BuiltInTypeKind.Null] = AddType(factory, new BuiltInType(BuiltInTypeKind.Null)),
                [BuiltInTypeKind.Bool] = AddType(factory, new BuiltInType(BuiltInTypeKind.Bool)),
                [BuiltInTypeKind.Int] = AddType(factory, new BuiltInType(BuiltInTypeKind.Int)),
                [BuiltInTypeKind.String] = AddType(factory, new BuiltInType(BuiltInTypeKind.String)),
                [BuiltInTypeKind.Object] = AddType(factory, new BuiltInType(BuiltInTypeKind.Object)),
                [BuiltInTypeKind.Array] = AddType(factory, new BuiltInType(BuiltInTypeKind.Array)),
            };

            RegisterBasicTestsType();
            RegisterReadWriteTestsType();
            RegisterDiscriminatorTestsType();
        }

        private IReadOnlyDictionary<string, ObjectProperty> GetDefaultResourceProperties(ResourceTypeReference resourceType)
        {
            var apiVersionType = AddType(factory, new StringLiteralType(resourceType.ApiVersion));
            var typeType = AddType(factory, new StringLiteralType(resourceType.FullyQualifiedType));
            var stringType = builtInTypes[BuiltInTypeKind.String];

            return new Dictionary<string, ObjectProperty>
            {
                ["name"] = new(stringType, ObjectPropertyFlags.DeployTimeConstant | ObjectPropertyFlags.Required, "name property"),
                ["type"] = new(typeType, ObjectPropertyFlags.DeployTimeConstant | ObjectPropertyFlags.ReadOnly, "type property"),
                ["apiVersion"] = new(apiVersionType, ObjectPropertyFlags.DeployTimeConstant | ObjectPropertyFlags.ReadOnly, "apiVersion property"),
                ["id"] = new(stringType, ObjectPropertyFlags.DeployTimeConstant | ObjectPropertyFlags.ReadOnly, "id property"),
            };
        }

        private void RegisterBasicTestsType()
        {
            var resourceType = ResourceTypeReference.Parse("Test.Rp/basicTests@2020-01-01");

            var stringType = builtInTypes[BuiltInTypeKind.String];

            var bodyType = AddType(factory, new ObjectType(
                resourceType.FormatName(),
                new Dictionary<string, ObjectProperty>(GetDefaultResourceProperties(resourceType))
                {
                    ["kind"] = new(stringType, ObjectPropertyFlags.ReadOnly, "kind property"),
                },
                null));

            AddType(factory, new ResourceType(resourceType.FormatName(), ScopeType.ResourceGroup, bodyType));
        }

        private void RegisterReadWriteTestsType()
        {
            var resourceType = ResourceTypeReference.Parse("Test.Rp/readWriteTests@2020-01-01");

            var stringType = builtInTypes[BuiltInTypeKind.String];

            var propertiesType = AddType(factory, new ObjectType(
                "Properties",
                new Dictionary<string, ObjectProperty>
                {
                    ["readwrite"] = new(stringType, ObjectPropertyFlags.None, "This is a property which supports reading AND writing!"),
                    ["readonly"] = new(stringType, ObjectPropertyFlags.ReadOnly, "This is a property which only supports reading."),
                    ["writeonly"] = new(stringType, ObjectPropertyFlags.WriteOnly, "This is a property which only supports writing."),
                    ["required"] = new(stringType, ObjectPropertyFlags.Required, "This is a property which is required."),
                },
                null));

            var bodyType = AddType(factory, new ObjectType(
                resourceType.FormatName(),
                new Dictionary<string, ObjectProperty>(GetDefaultResourceProperties(resourceType))
                {
                    ["properties"] = new(propertiesType, ObjectPropertyFlags.Required, "properties property"),
                },
                null));

            AddType(factory, new ResourceType(resourceType.FormatName(), ScopeType.ResourceGroup, bodyType));
        }

        private void RegisterDiscriminatorTestsType()
        {
            var resourceType = ResourceTypeReference.Parse("Test.Rp/discriminatorTests@2020-01-01");

            var stringType = builtInTypes[BuiltInTypeKind.String];

            var bodyAProps = AddType(factory, new ObjectType(
                "BodyAProperties",
                new Dictionary<string, ObjectProperty>
                {
                    ["propA"] = new(stringType, ObjectPropertyFlags.None, "This is the description for propA!"),
                },
                null));

            var bodyBProps = AddType(factory, new ObjectType(
                "BodyBProperties",
                new Dictionary<string, ObjectProperty>
                {
                    ["propB"] = new(stringType, ObjectPropertyFlags.None, "This is the description for propB!"),
                },
                null));

            var bodyType = AddType(factory, new DiscriminatedObjectType(
                resourceType.FormatName(),
                "kind",
                new Dictionary<string, ObjectProperty>(GetDefaultResourceProperties(resourceType)),
                new Dictionary<string, ITypeReference>
                {
                    ["BodyA"] = AddType(factory, new ObjectType(
                        "BodyA",
                        new Dictionary<string, ObjectProperty>
                        {
                            ["kind"] = new(AddType(factory, new StringLiteralType("BodyA")), ObjectPropertyFlags.None, "This is the kind of body A"),
                            ["properties"] = new(bodyAProps, ObjectPropertyFlags.None, "These are the properties for body A"),
                        },
                        null)),
                    ["BodyB"] = AddType(factory, new ObjectType(
                        "BodyB",
                        new Dictionary<string, ObjectProperty>
                        {
                            ["kind"] = new(AddType(factory, new StringLiteralType("BodyB")), ObjectPropertyFlags.None, "This is the kind of body B"),
                            ["properties"] = new(bodyBProps, ObjectPropertyFlags.None, "These are the properties for body B"),
                        },
                        null)),
                }));

            AddType(factory, new ResourceType(resourceType.FormatName(), ScopeType.ResourceGroup, bodyType));
        }

        private static ITypeReference AddType(TypeFactory factory, TypeBase type)
        {
            type = factory.Create(() => type);

            return factory.GetReference(type);
        }

        public static Core.TypeSystem.IResourceTypeProvider Create()
            => ResourceTypeProviderHelper.CreateAzResourceTypeProvider(factory => {
                new BuiltInTestTypes(factory);
            });
    }
}