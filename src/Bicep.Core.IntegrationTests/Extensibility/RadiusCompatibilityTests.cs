// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests.Extensibility;

[TestClass]
public class RadiusCompatibilityTests
{
    private static ServiceBuilder GetServiceBuilder(IFileSystem fileSystem,
        string registryHost,
        string repositoryPath,
        bool extensibilityEnabledBool,
        bool providerRegistryBool,
        bool dynamicTypeLoadingEnabled)
    {
        var clientFactory = RegistryHelper.CreateMockRegistryClient(registryHost, repositoryPath);

        return new ServiceBuilder()
            .WithFeatureOverrides(new(
                ExtensibilityEnabled: extensibilityEnabledBool,
                ProviderRegistry: providerRegistryBool,
                DynamicTypeLoadingEnabled: dynamicTypeLoadingEnabled))
            .WithFileSystem(fileSystem)
            .WithContainerRegistryClientFactory(clientFactory);
    }

    [TestMethod]
    public async Task Radius_identifier_passing_works_as_defined()
    {
        // repro for https://github.com/Azure/bicep/issues/13465
        var registry = "example.azurecr.io";
        var repository = $"test/radius";

        var services = GetServiceBuilder(new MockFileSystem(), registry, repository, true, true, true);

        var tgzData = ThirdPartyTypeHelper.GetMockRadiusTypesTgz();
        await RegistryHelper.PublishProviderToRegistryAsync(services.Build(), $"br:{registry}/{repository}:1.0.0", tgzData);

        var result = await CompilationHelper.RestoreAndCompile(services, """
provider 'br:example.azurecr.io/test/radius@1.0.0'

@description('The base name of the test, used to qualify resources and namespaces')
param basename string

@description('The recipe name used to register the recipe')
param environmentRecipeName string = 'default'

resource env 'Applications.Core/environments@2023-10-01-preview' existing = {
  name: basename
}

resource app 'Applications.Core/applications@2023-10-01-preview' existing = {
  name: basename
}

resource extender 'Applications.Core/extenders@2023-10-01-preview' = {
  name: basename
  properties: {
    application: app.id // <-- error thrown here
    environment: env.id // <-- error thrown here
    recipe: {
      name: environmentRecipeName
    }
  }
}
""");

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
    }
}
