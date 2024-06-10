// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Text;
using Azure.Bicep.Types;
using Azure.Bicep.Types.Concrete;
using Azure.Bicep.Types.Index;
using Azure.Bicep.Types.Serialization;

namespace Bicep.Local.Extension.Mock.Types;

public static class TypeGenerator
{
    private static string GetString(Action<Stream> streamWriteFunc)
    {
        using var memoryStream = new MemoryStream();
        streamWriteFunc(memoryStream);

        return Encoding.UTF8.GetString(memoryStream.ToArray());
    }

    private static Dictionary<string, string> GenerateTypes()
    {
        var factory = new TypeFactory([]);

        var secureStringType = factory.Create(() => new StringType(sensitive: true));
        var stringType = factory.Create(() => new StringType());

        var echoBodyType = factory.Create(() => new ObjectType("body", new Dictionary<string, ObjectTypeProperty>
        {
            ["payload"] = new(factory.GetReference(stringType), ObjectTypePropertyFlags.Required, null),
        }, null));

        var echoType = factory.Create(() => new ResourceType(
            "echo",
            ScopeType.Unknown,
            null,
            factory.GetReference(echoBodyType),
            ResourceFlags.None,
            null));

        var settings = new TypeSettings(
            name: "Mock",
            version: "0.0.1",
            isSingleton: true,
            configurationType: null!);

        var resourceTypes = new[] {
            echoType,
        };

        var index = new TypeIndex(
            resourceTypes.ToDictionary(x => x.Name, x => new CrossFileTypeReference("types.json", factory.GetIndex(x))),
            new Dictionary<string, IReadOnlyDictionary<string, IReadOnlyList<CrossFileTypeReference>>>(),
            settings,
            null);

        return new Dictionary<string, string>
        {
            ["index.json"] = GetString(stream => TypeSerializer.SerializeIndex(stream, index)),
            ["types.json"] = GetString(stream => TypeSerializer.Serialize(stream, factory.GetTypes())),
        };
    }

    public static void WriteTypes(string outdir)
    {
        var types = TypeGenerator.GenerateTypes();

        Directory.CreateDirectory(outdir);
        foreach (var (relativePath, contents) in types)
        {
            var filePath = Path.Combine(outdir, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

            File.WriteAllText(filePath, contents);
        }
    }
}
