// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RpcClient.SemanticVersioning;
using FluentAssertions;

namespace Bicep.RpcClient.Tests.SemanticVersioning;

[TestClass]
public class SemanticVersionTests
{
    [TestMethod]
    [DataRow("1.2.3", "1.2.3")]
    [DataRow("0.0.0", "0.0.0")]
    [DataRow("0.47.12", "0.47.12")]
    [DataRow("1.2", "1.2.0")]
    [DataRow("1", "1.0.0")]
    [DataRow("0", "0.0.0")]
    [DataRow("2147483647.0.0", "2147483647.0.0")]
    public void TryParse_accepts_versions_and_defaults_omitted_components(string input, string expected)
    {
        SemanticVersion.TryParse(input, out var version).Should().BeTrue();
        version!.ToString().Should().Be(expected);
        version.Should().Be(SemanticVersion.Parse(expected));
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("invalid")]
    [DataRow("1.2.3.4")]
    [DataRow("1..2")]
    [DataRow("1.")]
    [DataRow(".1.2")]
    [DataRow("-1.2.3")]
    [DataRow("+1.2.3")]
    [DataRow("01.2.3")]
    [DataRow("1.02.3")]
    [DataRow("1.2.03")]
    [DataRow("v1.2.3")]
    [DataRow("V1.2.3")]
    [DataRow("1.2.3-preview")]
    [DataRow("1.2.3+metadata")]
    [DataRow("1.2.3\n")]
    [DataRow(" 1.2.3")]
    [DataRow("1.2.3 ")]
    [DataRow("2147483648.0.0")]
    [DataRow("0.2147483648.0")]
    [DataRow("0.0.2147483648")]
    [DataRow("\u0661.\u0662.\u0663")]
    public void TryParse_rejects_invalid_versions(string input)
    {
        SemanticVersion.TryParse(input, out var version).Should().BeFalse();
        version.Should().BeNull();
        FluentActions.Invoking(() => SemanticVersion.Parse(input)).Should().Throw<FormatException>();
    }

    [TestMethod]
    [DataRow("1.2.3", "1.2.3", 0)]
    [DataRow("2.0.0", "1.99.99", 1)]
    [DataRow("1.99.99", "2.0.0", -1)]
    [DataRow("1.3.0", "1.2.99", 1)]
    [DataRow("1.2.99", "1.3.0", -1)]
    [DataRow("1.2.4", "1.2.3", 1)]
    [DataRow("1.2.3", "1.2.4", -1)]
    [DataRow("1.10.0", "1.9.0", 1)]
    public void CompareTo_orders_components_numerically(string left, string right, int expectedSign)
    {
        Math.Sign(SemanticVersion.Parse(left).CompareTo(SemanticVersion.Parse(right))).Should().Be(expectedSign);
    }

    [TestMethod]
    public void CompareTo_null_is_positive()
    {
        SemanticVersion.Parse("1.2.3").CompareTo(null).Should().BePositive();
    }
}
