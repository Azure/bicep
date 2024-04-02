// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Formats.Tar;
using System.IO.Compression;
using System.Text;
using Azure.Bicep.Types;
using Azure.Bicep.Types.Concrete;
using Azure.Bicep.Types.Index;
using Azure.Bicep.Types.Serialization;

namespace Bicep.Core.UnitTests.Utils;

public static class ThirdPartyTypeHelper
{
    /// <summary>
    /// Returns a .tgz file containing a set of pre-defined types for testing purposes.
    /// </summary>
    public static BinaryData GetTestTypesTgz()
    {
        var factory = new TypeFactory(Enumerable.Empty<TypeBase>());
        var rootFactory = new TypeFactory(Enumerable.Empty<TypeBase>());

        var stringType = factory.Create(() => new StringType());

        var fooBodyPropertiesType = factory.Create(() => new ObjectType("fooBody", new Dictionary<string, ObjectTypeProperty>
        {
            ["readwrite"] = new(factory.GetReference(stringType), ObjectTypePropertyFlags.None, "This is a property which supports reading AND writing!"),
            ["readonly"] = new(factory.GetReference(stringType), ObjectTypePropertyFlags.ReadOnly, "This is a property which only supports reading."),
            ["writeonly"] = new(factory.GetReference(stringType), ObjectTypePropertyFlags.WriteOnly, "This is a property which only supports writing."),
            ["required"] = new(factory.GetReference(stringType), ObjectTypePropertyFlags.Required, "This is a property which is required."),
        }, null));

        var fooBodyType = factory.Create(() => new ObjectType("fooBody", new Dictionary<string, ObjectTypeProperty>
        {
            ["identifier"] = new(factory.GetReference(stringType), ObjectTypePropertyFlags.Required | ObjectTypePropertyFlags.Identifier, "The resource identifier"),
            ["properties"] = new(factory.GetReference(fooBodyPropertiesType), ObjectTypePropertyFlags.Required, "Resource properties"),
        }, null));

        var barFunctionType = factory.Create(() => new FunctionType([
            new FunctionParameter("bar", factory.GetReference(stringType), "The bar parameter"),
        ], factory.GetReference(stringType)));

        var fooType = factory.Create(() => new ResourceType(
            "fooType@v1",
            ScopeType.Unknown,
            null,
            factory.GetReference(fooBodyType),
            ResourceFlags.None,
            new Dictionary<string, ResourceTypeFunction>
            {
                ["convertBarToBaz"] = new(factory.GetReference(barFunctionType), "Converts a bar into a baz!")
            }));

        var settings = new TypeSettings(name: "ThirdPartyProvider", version: "1.0.0", isSingleton: false, configurationType: null!);

        var index = new TypeIndex(new Dictionary<string, CrossFileTypeReference>
        {
            [fooType.Name] = new CrossFileTypeReference("types.json", factory.GetIndex(fooType)),
        }, new Dictionary<string, IReadOnlyDictionary<string, IReadOnlyList<CrossFileTypeReference>>>(),
            settings,
            null);

        return GetTypesTgzBytesFromFiles(
            ("index.json", StreamHelper.GetString(stream => TypeSerializer.SerializeIndex(stream, index))),
            ("types.json", StreamHelper.GetString(stream => TypeSerializer.Serialize(stream, factory.GetTypes()))));
    }

    public static BinaryData GetTestTypesTgzWithFallbackAndConfiguration()
    {
        var factory = new TypeFactory(Enumerable.Empty<TypeBase>());
        var rootFactory = new TypeFactory(Enumerable.Empty<TypeBase>());

        var stringType = factory.Create(() => new StringType());
        var stringTypeRoot = rootFactory.Create(() => new StringType());

        var fooBodyType = factory.Create(() => new ObjectType("fooBody", new Dictionary<string, ObjectTypeProperty>
        {
            ["identifier"] = new(factory.GetReference(stringType), ObjectTypePropertyFlags.Required | ObjectTypePropertyFlags.Identifier, "The resource identifier"),
            ["joke"] = new(factory.GetReference(stringType), ObjectTypePropertyFlags.Required | ObjectTypePropertyFlags.Identifier, "The foo body")
        }, null));

        var barFunctionType = factory.Create(() => new FunctionType([
            new FunctionParameter("bar", factory.GetReference(stringType), "The bar parameter"),
        ], factory.GetReference(stringType)));

        var fooType = factory.Create(() => new ResourceType(
            "fooType@v1",
            ScopeType.Unknown,
            null,
            factory.GetReference(fooBodyType),
            ResourceFlags.None,
            new Dictionary<string, ResourceTypeFunction>
            {
                ["convertBarToBaz"] = new(factory.GetReference(barFunctionType), "Converts a bar into a baz!")
            }));

        //setup fallback resource
        var fallbackBodyType = rootFactory.Create(() => new ObjectType("fallback body", new Dictionary<string, ObjectTypeProperty>
        {
            ["bodyProp"] = new(rootFactory.GetReference(stringTypeRoot), ObjectTypePropertyFlags.Required, "Body property"),
        }, null));

        var fallbackType = rootFactory.Create(() => new ResourceType(
            "fallback",
            ScopeType.Unknown,
            null,
            rootFactory.GetReference(fallbackBodyType),
            ResourceFlags.None,
            null));

        var fallbackResource = new CrossFileTypeReference("types.json", rootFactory.GetIndex(fallbackType));

        //setup configuration
        var configurationType = rootFactory.Create(() => new ObjectType("config", new Dictionary<string, ObjectTypeProperty>
        {
            ["namespace"] = new(rootFactory.GetReference(stringTypeRoot), ObjectTypePropertyFlags.Required, "The default ThirdParty namespace to deploy resources to."),
            ["config"] = new(rootFactory.GetReference(stringTypeRoot), ObjectTypePropertyFlags.Required, "Path to some configuration file."),
            ["context"] = new(rootFactory.GetReference(stringTypeRoot), ObjectTypePropertyFlags.None, "Not required context property.")
        }, null));

        var settings = new TypeSettings(name: "ThirdPartyProvider", version: "1.0.0", isSingleton: false, configurationType: new CrossFileTypeReference("types.json", rootFactory.GetIndex(configurationType)));

        var index = new TypeIndex(new Dictionary<string, CrossFileTypeReference>
        {
            [fooType.Name] = new CrossFileTypeReference("v1/types.json", factory.GetIndex(fooType)),
        }, new Dictionary<string, IReadOnlyDictionary<string, IReadOnlyList<CrossFileTypeReference>>>(),
            settings,
            fallbackResource);

        return GetTypesTgzBytesFromFiles(
            ("index.json", StreamHelper.GetString(stream => TypeSerializer.SerializeIndex(stream, index))),
            ("types.json", StreamHelper.GetString(stream => TypeSerializer.Serialize(stream, rootFactory.GetTypes()))),
            ("v1/types.json", StreamHelper.GetString(stream => TypeSerializer.Serialize(stream, factory.GetTypes()))));
    }

    public static BinaryData GetMockRadiusTypesTgz()
    {
        var factory = new TypeFactory(Enumerable.Empty<TypeBase>());

        var stringType = factory.Create(() => new StringType());

        var awsBucketPropertiesType = factory.Create(() => new ObjectType("properties", new Dictionary<string, ObjectTypeProperty>
        {
            ["BucketName"] = new(factory.GetReference(stringType), ObjectTypePropertyFlags.Identifier, null),
        }, null));

        var awsBucketsBodyType = factory.Create(() => new ObjectType("body", new Dictionary<string, ObjectTypeProperty>
        {
            ["name"] = new(factory.GetReference(stringType), ObjectTypePropertyFlags.None, "the resource name"),
            ["alias"] = new(factory.GetReference(stringType), ObjectTypePropertyFlags.Required | ObjectTypePropertyFlags.Identifier, "the resource alias"),
            ["properties"] = new(factory.GetReference(awsBucketPropertiesType), ObjectTypePropertyFlags.Identifier, "Bucket properties"),
        }, null));

        var awsBucketsType = factory.Create(() => new ResourceType(
            "AWS.S3/Bucket@default",
            ScopeType.Unknown,
            null,
            factory.GetReference(awsBucketsBodyType),
            ResourceFlags.None,
            null));

        var environmentsBodyType = factory.Create(() => new ObjectType("body", new Dictionary<string, ObjectTypeProperty>
        {
            ["name"] = new(factory.GetReference(stringType), ObjectTypePropertyFlags.Required | ObjectTypePropertyFlags.Identifier, "The resource name"),
            ["id"] = new(factory.GetReference(stringType), ObjectTypePropertyFlags.ReadOnly, "The resource id"),
        }, null));

        var environmentsType = factory.Create(() => new ResourceType(
            "Applications.Core/environments@2023-10-01-preview",
            ScopeType.Unknown,
            null,
            factory.GetReference(environmentsBodyType),
            ResourceFlags.None,
            null));

        var applicationsBodyType = factory.Create(() => new ObjectType("body", new Dictionary<string, ObjectTypeProperty>
        {
            ["name"] = new(factory.GetReference(stringType), ObjectTypePropertyFlags.Required | ObjectTypePropertyFlags.Identifier, "The resource name"),
            ["id"] = new(factory.GetReference(stringType), ObjectTypePropertyFlags.ReadOnly, "The resource id"),
        }, null));

        var applicationsType = factory.Create(() => new ResourceType(
            "Applications.Core/applications@2023-10-01-preview",
            ScopeType.Unknown,
            null,
            factory.GetReference(applicationsBodyType),
            ResourceFlags.None,
            null));

        var recipeType = factory.Create(() => new ObjectType("recipe", new Dictionary<string, ObjectTypeProperty>
        {
            ["name"] = new(factory.GetReference(stringType), ObjectTypePropertyFlags.Required, "The recipe name"),
        }, null));

        var extendersPropertiesType = factory.Create(() => new ObjectType("properties", new Dictionary<string, ObjectTypeProperty>
        {
            ["application"] = new(factory.GetReference(stringType), ObjectTypePropertyFlags.Required, "The application"),
            ["environment"] = new(factory.GetReference(stringType), ObjectTypePropertyFlags.Required, "The environment"),
            ["recipe"] = new(factory.GetReference(recipeType), ObjectTypePropertyFlags.Required, "The recipe"),
        }, null));

        var extendersBodyType = factory.Create(() => new ObjectType("body", new Dictionary<string, ObjectTypeProperty>
        {
            ["name"] = new(factory.GetReference(stringType), ObjectTypePropertyFlags.Required | ObjectTypePropertyFlags.Identifier, "The resource name"),
            ["properties"] = new(factory.GetReference(extendersPropertiesType), ObjectTypePropertyFlags.Required, "The resource properties"),
        }, null));

        var extendersType = factory.Create(() => new ResourceType(
            "Applications.Core/extenders@2023-10-01-preview",
            ScopeType.Unknown,
            null,
            factory.GetReference(extendersBodyType),
            ResourceFlags.None,
            null));

        var settings = new TypeSettings(name: "Radius", version: "1.0.0", isSingleton: false, configurationType: null!);

        var resourceTypes = new[] {
            environmentsType,
            applicationsType,
            extendersType,
            awsBucketsType,
        };

        var index = new TypeIndex(
            resourceTypes.ToDictionary(x => x.Name, x => new CrossFileTypeReference("types.json", factory.GetIndex(x))),
            new Dictionary<string, IReadOnlyDictionary<string, IReadOnlyList<CrossFileTypeReference>>>(),
            settings,
            null);

        return GetTypesTgzBytesFromFiles(
            ("index.json", StreamHelper.GetString(stream => TypeSerializer.SerializeIndex(stream, index))),
            ("types.json", StreamHelper.GetString(stream => TypeSerializer.Serialize(stream, factory.GetTypes()))));
    }

    public static BinaryData GetTypesTgzBytesFromFiles(params (string filePath, string contents)[] files)
    {
        var stream = new MemoryStream();
        using (var gzStream = new GZipStream(stream, CompressionMode.Compress, leaveOpen: true))
        {
            using var tarWriter = new TarWriter(gzStream, leaveOpen: true);
            foreach (var (filePath, contents) in files)
            {
                var tarEntry = new PaxTarEntry(TarEntryType.RegularFile, filePath)
                {
                    DataStream = new MemoryStream(Encoding.ASCII.GetBytes(contents))
                };
                tarWriter.WriteEntry(tarEntry);
            }
        }
        stream.Position = 0;
        return BinaryData.FromStream(stream);
    }
}
