// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.SemanticVersioning;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.SemanticVersioning;

[TestClass]
public class VersionRangeTests
{
    [DataTestMethod]
    // Exact versions.
    [DataRow("1.2.3", "1.2.3")]
    [DataRow("=0.31.0", "0.31.0")]
    // An exact version is held as a pair of inclusive bounds, so the two spellings normalize to the same constraint.
    [DataRow(">=1.2.3, <=1.2.3", "1.2.3")]
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
    // A range must pair at most one lower bound with one upper bound.
    [DataRow(">=1.0.0, >=2.0.0")]
    [DataRow("<1.0.0, <2.0.0")]
    [DataRow("=1.0.0, <2.0.0")]
    [DataRow("1.0.0, 2.0.0")]
    // A leading "v" is not part of the constraint syntax, even though release tags carry one.
    [DataRow("v1.2.3")]
    [DataRow(">=v1.0.0, <v2.0.0")]
    // Prerelease labels and build metadata are not supported.
    [DataRow("1.2.3-preview")]
    [DataRow(">=1.0.0-rc.1")]
    [DataRow("1.2.3+abc123")]
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
    [DataRow(">=1", "1.0.0", true)]
    [DataRow(">=1", "0.9.9", false)]
    // Omitting the patch component pins that exact version; it does not match the whole minor version line.
    [DataRow("1.2", "1.2.0", true)]
    [DataRow("1.2", "1.2.5", false)]
    [DataRow(">=1.2", "1.2.0", true)]
    // Ranges.
    [DataRow(">=0.31.0, <1.0.0", "0.31.0", true)]
    [DataRow(">=0.31.0, <1.0.0", "0.99.99", true)]
    [DataRow(">=0.31.0, <1.0.0", "1.0.0", false)]
    [DataRow(">=0.31.0, <1.0.0", "0.30.0", false)]
    [DataRow(">=0.31.0", "0.47.12", true)]
    public void Satisfies_ReturnsExpectedResult(string constraint, string version, bool expected)
    {
        var range = VersionRange.Parse(constraint);

        range.IsSatisfiedBy(SemanticVersion.Parse(version)).Should().Be(expected);
    }

    [DataTestMethod]
    [DataRow("1.0.0", true)]
    [DataRow(">=1.0.0", true)]
    [DataRow(">=1.0.0, <2.0.0", true)]
    [DataRow(">=1.0.0, <=1.0.0", true)]
    [DataRow(">=2.0.0, <1.0.0", false)]
    [DataRow(">=1.0.0, <1.0.0", false)]
    [DataRow(">1.0.0, <=1.0.0", false)]
    [DataRow(">=1.0.0, <1.0.1", true)]
    [DataRow(">1.0.0, <1.0.2", true)]
    // Exclusive bounds exactly one patch version apart admit no version, since there is nothing between them.
    [DataRow(">1.2.3, <1.2.4", false)]
    [DataRow(">1.2.4, <1.2.5", false)]
    public void IsSatisfiable_ReturnsExpectedResult(string constraint, bool expected)
    {
        VersionRange.Parse(constraint).IsSatisfiable.Should().Be(expected);
    }

    [DataTestMethod]
    [DataRow("1.2.3", true)]
    [DataRow("=1.2.3", true)]
    [DataRow(">=1.2.3, <=1.2.3", true)]
    [DataRow(">=1.2.3", false)]
    [DataRow("<=1.2.3", false)]
    [DataRow(">=1.0.0, <2.0.0", false)]
    [DataRow(">1.2.3, <1.2.3", false)]
    public void IsExactVersion_ReturnsExpectedResult(string constraint, bool expected)
    {
        VersionRange.Parse(constraint).IsExactVersion.Should().Be(expected);
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
    public void LowerBoundAndUpperBound_AreBothSetForAnExactVersion()
    {
        var range = VersionRange.Parse("0.31.0");

        range.LowerBound.Should().NotBeNull();
        range.LowerBound!.ToString().Should().Be(">=0.31.0");
        range.UpperBound.Should().NotBeNull();
        range.UpperBound!.ToString().Should().Be("<=0.31.0");
    }

    [TestMethod]
    public void LowerBoundAndUpperBound_AreNullWhenTheConstraintIsOpenEnded()
    {
        var range = VersionRange.Parse(">=0.31.0");

        range.LowerBound.Should().NotBeNull();
        range.UpperBound.Should().BeNull();
    }

    [DataTestMethod]
    // Fully open ranges always overlap.
    [DataRow(">=0.31.0", ">=0.15.0", true, ">=0.31.0")]
    [DataRow(">=0.31.0", "<1.0.0", true, ">=0.31.0, <1.0.0")]
    // Overlapping ranges combine to the narrower of each bound.
    [DataRow(">=0.31.0, <1.0.0", ">=0.50.0, <2.0.0", true, ">=0.50.0, <1.0.0")]
    [DataRow(">=0.31.0, <0.40.0", ">=0.31.0, <0.40.0", true, ">=0.31.0, <0.40.0")]
    // Disjoint ranges 
    [DataRow(">=0.31.0, <0.32.0", ">=0.15.0, <0.16.0", false, null)]
    [DataRow(">=1.0.0", "<0.9.0", false, null)]
    // Touching bounds where at least one side is exclusive admit no version.
    [DataRow(">=1.0.0", "<1.0.0", false, null)]
    [DataRow(">1.0.0", "<=1.0.0", false, null)]
    // Touching inclusive bounds admit exactly the shared version.
    [DataRow(">=1.0.0", "<=1.0.0", true, "1.0.0")]
    // Exact versions.
    [DataRow("1.2.3", "1.2.3", true, "1.2.3")]
    [DataRow("1.2.3", "1.2.4", false, null)]
    public void TryGetOverlap_ReturnsExpectedResult(string first, string second, bool expectedResult, string? expectedOverlap)
    {
        var result = VersionRange.Parse(first).TryGetOverlap(VersionRange.Parse(second), out var overlap);

        result.Should().Be(expectedResult);
        if (expectedResult)
        {
            overlap!.ToString().Should().Be(expectedOverlap);
        }
        else
        {
            overlap.Should().BeNull();
        }
    }

    [TestMethod]
    public void TryGetOverlap_IsSymmetric()
    {
        var a = VersionRange.Parse(">=0.31.0, <1.0.0");
        var b = VersionRange.Parse(">=0.50.0, <2.0.0");

        a.TryGetOverlap(b, out var ab).Should().BeTrue();
        b.TryGetOverlap(a, out var ba).Should().BeTrue();

        ab!.ToString().Should().Be(ba!.ToString());
    }

    [DataTestMethod]
    // A range is always a subset of itself and of a fully open range.
    [DataRow(">=0.31.0, <1.0.0", ">=0.31.0, <1.0.0", true)]
    [DataRow(">=0.31.0, <1.0.0", ">=0.15.0", true)]
    // The "looser entrypoint" example: an unbounded-above entrypoint constraint is not a subset of a
    // referenced file's upper-bounded constraint, since it admits versions the reference would reject.
    [DataRow(">=0.15.0", "<0.17.0", false)]
    // A narrower range is a subset of a wider range, but not vice versa.
    [DataRow(">=0.50.0, <0.60.0", ">=0.31.0, <1.0.0", true)]
    [DataRow(">=0.31.0, <1.0.0", ">=0.50.0, <0.60.0", false)]
    // Exclusive bounds are a subset of the equivalent inclusive bound, but not the reverse.
    [DataRow(">1.0.0", ">=1.0.0", true)]
    [DataRow(">=1.0.0", ">1.0.0", false)]
    public void IsSubsetOf_ReturnsExpectedResult(string first, string second, bool expected)
    {
        VersionRange.Parse(first).IsSubsetOf(VersionRange.Parse(second)).Should().Be(expected);
    }
}
