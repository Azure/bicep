// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RpcClient.SemanticVersioning;
using FluentAssertions;

namespace Bicep.RpcClient.Tests.SemanticVersioning;

[TestClass]
public class VersionRangeTests
{
    [TestMethod]
    [DataRow("1.2.3", "1.2.3", true)]
    [DataRow("=1.2.3", "1.2.3", true)]
    [DataRow("1.2", "1.2.0", true)]
    [DataRow("1", "1.0.0", true)]
    [DataRow(">=1.2.3, <=1.2.3", "1.2.3", true)]
    [DataRow(">=0.40", ">=0.40.0", false)]
    [DataRow("<1", "<1.0.0", false)]
    [DataRow(">0.40.1", ">0.40.1", false)]
    [DataRow("<=0.40.1", "<=0.40.1", false)]
    [DataRow("  >= 0.40 , < 1  ", ">=0.40.0, <1.0.0", false)]
    [DataRow("<1, >=0.40", ">=0.40.0, <1.0.0", false)]
    public void TryParse_normalizes_valid_constraints(string input, string expected, bool exact)
    {
        VersionRange.TryParse(input, out var range).Should().BeTrue();
        range!.ToString().Should().Be(expected);
        range.IsExactVersion.Should().Be(exact);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("invalid")]
    [DataRow(">=")]
    [DataRow(",")]
    [DataRow(">=1,")]
    [DataRow(",<2")]
    [DataRow(">=1, <2, <3")]
    [DataRow(">=1, >=2")]
    [DataRow("<1, <=2")]
    [DataRow("1, 2")]
    [DataRow("=1, <2")]
    [DataRow("<2, =1")]
    [DataRow(">=1, =2")]
    [DataRow("v1.2.3")]
    [DataRow("1.2.3-preview")]
    [DataRow("1.2.3+metadata")]
    [DataRow("^1.2.3")]
    [DataRow("~1.2.3")]
    [DataRow("1.x")]
    [DataRow("(>=1, <2)")]
    public void TryParse_rejects_invalid_constraints(string input)
    {
        VersionRange.TryParse(input, out var range).Should().BeFalse();
        range.Should().BeNull();
        FluentActions.Invoking(() => VersionRange.Parse(input)).Should().Throw<FormatException>();
    }

    [TestMethod]
    [DataRow("1.2.3", true)]
    [DataRow(">=1", true)]
    [DataRow("<1", true)]
    [DataRow(">=1, <2", true)]
    [DataRow(">=2, <1", false)]
    [DataRow(">=1, <=1", true)]
    [DataRow(">=1, <1", false)]
    [DataRow(">1, <=1", false)]
    [DataRow(">1.2.3, <1.2.4", false)]
    [DataRow(">1.2.3, <1.2.5", true)]
    [DataRow(">=1.2.3, <1.2.4", true)]
    [DataRow(">1.2.3, <=1.2.4", true)]
    [DataRow(">1.2.2147483646, <1.2.2147483647", false)]
    [DataRow(">1.2.2147483647, <1.3.0", true)]
    public void IsSatisfiable_detects_empty_ranges(string input, bool expected)
    {
        VersionRange.Parse(input).IsSatisfiable.Should().Be(expected);
    }

    [TestMethod]
    [DataRow(">=0.40, <1", "0.40.0", true)]
    [DataRow(">=0.40, <1", "0.47.12", true)]
    [DataRow(">=0.40, <1", "0.39.9", false)]
    [DataRow(">=0.40, <1", "1.0.0", false)]
    [DataRow(">0.40.1, <=0.41", "0.40.1", false)]
    [DataRow(">0.40.1, <=0.41", "0.40.2", true)]
    [DataRow(">0.40.1, <=0.41", "0.41.0", true)]
    [DataRow(">0.40.1, <=0.41", "0.41.1", false)]
    [DataRow("0.40", "0.40.0", true)]
    [DataRow("0.40", "0.40.1", false)]
    [DataRow("<1", "0.40.1", true)]
    [DataRow(">=1", "2.0.0", true)]
    public void IsSatisfiedBy_requires_both_bounds(string input, string version, bool expected)
    {
        VersionRange.Parse(input).IsSatisfiedBy(SemanticVersion.Parse(version)).Should().Be(expected);
    }

    [TestMethod]
    public void Bounds_describe_exact_and_open_ended_ranges()
    {
        var exact = VersionRange.Parse("1.2.3");
        exact.LowerBound!.ToString().Should().Be(">=1.2.3");
        exact.UpperBound!.ToString().Should().Be("<=1.2.3");

        VersionRange.Parse(">=1").UpperBound.Should().BeNull();
        VersionRange.Parse("<1").LowerBound.Should().BeNull();
    }
}
