// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Bicep.Types.Concrete;
using Bicep.Core.UnitTests.Utils;

namespace Bicep.Core.Samples;

public static class MockExtensionFactory
{
    public static RegistrySourcedExtensionMockData CreateMockExtWithNoConfigType(string extName) =>
        ExtensionTestHelper.CreateMockExtensionMockData(
            extName, "1.2.3", "v1", CustomExtensionTypeFactoryDelegates.NoTypes);

    public static RegistrySourcedExtensionMockData CreateMockExtWithObjectConfigType(string extName) =>
        ExtensionTestHelper.CreateMockExtensionMockData(
            extName, "1.2.3", "v1", new CustomExtensionTypeFactoryDelegates
            {
                CreateConfigurationType = (ctx, tf) => tf.Create(() => new ObjectType(
                    "config",
                    new Dictionary<string, ObjectTypeProperty>
                    {
                        ["requiredString"] = new(ctx.CoreStringTypeRef, ObjectTypePropertyFlags.Required, null),
                        ["optionalString"] = new(ctx.CoreStringTypeRef, ObjectTypePropertyFlags.None, null)
                    },
                    null))
            });

    public static RegistrySourcedExtensionMockData CreateMockExtWithSecureConfigType(string extName) =>
        ExtensionTestHelper.CreateMockExtensionMockData(
            extName, "1.2.3", "v1", new CustomExtensionTypeFactoryDelegates
            {
                CreateConfigurationType = (ctx, tf) => tf.Create(() => new ObjectType(
                    "config",
                    new Dictionary<string, ObjectTypeProperty>
                    {
                        ["requiredSecureString"] = new(ctx.CoreSecureStringTypeRef, ObjectTypePropertyFlags.Required, null),
                        ["optionalString"] = new(ctx.CoreStringTypeRef, ObjectTypePropertyFlags.None, null)
                    },
                    null))
            });

    public static RegistrySourcedExtensionMockData CreateMockExtWithDiscriminatedConfigType(string extName) =>
        ExtensionTestHelper.CreateMockExtensionMockData(
            extName, "1.2.3", "v1", new CustomExtensionTypeFactoryDelegates
            {
                CreateConfigurationType = (ctx, tf) => tf.Create(() => new DiscriminatedObjectType(
                    "config",
                    "discrim",
                    new Dictionary<string, ObjectTypeProperty>
                    {
                        ["z1"] = new(ctx.CoreStringTypeRef, ObjectTypePropertyFlags.None, null)
                    },
                    new Dictionary<string, ITypeReference>
                    {
                        ["a"] = ctx.CreateObjectType(
                            "aType", new Dictionary<string, ObjectTypeProperty>
                            {
                                ["discrim"] = new(ctx.CreateStringLiteralType("a"), ObjectTypePropertyFlags.Required, null),
                                ["a1"] = new(ctx.CoreStringTypeRef, ObjectTypePropertyFlags.Required, null),
                                ["a2"] = new(ctx.CoreStringTypeRef, ObjectTypePropertyFlags.None, null)
                            }),
                        ["b"] = ctx.CreateObjectType(
                            "bType", new Dictionary<string, ObjectTypeProperty>
                            {
                                ["discrim"] = new(ctx.CreateStringLiteralType("b"), ObjectTypePropertyFlags.Required, null),
                                ["b1"] = new(ctx.CoreStringTypeRef, ObjectTypePropertyFlags.Required, null)
                            })
                    }))
            });
}
