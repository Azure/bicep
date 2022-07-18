// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.ApiVersions
{
    [TestClass]
    public class ApiVersionProviderTests
    {
        //using Bicep.Core.ApiVersion;
        //private readonly ApiVersionProvider ApiVersionProvider = new ApiVersionProvider(FakeResourceTypes.GetFakeTypes(FakeResourceTypes.ResourceScope));

        //[DataRow("", ApiVersionSuffixes.Preview, null)]
        //[DataRow("  ", ApiVersionSuffixes.Preview, null)]
        //[DataRow("invalid-text", ApiVersionSuffixes.Preview, null)]
        //[DataRow("fake.Network/dnszones", null, null)]
        //[DataRow("fake.Network/dnsZones", null, null)]
        //[DataRow("fake.Network/dnszones", "-invalid-prefix", null)]
        //[DataRow("fake.Network/dnsZones", "-invalid-prefix", null)]
        //[DataRow("fake.Network/dnszones", ApiVersionSuffixes.GA, "2018-05-01")]
        //[DataRow("fake.Network/dnsZones", ApiVersionSuffixes.GA, "2018-05-01")]
        //[DataRow("fake.Network/dnszones", ApiVersionSuffixes.Preview, "2018-03-01")]
        //[DataRow("fake.Network/dnsZones", ApiVersionSuffixes.Preview, "2018-03-01")]
        //[DataRow("fAKE.NETWORK/DNSZONES", ApiVersionSuffixes.Preview, "2018-03-01")]
        //[DataTestMethod] //using Bicep.Core.ApiVersion; casing
        //public void GetRecentApiVersion(string fullyQualifiedName, string? prefix, string? expected)
        //{
        //    string? actual = ApiVersionProvider.GetRecentApiVersion(fullyQualifiedName, prefix);

        //    actual.Should().Be(expected);
        //}

        //using Bicep.Core.ApiVersion;
        //[DataRow("", null, null)]
        //[DataRow("  ", null, null)]
        //[DataRow("invalid-text", null, null)]
        //[DataRow("2020-06-01-intpreview", null, null)]
        //[DataRow("2014-04-01", "2014-04-01", ApiVersionSuffixes.GA)]
        //[DataRow("2004-08-11-alpha", "2004-08-11", ApiVersionSuffixes.Alpha)]
        //[DataRow("2017-09-12-beta", "2017-09-12", ApiVersionSuffixes.Beta)]
        //[DataRow("2020-06-01-preview", "2020-06-01", ApiVersionSuffixes.Preview)]
        //[DataRow("2016-04-01-privatepreview", "2016-04-01", ApiVersionSuffixes.PrivatePreview)]
        //[DataRow("2016-04-01-PRIVATEPREVIEW", "2016-04-01", ApiVersionSuffixes.PrivatePreview)] //using Bicep.Core.ApiVersion;?
        //[DataRow("2015-04-01-rc", "2015-04-01", ApiVersionSuffixes.RC)]
        //[DataTestMethod]
        //public void GetApiVersionAndPrefix(string apiVersionWithPrefix, string? expectedVersion, string? expectedPrefix)
        //{
        //    (string? version, string? prefix) = ApiVersionProvider.GetApiVersionAndPrefix(apiVersionWithPrefix);

        //    version.Should().Be(expectedVersion);
        //    prefix.Should().Be(expectedPrefix);
        //}
    }
}
