// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bicep.Core.UnitTests.Features;

[TestClass]
public class FeatureProviderTests
{
    [NotNull]
    public TestContext? TestContext { get; set; }

    [TestMethod]
    public void PropertyLookup_FallsBackToDefaultIfNoSourcesSupplyIt()
    {
        var sources = Enumerable.Range(0, 3).Select(i => {
            var mock = new Mock<IFeatureProviderSource>(MockBehavior.Strict);
            mock.Setup(s => s.Priority).Returns((sbyte) i);
            mock.Setup(s => s.ImportsEnabled).Returns(new Nullable<bool>());
            return mock;
        }).ToArray();
        var defaults = new Mock<IFeatureProvider>(MockBehavior.Strict);
        defaults.Setup(d => d.ImportsEnabled).Returns(true);

        var sut = new FeatureProvider(defaults.Object, sources.Select(s => s.Object));
        sut.ImportsEnabled.Should().BeTrue();

        foreach (var source in sources)
        {
            source.Verify(s => s.ImportsEnabled, Times.Once);
        }
    }

    [TestMethod]
    public void PropertyLookup_PollsSourcesInPriorityOrder()
    {
        var sources = Enumerable.Range(1, 3).Select(i => {
            var mock = new Mock<IFeatureProviderSource>(MockBehavior.Strict);
            mock.Setup(s => s.Priority).Returns((sbyte) -i);
            mock.Setup(s => s.ImportsEnabled).Returns(i == 3 ? new Nullable<bool>(true) : new Nullable<bool>(false));
            return mock;
        }).ToArray();
        var defaults = new Mock<IFeatureProvider>(MockBehavior.Strict);

        var sut = new FeatureProvider(defaults.Object, sources.Select(s => s.Object));
        sut.ImportsEnabled.Should().BeTrue();

        defaults.Verify(d => d.ImportsEnabled, Times.Never);
        sources[^1].Verify(s => s.ImportsEnabled, Times.Once);
        foreach(var source in sources[..^1])
        {
            source.Verify(s => s.ImportsEnabled, Times.Never);
        }
    }

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
