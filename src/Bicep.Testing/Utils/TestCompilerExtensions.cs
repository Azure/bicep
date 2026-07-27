// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Testing.Utils;

public static class TestCompilerExtensions
{
    public static TestCompiler WithConfiguration(this TestCompiler compiler, RootConfiguration configuration) => compiler.ConfigureServices(services =>
        services
            .RemoveAll<IConfigurationManager>()
            .AddSingleton<IConfigurationManager>(IConfigurationManager.WithStaticConfiguration(configuration)));

    public static TestCompiler WithAzResources(this TestCompiler compiler, IEnumerable<ResourceTypeComponents> resourceTypes) => compiler.ConfigureServices(services =>
        services
            .RemoveAll<IResourceTypeProviderFactory>()
            .AddAzureResourceTypes(resourceTypes));

    public static TestCompiler WithEmptyAzResources(this TestCompiler compiler) => compiler.WithAzResources([]);
}
