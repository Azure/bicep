// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions.TestingHelpers;
using Bicep.Core.Configuration;
using Bicep.Core.FileSystem;
using Bicep.Core.Json;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Configuration;

[TestClass]
public class ProvidersConfigurationTests
{
    [TestMethod]
    public void ProviderConfiguration_deserialization()
    {
        var data = JsonElementFactory.CreateElement("""
        {
            "providers": {
                "az": "br:mcr.microsoft.com/bicep/providers/az:0.2.3",
                "kubernetes": "builtin:"
            }
        }
        """);

        var providers = ProvidersConfiguration.Bind(data.GetProperty(RootConfiguration.ProvidersConfigurationKey));
        providers.Should().NotBeNull();

        providers.TryGetProviderSource("az").IsSuccess(out var azProvider).Should().BeTrue();
        azProvider!.Value.Should().Be("br:mcr.microsoft.com/bicep/providers/az:0.2.3");

        providers.TryGetProviderSource("kubernetes").IsSuccess(out var k8sProvider).Should().BeTrue();
        k8sProvider.Should().NotBeNull();
        k8sProvider!.Value.Should().Be("builtin:");

        providers.TryGetProviderSource("unspecified").IsSuccess(out var provider, out var errorBuilder).Should().BeFalse();
        provider.Should().BeNull();
        errorBuilder!.Should().NotBeNull();
        errorBuilder!.Should().HaveCode("BCP204");
        errorBuilder!.Should().HaveMessage($"Provider namespace \"unspecified\" is not recognized.");
    }

    [TestMethod]
    public void ProviderConfiguration_default_configuration_returns_known_list_of_built_in_providers_with_expected_default_values()
    {
        var config = IConfigurationManager.GetBuiltInConfiguration();

        config.Should().NotBeNull();
        config!.ProvidersConfig.Should().NotBeNull();

        foreach (var providerName in new[] { "az", "kubernetes", "microsoftGraph" })
        {
            config.ProvidersConfig!.TryGetProviderSource(providerName).IsSuccess(out var provider).Should().BeTrue();
            provider!.Value.Should().Be("builtin:");
        }

        // assert that 'sys' is not present in the default configuration
        config.ProvidersConfig!.TryGetProviderSource("sys").IsSuccess().Should().BeFalse();
    }

    [TestMethod]
    public void ProviderConfiguration_user_provided_configuration_overrides_default_configuration()
    {
        var bicepConfigFileName = "bicepconfig.json";
        var fs = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            [bicepConfigFileName] = new("""
            {
                "providers": {
                    "foo": "br:example.azurecr.io/some/fake/path:1.0.0",
                    "az": "br:mcr.microsoft.com/bicep/providers/az:0.2.3"
                }
            }
            """)
        });

        var configManager = new ConfigurationManager(fs);
        var testFilePath = fs.Path.GetFullPath(bicepConfigFileName);
        var config = configManager.GetConfiguration(PathHelper.FilePathToFileUrl(testFilePath));
        config.DiagnosticBuilders.Should().BeEmpty();
        config.Should().NotBeNull();
        config!.ProvidersConfig.Should().NotBeNull();

        var providers = config.ProvidersConfig!;
        // assert 'source' and 'version' are valid properties for 'foo'
        providers.TryGetProviderSource("foo").IsSuccess(out var fooProvider).Should().BeTrue();
        fooProvider!.Value.Should().Be("br:example.azurecr.io/some/fake/path:1.0.0");

        // assert 'az' provider properties are overridden by the user provided configuration
        providers.TryGetProviderSource("az").IsSuccess(out var azProvider).Should().BeTrue();
        azProvider!.Value.Should().Be("br:mcr.microsoft.com/bicep/providers/az:0.2.3");

        // assert that 'sys' is not present in the merged configuration
        providers.TryGetProviderSource("sys").IsSuccess(out var provider, out var errorBuilder).Should().BeFalse();
        provider.Should().BeNull();
        errorBuilder!.Should().NotBeNull();
        errorBuilder!.Should().HaveCode("BCP204");
        errorBuilder!.Should().HaveMessage($"Provider namespace \"sys\" is not recognized.");
    }
}
