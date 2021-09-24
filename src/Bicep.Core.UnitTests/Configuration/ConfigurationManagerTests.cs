// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using IOFileSystem = System.IO.Abstractions.FileSystem;

namespace Bicep.Core.UnitTests.Configuration
{
    [TestClass]
    public class ConfigurationManagerTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public void GetBuiltInConfiguration_NoParameter_ReturnsBuiltInConfigurationWithAnalyzerSettings()
        {
            // Arrange.
            var sut = new ConfigurationManager(new IOFileSystem());

            // Act.
            var configuration = sut.GetBuiltInConfiguration();

            // Assert.
            configuration.Cloud.CurrentProfile.Should().NotBeNull();
            configuration.Cloud.CurrentProfile!.ResourceManagerEndpoint.Should().Be("https://management.azure.com");

            configuration.ModuleAliases.TemplateSpecModuleAliases.Should().BeEmpty();
            configuration.ModuleAliases.BicepRegistryModuleAliases.Should().BeEmpty();

            configuration.Analyzers.GetValue("core:verbose", true).Should().BeFalse();
            configuration.Analyzers.GetValue("core:enabled", false).Should().BeTrue();
            configuration.Analyzers.GetValue("core:rules:no-hardcoded-env-urls:level", "").Should().Be("warning");
            configuration.Analyzers.GetValue("core:rules:no-hardcoded-env-urls:disallowedhosts", Array.Empty<string>()).Should().NotBeEmpty();
            configuration.Analyzers.GetValue("core:rules:no-hardcoded-env-urls:excludedhosts", Array.Empty<string>()).Should().NotBeEmpty();
        }

        [TestMethod]
        public void GetBuiltInConfiguration_DisableAnalyzers_ReturnsBuiltInConfigurationWithoutAnalyzerSettings()
        {
            // Arrange.
            var sut = new ConfigurationManager(new IOFileSystem());

            // Act.
            var configuration = sut.GetBuiltInConfiguration(disableAnalyzers: true);

            // Assert.
            configuration.Cloud.CurrentProfile.Should().NotBeNull();
            configuration.Cloud.CurrentProfile!.ResourceManagerEndpoint.Should().Be("https://management.azure.com");

            configuration.ModuleAliases.TemplateSpecModuleAliases.Should().BeEmpty();
            configuration.ModuleAliases.BicepRegistryModuleAliases.Should().BeEmpty();

            configuration.Analyzers.GetValue("core:verbose", true).Should().BeTrue();
            configuration.Analyzers.GetValue("core:enabled", false).Should().BeFalse();
            configuration.Analyzers.GetValue("core:rules:no-hardcoded-env-urls:level", "").Should().BeEmpty();
            configuration.Analyzers.GetValue("core:rules:no-hardcoded-env-urls:disallowedhosts", Array.Empty<string>()).Should().BeEmpty();
            configuration.Analyzers.GetValue("core:rules:no-hardcoded-env-urls:excludedhosts", Array.Empty<string>()).Should().BeEmpty();
        }

        [TestMethod]
        public void GetConfiguration_CustomConfigurationNotFound_ReturnsBuiltInConfiguration()
        {
            // Arrange.
            var sut = new ConfigurationManager(new IOFileSystem());
            var sourceFileUri = new Uri(this.CreatePath("foo/bar/main.bicep"));

            // Act.
            var configuration = sut.GetConfiguration(sourceFileUri);

            // Assert.
            configuration.Should().Be(sut.GetBuiltInConfiguration());
        }

        [TestMethod]
        public void GetConfiguration_InvalidCustomConfiguration_ThrowsInvalidConfigurationException()
        {
            // Arrange.
            var configurationFilePath = CreatePath("path/to/bicepconfig.json");
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                [configurationFilePath] = "",
            });

            var sut = new ConfigurationManager(fileSystem);
            var sourceFileUri = new Uri(CreatePath("path/to/main.bicep"));

            // Act & Assert.
            FluentActions.Invoking(() => sut.GetConfiguration(sourceFileUri)).Should()
                .Throw<ConfigurationException>()
                .WithMessage($"Could not load the bicep configuration file \"{configurationFilePath}\". The input does not contain any JSON tokens*");
        }

        [TestMethod]
        public void GetConfiguration_ValidCustomConfiguration_OverridesBuiltInConfiguration()
        {
            // Arrange.
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                [CreatePath("repo")] = new MockDirectoryData(),
                [CreatePath("repo/modules")] = new MockDirectoryData(),
                [CreatePath("repo/bicepconfig.json")] = @"{
  ""cloud"": {
    ""currentProfile"": ""MyCloud"",
    ""profiles"": {
      ""MyCloud"": {
        ""resourceManagerEndpoint"": ""https://bicep.example.com""
      }
    }
  },
  ""moduleAliases"": {
    ""ts"": {
      ""mySpecPath"": {
        ""subscription"": ""B34C8680-F688-48C2-A44F-E1EFF5E01173""
      }
    }
  },
  ""analyzers"": {
    ""core"": {
      ""enabled"": false
    }
  }
}"
            });

            // Act.
            var sut = new ConfigurationManager(fileSystem);
            var sourceFileUri = new Uri(this.CreatePath("repo/modules/vnet.bicep"));
            var configuration = sut.GetConfiguration(sourceFileUri);

            // Assert.
            configuration.Cloud.CurrentProfile.Should().NotBeNull();
            configuration.Cloud.CurrentProfile!.ResourceManagerEndpoint.Should().Be("https://bicep.example.com");

            configuration.ModuleAliases.TemplateSpecModuleAliases.Should().NotBeEmpty();
            configuration.ModuleAliases.BicepRegistryModuleAliases.Should().BeEmpty();

            var alias = configuration.ModuleAliases.TemplateSpecModuleAliases.GetValueOrDefault("mySpecPath");
            alias.Should().NotBeNull();
            alias!.Subscription.Should().Be("B34C8680-F688-48C2-A44F-E1EFF5E01173");

            configuration.Analyzers.GetValue("core:verbose", true).Should().BeFalse();
            configuration.Analyzers.GetValue("core:enabled", true).Should().BeFalse();
            configuration.Analyzers.GetValue("core:rules:no-hardcoded-env-urls:level", "").Should().Be("warning");
            configuration.Analyzers.GetValue("core:rules:no-hardcoded-env-urls:disallowedhosts", Array.Empty<string>()).Should().NotBeEmpty();
            configuration.Analyzers.GetValue("core:rules:no-hardcoded-env-urls:excludedhosts", Array.Empty<string>()).Should().NotBeEmpty();
        }

        private string CreatePath(string path) => Path.Combine(this.TestContext.ResultsDirectory, path.Replace('/', Path.DirectorySeparatorChar));
    }
}
