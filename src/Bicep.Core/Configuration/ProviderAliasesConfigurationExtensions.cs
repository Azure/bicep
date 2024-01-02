// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;

namespace Bicep.Core.Configuration;

public static class ProviderAliasesConfigurationExtensions
{
    public static ProviderAliasesConfiguration WithProviderAlias(this ProviderAliasesConfiguration c, string providerAliasJsonString)
    {
        return ProviderAliasesConfiguration.Bind(
                JsonDocument.Parse(providerAliasJsonString).RootElement,
                null
        );
    }

    public static RootConfiguration WithProviderAlias(this RootConfiguration rootConfiguration, string payload)
    {
        return new RootConfiguration(
            rootConfiguration.Cloud,
            rootConfiguration.ModuleAliases,
            rootConfiguration.ProviderAliases.WithProviderAlias(payload),
            rootConfiguration.Analyzers,
            rootConfiguration.CacheRootDirectory,
            rootConfiguration.ExperimentalFeaturesEnabled,
            rootConfiguration.Formatting,
            rootConfiguration.ConfigurationPath,
            rootConfiguration.DiagnosticBuilders);
    }
}