// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.IO.Abstractions;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Bicep.Core.SourceLink;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;
using FluentAssertions;

namespace Bicep.Core.UnitTests.Registry;

// Represents the current state of the local on-disk registry cache

public static class CachedModules
{
    // Get all cached modules from the local on-disk registry cache
    public static ImmutableArray<CachedModule> GetCachedModules(IFileSystem fileSystem, IDirectoryHandle cacheRootDirectory)
    {
        var cacheDir = fileSystem.DirectoryInfo.New(cacheRootDirectory.Uri.GetLocalFilePath());
        if (!cacheDir.Exists)
        {
            return [];
        }

        // we create the "br" folder with same casing on all file systems
        var brDir = cacheDir.EnumerateDirectories().SingleOrDefault(dir => string.Equals(dir.Name, "br"));

        // the directory structure is .../br/<registry>/<repository>/<tag>
        var moduleDirectories = brDir?
            .EnumerateDirectories()
            .SelectMany(registryDir => registryDir.EnumerateDirectories())
            .SelectMany(repoDir => repoDir.EnumerateDirectories());

        return moduleDirectories?
            .Select(moduleDirectory => new CachedModule(
                fileSystem,
                moduleDirectory.FullName,
                UnobfuscateFolderName(moduleDirectory.Parent!.Parent!.Name),
                UnobfuscateFolderName(moduleDirectory.Parent!.Name),
                UnobfuscateFolderName(moduleDirectory.Name)))
            .ToImmutableArray()
        ?? [];
    }

    private static string UnobfuscateFolderName(string folderName)
    {
        return folderName.Replace("$", "/").TrimEnd('/');
    }
}

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
record Module(Layer[] layers);

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
record Layer(string mediaType);

// Represents a cached module in the local on-disk registry cache
public record CachedModule(
    IFileSystem FileSystem,
    string ModuleCacheFolder,
    string Registry,
    string Repository,
    string Tag)
{
    public string ManifestContents => File.ReadAllText(Path.Combine(ModuleCacheFolder, "manifest"));
    public JsonObject ManifestJson => (JsonObject)JsonNode.Parse(ManifestContents)!;

    public string MetadataContents => File.ReadAllText(Path.Combine(ModuleCacheFolder, "metadata"));
    public JsonObject MetadataJson => (JsonObject)JsonNode.Parse(MetadataContents)!;

    public string[] LayerMediaTypes
    {
        get
        {
            // Deserialize the JSON into an object
            var module = JsonSerializer.Deserialize<Module>(ManifestJson)!;
            module.layers.Should().NotBeNull("layers property should exist in manifest");
            string[] layerMediaTypes = module.layers.Select(layer => layer.mediaType).ToArray();
            return layerMediaTypes;
        }
    }

    public bool HasSourceLayer => LayerMediaTypes.Contains("application/vnd.ms.bicep.module.source.v1.tar+gzip");

    public ResultWithException<SourceArchive> TryGetSource()
    {
        var sourceArchivePath = FileSystem.Path.Combine(ModuleCacheFolder, $"source.tgz");
        if (FileSystem.File.Exists(sourceArchivePath))
        {
            var sourceTgzFileMock = StrictMock.Of<IFileHandle>();
            sourceTgzFileMock.Setup(x => x.Exists()).Returns(true);
            sourceTgzFileMock.Setup(x => x.OpenRead()).Returns(FileSystem.File.OpenRead(sourceArchivePath));
            return SourceArchive.TryUnpackFromFile(sourceTgzFileMock.Object);
        }

        return new(new SourceNotAvailableException());
    }
}
