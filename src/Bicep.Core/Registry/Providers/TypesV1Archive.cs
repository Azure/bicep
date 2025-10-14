// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Formats.Tar;
using System.IO.Abstractions;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
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
        var typeIndexPath = typeIndexFile.Uri.Path;
        var typeIndexJson = await typeIndexFile.ReadAllTextAsync();

        if (string.IsNullOrWhiteSpace(typeIndexJson))
        {
            throw new InvalidOperationException($"Extension type index \"{typeIndexPath}\" is empty.");
        }

        TypeIndex typeIndex;
        try
        {
            typeIndex = TypeSerializer.DeserializeIndex(new MemoryStream(Encoding.UTF8.GetBytes(typeIndexJson)));
        }
        catch (JsonException exception)
        {
            throw new InvalidOperationException($"Extension type index \"{typeIndexPath}\" could not be parsed: {exception.Message}", exception);
        }

        if (typeIndex.Resources is null || typeIndex.Resources.Count == 0)
        {
            throw new InvalidOperationException($"Extension type index \"{typeIndexPath}\" must define at least one entry in the \"resources\" object.");
        }

        using var stream = new MemoryStream();
        using (var tgzWriter = new TgzWriter(stream, leaveOpen: true))
        {
            await tgzWriter.WriteEntryAsync("index.json", typeIndexJson);

            var typeDirectory = typeIndexFile.GetParent();

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
}
