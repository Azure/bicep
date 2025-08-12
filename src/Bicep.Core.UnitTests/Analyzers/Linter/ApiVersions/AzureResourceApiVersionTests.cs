// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Globalization;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Analyzers.Linter.ApiVersions
{
    [TestClass]
    public class AzureResourceApiVersionTests
    {
        [DataTestMethod]
        [DataRow("2001-01-01", "2001-01-01", "")]
        [DataRow("9999-12-31", "9999-12-31", "")]
        [DataRow("9999-12-31-alpha", "9999-12-31", "-alpha")]
        [DataRow("9999-12-31-preview", "9999-12-31", "-preview")]
        [DataRow("9999-12-31-privatepreview", "9999-12-31", "-privatepreview")]
        [DataRow("9999-12-31-beta", "9999-12-31", "-beta")]
        [DataRow("9999-12-31-rc", "9999-12-31", "-rc")]
        public void TryParse_ValidApiVersion_ReturnsTrueAndParsedApiVersion(string value, string expectedDate, string suffix)
        {
            var result = AzureResourceApiVersion.TryParse(value, out var apiVersion);

            result.Should().BeTrue();
            apiVersion.Date.ToString(AzureResourceApiVersion.DateFormat, CultureInfo.InvariantCulture).Should().Be(expectedDate);
            apiVersion.Suffix.Should().Be(suffix);
        }

        [DataTestMethod]
        [DataRow("2001-01-011")]
        [DataRow("whatever")]
        [DataRow("9999-12-31-")]
        [DataRow("9999-12-31-foobar")]
        public void TryParse_InvalidApiVersion_ReturnsFalseAndDefaultApiVersion(string value)
        {
            var result = AzureResourceApiVersion.TryParse(value, out var apiVersion);

            result.Should().BeFalse();
            apiVersion.Should().Be(default(AzureResourceApiVersion));
        }
    }
}
