// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.IO;
using System.IO.Abstractions;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using Azure.Bicep.Types.Serialization;

namespace Bicep.Core.Registry.Providers;

public static class TypesV1Archive
{
    public static async Task<Stream> GenerateProviderTarStream(IFileSystem fileSystem, string indexJsonPath)
    {
        using var stream = new MemoryStream();

        using (var gzStream = new GZipStream(stream, CompressionMode.Compress, leaveOpen: true))
        {
            using var tarWriter = new TarWriter(gzStream, leaveOpen: true);

            var indexJson = await fileSystem.File.ReadAllTextAsync(indexJsonPath);
            await AddFileToTar(tarWriter, "index.json", indexJson);

            HashSet<string> uniqueTypePaths = getAllUniqueTypePaths(indexJsonPath);

            foreach (string path in uniqueTypePaths)
            {
                var typesJson = await fileSystem.File.ReadAllTextAsync(path);
                await AddFileToTar(tarWriter, "types.json", typesJson);
            }
        }

        //stream.Seek(0, SeekOrigin.Begin);

        return new MemoryStream(stream.ToArray(), 0, stream.ToArray().Length, true);
    }

    private static async Task AddFileToTar(TarWriter tarWriter, string archivePath, string contents)
    {
        var tarEntry = new PaxTarEntry(TarEntryType.RegularFile, archivePath)
        {
            DataStream = new MemoryStream(Encoding.UTF8.GetBytes(contents))
        };

        await tarWriter.WriteEntryAsync(tarEntry);
    }

    private static HashSet<string> getAllUniqueTypePaths(string pathToIndex)
    {
        FileStream indexStream = new(pathToIndex, FileMode.Open, FileAccess.Read);

        var index = TypeSerializer.DeserializeIndex(indexStream);

        HashSet<string> uniqueTypePaths = new();

        foreach (var typeInformation in index.Resources)
        {
            uniqueTypePaths.Add(Path.GetDirectoryName(pathToIndex) + "\\" + typeInformation.Value.RelativePath);
        }

        return uniqueTypePaths;
    }
}

