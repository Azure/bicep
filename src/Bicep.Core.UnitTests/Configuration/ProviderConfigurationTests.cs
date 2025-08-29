// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions.TestingHelpers;
using Bicep.Core.Configuration;
using Bicep.Core.FileSystem;
using Bicep.Core.Json;
using Bicep.Core.UnitTests.Assertions;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;
using Bicep.TextFixtures.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Configuration;

[TestClass]
public class ExtensionsConfigurationTests
{
    [TestMethod]
    public void ExtensionConfiguration_deserialization()
    {
        var data = JsonElementFactory.CreateElement("""
        {
            "extensions": {
                "az": "br:mcr.microsoft.com/bicep/extensions/az:0.2.3",
                "kubernetes": "builtin:"
            }
        }
        """);

        var extensions = ExtensionsConfiguration.Bind(data.GetProperty(RootConfiguration.ExtensionsKey));
        extensions.Should().NotBeNull();

        extensions.TryGetExtensionSource("az").IsSuccess(out var azExtension).Should().BeTrue();
        azExtension!.Value.Should().Be("br:mcr.microsoft.com/bicep/extensions/az:0.2.3");

        extensions.TryGetExtensionSource("kubernetes").IsSuccess(out var k8sExtension).Should().BeTrue();
        k8sExtension.Should().NotBeNull();
        k8sExtension!.Value.Should().Be("builtin:");

        extensions.TryGetExtensionSource("unspecified").IsSuccess(out var extension, out var errorBuilder).Should().BeFalse();
        extension.Should().BeNull();
        errorBuilder!.Should().NotBeNull();
        errorBuilder!.Should().HaveCode("BCP204");
        errorBuilder!.Should().HaveMessage($"Extension \"unspecified\" is not recognized.");
    }

    [TestMethod]
    public void ExtensionConfiguration_default_configuration_returns_known_list_of_built_in_extensions_with_expected_default_values()
    {
        var config = IConfigurationManager.GetBuiltInConfiguration();

        config.Should().NotBeNull();
        config!.Extensions.Should().NotBeNull();

        foreach (var extensionName in new[] { "az", "kubernetes" })
        {
            config.Extensions!.TryGetExtensionSource(extensionName).IsSuccess(out var extension).Should().BeTrue();
            extension!.Value.Should().Be("builtin:");
        }

        // assert that 'sys' is not present in the default configuration
        config.Extensions!.TryGetExtensionSource("sys").IsSuccess().Should().BeFalse();
    }

    [TestMethod]
    public void ExtensionConfiguration_user_provided_configuration_overrides_default_configuration()
    {
        var fileSet = InMemoryTestFileSet.Create(("bicepconfig.json", """
            {
              "extensions": {
                "foo": "br:example.azurecr.io/some/fake/path:1.0.0",
                "az": "br:mcr.microsoft.com/bicep/extensions/az:0.2.3"
              }
            }
            """));

        var configManager = new ConfigurationManager(fileSet.FileExplorer);
        var config = configManager.GetConfiguration(fileSet.GetUri("main.bicep"));

        config.Diagnostics.Should().BeEmpty();
        config.Should().NotBeNull();
        config!.Extensions.Should().NotBeNull();

        var extensions = config.Extensions!;
        // assert 'source' and 'version' are valid properties for 'foo'
        extensions.TryGetExtensionSource("foo").IsSuccess(out var fooExtension).Should().BeTrue();
        fooExtension!.Value.Should().Be("br:example.azurecr.io/some/fake/path:1.0.0");

        // assert 'az' extension properties are overridden by the user provided configuration
        extensions.TryGetExtensionSource("az").IsSuccess(out var azExtension).Should().BeTrue();
        azExtension!.Value.Should().Be("br:mcr.microsoft.com/bicep/extensions/az:0.2.3");

        // assert that 'sys' is not present in the merged configuration
        extensions.TryGetExtensionSource("sys").IsSuccess(out var extension, out var errorBuilder).Should().BeFalse();
        extension.Should().BeNull();
        errorBuilder!.Should().NotBeNull();
        errorBuilder!.Should().HaveCode("BCP204");
        errorBuilder!.Should().HaveMessage($"Extension \"sys\" is not recognized.");
    }
}
