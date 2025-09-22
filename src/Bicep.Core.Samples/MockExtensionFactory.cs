// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Bicep.Types.Concrete;
using Bicep.Core.UnitTests.Utils;
using Bicep.TextFixtures.Mocks;

namespace Bicep.Core.Samples;

public static class MockExtensionFactory
{
    public static MockExtensionData CreateMockExtWithNoConfigType(string extName) =>
        ExtensionTestHelper.CreateMockExtensionMockData(
            extName, "1.2.3", "v1", CustomExtensionTypeFactoryDelegates.NoTypes);

    public static MockExtensionData CreateMockExtWithObjectConfigType(string extName, Func<CreateCustomExtensionTypeContext, IReadOnlyDictionary<string, ObjectTypeProperty>>? configTypeFn = null) =>
        ExtensionTestHelper.CreateMockExtensionMockData(
            extName, "1.2.3", "v1", new CustomExtensionTypeFactoryDelegates
            {
                CreateConfigurationType = (ctx, tf) => tf.Create(() => new ObjectType(
                    "config",
                    configTypeFn?.Invoke(ctx) ?? new Dictionary<string, ObjectTypeProperty>
                    {
                        ["requiredString"] = new(ctx.CoreStringTypeRef, ObjectTypePropertyFlags.Required, null),
                        ["optionalString"] = new(ctx.CoreStringTypeRef, ObjectTypePropertyFlags.None, null)
                    },
                    null))
            });

    public static MockExtensionData CreateMockExtWithOptionalObjectConfigType(string extName) => CreateMockExtWithObjectConfigType(
        extName,
        ctx => new Dictionary<string, ObjectTypeProperty>
        {
            ["optionalSecureString"] = new(ctx.CoreSecureStringTypeRef, ObjectTypePropertyFlags.None, null),
            ["optionalString"] = new(ctx.CoreStringTypeRef, ObjectTypePropertyFlags.None, null)
        });

    public static MockExtensionData CreateMockExtWithSecureConfigType(string extName) =>
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

    public static MockExtensionData CreateMockExtWithDiscriminatedConfigType(string extName) =>
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
