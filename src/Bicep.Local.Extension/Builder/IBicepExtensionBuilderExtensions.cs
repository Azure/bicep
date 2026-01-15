// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Reflection;
using Azure.Bicep.Types.Concrete;
using Bicep.Local.Extension.Builder;
using Bicep.Local.Extension.Host.Extensions;
using Bicep.Local.Extension.Host.Handlers;
using Bicep.Local.Extension.Types;
using Bicep.Local.Extension.Types.Attributes;
using Bicep.Local.Extension.Types.Models;

namespace Microsoft.Extensions.DependencyInjection;

public static class IBicepExtensionBuilderExtensions
{
    /// <summary>
    /// Registers extension metadata with the builder, including the extension's name, version, and singleton status.
    /// </summary>
    /// <param name="builder">The extension builder to which the extension information will be added. Cannot be null.</param>
    /// <param name="name">The name of the extension to register. Cannot be null or empty.</param>
    /// <param name="version">The version string representing the extension's version. Cannot be null or empty.</param>
    /// <param name="isSingleton">A value indicating whether the extension should be registered as a singleton. If <see langword="true"/>, only
    /// one instance of the extension will be created.</param>
    /// <returns>The same <see cref="IBicepExtensionBuilder"/> instance, enabling method chaining.</returns>
    public static IBicepExtensionBuilder WithExtensionInfo(this IBicepExtensionBuilder builder, string name, string version, bool isSingleton = true)
    {
        builder.Services.AddSingleton(new BicepExtensionInfo(name, version, isSingleton));

        return builder;
    }

    /// <summary>
    /// Adds a singleton type configuration to the extension builder, allowing services to access the specified
    /// configuration type during dependency injection.
    /// </summary>
    /// <remarks>This method registers the provided configuration type as a singleton service. Subsequent
    /// calls with a non-null configuration will overwrite any previously registered type configuration.</remarks>
    /// <param name="builder">The extension builder to which the type configuration will be added.</param>
    /// <param name="configuration">The type representing the configuration to register. If <see langword="null"/>, no configuration is added.</param>
    /// <returns>The same <see cref="IBicepExtensionBuilder"/> instance, enabling method chaining.</returns>
    public static IBicepExtensionBuilder WithTypeConfiguration(this IBicepExtensionBuilder builder, Type? configuration)
    {
        if (configuration is not null)
        {
            builder.Services.AddSingleton<Type>(configuration);
        }

        return builder;
    }

    /// <summary>
    /// Configures the specified Bicep extension builder to use the default type provider and type builder services for
    /// type resolution.
    /// </summary>
    /// <remarks>This method registers default type resolution services, including a type provider, type
    /// factory, and a mapping of common .NET types to Bicep type representations. It also optionally registers a
    /// configuration type if provided. Call this method to enable standard type handling in Bicep extension
    /// scenarios.</remarks>
    /// <param name="builder">The Bicep extension builder to configure with default type provider and type builder services.</param>
    /// <param name="typeAssembly">The assembly containing type definitions to be registered with the type provider.</param>
    /// <param name="configurationType">An optional configuration type to be registered. If not null, this type will be added to the service collection.</param>
    /// <returns>The same Bicep extension builder instance, configured with default type provider and type builder services.</returns>
    public static IBicepExtensionBuilder WithDefaultTypeBuilder(this IBicepExtensionBuilder builder, Assembly typeAssembly, Type? configurationType = null)
    {
        builder.Services.AddSingleton<ITypeProvider>(new TypeProvider([typeAssembly]));
        builder.Services.AddSingleton(new TypeDefinitionBuilderOptions(configurationType));

        builder.Services.AddSingleton<ITypeDefinitionBuilder>(sp => new TypeDefinitionBuilder
        (
            extensionInfo: sp.GetRequiredService<BicepExtensionInfo>(),
            typeProvider: sp.GetRequiredService<ITypeProvider>(),
            options: sp.GetRequiredService<TypeDefinitionBuilderOptions>()
        ));

        return builder;
    }

    public static IBicepExtensionBuilder WithTypeBuilder(this IBicepExtensionBuilder builder, ITypeDefinitionBuilder typeDefinitionBuilder)
    {
        builder.Services.AddSingleton<ITypeDefinitionBuilder>(typeDefinitionBuilder);
        return builder;
    }

    public static IBicepExtensionBuilder WithFallbackType<TFallback>(this IBicepExtensionBuilder builder)
        where TFallback : class
    {        
        if (builder.Services.Any(sd => sd.ServiceType == typeof(FallbackTypeRegistration)))
        {
            throw new InvalidOperationException(
                $"A fallback type has already been registered. Only one fallback type is allowed per extension.");
        }
        
        if(typeof(TFallback).GetCustomAttribute<ResourceTypeAttribute>() is null)
        {
            throw new InvalidOperationException(
                $"The fallback type '{typeof(TFallback).FullName}' must be annotated with the ResourceTypeAttribute.");
        }

        builder.Services.AddSingleton(new FallbackTypeRegistration(typeof(TFallback)));
        return builder;
    }

    /// <summary>
    /// Registers a custom type definition builder with the Bicep extension builder using the specified factory
    /// function.
    /// </summary>
    /// <remarks>Use this method to supply a custom implementation of <see cref="ITypeDefinitionBuilder"/> for
    /// advanced type definition scenarios. The registered builder will be available as a singleton within the
    /// extension's service collection.</remarks>
    /// <param name="builder">The Bicep extension builder to configure.</param>
    /// <param name="implementationFactory">A factory function that creates an instance of <see cref="ITypeDefinitionBuilder"/> using the provided <see
    /// cref="IServiceProvider"/>.</param>
    /// <returns>The same <see cref="IBicepExtensionBuilder"/> instance, enabling method chaining.</returns>
    public static IBicepExtensionBuilder WithTypeBuilder(this IBicepExtensionBuilder builder, Func<IServiceProvider, ITypeDefinitionBuilder> implementationFactory)
    {
        builder.Services.AddSingleton<ITypeDefinitionBuilder>(implementationFactory);
        return builder;
    }

    /// <summary>
    /// Registers a singleton type builder of the specified type with the extension builder's service collection.
    /// </summary>
    /// <remarks>Use this method to add a custom <see cref="ITypeDefinitionBuilder"/> implementation to the
    /// extension builder. Subsequent calls to this method will replace any previously registered type builder of the
    /// same interface.</remarks>
    /// <typeparam name="T">The type of the type definition builder to register. Must implement <see cref="ITypeDefinitionBuilder"/> and be
    /// a reference type.</typeparam>
    /// <param name="builder">The extension builder to which the type builder will be added.</param>
    /// <returns>The same <see cref="IBicepExtensionBuilder"/> instance, enabling method chaining.</returns>
    public static IBicepExtensionBuilder WithTypeBuilder<T>(this IBicepExtensionBuilder builder)
        where T : class, ITypeDefinitionBuilder
    {
        builder.Services.AddSingleton<ITypeDefinitionBuilder, T>();

        return builder;
    }

    /// <summary>
    /// Registers a resource handler of the specified type with the extension builder for dependency injection.
    /// </summary>
    /// <remarks>This method adds the specified resource handler type as a singleton service. Subsequent calls
    /// to this method can be used to register additional resource handlers.</remarks>
    /// <typeparam name="T">The type of resource handler to register. Must implement <see cref="IResourceHandler"/> and have a public
    /// constructor.</typeparam>
    /// <param name="builder">The extension builder to which the resource handler will be added.</param>
    /// <returns>The same <see cref="IBicepExtensionBuilder"/> instance to allow for method chaining.</returns>
    public static IBicepExtensionBuilder WithResourceHandler<T>(this IBicepExtensionBuilder builder)
        where T : class, IResourceHandler
    {
        builder.Services.AddSingleton<IResourceHandler, T>();

        return builder;
    }

    /// <summary>
    /// Registers a resource handler of the specified type with the extension builder for dependency injection.
    /// </summary>
    /// <remarks>This method adds the specified resource handler type as a singleton service. Subsequent calls
    /// to this method can be used to register additional resource handlers.</remarks>
    /// <param name="builder">The extension builder to configure with the resource handler. Cannot be null.</param>
    /// <param name="resourceHandler">The resource handler instance to register. Cannot be null.</param>
    /// <returns>The same extension builder instance, enabling method chaining.</returns>
    public static IBicepExtensionBuilder WithResourceHandler(this IBicepExtensionBuilder builder, IResourceHandler resourceHandler)
    {
        builder.Services.AddSingleton<IResourceHandler>(resourceHandler);

        return builder;
    }

    /// <summary>
    /// Registers all resource handlers found in the specified assembly with the extension builder for dependency injection.
    /// </summary>
    /// <remarks>This method scans the provided assembly for all concrete types that implement <see cref="IResourceHandler"/>
    /// and registers each one as a singleton service. Abstract classes and interfaces are excluded from registration.
    /// This is useful for bulk registration of resource handlers from a single assembly without needing to register
    /// each handler individually.</remarks>
    /// <param name="builder">The extension builder to which the resource handlers will be added. Cannot be null.</param>
    /// <param name="handlerAssembly">The assembly to scan for resource handler implementations. Cannot be null.</param>
    /// <returns>The same <see cref="IBicepExtensionBuilder"/> instance, enabling method chaining.</returns>
    public static IBicepExtensionBuilder WithResourceHandlerAssembly(this IBicepExtensionBuilder builder, Assembly handlerAssembly)
    {
        var handlerTypes = handlerAssembly.GetTypes()
            .Where(t => typeof(IResourceHandler).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var handlerType in handlerTypes)
        {
            builder.Services.AddSingleton(typeof(IResourceHandler), handlerType);
        }

        return builder;
    }
}
