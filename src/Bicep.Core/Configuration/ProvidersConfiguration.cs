// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.Json;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics.Namespaces;

namespace Bicep.Core.Configuration;

public record ProviderConfigEntry
{
    public bool BuiltIn => this.Value == "builtin:";

    public string Value { get; }

    public ProviderConfigEntry(string value)
    {
        Value = value;
    }

    public override string ToString()
        => Value;
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

    public bool IsSysOrBuiltIn(string providerName)
        => providerName == SystemNamespaceType.BuiltInName || this.Data.TryGetValue(providerName)?.BuiltIn == true;
}
