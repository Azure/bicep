// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry.Oci;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Registry;

[TestClass]
public class OciSecurityConstantsTests
{
    // ── Group A: IsRegistryTrusted – positive (built-ins) ────────────────────

    [DataTestMethod]
    [DataRow("contoso.azurecr.io")]
    [DataRow("myregistry.azurecr.io")]
    [DataRow("a.b.azurecr.io")]        // multi-label subdomain
    public void IsRegistryTrusted_BuiltInAcr_ReturnsTrue(string hostname) =>
        OciSecurityConstants.IsRegistryTrusted(hostname).Should().BeTrue();

    [DataTestMethod]
    [DataRow("contoso.azurecr.cn")]
    [DataRow("a.b.azurecr.cn")]
    public void IsRegistryTrusted_BuiltInAcrCn_ReturnsTrue(string hostname) =>
        OciSecurityConstants.IsRegistryTrusted(hostname).Should().BeTrue();

    [DataTestMethod]
    [DataRow("contoso.azurecr.us")]
    [DataRow("a.b.azurecr.us")]
    public void IsRegistryTrusted_BuiltInAcrUs_ReturnsTrue(string hostname) =>
        OciSecurityConstants.IsRegistryTrusted(hostname).Should().BeTrue();

    [TestMethod]
    public void IsRegistryTrusted_BuiltInMcrGlobal_ReturnsTrue() =>
        OciSecurityConstants.IsRegistryTrusted("mcr.microsoft.com").Should().BeTrue();

    [TestMethod]
    public void IsRegistryTrusted_BuiltInMcrChina_ReturnsTrue() =>
        OciSecurityConstants.IsRegistryTrusted("mcr.azure.cn").Should().BeTrue();

    [TestMethod]
    public void IsRegistryTrusted_BuiltInGhcr_ReturnsTrue() =>
        OciSecurityConstants.IsRegistryTrusted("ghcr.io").Should().BeTrue();

    [TestMethod]
    public void IsRegistryTrusted_BuiltInLocalhost_ReturnsTrue() =>
        OciSecurityConstants.IsRegistryTrusted("localhost").Should().BeTrue();

    [TestMethod]
    public void IsRegistryTrusted_BuiltInLoopbackIPv4_ReturnsTrue() =>
        OciSecurityConstants.IsRegistryTrusted("127.0.0.1").Should().BeTrue();

    [TestMethod]
    public void IsRegistryTrusted_BuiltInLoopbackIPv6_ReturnsTrue() =>
        OciSecurityConstants.IsRegistryTrusted("[::1]").Should().BeTrue();

    // ── Group B: IsRegistryTrusted – negative (bypass attempts) ─────────────

    [TestMethod]
    public void IsRegistryTrusted_MissingDotBeforeAcrSuffix_ReturnsFalse() =>
        OciSecurityConstants.IsRegistryTrusted("evilazurecr.io").Should().BeFalse();

    [TestMethod]
    public void IsRegistryTrusted_AcrSuffixAppendedAsSubdomain_ReturnsFalse() =>
        OciSecurityConstants.IsRegistryTrusted("evil.azurecr.io.attacker.com").Should().BeFalse();

    [TestMethod]
    public void IsRegistryTrusted_AcrRootWithoutSubdomain_ReturnsFalse() =>
        OciSecurityConstants.IsRegistryTrusted("azurecr.io").Should().BeFalse();

    [TestMethod]
    public void IsRegistryTrusted_SubdomainOfExactMcr_ReturnsFalse() =>
        OciSecurityConstants.IsRegistryTrusted("foo.mcr.microsoft.com").Should().BeFalse();

    [TestMethod]
    public void IsRegistryTrusted_SubdomainOfExactGhcr_ReturnsFalse() =>
        OciSecurityConstants.IsRegistryTrusted("foo.ghcr.io").Should().BeFalse();

    [TestMethod]
    public void IsRegistryTrusted_McrWithExtraSuffix_ReturnsFalse() =>
        OciSecurityConstants.IsRegistryTrusted("mcr.microsoft.com.attacker.com").Should().BeFalse();

    [TestMethod]
    public void IsRegistryTrusted_TypoDomain_ReturnsFalse() =>
        OciSecurityConstants.IsRegistryTrusted("gchr.io").Should().BeFalse();

    [TestMethod]
    public void IsRegistryTrusted_UnknownDomain_ReturnsFalse() =>
        OciSecurityConstants.IsRegistryTrusted("evil.example.com").Should().BeFalse();

    // ── Group C: IsRegistryTrusted – normalization ───────────────────────────

    [TestMethod]
    public void IsRegistryTrusted_UppercaseInput_NormalizesAndReturnsTrue() =>
        OciSecurityConstants.IsRegistryTrusted("CONTOSO.AZURECR.IO").Should().BeTrue();

    [TestMethod]
    public void IsRegistryTrusted_TrailingDotOnSubdomain_NormalizesAndReturnsTrue() =>
        OciSecurityConstants.IsRegistryTrusted("contoso.azurecr.io.").Should().BeTrue();

    [TestMethod]
    public void IsRegistryTrusted_TrailingDotOnExactEntry_NormalizesAndReturnsTrue() =>
        OciSecurityConstants.IsRegistryTrusted("ghcr.io.").Should().BeTrue();

    [TestMethod]
    public void IsRegistryTrusted_UrlFormHttps_ReturnsFalse() =>
        OciSecurityConstants.IsRegistryTrusted("https://contoso.azurecr.io").Should().BeFalse();

    [TestMethod]
    public void IsRegistryTrusted_PathAppended_ReturnsFalse() =>
        OciSecurityConstants.IsRegistryTrusted("contoso.azurecr.io/v2/").Should().BeFalse();

    [TestMethod]
    public void IsRegistryTrusted_UserinfoPresent_ReturnsFalse() =>
        OciSecurityConstants.IsRegistryTrusted("user:pass@contoso.azurecr.io").Should().BeFalse();

    // ── Group G: IPv6 address support ────────────────────────────────────────

    [TestMethod]
    public void IsRegistryTrusted_IPv6LoopbackWithPort_ReturnsTrue() =>
        OciSecurityConstants.IsRegistryTrusted("[::1]:5000").Should().BeTrue();

    [TestMethod]
    public void IsRegistryTrusted_IPv6NonLoopback_ReturnsFalse() =>
        OciSecurityConstants.IsRegistryTrusted("[::2]").Should().BeFalse();

    [TestMethod]
    public void IsRegistryTrusted_LocalhostWithPort_ReturnsTrue() =>
        OciSecurityConstants.IsRegistryTrusted("localhost:5000").Should().BeTrue();
}
