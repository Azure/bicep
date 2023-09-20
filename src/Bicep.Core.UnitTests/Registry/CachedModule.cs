// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using FluentAssertions;

namespace Bicep.Core.UnitTests.Registry;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
record Module(Layer[] layers);

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
record Layer(string mediaType);

// Represents a cached module in the local on-disk registry cache
public record CachedModule(
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

    public bool HasSourceLayer => LayerMediaTypes.Contains("application/vnd.ms.bicep.module.source.v1+zip");

    public SourceArchive? TryGetSource()
    {
        var sourceArchivePath = Path.Combine(ModuleCacheFolder, $"source.zip");
        if (File.Exists(sourceArchivePath))
        {
            return new SourceArchive(File.OpenRead(sourceArchivePath));
        }

        return null;
    }
}
