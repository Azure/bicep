// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Resources;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Bicep.LanguageServer.Snippets;

public record SnippetCache(
    ImmutableDictionary<ResourceTypeReference, SnippetCache.SnippetCacheSnippet> ResourceTypeReferenceInfoMap,
    ImmutableDictionary<ResourceTypeReference, string> ResourceTypeReferenceToDependentsMap,
    ImmutableDictionary<ResourceTypeReference, ImmutableArray<ResourceTypeReference>> ResourceTypeReferenceToChildTypeSymbolsMap,
    ImmutableArray<Snippet> TopLevelNamedDeclarationSnippets)
{
    private static JsonSerializerOptions GetSerializerOptions()
    {
        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };
        options.Converters.Add(new ResourceTypeReferenceJsonConverter());

        return options;
    }

    public record SnippetCacheSnippet(
        string Prefix,
        string Identifier,
        string BodyText,
        string Description);

    public static SnippetCache Deserialize(Stream stream)
        => JsonSerializer.Deserialize<SnippetCache>(stream, GetSerializerOptions())
            ?? throw new InvalidOperationException($"Failed to deserialize embedded snippet cache");

    public static string Serialize(SnippetCache cache)
        => JsonSerializer.Serialize<SnippetCache>(cache, GetSerializerOptions());

    private static readonly Lazy<SnippetCache> lazySnippetCache = new(FromManifestInternal);
    private static SnippetCache FromManifestInternal()
    {
        var assembly = typeof(SnippetCache).Assembly;
        var stream = assembly.GetManifestResourceStream("Files/SnippetCache.json")
            ?? throw new InvalidOperationException($"Failed to find embedded snippet cache");

        return Deserialize(stream);
    }

    public static SnippetCache FromManifest() => lazySnippetCache.Value;
}
