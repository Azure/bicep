// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using static Bicep.Core.Diagnostics.DiagnosticBuilder;

namespace Bicep.Core.Configuration;

public record ProvidersConfigurationSection(ImmutableSortedDictionary<string, ProviderSource> Providers) { }

public record ProviderSource(bool Builtin, string? Registry, string? Version)
{
    public bool IsImplicit;
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


