// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Types;
using Bicep.Testing.Fakes.TypeSystem;

namespace Bicep.Testing;

public static class TestCompilerExtensions
{
    public static TestCompiler WithConfiguration(this TestCompiler compiler, IBicepConfiguration configuration) => compiler.ConfigureServices(services =>
        services.ReplaceSingleton<IBicepConfigurationManager>(configuration.WithStaticConfiguration()));

    public static TestCompiler WithAzResourceTypeLoader(this TestCompiler compiler, IResourceTypeLoader resourceTypeLoader) => compiler.ConfigureServices(services =>
        services.ReplaceSingleton<IResourceTypeProviderFactory>(FakeResourceTypeProviderFactory.ForAzureResourceTypeLoader(resourceTypeLoader)));

    public static TestCompiler WithAzResources(this TestCompiler compiler, IEnumerable<ResourceTypeComponents> resourceTypes) => compiler.ConfigureServices(services =>
        services.ReplaceSingleton<IResourceTypeProviderFactory>(FakeResourceTypeProviderFactory.ForAzureResourceTypes(resourceTypes)));

    public static TestCompiler WithEmptyAzResources(this TestCompiler compiler) => compiler.WithAzResources([]);
}
