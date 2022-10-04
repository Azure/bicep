// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.UnitTests.Assertions;
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
        var configManager = new ConfigurationManager(fileSystem);
        var configuration = configManager.GetConfiguration(new Uri(this.CreatePath("repo/main.bicep")));
        var fpm = new FeatureProviderFactory(configManager);

        var control = fpm.GetFeatureProvider(new Uri("inmemory:///main.bicp"));
        var sut = fpm.GetFeatureProvider(new Uri(this.CreatePath("repo/main.bicep")));
        sut.ImportsEnabled.Should().Be(control.ImportsEnabled);
    }

    [TestMethod]
    public void PropertyLookup_WithFeatureEnabledViaBicepConfig_ReturnsTrue()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            [CreatePath("repo")] = new MockDirectoryData(),
            [CreatePath("repo/bicepconfig.json")] = @"{""experimentalFeaturesEnabled"": {}}",
            [CreatePath("repo/subdir")] = new MockDirectoryData(),
            [CreatePath("repo/subdir/bicepconfig.json")] = @"{""experimentalFeaturesEnabled"": {""imports"": true}}",
        });
        var configManager = new ConfigurationManager(fileSystem);
        var configuration = configManager.GetConfiguration(new Uri(this.CreatePath("repo/main.bicep")));
        var fpm = new FeatureProviderFactory(configManager);

        var control = fpm.GetFeatureProvider(new Uri("inmemory:///main.bicp"));
        control.ImportsEnabled.Should().BeFalse();
        var mainDirFeatures = fpm.GetFeatureProvider(new Uri(this.CreatePath("repo/main.bicep")));
        mainDirFeatures.ImportsEnabled.Should().BeFalse();
        var subDirFeatures = fpm.GetFeatureProvider(new Uri(this.CreatePath("repo/subdir/module.bicep")));
        subDirFeatures.ImportsEnabled.Should().BeTrue();
    }

    private string CreatePath(string path) => Path.Combine(this.TestContext.ResultsDirectory, path.Replace('/', Path.DirectorySeparatorChar));
}
