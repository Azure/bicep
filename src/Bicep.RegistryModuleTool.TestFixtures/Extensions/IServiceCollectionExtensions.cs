// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using Bicep.Core.Features;
using Bicep.RegistryModuleTool.Extensions;
using Bicep.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.RegistryModuleTool.TestFixtures.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddBicepCompilerWithFileSystem(this IServiceCollection serviceCollection, IFileSystem fileSystem) => serviceCollection
            .AddSingleton<FeatureProviderFactory>()
            .AddSingleton<IFeatureProviderFactory>(services => TestFeatureProviderFactory.WithAssemblyVersion(services.GetRequiredService<FeatureProviderFactory>(), "dev"))
            .AddSingleton(fileSystem)
            .AddBicepCore();
    }
}
