// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json;
using Bicep.Core.Extensions;

namespace Bicep.Core.Configuration;

public record ProvidersConfigurationSection(ImmutableSortedDictionary<string, ProviderSource> Providers) { }

public record ProviderSource(bool Builtin, string? Registry, string? Version) { }

public partial class ProvidersConfiguration : ConfigurationSection<ProvidersConfigurationSection>
{
    private readonly string? configurationPath;

    private ProvidersConfiguration(ProvidersConfigurationSection data, string? configurationPath): base(data)
    {
        this.configurationPath = configurationPath;
    }

    public static ProvidersConfiguration Bind(JsonElement element, string? configurationPath) => new(element.ToNonNullObject<ProvidersConfigurationSection>(), configurationPath);

}


