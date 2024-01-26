// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;

namespace Bicep.Core.Configuration;

public record ProvidersConfigurationSection(ImmutableSortedDictionary<string, ProviderSource> Providers) { }

public record ProviderSource{
    public bool Builtin { get; }
    public string? Registry { get; }
    public string? Version { get; }
    // public bool IsImplicit { get; set;}
    
    public ProviderSource(bool builtin, string? registry, string? version) 
    {
        Builtin = builtin;
        Registry = registry;
        Version = version;
        if (Builtin && (Registry != null || Version != null))
        {
            throw new ArgumentException("The 'builtin' property is mutually exclusive with 'registry' and 'version'.");
        }
    }    
}

public partial class ProvidersConfiguration : ConfigurationSection<ProvidersConfigurationSection>
{
    private readonly Uri? configurationPath;

    private ProvidersConfiguration(ProvidersConfigurationSection data, Uri? configurationPath) : base(data)
    {
        this.configurationPath = configurationPath;
    }

    public static ProvidersConfiguration Bind(JsonElement element, Uri? configurationPath) => new(element.ToNonNullObject<ProvidersConfigurationSection>(), configurationPath);

    public ResultWithDiagnostic<ProviderSource> TryGetProviderSource(string providerName)
    {
        if (!this.Data.Providers.TryGetValue(providerName, out var providerSource))
        {
            return new(x => x.ProviderNameDoesNotExistInConfiguration(providerName, configurationPath));
        }
        return new(providerSource);
    }
}


