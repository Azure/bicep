// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Reflection;
using Azure.Bicep.Types.Concrete;
using Bicep.Local.Extension.Builder;
using Bicep.Local.Extension.Host.Extensions;
using Bicep.Local.Extension.Host.Handlers;
using Bicep.Local.Extension.Types;

namespace Microsoft.Extensions.DependencyInjection;

public static class IBicepExtensionBuilderExtensions
{
    public static IBicepExtensionBuilder WithExtensionInfo(this IBicepExtensionBuilder builder, string name, string version, bool isSingleton)
    {
        builder.Services.AddSingleton<BicepExtensionInfo>(new BicepExtensionInfo(name, version, isSingleton));

        return builder;
    }

    public static IBicepExtensionBuilder WithTypeAssembly(this IBicepExtensionBuilder builder, Assembly typeAssembly)
    {
        builder.Services.AddSingleton<ITypeProvider>(new TypeProvider([typeAssembly]));



        return builder;
    }

    public static IBicepExtensionBuilder WithTypeConfiguration<T>(this IBicepExtensionBuilder builder)
        where T : class
    {
        builder.Services.AddSingleton<T>();

        return builder;
    }

    public static IBicepExtensionBuilder WithDefaultTypeBuilder(this IBicepExtensionBuilder builder)
    {
        var typeDictionary = new Dictionary<Type, Func<TypeBase>>
                    {
                        { typeof(string), () => new StringType() },
                        { typeof(bool), () => new BooleanType() },
                        { typeof(int), () => new IntegerType() }
                    }.ToImmutableDictionary();
        var typeFactory = new TypeFactory([]);

        foreach (var type in typeDictionary)
        {
            typeFactory.Create(type.Value);
        }

        builder.Services.AddSingleton<TypeFactory>(typeFactory);
        builder.Services.AddSingleton<IDictionary<Type, Func<TypeBase>>>(typeDictionary);

        builder.WithTypeBuilder<TypeDefinitionBuilder>();

        return builder;
    }

    public static IBicepExtensionBuilder WithTypeBuilder<T>(this IBicepExtensionBuilder builder)
        where T : class, ITypeDefinitionBuilder
    {
        builder.Services.AddSingleton<ITypeDefinitionBuilder, T>();

        return builder;
    }

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
