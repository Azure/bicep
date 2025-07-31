// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Formats.Tar;
using System.IO.Abstractions;
using System.IO.Compression;
using System.Text;
using System.Web.Services.Description;
using Azure.Bicep.Types;
using Azure.Bicep.Types.Index;
using Azure.Bicep.Types.Serialization;
using Azure.Deployments.Core.Extensions;
using Bicep.Core.Extensions;
using Bicep.Core.Intermediate;
using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;
using Bicep.IO.Utils;

namespace Bicep.Core.Registry.Extensions;

public static class TypesV1Archive
{
    public static async Task<BinaryData> PackIntoBinaryData(IFileHandle typeIndexFile)
    {
        using var stream = new MemoryStream();
        using (var tgzWriter = new TgzWriter(stream, leaveOpen: true))
        {
            var typeIndexJson = await typeIndexFile.ReadAllTextAsync();
            await tgzWriter.WriteEntryAsync("index.json", typeIndexJson);

            var typeDirectory = typeIndexFile.GetParent();
            var typeIndex = TypeSerializer.DeserializeIndex(new MemoryStream(Encoding.UTF8.GetBytes(typeIndexJson)));

            foreach (var typesJsonPath in EnumerateDistinctTypeReferences(typeIndex))
            {
                var typesJson = await typeDirectory.GetFile(typesJsonPath).ReadAllTextAsync();
                await tgzWriter.WriteEntryAsync(typesJsonPath, typesJson);
            }
        }

        stream.Seek(0, SeekOrigin.Begin);

        return BinaryData.FromStream(stream);
    }

    private static IEnumerable<string> EnumerateDistinctTypeReferences(TypeIndex index)
    {
        var allTypeReferences = index.Resources.Values
            .Concat(index.FallbackResourceType is { } fallbackType ? [fallbackType] : [])
            .Concat(index.Settings?.ConfigurationType is { } configType ? [configType] : []);

        return allTypeReferences.Select(r => r.RelativePath).Distinct();
    }

    // TODO(file-io-abstraction): This is only used by tests. Needs refactoring.
    public static async Task<BinaryData> GenerateExtensionTarStream(IFileSystem fileSystem, string indexJsonPath)
    {
        using var stream = new MemoryStream();
        using (var tgzWriter = new TgzWriter(stream, leaveOpen: true))
        {
            var indexJson = await fileSystem.File.ReadAllTextAsync(indexJsonPath);
            await tgzWriter.WriteEntryAsync("index.json", indexJson);

            var indexJsonParentPath = Path.GetDirectoryName(indexJsonPath);
            var uniqueTypePaths = GetAllUniqueTypePaths(indexJsonPath, fileSystem);

            foreach (var relativePath in uniqueTypePaths)
            {
                var absolutePath = Path.Combine(indexJsonParentPath!, relativePath);
                var typesJson = await fileSystem.File.ReadAllTextAsync(absolutePath);
                await tgzWriter.WriteEntryAsync(relativePath, typesJson);
            }
        }

        stream.Seek(0, SeekOrigin.Begin);

        return BinaryData.FromStream(stream);
    }

    private static IEnumerable<string> GetAllUniqueTypePaths(string pathToIndex, IFileSystem fileSystem)
    {
        using var indexStream = fileSystem.FileStream.New(pathToIndex, FileMode.Open, FileAccess.Read);

        var index = TypeSerializer.DeserializeIndex(indexStream);

        var typeReferences = index.Resources.Values.ToList();
        if (index.Settings?.ConfigurationType is { } configType)
        {
            typeReferences.Add(configType);
        }
        if (index.FallbackResourceType is { } fallbackType)
        {
            typeReferences.Add(fallbackType);
        }

        return typeReferences.Select(x => x.RelativePath).Distinct();
    }
}

