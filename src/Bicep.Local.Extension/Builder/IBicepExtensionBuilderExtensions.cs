// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Local.Extension.Host.Extensions;
using Bicep.Local.Extension.Host.Handlers;

namespace Microsoft.Extensions.DependencyInjection;

public static class IBicepExtensionBuilderExtensions
{
    private static void CheckIfDuplicateHandlerRegistered(IBicepExtensionBuilder builder, Type resourceHandler)
    {
        // check that only one handler is registered for the given resource type
        if (resourceHandler.TryGetTypedResourceHandlerInterface(out var baseInterface))
        {
            var resourceType = baseInterface.GetGenericArguments()[0];

            var existingHandler = builder.Services
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
                throw new InvalidOperationException($"A handler is already registered for type [`{resourceType.FullName}`]");
            }
        }
        else
        {
            var existingHandler = builder.Services
                .Where(st => st.ServiceType == typeof(IResourceHandler))
                .FirstOrDefault();

            if (existingHandler is not null)
            {
                throw new InvalidOperationException($"A generic handler is already registered");
            }
        }
    }

    /// <summary>
    /// Registers a resource handler that manages a specific resource type in your Bicep extension.
    /// </summary>
    public static IBicepExtensionBuilder WithResourceHandler<T>(this IBicepExtensionBuilder builder)
        where T : class, IResourceHandler
    {
        CheckIfDuplicateHandlerRegistered(builder, typeof(T));

        builder.Services.AddSingleton<IResourceHandler, T>();

        return builder;
    }
    
    /// <summary>
    /// Registers a resource handler that manages a specific resource type in your Bicep extension.
    /// </summary>
    public static IBicepExtensionBuilder WithResourceHandler(this IBicepExtensionBuilder builder, IResourceHandler resourceHandler)
    {
        CheckIfDuplicateHandlerRegistered(builder, resourceHandler.GetType());

        builder.Services.AddSingleton<IResourceHandler>(resourceHandler);

        return builder;
    }
}