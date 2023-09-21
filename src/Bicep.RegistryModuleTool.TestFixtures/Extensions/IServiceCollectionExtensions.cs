// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using Bicep.Core.Features;
using Bicep.Core.UnitTests.Features;
using Bicep.RegistryModuleTool.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bicep.RegistryModuleTool.TestFixtures.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddBicepCompilerWithFileSystem(this IServiceCollection serviceCollection, IFileSystem fileSystem) => serviceCollection
            .AddBicepCompiler()
            .AddSingleton(new FeatureProviderOverrides())
            .AddSingleton<FeatureProviderFactory>()
            .Replace(ServiceDescriptor.Singleton<IFeatureProviderFactory, OverriddenFeatureProviderFactory>())
            .Replace(ServiceDescriptor.Singleton(fileSystem));
    }
}
