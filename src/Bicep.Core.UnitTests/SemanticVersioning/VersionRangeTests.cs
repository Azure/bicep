// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.SemanticVersioning;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Semver;

namespace Bicep.Core.UnitTests.SemanticVersioning;

[TestClass]
public class VersionRangeTests
{
    [DataTestMethod]
    // Exact versions.
    [DataRow("1.2.3", "1.2.3")]
    [DataRow("=0.31.0", "0.31.0")]
    [DataRow("v1.2.3", "1.2.3")]
    [DataRow("1.2.3-preview", "1.2.3-preview")]
    // Single comparators.
    [DataRow(">=0.31.0", ">=0.31.0")]
    [DataRow("<0.31.0", "<0.31.0")]
    [DataRow(">0.31.0", ">0.31.0")]
    [DataRow("<=0.31.0", "<=0.31.0")]
    // Omitted minor and patch components are filled in with zeroes.
    [DataRow(">=1.2", ">=1.2.0")]
    [DataRow(">=1", ">=1.0.0")]
    // Ranges, including whitespace handling and bound reordering.
    [DataRow(">=0.31.0, <1.0.0", ">=0.31.0, <1.0.0")]
    [DataRow(">=0.31.0,<1.0.0", ">=0.31.0, <1.0.0")]
    [DataRow("   >=0.31.0   ,   <1.0.0   ", ">=0.31.0, <1.0.0")]
    [DataRow("<1.0.0, >=0.31.0", ">=0.31.0, <1.0.0")]
    [DataRow(">0.31.0, <=1.0.0", ">0.31.0, <=1.0.0")]
    public void TryParse_ValidConstraint_ReturnsTrueAndNormalizedRange(string value, string expected)
    {
        var result = VersionRange.TryParse(value, out var range);

        result.Should().BeTrue();
        range.Should().NotBeNull();
        range!.ToString().Should().Be(expected);
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("whatever")]
    [DataRow(">=")]
    // More than two comparators are not supported.
    [DataRow(">=1.0.0, <2.0.0, <3.0.0")]
    // Empty comparators.
    [DataRow(">=1.0.0,")]
    [DataRow(",<2.0.0")]
    [DataRow(",")]
    // A range must pair exactly one lower bound with one upper bound.
    [DataRow(">=1.0.0, >=2.0.0")]
    [DataRow("<1.0.0, <2.0.0")]
    [DataRow("=1.0.0, <2.0.0")]
    [DataRow("1.0.0, 2.0.0")]
    // Range syntax is not wrapped in parentheses when used in bicepconfig.json.
    [DataRow("(>=1.0.0, <2.0.0)")]
    // npm-style advanced range syntax is intentionally unsupported.
    [DataRow("^1.0.0")]
    [DataRow("~1.0.0")]
    [DataRow("1.x")]
    public void TryParse_InvalidConstraint_ReturnsFalseAndNull(string value)
    {
        var result = VersionRange.TryParse(value, out var range);

        result.Should().BeFalse();
        range.Should().BeNull();
    }

    [DataTestMethod]
    [DataRow(">=0.31.0", "0.31.0", true)]
    [DataRow(">=0.31.0", "0.31.1", true)]
    [DataRow(">=0.31.0", "1.0.0", true)]
    [DataRow(">=0.31.0", "0.30.9", false)]
    [DataRow(">0.31.0", "0.31.0", false)]
    [DataRow("<0.31.0", "0.30.0", true)]
    [DataRow("<0.31.0", "0.31.0", false)]
    [DataRow("<=0.31.0", "0.31.0", true)]
    [DataRow("0.31.0", "0.31.0", true)]
    [DataRow("0.31.0", "0.31.1", false)]
    [DataRow("=0.31.0", "0.31.0", true)]
    [DataRow(">=1.2", "1.2.0", true)]
    [DataRow(">=1", "1.0.0", true)]
    [DataRow(">=1", "0.9.9", false)]
    // Ranges.
    [DataRow(">=0.31.0, <1.0.0", "0.31.0", true)]
    [DataRow(">=0.31.0, <1.0.0", "0.99.99", true)]
    [DataRow(">=0.31.0, <1.0.0", "1.0.0", false)]
    [DataRow(">=0.31.0, <1.0.0", "0.30.0", false)]
    // A prerelease sorts below its own release.
    [DataRow(">=1.0.0", "1.0.0-preview", false)]
    [DataRow("<1.0.0", "1.0.0-preview", true)]
    [DataRow(">=1.0.0-alpha, <2.0.0", "1.0.0-beta", true)]
    // Build metadata does not affect precedence.
    [DataRow("=1.0.0", "1.0.0+abc123", true)]
    [DataRow(">=1.0.0", "1.0.0+abc123", true)]
    public void Satisfies_ReturnsExpectedResult(string constraint, string version, bool expected)
    {
        var range = VersionRange.Parse(constraint);

        range.Satisfies(SemVersion.Parse(version, SemVersionStyles.Strict)).Should().Be(expected);
    }

    [DataTestMethod]
    [DataRow("1.0.0", true)]
    [DataRow(">=1.0.0", true)]
    [DataRow(">=1.0.0, <2.0.0", true)]
    [DataRow(">=1.0.0, <=1.0.0", true)]
    [DataRow(">=2.0.0, <1.0.0", false)]
    [DataRow(">=1.0.0, <1.0.0", false)]
    [DataRow(">1.0.0, <=1.0.0", false)]
    public void IsSatisfiable_ReturnsExpectedResult(string constraint, bool expected)
    {
        VersionRange.Parse(constraint).IsSatisfiable.Should().Be(expected);
    }

    [TestMethod]
    public void Parse_InvalidConstraint_ThrowsFormatException()
    {
        var action = () => VersionRange.Parse("not-a-version");

        action.Should().Throw<FormatException>().WithMessage("The provided value 'not-a-version' is not a valid version constraint.");
    }

    [TestMethod]
    public void LowerBoundAndUpperBound_ExposeTheCorrespondingComparators()
    {
        var range = VersionRange.Parse(">=0.31.0, <1.0.0");

        range.LowerBound.Should().NotBeNull();
        range.LowerBound!.ToString().Should().Be(">=0.31.0");
        range.UpperBound.Should().NotBeNull();
        range.UpperBound!.ToString().Should().Be("<1.0.0");
    }

    [TestMethod]
    public void LowerBoundAndUpperBound_AreNullForAnExactVersion()
    {
        var range = VersionRange.Parse("0.31.0");

        range.LowerBound.Should().BeNull();
        range.UpperBound.Should().BeNull();
    }
}
