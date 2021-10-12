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
        private readonly ApiVersionProvider ApiVersionProvider = new ApiVersionProvider();

        [DataRow("", true, null)]
        [DataRow("  ", true, null)]
        [DataRow("invalid-text", true, null)]
        [DataRow("", false, null)]
        [DataRow("  ", false, null)]
        [DataRow("invalid-text", false, null)]
        [DataRow("Microsoft.Network/dnsZones", true, "2018-05-01")]
        [DataRow("Microsoft.Sql/servers", true, "2014-04-01")]
        [DataRow("Microsoft.Network/dnsZones", false, "2018-03-01")]
        [DataRow("Microsoft.Sql/servers", false, "2021-02-01")]
        [DataTestMethod]
        public void GetRecentApiVersion(string fullyQualifiedName, bool useNonApiVersionCache, string expected)
        {
            string? actual = ApiVersionProvider.GetRecentApiVersion(fullyQualifiedName, useNonApiVersionCache);

            expected.Should().Be(actual);
        }
    }
}
