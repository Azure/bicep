// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.ApiVersion;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.ApiVersion
{
    [TestClass]
    public class ApiVersionHelperTests
    {
        [DataTestMethod]
        // d1 < d2
        [DataRow("2000-01-01", "2001-01-01", -1)]
        [DataRow("2000-01-01", "2000-01-02", -1)]
        [DataRow("1999-12-31", "2000-01-01", -1)]
        [DataRow("2000-01-01-beta", "2001-01-01", -1)]
        [DataRow("2000-01-01-beta", "2001-01-01-alpha", -1)]
        [DataRow("2000-01-01", "2001-01-01-alpha", -1)]
        // d1 > d2
        [DataRow("2001-01-01", "2000-01-01", 1)]
        [DataRow("2000-01-02", "2000-01-01", 1)]
        [DataRow("2000-01-01", "1999-12-31", 1)]
        [DataRow("2001-01-01", "2000-01-01-preview", 1)]
        [DataRow("2001-01-01-privewpreview", "2000-01-01-beta", 1)]
        [DataRow("2001-01-01-rc", "2000-01-01", 1)]
        // d1 = d2
        [DataRow("2001-01-01", "2001-01-01", 0)]
        [DataRow("1999-12-31", "1999-12-31", 0)]
        [DataRow("1999-12-31-alpha", "1999-12-31", 0)]
        [DataRow("1999-12-31-alpha", "1999-12-31-preview", 0)]
        [DataRow("1999-12-31", "1999-12-31", 0)]
        public void CompareApiVersionDates(string date1, string date2, int expectedResult)
        {
            int result = ApiVersionHelper.CompareApiVersionDates(date1, date2);
            result.Should().Be(expectedResult);
        }

        [DataRow(2001, 1, 1, null, "2001-01-01")]
        [DataRow(2000, 1, 1, "", "2000-01-01")]
        [DataRow(2000, 01, 01, "-alpha", "2000-01-01-alpha")]
        [DataRow(1999, 12, 31, null, "1999-12-31")]
        [DataRow(2021, 7, 9, null, "2021-07-09")]
        [DataTestMethod]
        public void Format(int year, int month, int day, string? suffix, string expected)
        {
            var date = new DateTime(year, month, day);
            var result = ApiVersionHelper.Format(date, suffix);

            result.Should().Be(expected);
        }

        [DataRow("2001-01-01", "2001-01-01", null)]
        [DataRow("9999-12-31", "9999-12-31", null)]
        [DataRow("9999-12-31-alpha", "9999-12-31", "-alpha")]
        [DataRow("9999-12-31-preview", "9999-12-31", "-preview")]
        [DataRow("9999-12-31-privatepreview", "9999-12-31", "-privatepreview")]
        [DataRow("9999-12-31-beta", "9999-12-31", "-beta")]
        [DataRow("9999-12-31-alpha", "9999-12-31", "-alpha")]
        [DataRow("9999-12-31-rc", "9999-12-31", "-rc")]
        [DataRow("9999-12-31-", null, null)]
        [DataTestMethod]
        public void Parse(string apiVersion, string expectedDate, string? expectedSuffix)
        {
            var result = ApiVersionHelper.TryParse(apiVersion);

            result.date.Should().Be(expectedDate);
            result.suffixWithHypen.Should().Be(expectedSuffix);
        }
    }
}
