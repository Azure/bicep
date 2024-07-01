// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;

namespace Bicep.Core.Configuration;

public static class ProviderAliasesConfigurationExtensions
{
    public static ProviderAliasesConfiguration WithProviderAlias(this ProviderAliasesConfiguration c, string providerAliasJsonString)
        => ProviderAliasesConfiguration.Bind(
                JsonDocument.Parse(providerAliasJsonString).RootElement,
                null);

    public static ProvidersConfiguration WithProvidersConfiguration(this ProvidersConfiguration c, string providersJsonString)
        => ProvidersConfiguration.Bind(JsonDocument.Parse(providersJsonString).RootElement);

    public static ImplicitProvidersConfiguration WithImplicitProvidersConfiguration(this ImplicitProvidersConfiguration c, string implicitProvidersJsonString)
        => ImplicitProvidersConfiguration.Bind(JsonDocument.Parse(implicitProvidersJsonString).RootElement);


    public static RootConfiguration WithProviderAlias(this RootConfiguration rootConfiguration, string payload)
    {
        return new RootConfiguration(
            rootConfiguration.Cloud,
            rootConfiguration.ModuleAliases,
            rootConfiguration.ExtensionAliases.WithProviderAlias(payload),
            rootConfiguration.Extensions,
            rootConfiguration.ImplicitExtensions,
            rootConfiguration.Analyzers,
            rootConfiguration.CacheRootDirectory,
            rootConfiguration.ExperimentalFeaturesEnabled,
            rootConfiguration.Formatting,
            rootConfiguration.ConfigFileUri,
            rootConfiguration.DiagnosticBuilders);
    }

    public static RootConfiguration WithProvidersConfiguration(this RootConfiguration rootConfiguration, string payload)
    {
        return new RootConfiguration(
            rootConfiguration.Cloud,
            rootConfiguration.ModuleAliases,
            rootConfiguration.ExtensionAliases,
            rootConfiguration.Extensions.WithProvidersConfiguration(payload),
            rootConfiguration.ImplicitExtensions,
            rootConfiguration.Analyzers,
            rootConfiguration.CacheRootDirectory,
            rootConfiguration.ExperimentalFeaturesEnabled,
            rootConfiguration.Formatting,
            rootConfiguration.ConfigFileUri,
            rootConfiguration.DiagnosticBuilders);
    }

    public static RootConfiguration WithImplicitProvidersConfiguration(this RootConfiguration rootConfiguration, string payload)
    {
        return new RootConfiguration(
            rootConfiguration.Cloud,
            rootConfiguration.ModuleAliases,
            rootConfiguration.ExtensionAliases,
            rootConfiguration.Extensions,
            rootConfiguration.ImplicitExtensions.WithImplicitProvidersConfiguration(payload),
            rootConfiguration.Analyzers,
            rootConfiguration.CacheRootDirectory,
            rootConfiguration.ExperimentalFeaturesEnabled,
            rootConfiguration.Formatting,
            rootConfiguration.ConfigFileUri,
            rootConfiguration.DiagnosticBuilders);
    }
}
