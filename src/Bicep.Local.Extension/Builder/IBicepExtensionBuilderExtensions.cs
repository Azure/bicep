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
    /// Registers a resource handler of the specified type with the Bicep extension builder.
    /// </summary>
    /// <remarks>Use this method to add a custom resource handler to the extension builder. The handler will
    /// be registered as a singleton and used to manage resource operations within the extension.</remarks>
    /// <typeparam name="T">The type of the resource handler to register. Must implement the IResourceHandler interface.</typeparam>
    /// <param name="builder">The Bicep extension builder to which the resource handler is added.</param>
    /// <returns>The same IBicepExtensionBuilder instance, enabling further configuration.</returns>
    public static IBicepExtensionBuilder WithResourceHandler<T>(this IBicepExtensionBuilder builder)
        where T : class, IResourceHandler
    {
        builder.Services.AddSingleton<IResourceHandler, T>();

        return builder;
    }

    /// <summary>
    /// Configures the specified Bicep extension builder to use the provided resource handler for managing resources
    /// within the extension.
    /// </summary>
    /// <remarks>This method registers the provided resource handler as a singleton service, making it
    /// available throughout the extension's lifecycle. Subsequent calls will replace any previously registered resource
    /// handler.</remarks>
    /// <param name="builder">The Bicep extension builder to configure.</param>
    /// <param name="resourceHandler">The resource handler instance to register for resource management.</param>
    /// <returns>The same instance of the Bicep extension builder, configured with the specified resource handler.</returns>
    public static IBicepExtensionBuilder WithResourceHandler(this IBicepExtensionBuilder builder, IResourceHandler resourceHandler)
    {
        ArgumentNullException.ThrowIfNull(resourceHandler);

        builder.Services.AddSingleton<IResourceHandler>(resourceHandler);

        return builder;
    }

    /// <summary>
    /// Adds extension metadata to the specified Bicep extension builder for use by the Bicep extension framework.
    /// </summary>
    /// <remarks>Use this method to associate identifying information with an extension, which can be
    /// leveraged by the Bicep extension framework for discovery and management. Registering as a singleton ensures only
    /// one instance of the extension is created within the builder's service collection.</remarks>
    /// <param name="builder">The Bicep extension builder to which the extension information will be added. Cannot be null.</param>
    /// <param name="name">The name of the extension to register. Cannot be null or empty.</param>
    /// <param name="version">The version of the extension to register. Cannot be null or empty.</param>
    /// <param name="isSingleton">Indicates whether the extension should be registered as a singleton. The default is <see langword="true"/>.</param>
    /// <returns>The same <see cref="IBicepExtensionBuilder"/> instance with the extension information registered.</returns>
    public static IBicepExtensionBuilder WithExtensionInfo(this IBicepExtensionBuilder builder, string name, string version, bool isSingleton = true)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(version);

        builder.Services.AddSingleton(new ExtensionInfo(name, version, isSingleton));
        return builder;
    }

    /// <summary>
    /// Adds a type definition builder to the Bicep extension builder, enabling custom type definitions to be registered
    /// for use within the extension.
    /// </summary>
    /// <remarks>This method registers the specified type definition builder as a singleton service, making it
    /// available for dependency injection throughout the Bicep extension.</remarks>
    /// <param name="builder">The Bicep extension builder to which the type definition builder will be added. Cannot be null.</param>
    /// <param name="typeDefinitionBuilder">The type definition builder instance to register. Cannot be null.</param>
    /// <returns>The updated Bicep extension builder instance, allowing for method chaining.</returns>
    public static IBicepExtensionBuilder WithTypeDefinitionBuilder(this IBicepExtensionBuilder builder, ITypeDefinitionBuilder typeDefinitionBuilder)
    {
        ArgumentNullException.ThrowIfNull(typeDefinitionBuilder);
        builder.Services.AddSingleton<ITypeDefinitionBuilder>(typeDefinitionBuilder);
        return builder;
    }

    /// <summary>
    /// Adds a type definition builder to the Bicep extension builder, enabling custom type definitions to be registered
    /// for use within the extension.
    /// </summary>
    /// <remarks>This method registers the specified type definition builder as a singleton service, making it
    /// available for dependency injection throughout the Bicep extension.</remarks>
    /// <typeparam name="TDefinitionBuilder">The type of the definition builder to register. Must implement the ITypeDefinitionBuilder interface.</typeparam>
    /// <param name="builder">The Bicep extension builder instance to which the type definition builder will be added.</param>
    /// <returns>The updated IBicepExtensionBuilder instance, enabling method chaining.</returns>
    public static IBicepExtensionBuilder WithTypeDefinitionBuilder<TDefinitionBuilder>(this IBicepExtensionBuilder builder)
        where TDefinitionBuilder : class, ITypeDefinitionBuilder
    {
        builder.Services.AddSingleton<ITypeDefinitionBuilder, TDefinitionBuilder>();
        return builder;
    }

    /// <summary>
    /// Configures the specified Bicep extension builder with default settings, including extension metadata and
    /// standard type providers.
    /// </summary>
    /// <remarks>This method applies a standard configuration to the extension builder, ensuring that
    /// essential type definitions and providers are included. It is recommended for most Bicep extension scenarios to
    /// promote consistency and reduce boilerplate setup.</remarks>
    /// <param name="builder">The Bicep extension builder to configure with default settings. Cannot be null.</param>
    /// <param name="name">The name of the Bicep extension to be configured. Cannot be null or empty.</param>
    /// <param name="version">The version of the Bicep extension to be configured. Cannot be null or empty.</param>
    /// <param name="isSingleton">A value indicating whether the extension should be registered as a singleton. The default is <see
    /// langword="true"/>.</param>
    /// <returns>The configured <see cref="IBicepExtensionBuilder"/> instance with default extension information and type
    /// providers applied.</returns>
    public static IBicepExtensionBuilder WithDefaults(this IBicepExtensionBuilder builder, string name, string version, bool isSingleton = true)
        => builder
            .WithExtensionInfo(name, version, isSingleton)
            .WithTypeDefinitionBuilder<TypeDefinitionBuilder>()
            .WithTypeProvider<TypeProvider>();

    /// <summary>
    /// Configures the specified Bicep extension builder to use the provided type provider for type resolution.
    /// </summary>
    /// <remarks>The type provider is registered as a singleton service, ensuring that the same instance is
    /// used throughout the application's lifetime.</remarks>
    /// <param name="builder">The Bicep extension builder to configure with the type provider.</param>
    /// <param name="typeProvider">The type provider to be registered with the Bicep extension builder. This instance will be used for resolving
    /// types within the extension.</param>
    /// <returns>The configured Bicep extension builder instance, enabling method chaining.</returns>
    public static IBicepExtensionBuilder WithTypeProvider(IBicepExtensionBuilder builder, ITypeProvider typeProvider)
    {
        ArgumentNullException.ThrowIfNull(typeProvider); 
        builder.Services.AddSingleton<ITypeProvider>(typeProvider);
        return builder;
    }

    /// <summary>
    /// Configures the specified Bicep extension builder to use the provided type provider for type resolution.
    /// </summary>
    /// <remarks>The type provider is registered as a singleton service, ensuring that the same instance is
    /// used throughout the application's lifetime.</remarks>
    /// <typeparam name="TTypeProvider">The type of the type provider to register. This type must implement the ITypeProvider interface.</typeparam>
    /// <param name="builder">The Bicep extension builder instance to which the type provider will be added.</param>
    /// <returns>The updated instance of the IBicepExtensionBuilder for further configuration.</returns>
    public static IBicepExtensionBuilder WithTypeProvider<TTypeProvider>(this IBicepExtensionBuilder builder)
        where TTypeProvider : class, ITypeProvider
    {
        builder.Services.AddSingleton<ITypeProvider, TTypeProvider>();
        return builder;
    }

    /// <summary>
    /// Configures the Bicep extension builder to use the specified assembly for type resolution.
    /// </summary>
    /// <remarks>This method allows the user to specify an assembly that contains type definitions, which can
    /// be utilized by the Bicep extension during processing.</remarks>
    /// <param name="builder">The Bicep extension builder instance to configure.</param>
    /// <param name="assembly">The assembly to be used for type resolution within the Bicep extension.</param>
    /// <returns>The configured instance of the Bicep extension builder.</returns>
    public static IBicepExtensionBuilder WithTypeAssembly(this IBicepExtensionBuilder builder, Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        builder.Services.AddSingleton(assembly);
        return builder;
    }

    /// <summary>
    /// Configures the Bicep extension builder to use the specified assembly for type resolution.
    /// </summary>
    /// <remarks>This method allows the user to specify an assembly that contains type definitions, which can
    /// be utilized by the Bicep extension during processing.</remarks>
    /// <typeparam name="TEntry">The type whose containing assembly will be used for configuration.</typeparam>
    /// <param name="builder">The Bicep extension builder to configure.</param>
    /// <returns>The configured instance of <see cref="IBicepExtensionBuilder"/>, enabling further configuration.</returns>
    public static IBicepExtensionBuilder WithTypeAssembly<TEntry>(this IBicepExtensionBuilder builder)
        => builder.WithTypeAssembly(typeof(TEntry).Assembly);
    
    /// <summary>
    /// Configures the Bicep extension builder to include the specified type assemblies.
    /// </summary>
    /// <remarks>This method iterates through the provided assemblies and adds each one to the builder. Ensure
    /// that the assemblies contain the necessary types for the Bicep extension to function correctly.</remarks>
    /// <param name="builder">The Bicep extension builder to configure with the type assemblies.</param>
    /// <param name="assemblies">An array of assemblies that contain types to be included in the Bicep extension. Cannot be null.</param>
    /// <returns>The configured instance of the Bicep extension builder, allowing for method chaining.</returns>
    public static IBicepExtensionBuilder WithTypeAssemblies(this IBicepExtensionBuilder builder, Assembly[] assemblies)
    {
        if (assemblies is null || assemblies.Length == 0)
        {
            throw new ArgumentException("Assembly array cannot be null or empty. Use at least one assembly.", nameof(assemblies));
        }

        foreach (var a in  assemblies)
        {
            builder.WithTypeAssembly(a);
        }

        return builder;
    }

    /// <summary>
    /// Registers a configuration type for the Bicep extension, enabling customization of the extension's behavior using
    /// the specified configuration type.
    /// </summary>
    /// <remarks>Call this method before invoking any other configuration-related methods to ensure the
    /// configuration type is set correctly.</remarks>
    /// <param name="builder">The builder instance used to configure the Bicep extension.</param>
    /// <param name="configuartion">The type that represents the configuration to be registered. This type must not have been registered previously.</param>
    /// <returns>The updated instance of the IBicepExtensionBuilder, allowing for method chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown if a configuration type has already been registered for the extension.</exception>
    public static IBicepExtensionBuilder WithConfigurationType(this IBicepExtensionBuilder builder, Type configuartion)
    {
        ArgumentNullException.ThrowIfNull(configuartion);
        if(builder.Services.Any(s => s.ServiceType == typeof(ConfigurationTypeContainer)))
        {
            throw new InvalidOperationException("A configuration type has already been registered. Only one configuration type can be registered per extension.");
        }

        builder.Services.AddSingleton(new ConfigurationTypeContainer(configuartion));
        return builder;
    }

    /// <summary>
    /// Configures the Bicep extension builder to use the specified configuration type.
    /// </summary>
    /// <remarks>Use this method to specify a custom configuration type for the Bicep extension. The provided
    /// type must be compatible with the extension's expected configuration requirements.</remarks>
    /// <typeparam name="TConfig">The type of configuration to associate with the Bicep extension builder.</typeparam>
    /// <param name="builder">The Bicep extension builder instance to configure.</param>
    /// <returns>The same Bicep extension builder instance configured to use the specified configuration type.</returns>
    public static IBicepExtensionBuilder WithConfigurationType<TConfig>(this IBicepExtensionBuilder builder)
        => builder.WithConfigurationType(typeof(TConfig));

    /// <summary>
    /// Registers a fallback type with the Bicep extension builder to be used when no explicit type is specified for a
    /// resource or operation.
    /// </summary>
    /// <remarks>Only one fallback type can be registered per extension. Attempting to register multiple
    /// fallback types will result in an exception.</remarks>
    /// <param name="builder">The Bicep extension builder to configure with the fallback type.</param>
    /// <param name="fallbackType">The type to use as the fallback when no other type is specified. This parameter must not be null.</param>
    /// <returns>The same instance of the Bicep extension builder, enabling method chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown if a fallback type has already been registered for the extension.</exception>
    public static IBicepExtensionBuilder WithFallbackType(this IBicepExtensionBuilder builder, Type fallbackType)
    {
        ArgumentNullException.ThrowIfNull(fallbackType);
        if(builder.Services.Any(s => s.ServiceType == typeof(FallbackTypeContainer)))
        {
            throw new InvalidOperationException("A fallback type has already been registered. Only one fallback type can be registered per extension.");
        }

        builder.Services.AddSingleton(new FallbackTypeContainer(fallbackType));
        return builder;
    }

    /// <summary>
    /// Registers a fallback type with the Bicep extension builder to be used when no explicit type is specified for a
    /// resource or operation.
    /// </summary>
    /// <remarks>Only one fallback type can be registered per extension. Attempting to register multiple
    /// fallback types will result in an exception.</remarks>
    /// <typeparam name="TFallback">The type to use as a fallback if the primary type is not applicable or available.</typeparam>
    /// <param name="builder">The Bicep extension builder instance to configure with the fallback type.</param>
    /// <returns>The updated Bicep extension builder instance with the fallback type applied.</returns>
    public static IBicepExtensionBuilder WithFallbackType<TFallback>(this IBicepExtensionBuilder builder)
        => builder.WithFallbackType(typeof(TFallback));

}
