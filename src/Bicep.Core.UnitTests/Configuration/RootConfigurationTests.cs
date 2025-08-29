// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Configuration;

[TestClass]
public class RootConfigurationTests
{
    [DataTestMethod]
    [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
    public void RootConfiguration_LeadingTildeInCacheRootDirectory_ExpandPath(string cacheRootDirectory, string expectedExpandedDirectory)
    {
        var configuration = new RootConfiguration(
                BicepTestConstants.BuiltInConfiguration.Cloud,
                BicepTestConstants.BuiltInConfiguration.ModuleAliases,
                BicepTestConstants.BuiltInConfiguration.Extensions,
                BicepTestConstants.BuiltInConfiguration.ImplicitExtensions,
                BicepTestConstants.BuiltInConfiguration.Analyzers,
                cacheRootDirectory,
                BicepTestConstants.BuiltInConfiguration.ExperimentalFeaturesWarning,
                BicepTestConstants.BuiltInConfiguration.ExperimentalFeaturesEnabled,
                BicepTestConstants.BuiltInConfiguration.Formatting,
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
