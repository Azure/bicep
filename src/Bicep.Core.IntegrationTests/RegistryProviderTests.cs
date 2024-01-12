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
using FluentAssertions.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileSystem = System.IO.Abstractions.FileSystem;

namespace Bicep.Core.IntegrationTests;

[TestClass]
public class RegistryProviderTests : TestBase
{
    private static ServiceBuilder GetServiceBuilder(IFileSystem fileSystem, string registry, string repository)
    {
        var (clientFactory, _) = DataSetsExtensions.CreateMockRegistryClients((new Uri($"https://{registry}"), repository));

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

        result.Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();
    }

    [TestMethod]
    public async Task Third_party_namespace_errors_with_configuration()
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
provider 'br:example.azurecr.io/test/provider/http@1.2.3' with {{}}

resource dadJoke 'request@v1' = {
  uri: 'https://icanhazdadjoke.com'
  method: 'GET'
  format: 'json'
}

output joke string = dadJoke.body.joke
""");

        result.Should().NotGenerateATemplate();
        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
            //Harsh - Why are there multiple errors showing? Is this okay? 
            ("BCP205", DiagnosticLevel.Error, "Provider namespace \"http\" does not support configuration."),
            ("BCP022", DiagnosticLevel.Error, "Expected a property name at this location."),
            ("BCP018", DiagnosticLevel.Error, "Expected the \":\" character at this location."),
            ("BCP012", DiagnosticLevel.Error, "Expected the \"as\" keyword at this location.")
        });
    }

    [TestMethod]
    public async Task Third_party_imports_are_disabled_unless_feature_is_enabled()
    {
        var service = new ServiceBuilder();
        var result = await CompilationHelper.RestoreAndCompile(service, @$"
provider 'br:example.azurecr.io/test/provider/http@1.2.3'
");
        result.Should().HaveDiagnostics(new[] {
                ("BCP203", DiagnosticLevel.Error, "Using provider statements requires enabling EXPERIMENTAL feature \"Extensibility\"."),
            });

        service = new ServiceBuilder().WithFeatureOverrides(new(ExtensibilityEnabled: true));
        var result2 = await CompilationHelper.RestoreAndCompile(service, @$"
provider 'br:example.azurecr.io/test/provider/http@1.2.3'
");
        result2.Should().HaveDiagnostics(new[] {
                ("BCP204", DiagnosticLevel.Error, "Provider namespace \"http\" is not recognized."),
            });

        service = new ServiceBuilder().WithFeatureOverrides(new(ExtensibilityEnabled: true, ProviderRegistry:true));
        var result3 = await CompilationHelper.RestoreAndCompile(service, @$"
provider 'br:example.azurecr.io/test/provider/http@1.2.3'
");
        result3.Should().NotHaveAnyDiagnostics();
        result3.Template.Should().NotBeNull();
    }
}
