// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.SemanticVersioning;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.SemanticVersioning;

[TestClass]
public class SemanticVersionTests
{
    [DataTestMethod]
    [DataRow("1.2.3", 1, 2, 3)]
    [DataRow("0.0.0", 0, 0, 0)]
    [DataRow("0.47.12", 0, 47, 12)]
    // Omitted components default to zero.
    [DataRow("1.2", 1, 2, 0)]
    [DataRow("1", 1, 0, 0)]
    [DataRow("0", 0, 0, 0)]
    // Each component may be any non-negative number that fits in an int.
    [DataRow("10.200.3000", 10, 200, 3000)]
    [DataRow("2147483647.0.0", 2147483647, 0, 0)]
    public void TryParse_ValidVersion_ReturnsTrueAndParsedComponents(string value, int major, int minor, int patch)
    {
        var result = SemanticVersion.TryParse(value, out var version);

        result.Should().BeTrue();
        version.Should().NotBeNull();
        version!.Major.Should().Be(major);
        version.Minor.Should().Be(minor);
        version.Patch.Should().Be(patch);
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("whatever")]
    // Whitespace is trimmed by the comparator, not by the version itself.
    [DataRow(" 1.2.3")]
    [DataRow("1.2.3 ")]
    // Leading zeros are not permitted.
    [DataRow("01.2.3")]
    [DataRow("1.02.3")]
    [DataRow("1.2.03")]
    // Malformed component lists.
    [DataRow("1.2.3.4")]
    [DataRow("1.")]
    [DataRow("1..2")]
    [DataRow(".1.2")]
    [DataRow("1.2.")]
    [DataRow("-1.2.3")]
    [DataRow("+1.2.3")]
    // Prerelease labels and build metadata are not supported.
    [DataRow("1.2.3-preview")]
    [DataRow("1.2.3-rc.1")]
    [DataRow("1.2.3+abc123")]
    [DataRow("1.2.3-rc.1+abc123")]
    // Release tags carry a leading "v", but the version syntax does not.
    [DataRow("v1.2.3")]
    [DataRow("V1.2.3")]
    // A component too large to represent is rejected rather than silently wrapping.
    [DataRow("2147483648.0.0")]
    [DataRow("99999999999999999999.0.0")]
    // Only ASCII digits are accepted; "\d" would otherwise match these and differ between implementations.
    [DataRow("١.٢.٣")]
    public void TryParse_InvalidVersion_ReturnsFalseAndNull(string value)
    {
        var result = SemanticVersion.TryParse(value, out var version);

        result.Should().BeFalse();
        version.Should().BeNull();
    }

    [DataTestMethod]
    [DataRow("1.2.3", "1.2.3", 0)]
    // Major takes precedence over minor and patch.
    [DataRow("2.0.0", "1.99.99", 1)]
    [DataRow("1.99.99", "2.0.0", -1)]
    // Then minor.
    [DataRow("1.3.0", "1.2.99", 1)]
    [DataRow("1.2.99", "1.3.0", -1)]
    // Then patch.
    [DataRow("1.2.4", "1.2.3", 1)]
    [DataRow("1.2.3", "1.2.4", -1)]
    // Components are compared as numbers, not as text.
    [DataRow("1.10.0", "1.9.0", 1)]
    [DataRow("0.47.12", "0.47.2", 1)]
    public void CompareTo_OrdersByMajorThenMinorThenPatch(string left, string right, int expectedSign)
    {
        var comparison = SemanticVersion.Parse(left).CompareTo(SemanticVersion.Parse(right));

        Math.Sign(comparison).Should().Be(expectedSign);
    }

    [TestMethod]
    public void CompareTo_Null_ReturnsPositive()
    {
        SemanticVersion.Parse("1.2.3").CompareTo(null).Should().BePositive();
    }

    [DataTestMethod]
    [DataRow("1.2.3", "1.2.3")]
    [DataRow("1.2", "1.2.0")]
    [DataRow("1", "1.0.0")]
    public void ToString_RendersAllThreeComponents(string value, string expected)
    {
        SemanticVersion.Parse(value).ToString().Should().Be(expected);
    }

    [TestMethod]
    public void Equality_ComparesComponents()
    {
        SemanticVersion.Parse("1.2.3").Should().Be(SemanticVersion.Parse("1.2.3"));
        SemanticVersion.Parse("1.2").Should().Be(SemanticVersion.Parse("1.2.0"));
        SemanticVersion.Parse("1.2.3").Should().NotBe(SemanticVersion.Parse("1.2.4"));
    }

    [TestMethod]
    public void Parse_InvalidVersion_ThrowsFormatException()
    {
        var action = () => SemanticVersion.Parse("not-a-version");

        action.Should().Throw<FormatException>().WithMessage("The provided value 'not-a-version' is not a valid version.");
    }
}
