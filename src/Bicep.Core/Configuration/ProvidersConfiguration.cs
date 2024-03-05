// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.Json;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;

namespace Bicep.Core.Configuration;

public record ProviderConfigEntry
{
    public bool BuiltIn => this.Scheme == "builtin";

    public string Path { get; }

    public string Scheme {get;}

    public ProviderConfigEntry(string providerConfigEntry)
    {
        var parts = providerConfigEntry.Split(':', StringSplitOptions.None);
        Debug.Assert(parts.Length is >= 1 and <= 3, "The provider configuration entry must have 1-3 parts separated by colons.");

        this.Scheme = parts[0]; // Is ensured to exist since there is a pattern match in the bicepconfig json schema
        this.Path = parts.Length > 1 ? string.Join(':', parts[1..]) : string.Empty;
    }

    public override string ToString()
    {
        return  $"{this.Scheme}:{this.Path}";
    }
}

public partial class ProvidersConfiguration : ConfigurationSection<ImmutableDictionary<string, ProviderConfigEntry>>
{
    private ProvidersConfiguration(ImmutableDictionary<string, ProviderConfigEntry> data) : base(data) { }

    public static ProvidersConfiguration Bind(JsonElement element)
        => new(element.ToNonNullObject<ImmutableDictionary<string, string>>()
            .ToImmutableDictionary(
                pair => pair.Key,
                pair => new ProviderConfigEntry(pair.Value))
        );

    public ResultWithDiagnostic<ProviderConfigEntry> TryGetProviderSource(string providerName)
    {
        if (!this.Data.TryGetValue(providerName, out var providerConfigEntry))
        {
            return new(x => x.UnrecognizedProvider(providerName));
        }
        return new(providerConfigEntry);
    }

    public override void WriteTo(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        foreach (var (key, value) in this.Data)
        {
            writer.WritePropertyName(key);
            writer.WriteStringValue(value.ToString());
        }
        writer.WriteEndObject();
    }
}


