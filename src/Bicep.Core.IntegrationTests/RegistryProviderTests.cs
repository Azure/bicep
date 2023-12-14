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
    private static ServiceBuilder GetServiceBuilder(IFileSystem fileSystem, IContainerRegistryClientFactory clientFactory)
        => new ServiceBuilder()
            .WithFeatureOverrides(new(ExtensibilityEnabled: true, ProviderRegistry: true))
            .WithFileSystem(fileSystem)
            .WithContainerRegistryClientFactory(clientFactory);

    [TestMethod]
    public async Task Providers_published_to_a_registry_can_be_compiled()
    {
        System.IO.Abstractions.FileSystem fileSystem = new();
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
        await DataSetsExtensions.PublishProviderToRegistryAsync(fileSystem, clientFactory, indexJson, $"br:{registry}/{repository}:1.2.3");

        var bicepPath = FileHelper.SaveResultFile(TestContext, "main.bicep", """
provider 'br:example.azurecr.io/test/provider/http@1.2.3'

resource dadJoke 'request@v1' = {
  uri: 'https://icanhazdadjoke.com'
  method: 'GET'
  format: 'json'
}

output joke string = dadJoke.body.joke
""");

        var bicepUri = PathHelper.FilePathToFileUrl(bicepPath);

        var compiler = GetServiceBuilder(fileSystem, clientFactory).Build().Construct<BicepCompiler>();
        var result = CompilationHelper.Compile(await compiler.CreateCompilation(bicepUri));

        // TODO uncomment the below once the 3rd party logic has been implemented
        // result.Should().NotHaveAnyDiagnostics();
        // result.Template.Should().NotBeNull();

        // TODO remove this once the above is uncommented
        result.Should().ContainDiagnostic("BCP204", DiagnosticLevel.Error, "Provider namespace \"http\" is not recognized");
        result.Should().NotGenerateATemplate();
    }
}