// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Bicep.Core.Configuration;

namespace Bicep.Testing.Extensions;

public static class ExtensionsConfigurationExtensions
{
    public static ExtensionsConfiguration WithExtensions(this ExtensionsConfiguration c, string extensionsJsonString)
        => ExtensionsConfiguration.Bind(JsonDocument.Parse(extensionsJsonString).RootElement);

    public static ImplicitExtensionsConfiguration WithImplicitExtensions(this ImplicitExtensionsConfiguration c, string implicitExtensionsJsonString)
        => ImplicitExtensionsConfiguration.Bind(JsonDocument.Parse(implicitExtensionsJsonString).RootElement);

    public static IBicepConfiguration WithExtensions(this IBicepConfiguration configuration, string payload)
        => configuration.With(extensions: ((ExtensionsConfiguration)configuration.Extensions).WithExtensions(payload));

    public static IBicepConfiguration WithImplicitExtensions(this IBicepConfiguration configuration, string payload)
        => configuration.With(implicitExtensions: ((ImplicitExtensionsConfiguration)configuration.ImplicitExtensions).WithImplicitExtensions(payload));
}
