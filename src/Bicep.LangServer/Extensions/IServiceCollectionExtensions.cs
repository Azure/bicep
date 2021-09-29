// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Extensions.DependencyInjection;

namespace Bicep.LanguageServer.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static void AddSingletonOrInstance<TService, TImplementation>(this IServiceCollection services, TService? nullableImplementation)
            where TService : class where TImplementation : class, TService
        {
            if (nullableImplementation is not null)
            {
                services.AddSingleton(nullableImplementation);
            }
            else
            {
                services.AddSingleton<TService, TImplementation>();
            }
        }
    }
}
