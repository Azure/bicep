// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions.TestingHelpers;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.UnitTests.Assertions;
using Bicep.IO.FileSystem;
using Bicep.TextFixtures.IO;
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
        var fileSet = InMemoryTestFileSet.Create(("repo/bicepconfig.json", """
            {
              "experimentalFeaturesEnabled": {}
            }
            """));

        var configManager = new ConfigurationManager(fileSet.FileExplorer);
        var configuration = configManager.GetConfiguration(fileSet.GetUri("repo/main.bicep"));
        var fpm = new FeatureProviderFactory(configManager, fileSet.FileExplorer);

        var control = fpm.GetFeatureProvider(fileSet.GetUri("main.bicep"));
        var sut = fpm.GetFeatureProvider(fileSet.GetUri("repo/main.bicep"));
        sut.SymbolicNameCodegenEnabled.Should().Be(control.SymbolicNameCodegenEnabled);
    }

    [TestMethod]
    public void PropertyLookup_WithFeatureEnabledViaBicepConfig_ReturnsTrue()
    {
        var fileSet = InMemoryTestFileSet.Create(
            ("repo/bicepconfig.json", """
                {
                  "experimentalFeaturesEnabled": {}
                }
                """),
            ("repo/subdir/bicepconfig.json", """
                {
                  "experimentalFeaturesEnabled": {
                    "symbolicNameCodegen": true
                  }
                }
                """));

        var configManager = new ConfigurationManager(fileSet.FileExplorer);
        var configuration = configManager.GetConfiguration(fileSet.GetUri("repo/main.bicep"));
        var fpm = new FeatureProviderFactory(configManager, fileSet.FileExplorer);

        var control = fpm.GetFeatureProvider(fileSet.GetUri("main.bicep"));
        control.SymbolicNameCodegenEnabled.Should().BeFalse();
        var mainDirFeatures = fpm.GetFeatureProvider(fileSet.GetUri("repo/main.bicep"));
        mainDirFeatures.SymbolicNameCodegenEnabled.Should().BeFalse();
        var subDirFeatures = fpm.GetFeatureProvider(fileSet.GetUri("repo/subdir/module.bicep"));
        subDirFeatures.SymbolicNameCodegenEnabled.Should().BeTrue();
    }

    [TestMethod]
    public void OciEnabled_ShouldHonorEnvironmentVariable()
    {
        var original = Environment.GetEnvironmentVariable("BICEP_EXPERIMENTAL_OCI");

        try
        {
            Environment.SetEnvironmentVariable("BICEP_EXPERIMENTAL_OCI", "1");

            var fileSet = InMemoryTestFileSet.Create(("repo/bicepconfig.json", """
                {
                  "experimentalFeaturesEnabled": {}
                }
                """));

            var configManager = new ConfigurationManager(fileSet.FileExplorer);
            var featureProvider = new FeatureProvider(configManager.GetConfiguration(fileSet.GetUri("repo/main.bicep")), fileSet.FileExplorer);

            featureProvider.OciEnabled.Should().BeTrue();
        }
        finally
        {
            Environment.SetEnvironmentVariable("BICEP_EXPERIMENTAL_OCI", original);
        }
    }
}
