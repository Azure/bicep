// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using Bicep.Core.Diagnostics;
using Bicep.Core.Samples;
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
        var (clientFactory, _) = DataSetsExtensions.CreateMockRegistryClients((registryHost, repositoryPath));

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

        var services = GetServiceBuilder(fileSystem, registry, repository, true, true, true);

        await DataSetsExtensions.PublishProviderToRegistryAsync(services.Build(), "/types/index.json", $"br:{registry}/{repository}:1.2.3");

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
    public async Task Third_party_imports_are_enabled_when_feature_is_enabled()
    {
        var fileSystem = FileHelper.CreateMockFileSystemForEmbeddedFiles(
            typeof(RegistryProviderTests).Assembly,
            "Files/RegistryProviderTests/HttpProvider");

        var registry = "example.azurecr.io";
        var repository = $"test/provider/http";

        var services = GetServiceBuilder(fileSystem, registry, repository, true, true, true);

        await DataSetsExtensions.PublishProviderToRegistryAsync(services.Build(), "/types/index.json", $"br:{registry}/{repository}:1.2.3");

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

        await DataSetsExtensions.PublishProviderToRegistryAsync(services.Build(), "/types/index.json", $"br:{registry}/{repository}:1.2.3");

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

        var result = await CompilationHelper.RestoreAndCompile(services, @$"
        provider 'br:{LanguageConstants.BicepPublicMcrRegistry}/bicep/provider/az@0.0.0'
        ");
        result.Should().NotGenerateATemplate();
        result.Should().HaveDiagnostics(new[]{
            ("BCP204", DiagnosticLevel.Error, "Provider namespace \"az\" is not recognized."),
            ("BCP084",DiagnosticLevel.Error,"The symbolic name \"az\" is reserved. Please use a different symbolic name. Reserved namespaces are \"az\", \"sys\".")
        });
    }
}
