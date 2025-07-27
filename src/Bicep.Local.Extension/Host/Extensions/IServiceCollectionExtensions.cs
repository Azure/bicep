// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Azure.Bicep.Types;
using Azure.Bicep.Types.Concrete;
using Azure.Bicep.Types.Index;
using Bicep.Local.Extension.Host.Handlers;
using Bicep.Local.Extension.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Bicep.Local.Extension.Host.Extensions;

public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Configures the dependency injection container with core Bicep extension services and type definitions.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="name">The unique name of your Bicep extension (e.g., "MyCompany.MyExtension").</param>
    /// <param name="version">The semantic version of your extension (e.g., "1.0.0").</param>
    /// <param name="isSingleton">True if this extension should be treated as a singleton; false for multiple instances.</param>
    /// <param name="typeConfiguration">Optional callback to configure custom Bicep types and configuration schema for your extension.</param>
    /// <returns>The configured service collection for method chaining.</returns>
    /// <remarks>
    /// This method sets up the foundation for your Bicep extension by registering core services including
    /// type factories, resource dispatchers, gRPC services, and basic type mappings (string, bool, int).
    /// Call this method once during startup before registering resource handlers.
    /// </remarks>
    /// <example>
    /// <code>
    /// services.AddBicepExtensionServices(
    ///     name: "MyCompany.KubernetesExtension",
    ///     version: "1.0.0",
    ///     isSingleton: true,
    ///     typeConfiguration: (typeFactory, config) => {
    ///         var stringType = typeFactory.Create(() => new StringType());
    ///         config["apiUrl"] = new ObjectTypeProperty(
    ///             typeFactory.GetReference(stringType), 
    ///             ObjectTypePropertyFlags.Required, 
    ///             "The Kubernetes API server URL");
    ///     });
    /// </code>
    /// </example>
    public static IServiceCollection AddBicepExtensionServices(this IServiceCollection services
                                                            , string name, string version, bool isSingleton
                                                            , Action<TypeFactory, Dictionary<string, ObjectTypeProperty>>? typeConfiguration = null)
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

        var configuration = new Dictionary<string, ObjectTypeProperty>();

        if (typeConfiguration is not null)
        {
            typeConfiguration(typeFactory, configuration);
        }

        var configurationType = typeFactory.Create(() => new ObjectType("configuration", configuration, null));
        var typeSettings = new TypeSettings
            (
                name,
                version,
                isSingleton,
                new CrossFileTypeReference("types.json", typeFactory.GetIndex(configurationType))
            );

        services.AddSingleton(typeSettings)
            .AddSingleton(typeFactory)
            .AddSingleton<IResourceHandlerDispatcher, ResourceHandlerDispatcher>()
            .AddSingleton<ITypeDefinitionBuilder, TypeDefinitionBuilder>()
            .AddSingleton(sp => typeDictionary);

        services.AddGrpc();
        services.AddGrpcReflection();
        return services;
    }

    /// <summary>
    /// Registers a resource handler that manages a specific resource type in your Bicep extension.
    /// </summary>
    /// <typeparam name="T">The resource handler class that implements <see cref="IResourceHandler"/>.</typeparam>
    /// <param name="services">The service collection to register the handler with.</param>
    /// <returns>The service collection for method chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a handler is already registered for the same resource type.</exception>
    /// <remarks>
    /// Resource handlers define how your extension creates, updates, deletes, and retrieves resources.
    /// Each resource type can only have one registered handler. For generic resource handling, 
    /// implement <see cref="IResourceHandler"/> without a specific type parameter.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Register a handler for Kubernetes Pod resources
    /// services.AddBicepResourceHandler&lt;KubernetesPodHandler&gt;();
    /// 
    /// // Register a generic handler for all untyped resources
    /// services.AddBicepResourceHandler&lt;GenericResourceHandler&gt;();
    /// </code>
    /// </example>
    public static IServiceCollection AddBicepResourceHandler<T>(this IServiceCollection services)
        where T : class, IResourceHandler
    {
        var resourceHandler = typeof(T);

        // check that only one handler is registered for the given resource type
        if (resourceHandler.TryGetTypedResourceHandlerInterface(out var baseInterface))
        {
            var resourceType = baseInterface.GetGenericArguments()[0];

            var existingHandler = services
                .Where(st => st.ServiceType.IsAssignableFrom(typeof(IResourceHandler<>)))
                .Select(t =>
                {
                    var implementationType = t.IsKeyedService ? t.KeyedImplementationType : t.ImplementationType;

                    if (implementationType?.TryGetTypedResourceHandlerInterface(out Type? typedInterface) == true)
                    {
                        var genericType = typedInterface.GetGenericArguments()[0];
                        return genericType;
                    }

                    return null;
                })
                .OfType<Type>()
                .FirstOrDefault(et => et == resourceType);

            if (existingHandler is not null)
            {
                throw new InvalidOperationException($"A handler [`{existingHandler.FullName}`] is already registered for type [`{resourceType.FullName}`]");
            }

        }

        services.AddSingleton<IResourceHandler, T>();

        return services;
    }

}
