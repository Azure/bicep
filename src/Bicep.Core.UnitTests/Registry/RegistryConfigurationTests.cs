// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Registry
{
    [TestClass]
    public class RegistryConfigurationTests
    {
        [TestMethod]
        [DataRow("anything.example.com")]
        [DataRow("localhost")]
        [DataRow("192.168.1.1")]
        [DataRow("myregistry.io")]
        public void IsRegistryTrusted_WhenPermitUntrustedRegistries_AlwaysReturnsTrue(string hostname)
        {
            var config = new RegistryConfiguration(PermitUntrustedRegistries: true);
            config.IsRegistryTrusted(hostname).Should().BeTrue();
        }

        [TestMethod]
        [DataRow("contoso.azurecr.io")]
        [DataRow("CONTOSO.azurecr.io")]          // case-insensitive
        [DataRow("a.b.azurecr.io")]              // deeper subdomain
        public void IsRegistryTrusted_AzureContainerRegistry_ReturnsTrue(string hostname)
        {
            var config = new RegistryConfiguration(PermitUntrustedRegistries: false);
            config.IsRegistryTrusted(hostname).Should().BeTrue();
        }

        [TestMethod]
        [DataRow("azurecr.io")]                  // no subdomain — wildcard requires at least one label
        [DataRow("notazurecr.io")]
        [DataRow("evil-azurecr.io")]             // suffix must start with a dot
        public void IsRegistryTrusted_AzureContainerRegistryRoot_ReturnsFalse(string hostname)
        {
            var config = new RegistryConfiguration(PermitUntrustedRegistries: false);
            config.IsRegistryTrusted(hostname).Should().BeFalse();
        }

        [TestMethod]
        [DataRow("contoso.azurecr.cn")]
        [DataRow("contoso.azurecr.us")]
        public void IsRegistryTrusted_AzureContainerRegistrySovereign_ReturnsTrue(string hostname)
        {
            var config = new RegistryConfiguration(PermitUntrustedRegistries: false);
            config.IsRegistryTrusted(hostname).Should().BeTrue();
        }

        [TestMethod]
        [DataRow("mcr.microsoft.com")]
        [DataRow("MCR.MICROSOFT.COM")]           // case-insensitive
        [DataRow("mcr.azure.cn")]
        [DataRow("ghcr.io")]
        public void IsRegistryTrusted_BuiltInExactMatch_ReturnsTrue(string hostname)
        {
            var config = new RegistryConfiguration(PermitUntrustedRegistries: false);
            config.IsRegistryTrusted(hostname).Should().BeTrue();
        }

        [TestMethod]
        [DataRow("evil.com")]
        [DataRow("docker.io")]
        [DataRow("mycompany.registry.example.com")]
        [DataRow("localhost")]
        [DataRow("localhost:5000")]
        [DataRow("192.168.1.1")]
        [DataRow("192.168.1.1:5000")]
        public void IsRegistryTrusted_UnknownRegistry_ReturnsFalse(string hostname)
        {
            var config = new RegistryConfiguration(PermitUntrustedRegistries: false);
            config.IsRegistryTrusted(hostname).Should().BeFalse();
        }

        [TestMethod]
        [DataRow("https://contoso.azurecr.io")]
        [DataRow("oci://contoso.azurecr.io")]
        [DataRow("user@contoso.azurecr.io")]
        [DataRow("contoso.azurecr.io/mymodule")]
        public void IsRegistryTrusted_UrlOrPathForm_ReturnsFalse(string hostname)
        {
            var config = new RegistryConfiguration(PermitUntrustedRegistries: false);
            config.IsRegistryTrusted(hostname).Should().BeFalse();
        }

        [TestMethod]
        [DataRow("[::1]")]
        [DataRow("[::1]:5000")]
        [DataRow("[2001:db8::1]")]
        public void IsRegistryTrusted_IPv6_ReturnsFalse(string hostname)
        {
            // IPv6 addresses are not in the built-in trusted list
            var config = new RegistryConfiguration(PermitUntrustedRegistries: false);
            config.IsRegistryTrusted(hostname).Should().BeFalse();
        }

        [TestMethod]
        public void IsRegistryTrusted_MalformedIPv6_ReturnsFalse()
        {
            var config = new RegistryConfiguration(PermitUntrustedRegistries: false);
            config.IsRegistryTrusted("[::1").Should().BeFalse();
        }

        [TestMethod]
        [DataRow("contoso.example.com")]
        [DataRow("CONTOSO.EXAMPLE.COM")]           // case-insensitive
        public void IsRegistryTrusted_UserSuppliedExactMatch_ReturnsTrue(string hostname)
        {
            var config = new RegistryConfiguration(PermitUntrustedRegistries: false, additionalTrustedRegistries: ["contoso.example.com"]);
            config.IsRegistryTrusted(hostname).Should().BeTrue();
        }

        [TestMethod]
        [DataRow("harbor.contoso.io")]
        [DataRow("a.b.contoso.io")]
        public void IsRegistryTrusted_UserSuppliedWildcard_ReturnsTrue(string hostname)
        {
            var config = new RegistryConfiguration(PermitUntrustedRegistries: false, additionalTrustedRegistries: ["*.contoso.io"]);
            config.IsRegistryTrusted(hostname).Should().BeTrue();
        }

        [TestMethod]
        [DataRow("evil.com")]
        [DataRow("contoso.io")]                    // wildcard requires a subdomain label
        public void IsRegistryTrusted_UserSuppliedList_DoesNotTrustOtherHosts(string hostname)
        {
            var config = new RegistryConfiguration(PermitUntrustedRegistries: false, additionalTrustedRegistries: ["*.contoso.io"]);
            config.IsRegistryTrusted(hostname).Should().BeFalse();
        }

        [TestMethod]
        [DataRow(null, new string[0])]
        [DataRow("", new string[0])]
        [DataRow("   ", new string[0])]
        [DataRow("contoso.example.com", new[] { "contoso.example.com" })]
        [DataRow("a.example.com, b.example.com ; *.c.io", new[] { "a.example.com", "b.example.com", "*.c.io" })]
        public void ParseTrustedRegistries_ParsesDelimitedList(string? value, string[] expected)
        {
            RegistryConfiguration.ParseTrustedRegistries(value).Should().Equal(expected);
        }
    }
}
