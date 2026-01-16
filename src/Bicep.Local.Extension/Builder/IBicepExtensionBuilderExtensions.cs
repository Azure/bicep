// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Reflection;
using Bicep.Local.Extension.Builder.Models;
using Bicep.Local.Extension.Host.Extensions;
using Bicep.Local.Extension.Host.Handlers;
using Bicep.Local.Extension.Types;
using Bicep.Local.Extension.Types.Attributes;
using Bicep.Local.Extension.Types.Models;
using Bicep.Local.Rpc;

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

    public static IBicepExtensionBuilder WithExtensionInfo(this IBicepExtensionBuilder builder, string name, string version, bool isSingleton = true)
    {
        builder.Services.AddSingleton(new ExtensionInfo(name, version, isSingleton));
        return builder;
    }

    public static IBicepExtensionBuilder WithTypeDefinitionBuilder(this IBicepExtensionBuilder builder, ITypeDefinitionBuilder typeDefinitionBuilder)
    {
        builder.Services.AddSingleton<ITypeDefinitionBuilder>(typeDefinitionBuilder);
        return builder;
    }

    public static IBicepExtensionBuilder WithDefaultTypeDefinitionBuilder(this IBicepExtensionBuilder builder)
    {
        builder.Services.AddSingleton<ITypeProvider, TypeProvider>();
        builder.Services.AddSingleton<ITypeDefinitionBuilder, TypeDefinitionBuilder>();
        return builder;
    }

    public static IBicepExtensionBuilder WithTypeAssemblies(this IBicepExtensionBuilder builder, Assembly[] assemblies)
    {
        builder.Services.AddSingleton(new TypesAssemblyContainer(assemblies));
        return builder;
    }

    public static IBicepExtensionBuilder WithConfigurationType(this IBicepExtensionBuilder builder, Type configuartion)
    {
        builder.Services.AddSingleton(new ConfigurationTypeContainer(configuartion));
        return builder;
    }

    public static IBicepExtensionBuilder WithConfigurationType<TConfig>(this IBicepExtensionBuilder builder)
        => builder.WithConfigurationType(typeof(TConfig));

    public static IBicepExtensionBuilder WithFallbackType(this IBicepExtensionBuilder builder, Type fallbackType)
    {
        builder.Services.AddSingleton(new FallbackTypeContainer(fallbackType));
        return builder;
    }

    public static IBicepExtensionBuilder WithFallbackType<TFallback>(this IBicepExtensionBuilder builder)
        => builder.WithFallbackType(typeof(TFallback));

}
