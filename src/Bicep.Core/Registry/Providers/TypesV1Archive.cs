// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Formats.Tar;
using System.IO;
using System.IO.Abstractions;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

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

            /* TODO:
               * read index.json
               * figure out paths to other .json files that need to be included
               * read other .json files, add them to tgz
            */
        }

        stream.Seek(0, SeekOrigin.Begin);
        return stream;
    }

    private static async Task AddFileToTar(TarWriter tarWriter, string archivePath, string contents)
    {
        var tarEntry = new PaxTarEntry(TarEntryType.RegularFile, archivePath)
        {
            DataStream = new MemoryStream(Encoding.UTF8.GetBytes(contents))
        };

        await tarWriter.WriteEntryAsync(tarEntry);
    }
}