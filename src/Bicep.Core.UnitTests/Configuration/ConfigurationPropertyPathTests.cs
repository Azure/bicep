// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Configuration;

[TestClass]
public class ConfigurationPropertyPathTests
{
    [TestMethod]
    [DataRow("$.version", new[] { "version" })]
    [DataRow("$['my.cloud'].resourceManagerEndpoint", new[] { "my.cloud", "resourceManagerEndpoint" })]
    [DataRow("$.sources[0].path", new[] { "sources" })]
    [DataRow("$", new string[0])]
    [DataRow(null, new string[0])]
    [DataRow("not a path", new string[0])]
    public void FromJsonPath_ReturnsPropertyNames(string? jsonPath, string[] expectedNames)
    {
        ConfigurationPropertyPath.FromJsonPath(jsonPath).Names.Should().Equal(expectedNames);
    }

    [TestMethod]
    public void Paths_CompareByValue()
    {
        var path = ConfigurationPropertyPath.Create("cloud", "profiles");

        path.Should().Be(ConfigurationPropertyPath.Root.Append("cloud").Append("profiles"));
        path.GetHashCode().Should().Be(ConfigurationPropertyPath.Create("cloud", "profiles").GetHashCode());
        path.Should().NotBe(ConfigurationPropertyPath.Create("cloud.profiles"));
        default(ConfigurationPropertyPath).Should().Be(ConfigurationPropertyPath.Create());
    }

    [TestMethod]
    public void IsDescendantOf_MatchesWholeNames()
    {
        var path = ConfigurationPropertyPath.Create("cloud", "profiles", "x");

        path.IsDescendantOf(ConfigurationPropertyPath.Create("cloud")).Should().BeTrue();
        path.IsDescendantOf(ConfigurationPropertyPath.Root).Should().BeTrue();
        path.IsDescendantOf(path).Should().BeFalse();
        path.IsDescendantOf(ConfigurationPropertyPath.Create("cloud", "prof")).Should().BeFalse();
    }
}
