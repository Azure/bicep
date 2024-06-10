// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Formats.Tar;
using System.IO.Compression;
using Bicep.Core.Extensions;
using Bicep.Core.Registry.Oci;

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

            foreach (var binary in provider.Binaries)
            {
                await AddFileToTar(tarWriter, $"{binary.Architecture.Name}.bin", binary.Data);
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

        var binaries = new List<ProviderBinary>();
        foreach (var architecture in SupportedArchitectures.All)
        {
            if (dataDict.TryGetValue($"{architecture.Name}.bin") is { } binary)
            {
                binaries.Add(new(architecture, binary));
            }
        }

        return new(
            Types: dataDict["types.tgz"],
            LocalDeployEnabled: binaries.Count != 0,
            Binaries: [.. binaries]);
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

