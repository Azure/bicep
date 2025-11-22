// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bicep.Core.Extensions;

namespace Bicep.Core.Configuration;

// Represents the configuration object for a single external input resolver
public record ExternalInputResolverEntry
{
    // Path to the resolver target (executable / script). Required.
    public string? Target { get; init; }

    // Resolver-specific parameter bag passed to the resolver tooling. Required.
    // Properties are specific to the external input kind.
    public IDictionary<string, JsonElement>? Parameters { get; init; }
}

// The top-level map: key = external input kind (supports wildcards e.g. ev2.*) -> resolver entry
public partial class ExternalInputResolverConfiguration : ConfigurationSection<ImmutableDictionary<string, ExternalInputResolverEntry>>
{
    private ExternalInputResolverConfiguration(ImmutableDictionary<string, ExternalInputResolverEntry> data) : base(data) { }

    public static ExternalInputResolverConfiguration Empty => new(ImmutableDictionary.Create<string, ExternalInputResolverEntry>());

    public static ExternalInputResolverConfiguration Bind(JsonElement element)
    {
        // Treat an absent or null object as empty configuration
        if (element.ValueKind == JsonValueKind.Undefined || element.ValueKind == JsonValueKind.Null)
        {
            return Empty;
        }

        var dict = element.ToNonNullObject<ImmutableDictionary<string, ExternalInputResolverEntry>>();
        return new ExternalInputResolverConfiguration(dict);
    }

    public bool TryGetResolver(string kind, out ExternalInputResolverEntry entry)
        => this.Data.TryGetValue(kind, out entry!);
}
