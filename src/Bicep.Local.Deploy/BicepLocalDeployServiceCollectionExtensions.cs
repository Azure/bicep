// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry;
using Bicep.Local.Deploy;
using Bicep.Local.Deploy.Azure;
using Bicep.Local.Deploy.Extensibility;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class BicepLocalDeployServiceCollectionExtensions
{
    public static IServiceCollection AddBicepLocalDeploy(this IServiceCollection services)
    {
        services.TryAddSingleton<LocalExtensionDispatcherFactory>();
        services.TryAddSingleton<IArmDeploymentProvider, ArmDeploymentProvider>();
        services.TryAddSingleton<ILocalExtensionFactory, GrpcLocalExtensionFactory>();
        services.TryAddSingleton<IBinaryExtensionTypesFetcher, BinaryExtensionTypesFetcher>();

        return services;
    }
}
