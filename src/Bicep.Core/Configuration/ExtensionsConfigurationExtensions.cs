// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;

namespace Bicep.Core.Configuration;

public static class ExtensionsConfigurationExtensions
{
    public static ExtensionsConfiguration WithExtensions(this ExtensionsConfiguration c, string extensionsJsonString)
        => ExtensionsConfiguration.Bind(JsonDocument.Parse(extensionsJsonString).RootElement);

    public static ImplicitExtensionsConfiguration WithImplicitExtensions(this ImplicitExtensionsConfiguration c, string implicitExtensionsJsonString)
        => ImplicitExtensionsConfiguration.Bind(JsonDocument.Parse(implicitExtensionsJsonString).RootElement);

    public static RootConfiguration WithExtensions(this RootConfiguration rootConfiguration, string payload)
    {
        return new RootConfiguration(
            rootConfiguration.Cloud,
            rootConfiguration.ModuleAliases,
            rootConfiguration.Extensions.WithExtensions(payload),
            rootConfiguration.ImplicitExtensions,
            rootConfiguration.Analyzers,
            rootConfiguration.CacheRootDirectory,
            rootConfiguration.ExperimentalFeaturesWarning,
            rootConfiguration.ExperimentalFeaturesEnabled,
            rootConfiguration.Formatting,
            rootConfiguration.ConfigFileUri,
            rootConfiguration.Diagnostics);
    }

    public static RootConfiguration WithImplicitExtensions(this RootConfiguration rootConfiguration, string payload)
    {
        return new RootConfiguration(
            rootConfiguration.Cloud,
            rootConfiguration.ModuleAliases,
            rootConfiguration.Extensions,
            rootConfiguration.ImplicitExtensions.WithImplicitExtensions(payload),
            rootConfiguration.Analyzers,
            rootConfiguration.CacheRootDirectory,
            rootConfiguration.ExperimentalFeaturesWarning,
            rootConfiguration.ExperimentalFeaturesEnabled,
            rootConfiguration.Formatting,
            rootConfiguration.ConfigFileUri,
            rootConfiguration.Diagnostics);
    }
}
