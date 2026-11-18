// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RpcClient.SemanticVersioning;
using FluentAssertions;

namespace Bicep.RpcClient.Tests.SemanticVersioning;

[TestClass]
public class VersionComparatorTests
{
    [TestMethod]
    [DataRow("1.2.3", nameof(VersionComparatorOperator.Equal), false, false, true, "1.2.3")]
    [DataRow("=1.2.3", nameof(VersionComparatorOperator.Equal), false, false, true, "1.2.3")]
    [DataRow(">1.2.3", nameof(VersionComparatorOperator.GreaterThan), true, false, false, ">1.2.3")]
    [DataRow(">=1.2.3", nameof(VersionComparatorOperator.GreaterThanOrEqual), true, false, true, ">=1.2.3")]
    [DataRow("<1.2.3", nameof(VersionComparatorOperator.LessThan), false, true, false, "<1.2.3")]
    [DataRow("<=1.2.3", nameof(VersionComparatorOperator.LessThanOrEqual), false, true, true, "<=1.2.3")]
    [DataRow("  >= 1.2  ", nameof(VersionComparatorOperator.GreaterThanOrEqual), true, false, true, ">=1.2.0")]
    public void TryParse_recognizes_operator_and_bounds(string input, string expectedOperator,
        bool lowerBound, bool upperBound, bool inclusive, string normalized)
    {
        VersionComparator.TryParse(input, out var comparator).Should().BeTrue();
        comparator!.Operator.ToString().Should().Be(expectedOperator);
        comparator.IsLowerBound.Should().Be(lowerBound);
        comparator.IsUpperBound.Should().Be(upperBound);
        comparator.IsInclusive.Should().Be(inclusive);
        comparator.ToString().Should().Be(normalized);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(">=")]
    [DataRow("==1.2.3")]
    [DataRow(">>1.2.3")]
    [DataRow("!=1.2.3")]
    [DataRow("> =1.2.3")]
    [DataRow(">=v1.2.3")]
    [DataRow("1.2.3, <2")]
    public void TryParse_rejects_invalid_comparators(string input)
    {
        VersionComparator.TryParse(input, out var comparator).Should().BeFalse();
        comparator.Should().BeNull();
    }

    [TestMethod]
    [DataRow("1.2.3", "1.2.3", true)]
    [DataRow("1.2.3", "1.2.4", false)]
    [DataRow(">1.2.3", "1.2.3", false)]
    [DataRow(">1.2.3", "1.2.4", true)]
    [DataRow(">=1.2.3", "1.2.3", true)]
    [DataRow(">=1.2.3", "1.2.2", false)]
    [DataRow("<1.2.3", "1.2.3", false)]
    [DataRow("<1.2.3", "1.2.2", true)]
    [DataRow("<=1.2.3", "1.2.3", true)]
    [DataRow("<=1.2.3", "1.2.4", false)]
    public void IsSatisfiedBy_compares_versions(string input, string version, bool expected)
    {
        VersionComparator.TryParse(input, out var comparator).Should().BeTrue();
        comparator!.IsSatisfiedBy(SemanticVersion.Parse(version)).Should().Be(expected);
    }
}
