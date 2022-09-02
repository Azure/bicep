// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Extensions.DependencyInjection;

namespace Bicep.RegistryModuleTool.Extensions
{
    public static class ExtendedIServiceCollection
    {
        public static IServiceCollection AddSingletonNullable<TService, TImplementation>(this IServiceCollection services, TService? instance)
            where TService : class
            where TImplementation : class, TService
        {
            if (instance is not null)
            {
                return services.AddSingleton(instance);
            }

            return services.AddSingleton<TService, TImplementation>();
        }
    }
}
