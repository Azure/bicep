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
public class RadiusCompatibilityTests
{
    private static ServiceBuilder GetServiceBuilder(IFileSystem fileSystem, string registryHost, string repositoryPath)
    {
        var clientFactory = RegistryHelper.CreateMockRegistryClient(new RepoDescriptor(registryHost, repositoryPath, ["tag"]));

        return new ServiceBuilder()
            .WithFileSystem(fileSystem)
            .WithContainerRegistryClientFactory(clientFactory);
    }

    private static async Task<ServiceBuilder> GetServicesWithPrepublishedTypes()
    {
        var registry = "example.azurecr.io";
        var repository = $"test/radius";

        var services = GetServiceBuilder(new MockFileSystem(), registry, repository);

        var tgzData = ExtensionResourceTypeHelper.GetMockRadiusTypesTgz();
        await RegistryHelper.PublishExtensionToRegistryAsync(services.Build(), $"br:{registry}/{repository}:1.0.0", tgzData);

        return services;
    }

    [TestMethod]
    public async Task Radius_identifier_passing_works_as_defined()
    {
        // repro for https://github.com/Azure/bicep/issues/13465
        var services = await GetServicesWithPrepublishedTypes();

        var result = await CompilationHelper.RestoreAndCompile(services, """
extension 'br:example.azurecr.io/test/radius:1.0.0'

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

    [TestMethod]
    public async Task Radius_use_of_existing_works()
    {
        // repro for https://github.com/Azure/bicep/issues/13423#issuecomment-2030512429
        var services = await GetServicesWithPrepublishedTypes();

        var result = await CompilationHelper.RestoreAndCompile(services, """
extension 'br:example.azurecr.io/test/radius:1.0.0'

param bucketName string

resource bucket 'AWS.S3/Bucket@default' existing =  {
  alias: bucketName
  properties: {
    BucketName: bucketName
  }
}

output var string = bucket.properties.BucketName
""");

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
    }
}
