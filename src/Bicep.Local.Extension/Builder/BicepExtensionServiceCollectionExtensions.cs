// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

namespace Microsoft.Extensions.DependencyInjection;

public static class BicepExtensionServiceCollectionExtensions
{
    /// <summary>
    /// Configures the dependency injection container with core Bicep extension services and type definitions.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="name">The unique name of your Bicep extension (e.g., "MyCompany.MyExtension").</param>
    /// <param name="version">The semantic version of your extension (e.g., "1.0.0").</param>
    /// <param name="isSingleton">True if this extension should be treated as a singleton; false for multiple instances.</param>
    /// <param name="configurationType">Configuration type for your extension.</param>
    /// <returns>The configured service collection for method chaining.</returns>
    /// <remarks>
    /// This method sets up the foundation for your Bicep extension by registering core services including
    /// type factories, resource dispatchers, gRPC services, and basic type mappings (string, bool, int).
    /// Call this method once during startup before registering resource handlers.
    /// </remarks>
    /// <example>
    /// <code>
    /// services.AddBicepExtension(
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
    public static IBicepExtensionBuilder AddBicepExtension(
        this IServiceCollection services,
        string name,
        string version,
        bool isSingleton,
        Assembly typeAssembly,
        Type? configurationType = null)
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

        services.AddSingleton<ITypeDefinitionBuilder>(new TypeDefinitionBuilder(
            name,
            version,
            isSingleton,
            configurationType,
            typeFactory,
            new TypeProvider([typeAssembly]),
            typeDictionary));

        services.AddSingleton<IResourceHandlerDispatcher, ResourceHandlerDispatcher>();

        services.AddGrpc(options =>
        {
            options.EnableDetailedErrors = true;
        });
        services.AddGrpcReflection();

        return new DefaultBicepExtensionBuilder(services);
    }
}
