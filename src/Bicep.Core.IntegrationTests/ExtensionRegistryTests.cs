// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions.TestingHelpers;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry.Extensions;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Baselines;
using Bicep.Core.UnitTests.Extensions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Utils;
using Bicep.TextFixtures.Assertions;
using Bicep.TextFixtures.Utils;
using Bicep.TextFixtures.Utils.IO;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests;

[TestClass]
public class ExtensionRegistryTests : TestBase
{
    private static readonly FeatureProviderOverrides AllFeaturesEnabled = new();
    private static readonly FeatureProviderOverrides AllFeaturesEnabledForLocalDeploy = new(LocalDeployEnabled: true);

    [TestMethod]
    [TestCategory(BaselineHelper.BaselineTestCategory)]
    [EmbeddedFilesTestData(@"Files/ExtensionRegistryTests/http/types/index.json")]
    public void Http_extension_can_be_generated(EmbeddedFile indexJson)
    {
        var baselineFolder = BaselineFolder.BuildOutputFolder(TestContext, indexJson);
        var httpTypes = ExtensionResourceTypeHelper.GetHttpExtensionTypes();

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

    [DataTestMethod]
    [DataRow(false, "", false)]
    [DataRow(false, "as fooExt", false)]
    [DataRow(true, "", true)]
    [DataRow(true, "as fooExt", false)]
    public async Task Extensions_published_to_a_registry_can_be_compiled(bool moduleConfigsEnabled, string extensionAsSyntax, bool throwsErrorDiagnostic)
    {
        // types taken from https://github.com/Azure/bicep-registry-providers/tree/21aadf24cd6e8c9c5da2db0d1438df9def548b09/providers/http
        var fileSystem = FileHelper.CreateMockFileSystemForEmbeddedFiles(
            typeof(ExtensionRegistryTests).Assembly,
            "Files/ExtensionRegistryTests/http");

        var registry = "example.azurecr.io";
        var repository = $"test/extension/http";

        var services = ExtensionTestHelper.GetServiceBuilder(fileSystem, registry, repository, AllFeaturesEnabled with { ModuleExtensionConfigsEnabled = moduleConfigsEnabled });

        await RegistryHelper.PublishExtensionToRegistryAsync(services.Build(), "/types/index.json", $"br:{registry}/{repository}:1.2.3");

        var result = await CompilationHelper.RestoreAndCompile(
            services,
            $$"""
              extension 'br:example.azurecr.io/test/extension/http:1.2.3' {{extensionAsSyntax}}

              resource dadJoke 'request@v1' = {
                uri: 'https://icanhazdadjoke.com'
                method: 'GET'
                format: 'json'
              }

              output joke string = dadJoke.body.joke
              """);

        if (throwsErrorDiagnostic)
        {
            var expectedDiagnostic = DiagnosticBuilder.ForDocumentStart().ExtensionAliasMustBeDefinedForInlinedRegistryExtensionDeclaration();
            result.Should().OnlyContainDiagnostic(expectedDiagnostic.Code, expectedDiagnostic.Level, expectedDiagnostic.Message);

            return;
        }

        result.Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();
    }

    [DataTestMethod]
    [DataRow(false, "", false)]
    [DataRow(false, "as fooExt", false)]
    [DataRow(true, "", true)]
    [DataRow(true, "as fooExt", false)]
    public async Task Extensions_published_to_filesystem_can_be_compiled(bool moduleConfigsEnabled, string extensionAsSyntax, bool throwsErrorDiagnostic)
    {
        var cacheDirectory = FileHelper.GetCacheRootDirectory(TestContext).EnsureExists();
        var services = new ServiceBuilder().WithFeatureOverrides(new(CacheRootDirectory: cacheDirectory, ModuleExtensionConfigsEnabled: moduleConfigsEnabled));
        ;

        var typesTgz = ExtensionResourceTypeHelper.GetTestTypesTgz();
        var tempDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
        Directory.CreateDirectory(tempDirectory);

        var bicepPath = Path.Combine(tempDirectory, "main.bicep");
        var bicepUri = PathHelper.FilePathToFileUrl(bicepPath);
        await File.WriteAllTextAsync(
            bicepPath,
            $$"""
              extension './extension.tgz' {{extensionAsSyntax}}

              resource fooRes 'fooType@v1' = {
                identifier: 'foo'
                properties: {
                  required: 'bar'
                }
              }
              """);

        var extensionPath = Path.Combine(tempDirectory, "extension.tgz");
        await RegistryHelper.PublishExtensionToRegistryAsync(services.Build(), Path.Combine(tempDirectory, extensionPath), typesTgz, bicepUri);


        var compiler = services.Build().GetCompiler();
        var compilation = await compiler.CreateCompilation(bicepUri);

        var result = CompilationHelper.GetCompilationResult(compilation);

        if (throwsErrorDiagnostic)
        {
            var expectedDiagnostic = DiagnosticBuilder.ForDocumentStart().ExtensionAliasMustBeDefinedForInlinedRegistryExtensionDeclaration();
            result.Should().OnlyContainDiagnostic(expectedDiagnostic.Code, expectedDiagnostic.Level, expectedDiagnostic.Message);

            return;
        }

        result.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public async Task Filesystem_extensions_can_be_compiled()
    {
        // See https://github.com/Azure/bicep/issues/14770 for context
        var typesTgz = ExtensionResourceTypeHelper.GetTestTypesTgz();
        var extensionTgz = await ExtensionV1Archive.Build(new(typesTgz, false, []));

        var result = await CompilationHelper.RestoreAndCompile(
          ("main.bicep", new("""
extension '../extension.tgz'

resource fooRes 'fooType@v1' = {
  identifier: 'foo'
  properties: {
    required: 'bar'
  }
}
""")), ("../extension.tgz", extensionTgz));

        result.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public async Task Filesystem_extensions_can_be_compiled_bicepconfig()
    {
        // See https://github.com/Azure/bicep/issues/14770 for context
        var typesTgz = ExtensionResourceTypeHelper.GetTestTypesTgz();
        var extensionTgz = await ExtensionV1Archive.Build(new(typesTgz, false, []));

        var result = await CompilationHelper.RestoreAndCompile(
          ("main.bicep", new("""
extension myExtension

resource fooRes 'fooType@v1' = {
  identifier: 'foo'
  properties: {
    required: 'bar'
  }
}
""")), ("../bicepconfig.json", new("""
{
  "extensions": {
    "myExtension": "./extension.tgz"
  }
}
""")), ("../extension.tgz", extensionTgz));

        result.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public async Task Extensions_published_to_a_registry_requires_alias_with_module_configs_enabled()
    {
        // types taken from https://github.com/Azure/bicep-registry-providers/tree/21aadf24cd6e8c9c5da2db0d1438df9def548b09/providers/http
        var fileSystem = FileHelper.CreateMockFileSystemForEmbeddedFiles(
            typeof(ExtensionRegistryTests).Assembly,
            "Files/ExtensionRegistryTests/http");

        var registry = "example.azurecr.io";
        var repository = "test/extension/http";

        var services = ExtensionTestHelper.GetServiceBuilder(fileSystem, registry, repository, AllFeaturesEnabled with { ModuleExtensionConfigsEnabled = true });

        await RegistryHelper.PublishExtensionToRegistryAsync(services.Build(), "/types/index.json", $"br:{registry}/{repository}:1.2.3");

        var result = await CompilationHelper.RestoreAndCompile(
            services,
            """
            extension 'br:example.azurecr.io/test/extension/http:1.2.3'

            resource dadJoke 'request@v1' = {
              uri: 'https://icanhazdadjoke.com'
              method: 'GET'
              format: 'json'
            }

            output joke string = dadJoke.body.joke
            """);

        var expectedDiagnostic = DiagnosticBuilder.ForDocumentStart().ExtensionAliasMustBeDefinedForInlinedRegistryExtensionDeclaration();
        result.Should().OnlyContainDiagnostic(expectedDiagnostic.Code, expectedDiagnostic.Level, expectedDiagnostic.Message);
    }

    [TestMethod]
    public async Task Missing_extension_file_raises_a_diagnostic()
    {
        // See https://github.com/Azure/bicep/issues/14770 for context
        var result = await new TestCompiler().RestoreAndCompileMockFileSystemFiles(
            ("main.bicep", new("""
                extension './non_existent.tgz'
                """)));

        result.Should().HaveDiagnostics([
            ("BCP091", DiagnosticLevel.Error, $"An error occurred reading file. Could not find file '{TestFileUri.FromMockFileSystemPath("./non_existent.tgz")}'."),
        ]);
    }

    [TestMethod]
    public async Task Missing_extension_file_raises_a_diagnostic_bicepconfig()
    {
        // See https://github.com/Azure/bicep/issues/14770 for context
        var result = await new TestCompiler().RestoreAndCompileMockFileSystemFiles(
            ("main.bicep", """
                  extension nonExistent
                  """),
            ("../bicepconfig.json", """
                  {
                    "extensions": {
                      "nonExistent": "./non_existent.tgz"
                    }
                  }
                  """));

        //var sourceUri = InMemoryFileResolver.GetFileUri("/path/to/main.bicep");
        result.Should().HaveDiagnostics([
            ("BCP091", DiagnosticLevel.Error, $"An error occurred reading file. Could not find file '{TestFileUri.FromMockFileSystemPath("../non_existent.tgz")}'."),
        ]);
    }

    [TestMethod]
    public async Task Existing_resources_are_permitted_through_3p_type_registry()
    {
        var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(ExtensionResourceTypeHelper.GetTestTypesTgz(), AllFeaturesEnabled);

        var result = await CompilationHelper.RestoreAndCompile(services, """
extension 'br:example.azurecr.io/extensions/foo:1.2.3'

resource fooRes 'fooType@v1' existing = {
}
""");

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP035", DiagnosticLevel.Error, """The specified "resource" declaration is missing the following required properties: "identifier"."""),
        });

        result = await CompilationHelper.RestoreAndCompile(services, """
extension 'br:example.azurecr.io/extensions/foo:1.2.3'

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
            typeof(ExtensionRegistryTests).Assembly,
            "Files/ExtensionRegistryTests/http");

        var registry = "example.azurecr.io";
        var repository = $"test/extension/http";

        var services = ExtensionTestHelper.GetServiceBuilder(fileSystem, registry, repository, AllFeaturesEnabled);

        await RegistryHelper.PublishExtensionToRegistryAsync(services.Build(), "/types/index.json", $"br:{registry}/{repository}:1.2.3");

        var result = await CompilationHelper.RestoreAndCompile(services, """
extension 'br:example.azurecr.io/test/extension/http:1.2.3' with {}

resource dadJoke 'request@v1' = {
  uri: 'https://icanhazdadjoke.com'
  method: 'GET'
  format: 'json'
}

output joke string = dadJoke.body.joke
""");

        result.Should().NotGenerateATemplate();
        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
            ("BCP205", DiagnosticLevel.Error, "Extension \"http\" does not support configuration.")
        });
    }

    [TestMethod]
    public async Task Resource_function_types_are_permitted_through_3p_type_registry()
    {
        var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(ExtensionResourceTypeHelper.GetTestTypesTgz(), AllFeaturesEnabled);

        var result = await CompilationHelper.RestoreAndCompile(services, """
extension 'br:example.azurecr.io/extensions/foo:1.2.3'

resource fooRes 'fooType@v1' existing = {
  identifier: 'foo'
}

output baz string = fooRes.convertBarToBaz('bar')
""");

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveValueAtPath("$.outputs['baz'].value", "[invokeResourceMethod('fooRes', 'convertBarToBaz', createArray('bar'))]");
    }

    [TestMethod]
    public async Task Implicit_extensions_are_permitted_through_3p_type_registry()
    {
        var fileSystem = new MockFileSystem();
        var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(ExtensionResourceTypeHelper.GetTestTypesTgz(), AllFeaturesEnabled, fileSystem);

        fileSystem.File.WriteAllText("/bicepconfig.json", """
 {
   "extensions": {
     "foo": "br:example.azurecr.io/extensions/foo:1.2.3"
   },
  "implicitExtensions": ["foo"]
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
            typeof(ExtensionRegistryTests).Assembly,
            "Files/ExtensionRegistryTests/http");

        var registry = "example.azurecr.io";
        var repository = $"test/extension/http";

        var services = ExtensionTestHelper.GetServiceBuilder(fileSystem, registry, repository, AllFeaturesEnabled);

        await RegistryHelper.PublishExtensionToRegistryAsync(services.Build(), "/types/index.json", $"br:{registry}/{repository}:1.2.3");

        var result = await CompilationHelper.RestoreAndCompile(services, @$"
extension 'br:example.azurecr.io/test/extension/http:1.2.3'
");
        result.Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();
        result.Template.Should().DeepEqual(JToken.Parse("""
{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "languageVersion": "2.1-experimental",
  "contentVersion": "1.0.0.0",
  "metadata": {
    "_generator": {
      "name": "bicep",
      "version": "dev",
      "templateHash": "15182588323302093073"
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
    public async Task Using_interpolated_strings_in_extension_declaration_syntax_results_in_diagnostic()
    {
        var services = new ServiceBuilder()
            .WithFeatureOverrides(AllFeaturesEnabled);

        var result = await CompilationHelper.RestoreAndCompile(services, """
var registryHost = 'example.azurecr.io'
extension 'br:${registryHost}/test/extension/http:1.2.3'
""");
        result.Should().NotGenerateATemplate();
        result.Should().ContainDiagnostic("BCP303", DiagnosticLevel.Error, "String interpolation is unsupported for specifying the extension.");
    }

    [TestMethod]
    public async Task Missing_required_extension_configuration_blocks_compilation()
    {
        var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(ExtensionResourceTypeHelper.GetTestTypesTgzWithFallbackAndConfiguration(), AllFeaturesEnabled);

        var result = await CompilationHelper.RestoreAndCompile(services, """
extension 'br:example.azurecr.io/extensions/foo:1.2.3'

resource dadJoke 'fooType@v1' = {
  identifier: 'foo'
  joke: 'dad joke'
}

output joke string = dadJoke.joke
""");

        result.Should().NotGenerateATemplate();
        result.Should().HaveDiagnostics(new[]{
            ("BCP206", DiagnosticLevel.Error, "Extension \"ThirdPartyExtension\" requires configuration, but none was provided.")
        });
    }

    [TestMethod]
    public async Task Correct_local_deploy_extension_configuration_result_in_successful_compilation()
    {
        // tgzData provideds configType with the properties namespace, config, and context
        var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(ExtensionResourceTypeHelper.GetTestTypesTgzWithFallbackAndConfiguration(), AllFeaturesEnabledForLocalDeploy);

        var result = await CompilationHelper.RestoreAndCompile(services, """
targetScope = 'local'

extension 'br:example.azurecr.io/extensions/foo:1.2.3' with {
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

        result.Template.Should().HaveValueAtPath("$.imports['ThirdPartyExtension']", JObject.Parse("""
        {
          "provider": "ThirdPartyExtension",
          "version": "1.0.0",
          "config": {
            "namespace": "ThirdPartyNamespace",
            "config": "Some path to config file",
            "context": "Some ThirdParty context"
          }
        }
        """));

        result.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public async Task Correct_local_deploy_extension_configuration_result_in_successful_compilation_v2_api()
    {
        // tgzData provides configType with the properties namespace, config, and context
        var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(
            ExtensionResourceTypeHelper.GetTestTypesTgzWithFallbackAndConfiguration(),
            AllFeaturesEnabledForLocalDeploy with { ModuleExtensionConfigsEnabled = true });

        var result = await CompilationHelper.RestoreAndCompileParams(
            services,
            mainBicepFileContents:
            """
            targetScope = 'local'

            extension 'br:example.azurecr.io/extensions/foo:1.2.3' with {
              namespace: 'ThirdPartyNamespace'
            } as fooExt

            resource dadJoke 'fooType@v1' = {
              identifier: 'foo'
              joke: 'dad joke'
            }

            output joke string = dadJoke.joke
            """,
            bicepParamsFileContent:
            """
            using 'main.bicep'

            extensionConfig fooExt with {
              config: 'paramsFileConfig'
              context: 'paramsFileContext'
            }
            """);

        result.Should().NotHaveAnyDiagnostics();

        var template = JToken.Parse(result.GetTestTemplate());

        template.Should().NotBeNull();

        template.Should()
            .HaveValueAtPath(
                "$.extensions['fooExt']", JObject.Parse(
                    """
                    {
                      "name": "ThirdPartyExtension",
                      "version": "1.0.0",
                      "config": {
                        "namespace": {
                          "defaultValue": "ThirdPartyNamespace"
                        }
                      }
                    }
                    """));

        var parameters = JToken.Parse(result.GetTestParametersFile() ?? string.Empty);
        parameters.Should().NotBeNull();

        parameters.Should()
            .HaveValueAtPath(
                "$.extensionConfigs['fooExt']", JObject.Parse(
                    """
                    {
                      "config": {
                        "value": "paramsFileConfig"
                      },
                      "context": {
                        "value": "paramsFileContext"
                      }
                    }
                    """));
    }

    [TestMethod]
    public async Task Local_deploy_extension_with_configuration_defined_and_empty_configuration_provided_throws_errors()
    {
        // tgzData provideds configType with the properties namespace, config, and context
        var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(ExtensionResourceTypeHelper.GetTestTypesTgzWithFallbackAndConfiguration(), AllFeaturesEnabledForLocalDeploy);

        var result = await CompilationHelper.RestoreAndCompile(services, """
targetScope = 'local'

extension 'br:example.azurecr.io/extensions/foo:1.2.3' with { }

resource dadJoke 'fooType@v1' = {
  identifier: 'foo'
  joke: 'dad joke'
}

output joke string = dadJoke.joke
""");

        result.Template.Should().BeNull();

        result.Should().HaveDiagnostics([("BCP035", DiagnosticLevel.Error, "The specified \"object\" declaration is missing the following required properties: \"config\", \"namespace\".")], because: "Type checking should block the template compilation because required extension config properties hasn't been supplied.");
    }

    [TestMethod]
    public async Task Local_deploy_extension_without_configuration_defined_but_configuration_provided_throws_errors()
    {
        var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(ExtensionResourceTypeHelper.GetTestTypesTgz(), AllFeaturesEnabledForLocalDeploy);

        var result = await CompilationHelper.RestoreAndCompile(services, """
targetScope = 'local'

extension 'br:example.azurecr.io/extensions/foo:1.2.3' with {
  namespace: 'ThirdPartyNamespace'
  config: 'Some path to config file'
  context: 'Some ThirdParty context'
}

resource fooRes 'fooType@v1' existing = {
  identifier: 'foo'
}

output baz string = fooRes.convertBarToBaz('bar')

""");

        result.Template.Should().BeNull();

        result.Should().HaveDiagnostics([("BCP205", DiagnosticLevel.Error, "Extension \"ThirdPartyExtension\" does not support configuration.")], because: "Type checking should block the template compilation because extension does not support configuration but one has been provided.");
    }

    [TestMethod]
    public async Task Correct_extension_configuration_result_in_successful_compilation()
    {
        // tgzData provideds configType with the properties namespace, config, and context
        var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(ExtensionResourceTypeHelper.GetTestTypesTgzWithFallbackAndConfiguration(), AllFeaturesEnabled);

        var result = await CompilationHelper.RestoreAndCompile(services, """
extension 'br:example.azurecr.io/extensions/foo:1.2.3' with {
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

        result.Template.Should().HaveValueAtPath("$.imports['ThirdPartyExtension']['provider']", "ThirdPartyExtension");
        result.Template.Should().HaveValueAtPath("$.imports['ThirdPartyExtension']['version']", "1.0.0");

        result.Template.Should().HaveValueAtPath("$.imports['ThirdPartyExtension']['config']['namespace']", "ThirdPartyNamespace");
        result.Template.Should().HaveValueAtPath("$.imports['ThirdPartyExtension']['config']['config']", "Some path to config file");
        result.Template.Should().HaveValueAtPath("$.imports['ThirdPartyExtension']['config']['context']", "Some ThirdParty context");

        result.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public async Task Missing_configuration_property_throws_errors()
    {
        var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(ExtensionResourceTypeHelper.GetTestTypesTgzWithFallbackAndConfiguration(), AllFeaturesEnabled);

        // Missing the required configuration property: namespace
        var result = await CompilationHelper.RestoreAndCompile(services, """
extension 'br:example.azurecr.io/extensions/foo:1.2.3' with {
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
    public async Task Misspelled_required_configuration_property_throws_error()
    {
        var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(ExtensionResourceTypeHelper.GetTestTypesTgzWithFallbackAndConfiguration(), AllFeaturesEnabled);

        // Misspelled the required configuration property: namespace
        var result = await CompilationHelper.RestoreAndCompile(services, """
extension 'br:example.azurecr.io/extensions/foo:1.2.3' with {
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
    public async Task Misspelled_optional_configuration_property_throws_error()
    {
        var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(ExtensionResourceTypeHelper.GetTestTypesTgzWithFallbackAndConfiguration(), AllFeaturesEnabled);

        // Misspelled the optional configuration property: context
        var result = await CompilationHelper.RestoreAndCompile(services, """
extension 'br:example.azurecr.io/extensions/foo:1.2.3' with {
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
        var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(ExtensionResourceTypeHelper.GetTestTypesTgzWithFallbackAndConfiguration(), AllFeaturesEnabled);

        var result = await CompilationHelper.RestoreAndCompile(services, """
extension 'br:example.azurecr.io/extensions/foo:1.2.3' with {
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
        var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(ExtensionResourceTypeHelper.GetTestTypesTgz(), AllFeaturesEnabled);

        var result = await CompilationHelper.RestoreAndCompile(services, """
extension 'br:example.azurecr.io/extensions/foo:1.2.3'

resource test 'test@v1' = {
  bodyProp: 'fallback body'
}
""");

        result.Should().NotGenerateATemplate();
        result.Should().HaveDiagnostics(new[]{
            ("BCP029", DiagnosticLevel.Error, "The resource type is not valid. Specify a valid resource type of format \"<type-name>@<apiVersion>\"."),
        });
    }

    [TestMethod]
    public async Task Extension_imports_can_be_defined_in_config()
    {
        var fileSystem = new MockFileSystem();
        var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(ExtensionResourceTypeHelper.GetTestTypesTgz(), AllFeaturesEnabled, fileSystem);

        // incorrect extension version - verify it returns an error
        fileSystem.File.WriteAllText("/bicepconfig.json", """
            {
              "extensions": {
                "foo": "br:example.azurecr.io/extensions/foo:1.2.4"
              }
            }
            """);
        var result = await CompilationHelper.RestoreAndCompile(services, """
            extension foo
            """);

        result.Should().NotGenerateATemplate();
        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
            ("BCP192", DiagnosticLevel.Error, """Unable to restore the artifact with reference "br:example.azurecr.io/extensions/foo:1.2.4": The artifact does not exist in the registry.""")
        });

        // correct extension version
        fileSystem.File.WriteAllText("/bicepconfig.json", """
            {
              "extensions": {
                "foo": "br:example.azurecr.io/extensions/foo:1.2.3"
              }
            }
            """);
        result = await CompilationHelper.RestoreAndCompile(services, """
            extension foo

            resource fooRes 'fooType@v1' = {
              identifier: 'foo'
              properties: {
                required: 'bar'
              }
            }
            """);

        result.Should().GenerateATemplate();

        // correct extension version, defined implicitly
        fileSystem.File.WriteAllText("/bicepconfig.json", """
            {
              "extensions": {
                "foo": "br:example.azurecr.io/extensions/foo:1.2.3"
              },
              "implicitExtensions": ["foo"]
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

    [TestMethod]
    public async Task Implicit_extensions_are_included_in_output()
    {
        // https://github.com/Azure/bicep/issues/15395
        var fileSystem = new MockFileSystem();
        var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(ExtensionResourceTypeHelper.GetTestTypesTgz(), AllFeaturesEnabled, fileSystem);

        fileSystem.File.WriteAllText("/bicepconfig.json", """
{
  "extensions": {
    "foo": "br:example.azurecr.io/extensions/foo:1.2.3"
  },
  "implicitExtensions": ["foo"]
}
""");
        var result = await CompilationHelper.RestoreAndCompile(services, """
resource fooRes 'fooType@v1' = {
  identifier: 'foo'
  properties: {
    required: 'bar'
  }
}
""");

        result.Should().GenerateATemplate();
        result.Template.Should().HaveJsonAtPath("$.imports['foo']", """
{
  "provider": "ThirdPartyExtension",
  "version": "1.0.0"
}
""");

        result.Template.Should().HaveJsonAtPath("$.resources['fooRes']", """
{
  "import": "foo",
  "type": "fooType@v1",
  "properties": {
    "identifier": "foo",
    "properties": {
      "required": "bar"
    }
  }
}
""");
    }

    [TestMethod]
    public async Task Implicit_extensions_generate_correct_symbol_names()
    {
        // https://github.com/Azure/bicep/issues/15396
        var fileSystem = new MockFileSystem();
        var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(ExtensionResourceTypeHelper.GetTestTypesTgz(), AllFeaturesEnabled, fileSystem);

        fileSystem.File.WriteAllText("/bicepconfig.json", """
{
  "extensions": {
    "bar": "br:example.azurecr.io/extensions/foo:1.2.3"
  }
}
""");
        var result = await CompilationHelper.RestoreAndCompile(services, """
extension bar

resource fooRes 'fooType@v1' = {
  identifier: 'foo'
  properties: {
    required: 'bar'
  }
}

resource bazRes 'bar:fooType@v1' = {
  identifier: 'baz'
  properties: {
    required: 'bar'
  }
}
""");

        result.Should().GenerateATemplate();
        result.Template.Should().HaveJsonAtPath("$.imports['bar']", """
{
  "provider": "ThirdPartyExtension",
  "version": "1.0.0"
}
""");

        result.Template.Should().HaveJsonAtPath("$.resources['fooRes']", """
{
  "import": "bar",
  "type": "fooType@v1",
  "properties": {
    "identifier": "foo",
    "properties": {
      "required": "bar"
    }
  }
}
""");

        result.Template.Should().HaveJsonAtPath("$.resources['bazRes']", """
{
  "import": "bar",
  "type": "fooType@v1",
  "properties": {
    "identifier": "baz",
    "properties": {
      "required": "bar"
    }
  }
}
""");
    }
}
