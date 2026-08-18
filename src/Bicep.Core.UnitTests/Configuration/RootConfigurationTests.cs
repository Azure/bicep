// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Extensions;
using Bicep.Core.Json;
using Bicep.IO.Abstraction;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Configuration;

[TestClass]
public class RootConfigurationTests
{
    [TestMethod]
    public void Built_in_documentation_configuration_has_complete_defaults()
    {
        var documentation = BicepTestConstants.BuiltInConfiguration.Documentation.Data;

        documentation.Output.File.Should().Be("README.md");
        documentation.Template.File.Should().BeNull();
        documentation.Template.IncludeRoot.Should().BeNull();
        documentation.Template.Values.Should().BeEmpty();
        documentation.Examples.Sources.Should().HaveCount(2);
        documentation.Examples.Reassignments.Should().BeEmpty();
    }

    [TestMethod]
    public void Bind_and_serialize_preserve_documentation_configuration()
    {
        var configFileUri = IOUri.FromFilePath(Path.GetFullPath("bicepconfig.json"));
        var element = IConfigurationManager.BuiltInConfigurationElement.Merge(
            JsonElementFactory.CreateElement("""
                {
                  "documentation": {
                    "output": {
                      "file": "DOCS.md"
                    },
                    "template": {
                      "file": "templates/readme.scriban",
                      "values": {
                        "owner": "Platform"
                      }
                    },
                    "examples": {
                      "sources": []
                    }
                  }
                }
                """));

        var configuration = RootConfiguration.Bind(element, configFileUri);

        configuration.ConfigFileUri.Should().Be(configFileUri);
        configuration.Documentation.Data.Output.File.Should().Be("DOCS.md");
        configuration.Documentation.Data.Template.File.Should().Be("templates/readme.scriban");
        configuration.Documentation.Data.Template.Values.Should().Contain("owner", "Platform");
        configuration.Documentation.Data.Examples.Sources.Should().BeEmpty();
        configuration.ToUtf8Json().Should().ContainAll(
            "\"documentation\"",
            "\"file\": \"DOCS.md\"",
            "\"owner\": \"Platform\"");
    }

    [DataTestMethod]
    [DataRow("""{ "output": null }""", "output, template, and examples")]
    [DataRow("""{ "template": null }""", "output, template, and examples")]
    [DataRow("""{ "examples": null }""", "output, template, and examples")]
    [DataRow("""{ "template": { "values": null } }""", "template.values")]
    [DataRow("""{ "examples": { "sources": [{ "path": "/samples" }] } }""", "relative path")]
    [DataRow("""{ "examples": { "sources": [{ "path": "\\samples" }] } }""", "relative path")]
    [DataRow("""{ "examples": { "sources": [{ "path": "C:\\samples" }] } }""", "relative path")]
    public void Documentation_configuration_rejects_invalid_values(string json, string expectedMessage)
    {
        FluentActions.Invoking(() =>
                DocumentationConfiguration.Bind(JsonElementFactory.CreateElement(json)))
            .Should().Throw<ConfigurationException>()
            .WithMessage($"*{expectedMessage}*");
    }

    [TestMethod]
    public void Documentation_configuration_normalizes_omitted_nested_collections()
    {
        var configuration = DocumentationConfiguration.Bind(JsonElementFactory.CreateElement("""
            {
              "examples": {
                "sources": [
                  {
                    "path": "."
                  }
                ],
                "reassignments": [
                  {
                    "from": {
                      "include": ["**/*"]
                    },
                    "to": "child"
                  }
                ]
              }
            }
            """));

        configuration.Data.Examples.Sources.Single().Include.Should().BeEmpty();
        configuration.Data.Examples.Sources.Single().Exclude.Should().BeEmpty();
        configuration.Data.Examples.Reassignments.Single().From.Exclude.Should().BeEmpty();
    }

    [DataTestMethod]
    [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
    public void RootConfiguration_LeadingTildeInCacheRootDirectory_ExpandPath(string cacheRootDirectory, string expectedExpandedDirectory)
    {
        var configuration = new RootConfiguration(
                BicepTestConstants.BuiltInConfiguration.Cloud,
                BicepTestConstants.BuiltInConfiguration.ModuleAliases,
                BicepTestConstants.BuiltInConfiguration.ModuleAliasesMock,
                BicepTestConstants.BuiltInConfiguration.Extensions,
                BicepTestConstants.BuiltInConfiguration.ImplicitExtensions,
                BicepTestConstants.BuiltInConfiguration.Analyzers,
                cacheRootDirectory,
                BicepTestConstants.BuiltInConfiguration.ExperimentalFeaturesWarning,
                BicepTestConstants.BuiltInConfiguration.ExperimentalFeaturesEnabled,
                BicepTestConstants.BuiltInConfiguration.Formatting,
                BicepTestConstants.BuiltInConfiguration.Documentation,
                BicepTestConstants.BuiltInConfiguration.ConfigFileUri,
                BicepTestConstants.BuiltInConfiguration.Diagnostics);

        configuration.CacheRootDirectory.Should().Be(expectedExpandedDirectory);
    }

    private static IEnumerable<object[]> GetTestData()
    {
        var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        return new[]
        {
            new object[] { "~", homeDirectory },
            ["~/", $"{homeDirectory}/"],
            ["~\\", $"{homeDirectory}\\"],
            ["~/foo/bar", $"{homeDirectory}/foo/bar"],
            ["~\\foo\\bar", $"{homeDirectory}\\foo\\bar"],
            ["~\\foo/bar", $"{homeDirectory}\\foo/bar"],
        };
    }
}
