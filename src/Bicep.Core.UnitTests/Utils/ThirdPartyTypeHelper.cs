// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Formats.Tar;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
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

        var fooType = factory.Create(() => new ResourceType(
            "fooType@v1",
            ScopeType.Unknown,
            ScopeType.Unknown,
            factory.GetReference(fooBodyType),
            ResourceFlags.None,
            null));

        var index = new TypeIndex(new Dictionary<string, CrossFileTypeReference>
            {
                [fooType.Name] = new CrossFileTypeReference("types.json", factory.GetIndex(fooType)),
            }, new Dictionary<string, IReadOnlyDictionary<string, IReadOnlyList<CrossFileTypeReference>>>(),
            null!,
            null!);

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
