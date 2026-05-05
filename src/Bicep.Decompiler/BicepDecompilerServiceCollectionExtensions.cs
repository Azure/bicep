// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Decompiler;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class BicepDecompilerServiceCollectionExtensions
{
    public static IServiceCollection AddBicepDecompiler(this IServiceCollection services)
    {
        services.AddBicepCore();
        services.TryAddSingleton<BicepDecompiler>();

        return services;
    }
}
