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
using Bicep.Local.Extension.Rpc;
using Bicep.Local.Extension.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Bicep.Local.Extension.Host.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddBicepExtensionServices(this IServiceCollection services
                                                            , string name, string version, bool isSingleton
                                                            , Action<Dictionary<string, ObjectTypeProperty>>? typeConfiguration = null)
    {
        var typeDictionary = new Dictionary<Type, Func<TypeBase>>
                            {
                                { typeof(string), () => new StringType() },
                                { typeof(bool), () => new BooleanType() },
                                { typeof(int), () => new IntegerType() }
                            }.ToImmutableDictionary();

        var typeFactory = new TypeFactory([]);
        var configuration = new Dictionary<string, ObjectTypeProperty>();

        if (typeConfiguration is not null)
        {
            typeConfiguration(configuration);
        }

        var configurationType = typeFactory.Create(() => new ObjectType("configuration", configuration, null));

        foreach (var type in typeDictionary)
        {
            typeFactory.Create(type.Value);
        }

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
