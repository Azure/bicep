// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests.Mock;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.ApiVersions
{
    [TestClass]
    public class ApiVersionProviderTests
    {
        [DataRow("")]
        [DataRow("  ")]
        [DataRow("invalid-text")]
        [DataRow("fake.Network/dnszones", "2415-05-04-preview", "2416-04-01", "2417-09-01", "2417-10-01", "2418-03-01-preview", "2418-05-01")]
        [DataRow("fAKE.NETWORK/DNSZONES", "2415-05-04-preview", "2416-04-01", "2417-09-01", "2417-10-01", "2418-03-01-preview", "2418-05-01")]
        [DataTestMethod]
        public void GetApiVersions(string fullyQualifiedName, params string[] expected)
        {
            var apiVersionProvider = FakeResourceTypes.GetFakeApiVersionProvider(FakeResourceTypes.ResourceScopeTypes);

            string[] actual = apiVersionProvider.GetApiVersions(ResourceScope.ResourceGroup, fullyQualifiedName).Select(v => v.ToString()).ToArray();

            actual.Should().Equal(expected);
        }

        [TestMethod]
        public void GetApiVersions_DoesNotFilterByScope()
        {
            var apiVersionProvider = FakeResourceTypes.GetFakeApiVersionProvider([
                "fake.rg/whatever@2000-01-01",
                "fake.sub/whatever@2001-01-01",
                "fake.mg/whatever@2002-01-01",
                "fake.tenant/whatever@2003-01-01",
            ]);

            apiVersionProvider.GetApiVersions(ResourceScope.ResourceGroup, "fake.rg/whatever").Should().ContainSingle().Which.ToString().Should().Be("2000-01-01");
            apiVersionProvider.GetApiVersions(ResourceScope.Subscription, "fake.sub/whatever").Should().ContainSingle().Which.ToString().Should().Be("2001-01-01");
            apiVersionProvider.GetApiVersions(ResourceScope.ManagementGroup, "fake.mg/whatever").Should().ContainSingle().Which.ToString().Should().Be("2002-01-01");
            apiVersionProvider.GetApiVersions(ResourceScope.Tenant, "fake.tenant/whatever").Should().ContainSingle().Which.ToString().Should().Be("2003-01-01");
        }
    }
}
