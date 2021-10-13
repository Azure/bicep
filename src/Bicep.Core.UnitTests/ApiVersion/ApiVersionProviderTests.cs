// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.ApiVersion;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.ApiVersion
{
    [TestClass]
    public class ApiVersionProviderTests
    {
        private readonly ApiVersionProvider ApiVersionProvider = new ApiVersionProvider();

        [DataRow("", ApiVersionPrefixConstants.Preview, null)]
        [DataRow("  ", ApiVersionPrefixConstants.Preview, null)]
        [DataRow("invalid-text", ApiVersionPrefixConstants.Preview, null)]
        [DataRow("Microsoft.Network/dnsZones", null, null)]
        [DataRow("Microsoft.Network/dnsZones", "-invalid-prefix", null)]
        [DataRow("Microsoft.Network/dnsZones", ApiVersionPrefixConstants.GA, "2018-05-01")]
        [DataRow("Microsoft.Network/dnsZones", ApiVersionPrefixConstants.Preview, "2018-03-01")]
        [DataTestMethod]
        public void GetRecentApiVersion(string fullyQualifiedName, string? prefix, string? expected)
        {
            string? actual = ApiVersionProvider.GetRecentApiVersion(fullyQualifiedName, prefix);

            actual.Should().Be(expected);
        }

        [DataRow("", null, null)]
        [DataRow("  ", null, null)]
        [DataRow("invalid-text", null, null)]
        [DataRow("2020-06-01-intpreview", null, null)]
        [DataRow("2014-04-01", "2014-04-01", ApiVersionPrefixConstants.GA)]
        [DataRow("2004-08-11-alpha", "2004-08-11", ApiVersionPrefixConstants.Alpha)]
        [DataRow("2017-09-12-beta", "2017-09-12", ApiVersionPrefixConstants.Beta)]
        [DataRow("2020-06-01-preview", "2020-06-01", ApiVersionPrefixConstants.Preview)]
        [DataRow("2016-04-01-privatepreview", "2016-04-01", ApiVersionPrefixConstants.PrivatePreview)]
        [DataRow("2015-04-01-rc", "2015-04-01", ApiVersionPrefixConstants.RC)]
        [DataTestMethod]
        public void GetApiVersionAndPrefix(string apiVersionWithPrefix, string? expectedVersion, string? expectedPrefix)
        {
            (string? version, string? prefix) = ApiVersionProvider.GetApiVersionAndPrefix(apiVersionWithPrefix);

            version.Should().Be(expectedVersion);
            prefix.Should().Be(expectedPrefix);
        }

        [DataRow("invalid-text")]
        [DataRow("")]
        [DataRow("   ")]
        [TestMethod]
        public void GetApiVersionDate_WithInvalidVersion(string apiVersion)
        {
            DateTime? actual = ApiVersionProvider.GetApiVersionDate(apiVersion);

            actual.Should().BeNull();
        }

        [DataRow("2015-04-01-rc", "2015-04-01")]
        [DataRow("2016-04-01", "2016-04-01")]
        [DataRow("2016-04-01-privatepreview", "2016-04-01")]
        [TestMethod]
        public void GetApiVersionDate_WithValidVersion(string apiVersion, string expectedVersion)
        {
            DateTime? actual = ApiVersionProvider.GetApiVersionDate(apiVersion);

            actual.Should().Be(DateTime.Parse(expectedVersion));
        }
    }
}
