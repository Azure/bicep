// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions.TestingHelpers;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.UnitTests.Assertions;
using Bicep.IO.FileSystem;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Features;

[TestClass]
public class FeatureProviderTests
{
    [NotNull]
    public TestContext? TestContext { get; set; }

    [TestMethod]
    public void PropertyLookup_WithNothingConfigured_ReturnsDefault()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            [CreatePath("repo")] = new MockDirectoryData(),
            [CreatePath("repo/bicepconfig.json")] = @"{""experimentalFeaturesEnabled"": {}}",
        });

        var fileExplorer = new FileSystemFileExplorer(fileSystem);
        var configManager = new ConfigurationManager(fileExplorer);
        var configuration = configManager.GetConfiguration(new Uri(this.CreatePath("repo/main.bicep")));
        var fpm = new FeatureProviderFactory(configManager, fileExplorer);

        var control = fpm.GetFeatureProvider(new Uri("file:///main.bicep"));
        var sut = fpm.GetFeatureProvider(new Uri(this.CreatePath("repo/main.bicep")));
        sut.SymbolicNameCodegenEnabled.Should().Be(control.SymbolicNameCodegenEnabled);
    }

    [TestMethod]
    public void PropertyLookup_WithFeatureEnabledViaBicepConfig_ReturnsTrue()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            [CreatePath("repo")] = new MockDirectoryData(),
            [CreatePath("repo/bicepconfig.json")] = @"{""experimentalFeaturesEnabled"": {}}",
            [CreatePath("repo/subdir")] = new MockDirectoryData(),
            [CreatePath("repo/subdir/bicepconfig.json")] = @"{""experimentalFeaturesEnabled"": {""symbolicNameCodegen"": true}}",
        });
        var fileExplorer = new FileSystemFileExplorer(fileSystem);
        var configManager = new ConfigurationManager(fileExplorer);
        var configuration = configManager.GetConfiguration(new Uri(this.CreatePath("repo/main.bicep")));
        var fpm = new FeatureProviderFactory(configManager, fileExplorer);

        var control = fpm.GetFeatureProvider(new Uri("file:///main.bicep"));
        control.SymbolicNameCodegenEnabled.Should().BeFalse();
        var mainDirFeatures = fpm.GetFeatureProvider(new Uri(this.CreatePath("repo/main.bicep")));
        mainDirFeatures.SymbolicNameCodegenEnabled.Should().BeFalse();
        var subDirFeatures = fpm.GetFeatureProvider(new Uri(this.CreatePath("repo/subdir/module.bicep")));
        subDirFeatures.SymbolicNameCodegenEnabled.Should().BeTrue();
    }

    private string CreatePath(string path) => Path.Combine(this.TestContext.ResultsDirectory!, path.Replace('/', Path.DirectorySeparatorChar));
}
