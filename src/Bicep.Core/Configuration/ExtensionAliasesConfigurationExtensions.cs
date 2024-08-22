// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;

namespace Bicep.Core.Configuration;

public static class ExtensionAliasesConfigurationExtensions
{
    public static ExtensionAliasesConfiguration WithExtensionAlias(this ExtensionAliasesConfiguration c, string extensionAliasJsonString)
        => ExtensionAliasesConfiguration.Bind(
                JsonDocument.Parse(extensionAliasJsonString).RootElement,
                null);

    public static ExtensionsConfiguration WithExtensions(this ExtensionsConfiguration c, string extensionsJsonString)
        => ExtensionsConfiguration.Bind(JsonDocument.Parse(extensionsJsonString).RootElement);

    public static ImplicitExtensionsConfiguration WithImplicitExtensions(this ImplicitExtensionsConfiguration c, string implicitExtensionsJsonString)
        => ImplicitExtensionsConfiguration.Bind(JsonDocument.Parse(implicitExtensionsJsonString).RootElement);


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
