// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;

namespace Bicep.Core.Configuration;

public record ProviderConfigEntry
{
    public bool BuiltIn { get; }
    public string? Source { get; }
    public string? Version { get; }

    public ProviderConfigEntry(bool builtIn, string? source, string? version)
    {
        BuiltIn = builtIn;
        Source = source;
        Version = version;
        if (source is not null && version is not null && builtIn)
        {
            BuiltIn = false;
        }
    }
}

public partial class ProvidersConfiguration : ConfigurationSection<ImmutableDictionary<string, ProviderConfigEntry>>
{
    private readonly Uri? configurationPath;

    private ProvidersConfiguration(ImmutableDictionary<string, ProviderConfigEntry> data, Uri? configurationPath) : base(data)
    {
        this.configurationPath = configurationPath;
    }

    public static ProvidersConfiguration Bind(JsonElement element, Uri? configurationPath)
        => new(element.ToNonNullObject<ImmutableDictionary<string, ProviderConfigEntry>>(), configurationPath);

    public ResultWithDiagnostic<ProviderConfigEntry> TryGetProviderSource(string providerName)
    {
        if (!this.Data.TryGetValue(providerName, out var providerConfigEntry))
        {
            return new(x => x.ProviderNotFoundInConfiguration(providerName, configurationPath));
        }
        return new(providerConfigEntry);
    }
}


