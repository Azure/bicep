// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using System.Web.Services.Description;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileSystem = System.IO.Abstractions.FileSystem;

namespace Bicep.Core.IntegrationTests;

[TestClass]
public class RegistryProviderTests : TestBase
{
    private static ServiceBuilder GetServiceBuilder(IFileSystem fileSystem, string registryFqdn, string repositoryPath)
    {
        var (clientFactory, _) = DataSetsExtensions.CreateMockRegistryClients((registryFqdn, repositoryPath));

        return new ServiceBuilder()
            .WithFeatureOverrides(new(ExtensibilityEnabled: true, ProviderRegistry: true))
            .WithFileSystem(fileSystem)
            .WithContainerRegistryClientFactory(clientFactory);
    }

    [TestMethod]
    public async Task Providers_published_to_a_registry_can_be_compiled()
    {
        // types taken from https://github.com/Azure/bicep-registry-providers/tree/21aadf24cd6e8c9c5da2db0d1438df9def548b09/providers/http
        var fileSystem = FileHelper.CreateMockFileSystemForEmbeddedFiles(
            typeof(RegistryProviderTests).Assembly,
            "Files/RegistryProviderTests/HttpProvider");

        var registry = "example.azurecr.io";
        var repository = $"test/provider/http";

        var services = GetServiceBuilder(fileSystem, registry, repository);

        await DataSetsExtensions.PublishProviderToRegistryAsync(services.Build(), "/types/index.json", $"br:{registry}/{repository}:1.2.3");

        var result = await CompilationHelper.RestoreAndCompile(services, """
provider 'br:example.azurecr.io/test/provider/http@1.2.3'

resource dadJoke 'request@v1' = {
  uri: 'https://icanhazdadjoke.com'
  method: 'GET'
  format: 'json'
}

output joke string = dadJoke.body.joke
""");

        // TODO uncomment the below once the 3rd party logic has been implemented
        // result.Should().NotHaveAnyDiagnostics();
        // result.Template.Should().NotBeNull();

        // TODO remove this once the above is uncommented
        result.Should().ContainDiagnostic("BCP204", DiagnosticLevel.Error, "Provider namespace \"http\" is not recognized.");
        result.Should().NotGenerateATemplate();
    }
}
