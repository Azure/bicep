// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Services.Description;
using Bicep.Core.Registry;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IOFileSystem = System.IO.Abstractions.FileSystem;

namespace Bicep.Core.IntegrationTests;

[TestClass]
public class RegistryProviderTests : TestBase
{
    private static ServiceBuilder GetServiceBuilder(IContainerRegistryClientFactory clientFactory)
        => new ServiceBuilder()
            .WithFeatureOverrides(new(ExtensibilityEnabled: true))
            .WithContainerRegistryClientFactory(clientFactory);

    [TestMethod]
    [Ignore("In true TDD fashion, the functionality to fix this test is yet to be implemented")]
    public async Task Providers_published_to_a_registry_can_be_compiled()
    {
        var registry = "example.azurecr.io";
        var registryUri = new Uri($"https://{registry}");
        var repository = $"test/provider/http";

        // types taken from https://github.com/Azure/bicep-registry-providers/tree/21aadf24cd6e8c9c5da2db0d1438df9def548b09/providers/http
        var outputDirectory = FileHelper.SaveEmbeddedResourcesWithPathPrefix(
            TestContext,
            typeof(RegistryProviderTests).Assembly,
            "Files/RegistryProviderTests/HttpProvider");

        var indexJson = Path.Combine(outputDirectory, "types/index.json");

        var (clientFactory, _) = DataSetsExtensions.CreateMockRegistryClients(false, (registryUri, repository));
        await DataSetsExtensions.PublishProviderToRegistryAsync(new IOFileSystem(), clientFactory, indexJson, $"br:{registry}/{repository}:1.2.3");

        var result = CompilationHelper.Compile(GetServiceBuilder(clientFactory), """
provider 'br:example.azurecr.io/test/provider/http@1.2.3'

resource dadJoke 'request@v1' = {
  uri: 'https://icanhazdadjoke.com'
  method: 'GET'
  format: 'json'
}

output joke string = dadJoke.body.joke
""");

        // TODO fix the below and assert that the template contents are correctly formatted
        result.Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();
    }
}