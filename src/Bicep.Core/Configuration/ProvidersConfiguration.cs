// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using ProvidersDictionary = System.Collections.Immutable.ImmutableDictionary<string, Bicep.Core.Configuration.ProviderSource>;

namespace Bicep.Core.Configuration;

public record ProviderSource
{
    public bool Builtin { get; }
    public string? Source { get; }
    public string? Version { get; }
    // public bool IsImplicit { get; set;}

    public ProviderSource(bool builtin, string? source, string? version)
    {
        Builtin = builtin;
        Source = source;
        Version = version;
        if (Builtin && (Source != null || Version != null))
        {
            throw new ArgumentException("The 'builtin' property is mutually exclusive with 'registry' and 'version'.");
        }
    }
}

public partial class ProvidersConfiguration : ConfigurationSection<ProvidersDictionary>
{
    private readonly Uri? configurationPath;

    private ProvidersConfiguration(ProvidersDictionary data, Uri? configurationPath) : base(data)
    {
        this.configurationPath = configurationPath;
    }

    public static ProvidersConfiguration Bind(JsonElement element, Uri? configurationPath) => new(element.ToNonNullObject<ProvidersDictionary>(), configurationPath);

    public ResultWithDiagnostic<ProviderSource> TryGetProviderSource(string providerName)
    {
        if (!this.Data.TryGetValue(providerName, out var providerSource))
        {
            return new(x => x.ProviderNameDoesNotExistInConfiguration(providerName, configurationPath));
        }
        return new(providerSource);
    }
}


