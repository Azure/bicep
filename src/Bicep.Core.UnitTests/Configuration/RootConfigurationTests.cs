// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
                BicepTestConstants.BuiltInConfiguration.Analyzers,
                cacheRootDirectory,
                BicepTestConstants.BuiltInConfiguration.ExperimentalFeaturesEnabled,
                BicepTestConstants.BuiltInConfiguration.Formatting,
                BicepTestConstants.BuiltInConfiguration.ConfigurationPath,
                BicepTestConstants.BuiltInConfiguration.DiagnosticBuilders);

        configuration.CacheRootDirectory.Should().Be(expectedExpandedDirectory);
    }

    private static IEnumerable<object[]> GetTestData()
    {
        var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        return new[]
        {
            new object[] { "~", homeDirectory },
            new object[] { "~/foo/bar", $"{homeDirectory}/foo/bar" },
            new object[] { "~\\foo/bar", $"{homeDirectory}\\foo/bar" },
        };
    }
}
