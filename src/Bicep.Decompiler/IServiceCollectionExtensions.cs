// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bicep.Decompiler;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddBicepDecompiler(this IServiceCollection services)
    {
        services.TryAddSingleton<IBicepDecompiler, BicepDecompiler>();

        return services;
    }
}
