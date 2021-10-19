// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.ApiVersion;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.ApiVersion
{
    [TestClass]
    public class ApiVersionProviderTests
    {
        private readonly ApiVersionProvider ApiVersionProvider = new ApiVersionProvider(BicepTestConstants.NamespaceProvider);

        [DataRow("", ApiVersionSuffixConstants.Preview, null)]
        [DataRow("  ", ApiVersionSuffixConstants.Preview, null)]
        [DataRow("invalid-text", ApiVersionSuffixConstants.Preview, null)]
        [DataRow("Microsoft.Network/dnsZones", null, null)]
        [DataRow("Microsoft.Network/dnsZones", "-invalid-prefix", null)]
        [DataRow("Microsoft.Network/dnsZones", ApiVersionSuffixConstants.GA, "2018-05-01")]
        [DataRow("Microsoft.Network/dnsZones", ApiVersionSuffixConstants.Preview, "2018-03-01")]
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
        [DataRow("2014-04-01", "2014-04-01", ApiVersionSuffixConstants.GA)]
        [DataRow("2004-08-11-alpha", "2004-08-11", ApiVersionSuffixConstants.Alpha)]
        [DataRow("2017-09-12-beta", "2017-09-12", ApiVersionSuffixConstants.Beta)]
        [DataRow("2020-06-01-preview", "2020-06-01", ApiVersionSuffixConstants.Preview)]
        [DataRow("2016-04-01-privatepreview", "2016-04-01", ApiVersionSuffixConstants.PrivatePreview)]
        [DataRow("2015-04-01-rc", "2015-04-01", ApiVersionSuffixConstants.RC)]
        [DataTestMethod]
        public void GetApiVersionAndPrefix(string apiVersionWithPrefix, string? expectedVersion, string? expectedPrefix)
        {
            (string? version, string? prefix) = ApiVersionProvider.GetApiVersionAndPrefix(apiVersionWithPrefix);

            version.Should().Be(expectedVersion);
            prefix.Should().Be(expectedPrefix);
        }
    }
}
