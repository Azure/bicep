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
    public async Task Missing_required_provider_configuration_blocks_compilation()
    {
        var registry = "example.azurecr.io";
        var repository = $"test/provider/foo";

        var services = GetServiceBuilder(new MockFileSystem(), registry, repository, true, true, true);

        var tgzData = ThirdPartyTypeHelper.GetTestTypesTgzWithFallbackAndConfiguration();
        await RegistryHelper.PublishProviderToRegistryAsync(services.Build(), $"br:{registry}/{repository}:1.2.3", tgzData);

        var result = await CompilationHelper.RestoreAndCompile(services, """
        provider 'br:example.azurecr.io/test/provider/foo@1.2.3'

        resource dadJoke 'fooType@v1' = {
        identifier: 'foo'
        joke: 'dad joke'
        }

        output joke string = dadJoke.joke
        """);

        result.Should().NotGenerateATemplate();
        result.Should().HaveDiagnostics(new[]{
            ("BCP206", DiagnosticLevel.Error, "Provider namespace \"ThirdPartyProvider\" requires configuration, but none was provided.")
        });
    }

    [TestMethod]
    public async Task Correct_provider_configuration_result_in_successful_compilation()
    {
        var registry = "example.azurecr.io";
        var repository = $"test/provider/foo";

        var services = GetServiceBuilder(new MockFileSystem(), registry, repository, true, true, true);

        // tgzData provideds configType with the properties namespace, config, and context
        var tgzData = ThirdPartyTypeHelper.GetTestTypesTgzWithFallbackAndConfiguration();
        await RegistryHelper.PublishProviderToRegistryAsync(services.Build(), $"br:{registry}/{repository}:1.2.3", tgzData);

        var result = await CompilationHelper.RestoreAndCompile(services, """
        provider 'br:example.azurecr.io/test/provider/foo@1.2.3' with {
            namespace: 'ThirdPartyNamespace'
            config: 'Some path to config file'
            context: 'Some ThirdParty context'
        }

        resource dadJoke 'fooType@v1' = {
        identifier: 'foo'
        joke: 'dad joke'
        }

        output joke string = dadJoke.joke
        """);

        result.Template.Should().NotBeNull();

        result.Template.Should().HaveValueAtPath("$.imports['foo']['provider']", "ThirdPartyProvider");
        result.Template.Should().HaveValueAtPath("$.imports['foo']['version']", "1.0.0");

        result.Template.Should().HaveValueAtPath("$.imports['foo']['config']['namespace']", "ThirdPartyNamespace");
        result.Template.Should().HaveValueAtPath("$.imports['foo']['config']['config']", "Some path to config file");
        result.Template.Should().HaveValueAtPath("$.imports['foo']['config']['context']", "Some ThirdParty context");

        result.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public async Task Missing_configuration_property_throws_errors()
    {
        var registry = "example.azurecr.io";
        var repository = $"test/provider/foo";

        var services = GetServiceBuilder(new MockFileSystem(), registry, repository, true, true, true);

        var tgzData = ThirdPartyTypeHelper.GetTestTypesTgzWithFallbackAndConfiguration();
        await RegistryHelper.PublishProviderToRegistryAsync(services.Build(), $"br:{registry}/{repository}:1.2.3", tgzData);

        // Missing the required configuration property: namespace
        var result = await CompilationHelper.RestoreAndCompile(services, """
        provider 'br:example.azurecr.io/test/provider/foo@1.2.3' with {
            config: 'Some path to config file'
            context: 'Some ThirdParty context'
        }

        resource dadJoke 'fooType@v1' = {
        identifier: 'foo'
        joke: 'dad joke'
        }

        output joke string = dadJoke.joke
        """);

        result.Should().NotGenerateATemplate();
        result.Should().HaveDiagnostics(new[]{
            ("BCP035", DiagnosticLevel.Error, "The specified \"object\" declaration is missing the following required properties: \"namespace\".")
        });
    }

    [TestMethod]
    public async Task Mispelled_required_configuration_property_throws_error()
    {
        var registry = "example.azurecr.io";
        var repository = $"test/provider/foo";

        var services = GetServiceBuilder(new MockFileSystem(), registry, repository, true, true, true);

        var tgzData = ThirdPartyTypeHelper.GetTestTypesTgzWithFallbackAndConfiguration();
        await RegistryHelper.PublishProviderToRegistryAsync(services.Build(), $"br:{registry}/{repository}:1.2.3", tgzData);

        // Mispelled the required configuration property: namespace
        var result = await CompilationHelper.RestoreAndCompile(services, """
        provider 'br:example.azurecr.io/test/provider/foo@1.2.3' with {
            namespac: 'ThirdPartyNamespace'
            config: 'Some path to config file'
            context: 'Some ThirdParty context'
        }

        resource dadJoke 'fooType@v1' = {
        identifier: 'foo'
        joke: 'dad joke'
        }

        output joke string = dadJoke.joke
        """);

        result.Should().NotGenerateATemplate();
        result.Should().HaveDiagnostics(new[]{
            ("BCP035", DiagnosticLevel.Error, "The specified \"object\" declaration is missing the following required properties: \"namespace\"."),
            ("BCP089", DiagnosticLevel.Error, "The property \"namespac\" is not allowed on objects of type \"config\". Did you mean \"namespace\"?")
        });
    }

    [TestMethod]
    public async Task Mispelled_optional_configuration_property_throws_error()
    {
        var registry = "example.azurecr.io";
        var repository = $"test/provider/foo";

        var services = GetServiceBuilder(new MockFileSystem(), registry, repository, true, true, true);

        var tgzData = ThirdPartyTypeHelper.GetTestTypesTgzWithFallbackAndConfiguration();
        await RegistryHelper.PublishProviderToRegistryAsync(services.Build(), $"br:{registry}/{repository}:1.2.3", tgzData);

        // Mispelled the optional configuration property: context
        var result = await CompilationHelper.RestoreAndCompile(services, """
        provider 'br:example.azurecr.io/test/provider/foo@1.2.3' with {
            namespace: 'ThirdPartyNamespace'
            config: 'Some path to config file'
            contex: 'Some ThirdParty context'
        }

        resource dadJoke 'fooType@v1' = {
            identifier: 'foo'
            joke: 'dad joke'
        }

        output joke string = dadJoke.joke
        """);

        result.Should().NotGenerateATemplate();
        result.Should().HaveDiagnostics(new[]{
            ("BCP089", DiagnosticLevel.Error, "The property \"contex\" is not allowed on objects of type \"config\". Did you mean \"context\"?")
        });
    }

    [TestMethod]
    public async Task Warning_generated_and_fallback_type_type_accepted()
    {
        var registry = "example.azurecr.io";
        var repository = $"test/provider/foo";

        var services = GetServiceBuilder(new MockFileSystem(), registry, repository, true, true, true);

        var tgzData = ThirdPartyTypeHelper.GetTestTypesTgzWithFallbackAndConfiguration();
        await RegistryHelper.PublishProviderToRegistryAsync(services.Build(), $"br:{registry}/{repository}:1.2.3", tgzData);

        var result = await CompilationHelper.RestoreAndCompile(services, """
        provider 'br:example.azurecr.io/test/provider/foo@1.2.3' with {
            namespace: 'ThirdPartyNamespace'
            config: 'Some path to config file'
        }

        resource dadJoke 'test@v1' = {
            bodyProp: 'fallback body'
        }

        output joke string = dadJoke.bodyProp
        """);

        result.Should().GenerateATemplate();

        result.Template.Should().HaveValueAtPath("$.resources['dadJoke']['properties']['bodyProp']", "fallback body");

        result.Should().HaveDiagnostics(new[]{
            ("BCP081", DiagnosticLevel.Warning, "Resource type \"test@v1\" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed.")
        });
    }

    [TestMethod]
    public async Task Fallback_not_provided_in_json()
    {
        var registry = "example.azurecr.io";
        var repository = $"test/provider/foo";

        var services = GetServiceBuilder(new MockFileSystem(), registry, repository, true, true, true);

        // tgzData does have fallback type
        var tgzData = ThirdPartyTypeHelper.GetTestTypesTgz();
        await RegistryHelper.PublishProviderToRegistryAsync(services.Build(), $"br:{registry}/{repository}:1.2.3", tgzData);

        var result = await CompilationHelper.RestoreAndCompile(services, """
        provider 'br:example.azurecr.io/test/provider/foo@1.2.3'

        resource dadJoke 'test@v1' = {
            bodyProp: 'fallback body'
        }

        output joke string = dadJoke.bodyProp
        """);

        result.Should().NotGenerateATemplate();
        result.Should().HaveDiagnostics(new[]{
            ("BCP029", DiagnosticLevel.Error, "The resource type is not valid. Specify a valid resource type of format \"<types>@<apiVersion>\"."),
            ("BCP062", DiagnosticLevel.Error, "The referenced declaration with name \"dadJoke\" is not valid.")

        });
    }
}
