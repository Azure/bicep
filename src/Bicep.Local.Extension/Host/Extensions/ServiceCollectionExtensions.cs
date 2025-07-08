// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Bicep.Types;
using Azure.Bicep.Types.Concrete;
using Azure.Bicep.Types.Index;
using Bicep.Local.Extension.Host.Handlers;
using Bicep.Local.Extension.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Local.Extension.Host.Extensions;

public static class ServiceCollectionExtensions
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
}
