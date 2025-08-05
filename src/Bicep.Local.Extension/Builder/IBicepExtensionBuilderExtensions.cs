// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Local.Extension.Host.Extensions;
using Bicep.Local.Extension.Host.Handlers;

namespace Microsoft.Extensions.DependencyInjection;

public static class IBicepExtensionBuilderExtensions
{
    /// <summary>
    /// Registers a resource handler that manages a specific resource type in your Bicep extension.
    /// </summary>
    public static IBicepExtensionBuilder WithResourceHandler<T>(this IBicepExtensionBuilder builder)
        where T : class, IResourceHandler
    {
        builder.Services.AddSingleton<IResourceHandler, T>();

        return builder;
    }

    /// <summary>
    /// Registers a resource handler that manages a specific resource type in your Bicep extension.
    /// </summary>
    public static IBicepExtensionBuilder WithResourceHandler(this IBicepExtensionBuilder builder, IResourceHandler resourceHandler)
    {
        builder.Services.AddSingleton<IResourceHandler>(resourceHandler);

        return builder;
    }
}
