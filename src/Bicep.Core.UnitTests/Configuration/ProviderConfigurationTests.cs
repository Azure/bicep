// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bicep.Core.Configuration;
using Bicep.Core.Json;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Configuration;

[TestClass]
public class ProvidersConfigurationTests
{
    private static readonly JsonSerializerOptions DefaultDeserializeOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReadCommentHandling = JsonCommentHandling.Skip,
        Converters = { new JsonStringEnumConverter() },
    };

    [TestMethod]
    public void ProviderConfiguration_deserialization_haooy_path_succeeds()
    {
        var data = JsonElementFactory.CreateElement("""
        {
            "providers": {
                "az": {
                    "source": "mcr.microsoft.com/bicep/providers/az",
                    "version": "0.2.3"
                },
                "kubernetes": {
                    "builtin": true
                }
            }
        }
        """);

        var res = JsonSerializer.Deserialize<ImmutableDictionary<string, ProviderSource>>(data.GetProperty("providers"), DefaultDeserializeOptions);
        res.Should().NotBeNull();
        var providers = res!;
        providers.Should().NotBeNull();
        providers["az"].Should().NotBeNull();
        // // verifies that 'registry' and 'version' are valid properties for 'source'
        var azProvider = providers["az"];
        azProvider.Source.Should().Be("mcr.microsoft.com/bicep/providers/az");
        azProvider.Version.Should().Be("0.2.3");

        var k8sProvider = providers["kubernetes"];
        k8sProvider.Should().NotBeNull();
        // // verifies that 'builtin' is a valid property for 'source'
        k8sProvider.BuiltIn.Should().BeTrue();
    }

    [TestMethod]
    public void ProviderConfiguration_deserialization_enforces_mutually_exclusive_properties()
    {
        var data = JsonElementFactory.CreateElement("""
        {
            "providers": {
                "az": {
                    "source": "mcr.microsoft.com/bicep/providers",
                    "version": "0.2.3",
                    "builtin": true
                }
            }
        }
        """);

        Action deserializeFn = () => JsonSerializer.Deserialize<ImmutableDictionary<string, ProviderSource>>(data.GetProperty("providers"), DefaultDeserializeOptions);
        deserializeFn.Should().Throw<ArgumentException>().WithMessage("The 'builtin' property is mutually exclusive with 'registry' and 'version'.");
    }

    [TestMethod]
    public void ProviderConfiguration_user_provided_configuration_overrides_default_configuration()
    {
        var localConfigFilePath = "bicepconfig.json";
        var fs = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            [localConfigFilePath] = new("""
                {
                    "providers": {
                        "foo": {
                            "source": "example.azurecr.io/some/fake/path",
                            "version": "1.0.0"
                        },
                        "az": {
                            "source": "mcr.microsoft.com/bicep/providers/az",
                            "version": "0.2.3"
                        }
                    }
                }
                """)
        });

        var configManager = new ConfigurationManager(fs);
        var config = configManager.GetConfiguration(new(fs.Path.GetFullPath(localConfigFilePath)));

        config.Should().NotBeNull();
        config!.ProvidersConfig.Should().NotBeNull();

        // assert 'source' and 'version' are valid properties for 'foo'
        config.ProvidersConfig!.TryGetProviderSource("foo").IsSuccess(out var fooProvider).Should().BeTrue();
        fooProvider!.Source.Should().Be("example.azurecr.io/some/fake/path");
        fooProvider!.Version.Should().Be("1.0.0");

        // assert 'az' provider properties are overridden by the user provided configuration
        config.ProvidersConfig!.TryGetProviderSource("az").IsSuccess(out var azProvider).Should().BeTrue();
        azProvider!.Source.Should().Be("mcr.microsoft.com/bicep/providers/az");
        azProvider!.Version.Should().Be("0.2.3");

        // assert that 'sys' is not present in the merged configuration
        config.ProvidersConfig!.TryGetProviderSource("sys").IsSuccess().Should().BeFalse();
    }

    [TestMethod]
     public void ProviderConfiguration_default_configuration_returns_known_built_in_providers()
    {
        var config = IConfigurationManager.GetBuiltInConfiguration();

        config.Should().NotBeNull();
        config!.ProvidersConfig.Should().NotBeNull();

        foreach (var providerName in new[] { "az", "kubernetes", "microsoftGraph" })
        {
            config.ProvidersConfig!.TryGetProviderSource(providerName).IsSuccess(out var provider).Should().BeTrue();
            provider!.BuiltIn.Should().BeTrue();
            provider.Source.Should().BeNull();
            provider.Version.Should().BeNull();
        }

        // assert that 'sys' is not present in the default configuration
        config.ProvidersConfig!.TryGetProviderSource("sys").IsSuccess().Should().BeFalse();
    }
}
