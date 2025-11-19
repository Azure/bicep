// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Bicep.Core.UnitTests.Utils.RegistryHelper;

namespace Bicep.Core.IntegrationTests.Extensibility;

[TestClass]
public class DesiredStateConfigurationCompatibilityTests
{
    private static ServiceBuilder GetServiceBuilder(IFileSystem fileSystem, string registryHost, string repositoryPath)
    {
        var clientFactory = CreateMockRegistryClient(new RepoDescriptor(registryHost, repositoryPath, ["tag"]));

        return new ServiceBuilder()
            .WithFileSystem(fileSystem)
            .WithContainerRegistryClientFactory(clientFactory);
    }

    private static async Task<ServiceBuilder> GetServicesWithPrepublishedTypes()
    {
        var registry = "example.azurecr.io";
        var repository = $"test/dsc";

        var services = GetServiceBuilder(new MockFileSystem(), registry, repository);

        var tgzData = ExtensionResourceTypeHelper.GetMockDesiredStateConfigurationTypesTgz();
        await PublishExtensionToRegistryAsync(services.Build(), $"br:{registry}/{repository}:0.1.0", tgzData);

        return services;
    }

    [TestMethod]
    public async Task DesiredStateConfiguration_echo_resource_works()
    {
        var services = await GetServicesWithPrepublishedTypes();

        var result = await CompilationHelper.RestoreAndCompile(services, """
extension 'br:example.azurecr.io/test/dsc:0.1.0'

resource myEcho 'Microsoft.DSC.Debug/Echo@1.0.0' = {
    output: 'Hello world!'
    showSecrets: false
}
""");

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
    }
}
