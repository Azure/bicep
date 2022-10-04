// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Linq;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.UnitTests.Diagnostics.LinterRuleTests;
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
        [DataRow("fake.Network/dnszones", "2415-05-04-preview", "2416-04-01", "2417-09-01", "2417-10-01", "2418-03-01-preview", "2418-05-01" )]
        [DataRow("fAKE.NETWORK/DNSZONES","2415-05-04-preview", "2416-04-01", "2417-09-01", "2417-10-01", "2418-03-01-preview", "2418-05-01" )]
        [DataTestMethod]
        public void GetApiVersions(string fullyQualifiedName, params string[] expected)
        {
            var apiVersionProvider = CreateDefaultApiVersionProvider();
            apiVersionProvider.InjectTypeReferences(ResourceScope.ResourceGroup, FakeResourceTypes.GetFakeResourceTypeReferences(FakeResourceTypes.ResourceScopeTypes));

            string[] actual = apiVersionProvider.GetApiVersions(ResourceScope.ResourceGroup, fullyQualifiedName).Select(v => v.Formatted).ToArray();

            actual.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetResourceTypeNames_BadScope()
        {
            var apiVersionProvider = CreateDefaultApiVersionProvider();
            apiVersionProvider.InjectTypeReferences(ResourceScope.ResourceGroup, FakeResourceTypes.GetFakeResourceTypeReferences(FakeResourceTypes.ResourceScopeTypes));

            var lambda =
            (() =>
            {

                var types = apiVersionProvider.GetResourceTypeNames(ResourceScope.Resource);
            });
            lambda.Should().Throw<ArgumentException>();
        }

        [DataTestMethod]
        public void GetResourceTypeNames_ResourceGroup()
        {
            var apiVersionProvider = CreateDefaultApiVersionProvider();
            apiVersionProvider.InjectTypeReferences(ResourceScope.ResourceGroup, FakeResourceTypes.GetFakeResourceTypeReferences(FakeResourceTypes.ResourceScopeTypes));

            var types = apiVersionProvider.GetResourceTypeNames(ResourceScope.ResourceGroup);

            types.Should().Contain("Fake.Network/dnszones", "Fake.Network/publicIPAddresses", "Fake.Network/ddosProtectionPlans");
        }

        [DataTestMethod]
        public void GetResourceTypeNames_SeparateScopes()
        {
            var apiVersionProvider = CreateDefaultApiVersionProvider();
            apiVersionProvider.InjectTypeReferences(ResourceScope.ResourceGroup, FakeResourceTypes.GetFakeResourceTypeReferences(FakeResourceTypes.ResourceScopeTypes));
            apiVersionProvider.InjectTypeReferences(ResourceScope.Subscription, FakeResourceTypes.GetFakeResourceTypeReferences(FakeResourceTypes.SubscriptionScopeTypes));
            apiVersionProvider.InjectTypeReferences(ResourceScope.ManagementGroup, FakeResourceTypes.GetFakeResourceTypeReferences("fake.mg/whatever@2001-01-01"));
            apiVersionProvider.InjectTypeReferences(ResourceScope.Tenant, FakeResourceTypes.GetFakeResourceTypeReferences("fake.tenant/whatever@2002-01-01"));

            var rgTypes = apiVersionProvider.GetResourceTypeNames(ResourceScope.ResourceGroup);
            rgTypes.Should().Contain(new string[] { "Fake.Network/dnszones", "Fake.Network/publicIPAddresses", "Fake.Network/ddosProtectionPlans" });

            var subTypes = apiVersionProvider.GetResourceTypeNames(ResourceScope.Subscription);
            subTypes.Should().Contain(new String[] { "Fake.Web/publishingCredentials", "Fake.Security/deviceSecurityGroups" });

            var mgTypes = apiVersionProvider.GetResourceTypeNames(ResourceScope.ManagementGroup);
            mgTypes.Should().Contain("fake.mg/whatever");

            var tenantTypes = apiVersionProvider.GetResourceTypeNames(ResourceScope.Tenant);
            tenantTypes.Should().Contain("fake.tenant/whatever");
        }

        private ApiVersionProvider CreateDefaultApiVersionProvider()
            => new ApiVersionProvider(new FeatureProvider(IConfigurationManager.GetBuiltInConfiguration()), new DefaultNamespaceProvider(new AzResourceTypeLoader()));
    }
}
