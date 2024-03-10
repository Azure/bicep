// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests;

[TestClass]
public class RegistryProviderTests : TestBase
{
    private static ServiceBuilder GetServiceBuilder(
        IFileSystem fileSystem,
        string registryHost,
        string repositoryPath,
        bool extensibilityEnabledBool,
        bool providerRegistryBool,
        bool dynamicTypeLoadingEnabledBool)
    {
        var clientFactory = RegistryHelper.CreateMockRegistryClient(registryHost, repositoryPath);

        return new ServiceBuilder()
            .WithFeatureOverrides(new(ExtensibilityEnabled: extensibilityEnabledBool, DynamicTypeLoadingEnabled: dynamicTypeLoadingEnabledBool, ProviderRegistry: providerRegistryBool))
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

        var services = GetServiceBuilder(fileSystem, registry, repository, true, true, true);

        await RegistryHelper.PublishProviderToRegistryAsync(services.Build(), "/types/index.json", $"br:{registry}/{repository}:1.2.3");

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
    public async Task Existing_resources_are_permitted_through_3p_type_registry()
    {
        var registry = "example.azurecr.io";
        var repository = $"providers/foo";

        var services = GetServiceBuilder(new MockFileSystem(), registry, repository, true, true, true);

        var tgzData = ThirdPartyTypeHelper.GetTestTypesTgz();
        await RegistryHelper.PublishProviderToRegistryAsync(services.Build(), $"br:{registry}/{repository}:1.2.3", tgzData);

        var result = await CompilationHelper.RestoreAndCompile(services, """
provider 'br:example.azurecr.io/providers/foo@1.2.3'

resource fooRes 'fooType@v1' existing = {
}
""");

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP035", DiagnosticLevel.Warning, """The specified "resource" declaration is missing the following required properties: "identifier". If this is an inaccuracy in the documentation, please report it to the Bicep Team."""),
        });

        result = await CompilationHelper.RestoreAndCompile(services, """
provider 'br:example.azurecr.io/providers/foo@1.2.3'

resource fooRes 'fooType@v1' existing = {
  identifier: 'foo'
}
""");

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
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

        var services = GetServiceBuilder(fileSystem, registry, repository, true, true, true);

        await RegistryHelper.PublishProviderToRegistryAsync(services.Build(), "/types/index.json", $"br:{registry}/{repository}:1.2.3");

        var result = await CompilationHelper.RestoreAndCompile(services, """
provider 'br:example.azurecr.io/test/provider/http@1.2.3' with {}

resource dadJoke 'request@v1' = {
  uri: 'https://icanhazdadjoke.com'
  method: 'GET'
  format: 'json'
}

output joke string = dadJoke.body.joke
""");

        result.Should().NotGenerateATemplate();
        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
            ("BCP205", DiagnosticLevel.Error, "Provider namespace \"http\" does not support configuration.")
        });
    }

    [TestMethod]
    public async Task Resource_function_types_are_permitted_through_3p_type_registry()
    {
        var registry = "example.azurecr.io";
        var repository = $"providers/foo";

        var services = GetServiceBuilder(new MockFileSystem(), registry, repository, true, true, true);

        var tgzData = ThirdPartyTypeHelper.GetTestTypesTgz();
        await RegistryHelper.PublishProviderToRegistryAsync(services.Build(), $"br:{registry}/{repository}:1.2.3", tgzData);

        var result = await CompilationHelper.RestoreAndCompile(services, """
provider 'br:example.azurecr.io/providers/foo@1.2.3'

resource fooRes 'fooType@v1' existing = {
  identifier: 'foo'
}

output baz string = fooRes.convertBarToBaz('bar')
""");

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveValueAtPath("$.outputs['baz'].value", "[invokeResourceMethod('fooRes', 'convertBarToBaz', createArray('bar'))]");
    }

    [TestMethod]
    [Ignore("Not yet supported")]
    public async Task Implicit_providers_are_permitted_through_3p_type_registry()
    {
        var registry = "example.azurecr.io";
        var repository = $"providers/foo";

        var fileSystem = new MockFileSystem();
        var services = GetServiceBuilder(fileSystem, registry, repository, true, true, false);

        var tgzData = ThirdPartyTypeHelper.GetTestTypesTgz();
        await RegistryHelper.PublishProviderToRegistryAsync(services.Build(), $"br:{registry}/{repository}:1.2.3", tgzData);

        fileSystem.File.WriteAllText("/bicepconfig.json", """
{
  "providers": {
    "foo": {
      "source": "example.azurecr.io/providers/foo",
      "version": "1.2.3"
    }
  },
  "implicitProviders": ["foo"],
  "experimentalFeaturesEnabled": {
    "extensibility": true,
    "providerRegistry": true
  }
}
""");

        var result = await CompilationHelper.RestoreAndCompile(services, """
resource fooRes 'fooType@v1' existing = {
  identifier: 'foo'
}

output baz string = fooRes.convertBarToBaz('bar')
""");

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveValueAtPath("$.outputs['baz'].value", "[invokeResourceMethod('fooRes', 'convertBarToBaz', createArray('bar'))]");
    }

    [TestMethod]
    public async Task Third_party_imports_are_enabled_when_feature_is_enabled()
    {
        var fileSystem = FileHelper.CreateMockFileSystemForEmbeddedFiles(
            typeof(RegistryProviderTests).Assembly,
            "Files/RegistryProviderTests/HttpProvider");

        var registry = "example.azurecr.io";
        var repository = $"test/provider/http";

        var services = GetServiceBuilder(fileSystem, registry, repository, true, true, true);

        await RegistryHelper.PublishProviderToRegistryAsync(services.Build(), "/types/index.json", $"br:{registry}/{repository}:1.2.3");

        var result = await CompilationHelper.RestoreAndCompile(services, @$"
provider 'br:example.azurecr.io/test/provider/http@1.2.3'
");
        result.Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();
        result.Template.Should().DeepEqual(JToken.Parse("""
{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "languageVersion": "2.1-experimental",
  "contentVersion": "1.0.0.0",
  "metadata": {
    "_EXPERIMENTAL_WARNING": "This template uses ARM features that are experimental. Experimental features should be enabled for testing purposes only, as there are no guarantees about the quality or stability of these features. Do not enable these settings for any production usage, or your production environment may be subject to breaking.",
    "_EXPERIMENTAL_FEATURES_ENABLED": [
      "Extensibility"
    ],
    "_generator": {
      "name": "bicep",
      "version": "dev",
      "templateHash": "14577456470128607958"
    }
  },
  "imports": {
    "http": {
    "provider": "http",
    "version": "1.2.3"
    }
  },
  "resources": {}
}
"""));
    }

    [TestMethod]
    public async Task Third_party_imports_are_disabled_unless_feature_is_enabled()
    {
        var fileSystem = FileHelper.CreateMockFileSystemForEmbeddedFiles(
             typeof(RegistryProviderTests).Assembly,
             "Files/RegistryProviderTests/HttpProvider");

        var registry = "example.azurecr.io";
        var repository = $"test/provider/http";

        var services = GetServiceBuilder(fileSystem, registry, repository, false, false, false);

        await RegistryHelper.PublishProviderToRegistryAsync(services.Build(), "/types/index.json", $"br:{registry}/{repository}:1.2.3");

        var result = await CompilationHelper.RestoreAndCompile(services, @$"
        provider 'br:example.azurecr.io/test/provider/http@1.2.3'
        ");
        result.Should().HaveDiagnostics(new[] {
                ("BCP203", DiagnosticLevel.Error, "Using provider statements requires enabling EXPERIMENTAL feature \"Extensibility\"."),
            });

        services = GetServiceBuilder(fileSystem, registry, repository, true, false, false);

        var result2 = await CompilationHelper.RestoreAndCompile(services, @$"
        provider 'br:example.azurecr.io/test/provider/http@1.2.3'
        ");
        result2.Should().HaveDiagnostics(new[] {
        ("BCP204", DiagnosticLevel.Error, "Provider namespace \"http\" is not recognized."),
            });
    }

    [TestMethod]
    public async Task Using_interpolated_strings_in_provider_declaration_syntax_results_in_diagnostic()
    {
        var services = new ServiceBuilder()
            .WithFeatureOverrides(new(ExtensibilityEnabled: true, ProviderRegistry: true));

        var result = await CompilationHelper.RestoreAndCompile(services, """
        var registryHost = 'example.azurecr.io'
        provider 'br:${registryHost}/test/provider/http@1.2.3'
        """);
        result.Should().NotGenerateATemplate();
        result.Should().ContainDiagnostic("BCP303", DiagnosticLevel.Error, "String interpolation is unsupported for specifying the provider.");
    }

    [TestMethod]
    public async Task Cannot_import_az_whithout_dynamic_type_loading_enabled()
    {
        var services = new ServiceBuilder()
            .WithFeatureOverrides(new(ExtensibilityEnabled: true, DynamicTypeLoadingEnabled: false));

        var result = await CompilationHelper.RestoreAndCompile(services, @"
        provider 'br:mcr.microsoft.com/bicep/provider/az@0.0.0'
        ");
        result.Should().NotGenerateATemplate();
        result.Should().HaveDiagnostics(new[]{
            ("BCP204", DiagnosticLevel.Error, "Provider namespace \"az\" is not recognized."),
            ("BCP084",DiagnosticLevel.Error,"The symbolic name \"az\" is reserved. Please use a different symbolic name. Reserved namespaces are \"az\", \"sys\".")
        });
    }

    [TestMethod]
    //Change test name
    public async Task Contract_changes()
    {
        // types taken from https://github.com/Azure/bicep-registry-providers/tree/21aadf24cd6e8c9c5da2db0d1438df9def548b09/providers/http
        var fileSystem = FileHelper.CreateMockFileSystemForEmbeddedFiles(
            typeof(RegistryProviderTests).Assembly,
            "Files/RegistryProviderTests/HttpProvider");

        var registry = "example.azurecr.io";
        var repository = $"test/provider/http";

        var services = GetServiceBuilder(fileSystem, registry, repository, true, true, true);

        await RegistryHelper.PublishProviderToRegistryAsync(services.Build(), "/typesNewContractChanges/index.json", $"br:{registry}/{repository}:1.2.3");

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
}
