// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions.TestingHelpers;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Baselines;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using IOFileSystem = System.IO.Abstractions.FileSystem;

namespace Bicep.Core.IntegrationTests;

[TestClass]
public class RegistryProviderTests : TestBase
{
    private static readonly FeatureProviderOverrides AllFeaturesEnabled = new(ExtensibilityEnabled: true, ProviderRegistry: true, DynamicTypeLoadingEnabled: true);

    [TestMethod]
    [TestCategory(BaselineHelper.BaselineTestCategory)]
    [EmbeddedFilesTestData(@"Files/RegistryProviderTests/HttpProvider/types/index.json")]
    public void Http_provider_can_be_generated(EmbeddedFile indexJson)
    {
        var baselineFolder = BaselineFolder.BuildOutputFolder(TestContext, indexJson);
        var httpTypes = ThirdPartyTypeHelper.GetHttpProviderTypes();

        using (new AssertionScope())
        {
            foreach (var (relativePath, contents) in httpTypes)
            {
                var jsonFile = baselineFolder.GetFileOrEnsureCheckedIn(PathHelper.ResolvePath(relativePath, Path.GetDirectoryName(indexJson.FileName)));

                jsonFile.WriteToOutputFolder(contents);
                jsonFile.ShouldHaveExpectedJsonValue();
            }
        }
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

        var services = ProviderTestHelper.GetServiceBuilder(fileSystem, registry, repository, AllFeaturesEnabled);

        await RegistryHelper.PublishProviderToRegistryAsync(services.Build(), "/types/index.json", $"br:{registry}/{repository}:1.2.3");

        var result = await CompilationHelper.RestoreAndCompile(services, """
provider 'br:example.azurecr.io/test/provider/http:1.2.3'

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
    public async Task Providers_published_to_filesystem_can_be_compiled()
    {
        var cacheDirectory = FileHelper.GetCacheRootPath(TestContext);
        Directory.CreateDirectory(cacheDirectory);
        var services = new ServiceBuilder().WithFeatureOverrides(new(CacheRootDirectory: cacheDirectory, ExtensibilityEnabled: true, ProviderRegistry: true));

        var typesTgz = ThirdPartyTypeHelper.GetTestTypesTgz();
        var tempDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
        Directory.CreateDirectory(tempDirectory);

        var providerPath = Path.Combine(tempDirectory, "provider.tgz");
        await RegistryHelper.PublishProviderToRegistryAsync(services.Build(), Path.Combine(tempDirectory, providerPath), typesTgz);

        var bicepPath = Path.Combine(tempDirectory, "main.bicep");
        await File.WriteAllTextAsync(bicepPath, """
provider './provider.tgz'

resource fooRes 'fooType@v1' = {
  identifier: 'foo'
  properties: {
    required: 'bar'
  }
}
""");

        var bicepUri = PathHelper.FilePathToFileUrl(bicepPath);


        var compiler = services.Build().GetCompiler();
        var compilation = await compiler.CreateCompilation(bicepUri);

        var result = CompilationHelper.GetCompilationResult(compilation);

        result.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public async Task Existing_resources_are_permitted_through_3p_type_registry()
    {
        var services = await ProviderTestHelper.GetServiceBuilderWithPublishedProvider(ThirdPartyTypeHelper.GetTestTypesTgz(), AllFeaturesEnabled);

        var result = await CompilationHelper.RestoreAndCompile(services, """
provider 'br:example.azurecr.io/providers/foo:1.2.3'

resource fooRes 'fooType@v1' existing = {
}
""");

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP035", DiagnosticLevel.Warning, """The specified "resource" declaration is missing the following required properties: "identifier". If this is an inaccuracy in the documentation, please report it to the Bicep Team."""),
        });

        result = await CompilationHelper.RestoreAndCompile(services, """
provider 'br:example.azurecr.io/providers/foo:1.2.3'

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

        var services = ProviderTestHelper.GetServiceBuilder(fileSystem, registry, repository, AllFeaturesEnabled);

        await RegistryHelper.PublishProviderToRegistryAsync(services.Build(), "/types/index.json", $"br:{registry}/{repository}:1.2.3");

        var result = await CompilationHelper.RestoreAndCompile(services, """
provider 'br:example.azurecr.io/test/provider/http:1.2.3' with {}

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
        var services = await ProviderTestHelper.GetServiceBuilderWithPublishedProvider(ThirdPartyTypeHelper.GetTestTypesTgz(), AllFeaturesEnabled);

        var result = await CompilationHelper.RestoreAndCompile(services, """
provider 'br:example.azurecr.io/providers/foo:1.2.3'

resource fooRes 'fooType@v1' existing = {
  identifier: 'foo'
}

output baz string = fooRes.convertBarToBaz('bar')
""");

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveValueAtPath("$.outputs['baz'].value", "[invokeResourceMethod('fooRes', 'convertBarToBaz', createArray('bar'))]");
    }

    [TestMethod]
    public async Task Implicit_providers_are_permitted_through_3p_type_registry()
    {
        var fileSystem = new MockFileSystem();
        var services = await ProviderTestHelper.GetServiceBuilderWithPublishedProvider(ThirdPartyTypeHelper.GetTestTypesTgz(), AllFeaturesEnabled, fileSystem);

        fileSystem.File.WriteAllText("/bicepconfig.json", """
 {
   "providers": {
     "foo": "br:example.azurecr.io/providers/foo:1.2.3"
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

        var services = ProviderTestHelper.GetServiceBuilder(fileSystem, registry, repository, AllFeaturesEnabled);

        await RegistryHelper.PublishProviderToRegistryAsync(services.Build(), "/types/index.json", $"br:{registry}/{repository}:1.2.3");

        var result = await CompilationHelper.RestoreAndCompile(services, @$"
provider 'br:example.azurecr.io/test/provider/http:1.2.3'
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

        var services = ProviderTestHelper.GetServiceBuilder(fileSystem, registry, repository, new());

        await RegistryHelper.PublishProviderToRegistryAsync(services.Build(), "/types/index.json", $"br:{registry}/{repository}:1.2.3");

        var result = await CompilationHelper.RestoreAndCompile(services, @$"
provider 'br:example.azurecr.io/test/provider/http:1.2.3'
");
        result.Should().HaveDiagnostics([
            ("BCP203", DiagnosticLevel.Error, "Using provider statements requires enabling EXPERIMENTAL feature \"Extensibility\"."),
        ]);

        services = ProviderTestHelper.GetServiceBuilder(fileSystem, registry, repository, new(ExtensibilityEnabled: true));


        var result2 = await CompilationHelper.RestoreAndCompile(services, @$"
provider 'br:example.azurecr.io/test/provider/http:1.2.3'
");
        result2.Should().HaveDiagnostics([
            ("BCP400", DiagnosticLevel.Error, """Fetching types from the registry requires enabling EXPERIMENTAL feature "ProviderRegistry"."""),
        ]);
    }

    [TestMethod]
    public async Task Using_interpolated_strings_in_provider_declaration_syntax_results_in_diagnostic()
    {
        var services = new ServiceBuilder()
            .WithFeatureOverrides(AllFeaturesEnabled);

        var result = await CompilationHelper.RestoreAndCompile(services, """
var registryHost = 'example.azurecr.io'
provider 'br:${registryHost}/test/provider/http:1.2.3'
""");
        result.Should().NotGenerateATemplate();
        result.Should().ContainDiagnostic("BCP303", DiagnosticLevel.Error, "String interpolation is unsupported for specifying the provider.");
    }

    [TestMethod]
    public async Task Cannot_import_az_without_dynamic_type_loading_enabled()
    {
        var services = await ProviderTestHelper.GetServiceBuilderWithPublishedProvider(ThirdPartyTypeHelper.GetTestTypesTgz(), "mcr.microsoft.com/bicep/provider/az:1.2.3", AllFeaturesEnabled);
        services = services.WithFeatureOverrides(new(ExtensibilityEnabled: true, DynamicTypeLoadingEnabled: false));

        var result = await CompilationHelper.RestoreAndCompile(services, @"
provider 'br:mcr.microsoft.com/bicep/provider/az:1.2.3'
");
        result.Should().NotGenerateATemplate();
        result.Should().HaveDiagnostics([
            ("BCP399", DiagnosticLevel.Error, """Fetching az types from the registry requires enabling EXPERIMENTAL feature "DynamicTypeLoading".""")
        ]);
    }

    [TestMethod]
    public async Task Missing_required_provider_configuration_blocks_compilation()
    {
        var services = await ProviderTestHelper.GetServiceBuilderWithPublishedProvider(ThirdPartyTypeHelper.GetTestTypesTgzWithFallbackAndConfiguration(), AllFeaturesEnabled);

        var result = await CompilationHelper.RestoreAndCompile(services, """
provider 'br:example.azurecr.io/providers/foo:1.2.3'

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
        // tgzData provideds configType with the properties namespace, config, and context
        var services = await ProviderTestHelper.GetServiceBuilderWithPublishedProvider(ThirdPartyTypeHelper.GetTestTypesTgzWithFallbackAndConfiguration(), AllFeaturesEnabled);

        var result = await CompilationHelper.RestoreAndCompile(services, """
provider 'br:example.azurecr.io/providers/foo:1.2.3' with {
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

        result.Template.Should().HaveValueAtPath("$.imports['ThirdPartyProvider']['provider']", "ThirdPartyProvider");
        result.Template.Should().HaveValueAtPath("$.imports['ThirdPartyProvider']['version']", "1.0.0");

        result.Template.Should().HaveValueAtPath("$.imports['ThirdPartyProvider']['config']['namespace']", "ThirdPartyNamespace");
        result.Template.Should().HaveValueAtPath("$.imports['ThirdPartyProvider']['config']['config']", "Some path to config file");
        result.Template.Should().HaveValueAtPath("$.imports['ThirdPartyProvider']['config']['context']", "Some ThirdParty context");

        result.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public async Task Missing_configuration_property_throws_errors()
    {
        var services = await ProviderTestHelper.GetServiceBuilderWithPublishedProvider(ThirdPartyTypeHelper.GetTestTypesTgzWithFallbackAndConfiguration(), AllFeaturesEnabled);

        // Missing the required configuration property: namespace
        var result = await CompilationHelper.RestoreAndCompile(services, """
provider 'br:example.azurecr.io/providers/foo:1.2.3' with {
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
        var services = await ProviderTestHelper.GetServiceBuilderWithPublishedProvider(ThirdPartyTypeHelper.GetTestTypesTgzWithFallbackAndConfiguration(), AllFeaturesEnabled);

        // Mispelled the required configuration property: namespace
        var result = await CompilationHelper.RestoreAndCompile(services, """
provider 'br:example.azurecr.io/providers/foo:1.2.3' with {
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
        var services = await ProviderTestHelper.GetServiceBuilderWithPublishedProvider(ThirdPartyTypeHelper.GetTestTypesTgzWithFallbackAndConfiguration(), AllFeaturesEnabled);

        // Mispelled the optional configuration property: context
        var result = await CompilationHelper.RestoreAndCompile(services, """
provider 'br:example.azurecr.io/providers/foo:1.2.3' with {
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
        var services = await ProviderTestHelper.GetServiceBuilderWithPublishedProvider(ThirdPartyTypeHelper.GetTestTypesTgzWithFallbackAndConfiguration(), AllFeaturesEnabled);

        var result = await CompilationHelper.RestoreAndCompile(services, """
provider 'br:example.azurecr.io/providers/foo:1.2.3' with {
  namespace: 'ThirdPartyNamespace'
  config: 'Some path to config file'
}

resource test 'test@v1' = {
  bodyProp: 'fallback body'
}
""");

        result.Should().GenerateATemplate();

        result.Template.Should().HaveValueAtPath("$.resources['test']['properties']['bodyProp']", "fallback body");

        result.Should().HaveDiagnostics(new[]{
            ("BCP081", DiagnosticLevel.Warning, "Resource type \"test@v1\" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed.")
        });
    }

    [TestMethod]
    public async Task Fallback_not_provided_in_json()
    {
        // tgzData does have fallback type
        var services = await ProviderTestHelper.GetServiceBuilderWithPublishedProvider(ThirdPartyTypeHelper.GetTestTypesTgz(), AllFeaturesEnabled);

        var result = await CompilationHelper.RestoreAndCompile(services, """
provider 'br:example.azurecr.io/providers/foo:1.2.3'

resource test 'test@v1' = {
  bodyProp: 'fallback body'
}
""");

        result.Should().NotGenerateATemplate();
        result.Should().HaveDiagnostics(new[]{
            ("BCP029", DiagnosticLevel.Error, "The resource type is not valid. Specify a valid resource type of format \"<types>@<apiVersion>\"."),
        });
    }

    [TestMethod]
    public async Task Provider_imports_can_be_defined_in_config()
    {
        var fileSystem = new MockFileSystem();
        var services = await ProviderTestHelper.GetServiceBuilderWithPublishedProvider(ThirdPartyTypeHelper.GetTestTypesTgz(), AllFeaturesEnabled, fileSystem);

        // incorrect provider version - verify it returns an error
        fileSystem.File.WriteAllText("/bicepconfig.json", """
 {
   "providers": {
     "foo": "br:example.azurecr.io/providers/foo:1.2.4"
   },
  "experimentalFeaturesEnabled": {
    "extensibility": true,
    "providerRegistry": true
  }
}
""");
        var result = await CompilationHelper.RestoreAndCompile(services, """
provider foo
""");

        result.Should().NotGenerateATemplate();
        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
            ("BCP192", DiagnosticLevel.Error, """Unable to restore the artifact with reference "br:example.azurecr.io/providers/foo:1.2.4": The artifact does not exist in the registry.""")
        });

        // correct provider version
        fileSystem.File.WriteAllText("/bicepconfig.json", """
 {
   "providers": {
     "foo": "br:example.azurecr.io/providers/foo:1.2.3"
   },
  "experimentalFeaturesEnabled": {
    "extensibility": true,
    "providerRegistry": true
  }
}
""");
        result = await CompilationHelper.RestoreAndCompile(services, """
provider foo

resource fooRes 'fooType@v1' = {
  identifier: 'foo'
  properties: {
    required: 'bar'
  }
}
""");

        result.Should().GenerateATemplate();

        // correct provider version, defined implicitly
        fileSystem.File.WriteAllText("/bicepconfig.json", """
 {
  "providers": {
    "foo": "br:example.azurecr.io/providers/foo:1.2.3"
  },
  "implicitProviders": ["foo"],
  "experimentalFeaturesEnabled": {
    "extensibility": true,
    "providerRegistry": true
  }
}
""");
        result = await CompilationHelper.RestoreAndCompile(services, """
resource fooRes 'fooType@v1' = {
  identifier: 'foo'
  properties: {
    required: 'bar'
  }
}
""");

        result.Should().GenerateATemplate();
    }
}
