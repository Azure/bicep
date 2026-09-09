
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Extensions;
using Bicep.Core.Json;
using Bicep.Core.SemanticVersioning;
using Bicep.IO.Abstraction;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Configuration;

[TestClass]
public class BicepConfigurationTests
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
    public void Built_in_compiler_configuration_has_no_version_constraint()
    {
        BicepTestConstants.BuiltInConfiguration.Compiler.Version.Should().BeNull();
    }

    [TestMethod]
    public void Bind_and_serialize_preserve_compiler_configuration()
    {
        var element = BicepConfiguration.BuiltInConfigurationElement.Merge(
            JsonElementFactory.CreateElement("""
                {
                  "bicep": {
                    "version": "1.2.3"
                  }
                }
                """));

        var configuration = BicepConfiguration.Bind(element);

        configuration.Compiler.Version!.ToString().Should().Be(VersionRange.Parse("1.2.3").ToString());
        configuration.ToUtf8Json().Should().ContainAll(
            "\"bicep\"",
            "\"version\": \"1.2.3\"");
    }

    [DataTestMethod]
    [DataRow("1.2.3")]
    [DataRow(">=1.0.0")]
    [DataRow(">=1.0.0, <2.0.0")]
    public void Compiler_configuration_binds_valid_version_values(string version)
    {
        var configuration = CompilerConfiguration.Bind(JsonElementFactory.CreateElement($$"""{ "version": "{{version}}" }"""));

        configuration.Data.Version.Should().Be(version);
        configuration.Version!.ToString().Should().Be(VersionRange.Parse(version).ToString());
    }

    [TestMethod]
    public void Compiler_configuration_defaults_to_null_version_when_omitted()
    {
        var configuration = CompilerConfiguration.Bind(JsonElementFactory.CreateElement("{}"));

        configuration.Data.Version.Should().BeNull();
        configuration.Version.Should().BeNull();
    }

    [DataTestMethod]
    [DataRow("not-a-version")]
    [DataRow("*")]
    [DataRow("1.*")]
    [DataRow("")]
    public void Compiler_configuration_rejects_invalid_version_values(string version)
    {
        FluentActions.Invoking(() =>
                CompilerConfiguration.Bind(JsonElementFactory.CreateElement($$"""{ "version": "{{version}}" }""")))
            .Should().Throw<ConfigurationException>()
            .WithMessage("*is not a valid Bicep version or version range*");
    }

    [TestMethod]
    public void Bind_and_serialize_preserve_documentation_configuration()
    {
        var configFileUri = IOUri.FromFilePath(Path.GetFullPath("bicepconfig.json"));
        var element = BicepConfiguration.BuiltInConfigurationElement.Merge(
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

        var configuration = BicepConfiguration.Bind(element, configFileUri);

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
        var configuration = BicepTestConstants.BuiltInConfiguration.With(cacheRootDirectory: cacheRootDirectory);

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
