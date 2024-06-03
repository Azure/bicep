// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Formats.Tar;
using System.IO.Abstractions;
using System.IO.Compression;
using System.Text;
using Azure.Bicep.Types;
using Azure.Bicep.Types.Serialization;
using Azure.Deployments.Core.Extensions;
using Bicep.Core.Extensions;
using Bicep.Core.Intermediate;

namespace Bicep.Core.Registry.Providers;

public static class ProviderV1Archive
{
    public static async Task<BinaryData> Build(ProviderPackage provider)
    {
        using var stream = new MemoryStream();

        using (var gzStream = new GZipStream(stream, CompressionMode.Compress, leaveOpen: true))
        {
            using var tarWriter = new TarWriter(gzStream, leaveOpen: true);

            await AddFileToTar(tarWriter, "types.tgz", provider.Types);
            if (provider.OsxArm64Binary is {})
            {
                await AddFileToTar(tarWriter, "osx-arm64.bin", provider.OsxArm64Binary);
            }
            if (provider.LinuxX64Binary is {})
            {
                await AddFileToTar(tarWriter, "linux-x64.bin", provider.LinuxX64Binary);
            }
            if (provider.WinX64Binary is {})
            {
                await AddFileToTar(tarWriter, "win-x64.bin", provider.WinX64Binary);
            }
        }

        stream.Seek(0, SeekOrigin.Begin);

        return BinaryData.FromStream(stream);
    }

    public static ProviderPackage Read(BinaryData binaryData)
    {
        using var gzipStream = new GZipStream(binaryData.ToStream(), CompressionMode.Decompress);
        using var tarReader = new TarReader(gzipStream);

        var dataDict = new Dictionary<string, BinaryData>();

        while (tarReader.GetNextEntry() is { } entry)
        {
            var stream = entry.DataStream ?? throw new InvalidOperationException($"Stream for {entry.Name} is null.");
            dataDict[entry.Name] = BinaryData.FromStream(stream);
        }

        return new(
            dataDict["types.tgz"],
            dataDict.TryGetValue("win-x64.bin"),
            dataDict.TryGetValue("linux-x64.bin"),
            dataDict.TryGetValue("osx-arm64.bin"));
    }

    private static async Task AddFileToTar(TarWriter tarWriter, string archivePath, BinaryData binaryData)
    {
        var tarEntry = new PaxTarEntry(TarEntryType.RegularFile, archivePath)
        {
            DataStream = binaryData.ToStream(),
        };

        await tarWriter.WriteEntryAsync(tarEntry);
    }
}

public static class TypesV1Archive
{
    public static async Task<BinaryData> GenerateProviderTarStream(IFileSystem fileSystem, string indexJsonPath)
    {
        using var stream = new MemoryStream();

        using (var gzStream = new GZipStream(stream, CompressionMode.Compress, leaveOpen: true))
        {
            using var tarWriter = new TarWriter(gzStream, leaveOpen: true);

            var indexJson = await fileSystem.File.ReadAllTextAsync(indexJsonPath);
            await AddFileToTar(tarWriter, "index.json", indexJson);

            var indexJsonParentPath = Path.GetDirectoryName(indexJsonPath);
            var uniqueTypePaths = GetAllUniqueTypePaths(indexJsonPath, fileSystem);

            foreach (var relativePath in uniqueTypePaths)
            {
                var absolutePath = Path.Combine(indexJsonParentPath!, relativePath);
                var typesJson = await fileSystem.File.ReadAllTextAsync(absolutePath);
                await AddFileToTar(tarWriter, relativePath, typesJson);
            }
        }

        stream.Seek(0, SeekOrigin.Begin);

        return BinaryData.FromStream(stream);
    }

    private static async Task AddFileToTar(TarWriter tarWriter, string archivePath, string contents)
    {
        var tarEntry = new PaxTarEntry(TarEntryType.RegularFile, archivePath)
        {
            DataStream = new MemoryStream(Encoding.UTF8.GetBytes(contents))
        };

        await tarWriter.WriteEntryAsync(tarEntry);
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

