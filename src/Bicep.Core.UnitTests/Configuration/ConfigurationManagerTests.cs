// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.UnitTests.Mock;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
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
        public void GetConfiguration_InvalidCustomConfiguration_ThrowsFailedToParseConfigurationException()
        {
            // Arrange.
            var configurataionPath = CreatePath("path/to/bicepconfig.json");
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                [configurataionPath] = "",
            });

            var sut = new ConfigurationManager(fileSystem);
            var sourceFileUri = new Uri(CreatePath("path/to/main.bicep"));

            // Act & Assert.
            FluentActions.Invoking(() => sut.GetConfiguration(sourceFileUri)).Should()
                .Throw<ConfigurationException>()
                .WithMessage($"Failed to parse the contents of the Bicep configuration file \"{configurataionPath}\" as valid JSON: \"The input does not contain any JSON tokens. Expected the input to start with a valid JSON token, when isFinalBlock is true. LineNumber: 0 | BytePositionInLine: 0.\".");
        }

        [TestMethod]
        public void GetConfiguration_ConfigurationFileNotReadable_ThrowsCouldNotLoadConfigurationException()
        {
            // Arrange.
            var configurataionPath = CreatePath("path/to/bicepconfig.json");
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                [configurataionPath] = "",
            });

            var fileSystemMock = StrictMock.Of<IFileSystem>();
            fileSystemMock.SetupGet(x => x.Path).Returns(fileSystem.Path);
            fileSystemMock.SetupGet(x => x.Directory).Returns(fileSystem.Directory);
            fileSystemMock.SetupGet(x => x.File).Returns(fileSystem.File);
            fileSystemMock.Setup(x => x.FileStream.Create(It.IsAny<string>(), It.IsAny<FileMode>(), It.IsAny<FileAccess>()))
                .Throws(new UnauthorizedAccessException("Not allowed."));

            var sut = new ConfigurationManager(fileSystemMock.Object);
            var sourceFileUri = new Uri(CreatePath("path/to/main.bicep"));

            // Act & Assert.
            FluentActions.Invoking(() => sut.GetConfiguration(sourceFileUri)).Should()
                .Throw<ConfigurationException>()
                .WithMessage($"Could not load the Bicep configuration file \"{configurataionPath}\": \"Not allowed.\".");
        }

        [TestMethod]
        public void GetConfiguration_IOExceptionWhenDiscovringConfiguration_ReturnsDefaultConfiguration()
        {
            // Arrange.
            var fileSystemMock = StrictMock.Of<IFileSystem>();
            fileSystemMock.Setup(x => x.Path.GetDirectoryName(It.IsAny<string>())).Returns("foo");
            fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), It.IsAny<string>())).Returns("");
            fileSystemMock.Setup(x => x.File.Exists(It.IsAny<string>())).Returns(false);
            fileSystemMock.Setup(x => x.Directory.GetParent(It.IsAny<string>())).Throws(new IOException("Oops."));

            var sut = new ConfigurationManager(fileSystemMock.Object);
            var sourceFileUri = new Uri(CreatePath("path/to/main.bicep"));
            var configuration = sut.GetConfiguration(sourceFileUri);

            // Act & Assert.
            configuration.Should().Be(sut.GetBuiltInConfiguration());
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
