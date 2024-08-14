// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;

namespace Bicep.Core.Configuration;

public static class ProviderAliasesConfigurationExtensions
{
    public static ProviderAliasesConfiguration WithExtensionAlias(this ProviderAliasesConfiguration c, string extensionAliasJsonString)
        => ProviderAliasesConfiguration.Bind(
                JsonDocument.Parse(extensionAliasJsonString).RootElement,
                null);

    public static ProvidersConfiguration WithExtensions(this ProvidersConfiguration c, string providersJsonString)
        => ProvidersConfiguration.Bind(JsonDocument.Parse(providersJsonString).RootElement);

    public static ImplicitProvidersConfiguration WithImplicitExtensions(this ImplicitProvidersConfiguration c, string implicitExtensionsJsonString)
        => ImplicitProvidersConfiguration.Bind(JsonDocument.Parse(implicitExtensionsJsonString).RootElement);


    public static RootConfiguration WithExtensionAliases(this RootConfiguration rootConfiguration, string payload)
    {
        return new RootConfiguration(
            rootConfiguration.Cloud,
            rootConfiguration.ModuleAliases,
            rootConfiguration.ExtensionAliases.WithExtensionAlias(payload),
            rootConfiguration.Extensions,
            rootConfiguration.ImplicitExtensions,
            rootConfiguration.Analyzers,
            rootConfiguration.CacheRootDirectory,
            rootConfiguration.ExperimentalFeaturesEnabled,
            rootConfiguration.Formatting,
            rootConfiguration.ConfigFileUri,
            rootConfiguration.DiagnosticBuilders);
    }

    public static RootConfiguration WithExtensions(this RootConfiguration rootConfiguration, string payload)
    {
        return new RootConfiguration(
            rootConfiguration.Cloud,
            rootConfiguration.ModuleAliases,
            rootConfiguration.ExtensionAliases,
            rootConfiguration.Extensions.WithExtensions(payload),
            rootConfiguration.ImplicitExtensions,
            rootConfiguration.Analyzers,
            rootConfiguration.CacheRootDirectory,
            rootConfiguration.ExperimentalFeaturesEnabled,
            rootConfiguration.Formatting,
            rootConfiguration.ConfigFileUri,
            rootConfiguration.DiagnosticBuilders);
    }

    public static RootConfiguration WithImplicitExtensions(this RootConfiguration rootConfiguration, string payload)
    {
        return new RootConfiguration(
            rootConfiguration.Cloud,
            rootConfiguration.ModuleAliases,
            rootConfiguration.ExtensionAliases,
            rootConfiguration.Extensions,
            rootConfiguration.ImplicitExtensions.WithImplicitExtensions(payload),
            rootConfiguration.Analyzers,
            rootConfiguration.CacheRootDirectory,
            rootConfiguration.ExperimentalFeaturesEnabled,
            rootConfiguration.Formatting,
            rootConfiguration.ConfigFileUri,
            rootConfiguration.DiagnosticBuilders);
    }
}
