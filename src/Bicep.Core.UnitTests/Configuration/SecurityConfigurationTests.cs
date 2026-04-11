// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Bicep.Core.Configuration;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Configuration;

[TestClass]
public class SecurityConfigurationTests
{
    // ── Group A: IsRegistryTrusted – positive (built-ins) ────────────────────────────

    [DataTestMethod]
    [DataRow("contoso.azurecr.io")]
    [DataRow("myregistry.azurecr.io")]
    [DataRow("a.b.azurecr.io")]        // multi-label subdomain
    public void IsRegistryTrusted_BuiltInAcr_ReturnsTrue(string hostname) =>
        SecurityConfiguration.Default.IsRegistryTrusted(hostname).Should().BeTrue();

    [DataTestMethod]
    [DataRow("contoso.azurecr.cn")]
    [DataRow("a.b.azurecr.cn")]
    public void IsRegistryTrusted_BuiltInAcrCn_ReturnsTrue(string hostname) =>
        SecurityConfiguration.Default.IsRegistryTrusted(hostname).Should().BeTrue();

    [DataTestMethod]
    [DataRow("contoso.azurecr.us")]
    [DataRow("a.b.azurecr.us")]
    public void IsRegistryTrusted_BuiltInAcrUs_ReturnsTrue(string hostname) =>
        SecurityConfiguration.Default.IsRegistryTrusted(hostname).Should().BeTrue();

    [TestMethod]
    public void IsRegistryTrusted_BuiltInMcrGlobal_ReturnsTrue() =>
        SecurityConfiguration.Default.IsRegistryTrusted("mcr.microsoft.com").Should().BeTrue();

    [TestMethod]
    public void IsRegistryTrusted_BuiltInMcrChina_ReturnsTrue() =>
        SecurityConfiguration.Default.IsRegistryTrusted("mcr.azure.cn").Should().BeTrue();

    [TestMethod]
    public void IsRegistryTrusted_BuiltInGhcr_ReturnsTrue() =>
        SecurityConfiguration.Default.IsRegistryTrusted("ghcr.io").Should().BeTrue();

    // ── Group B: IsRegistryTrusted – negative (bypass attempts) ─────────────────────

    [TestMethod]
    public void IsRegistryTrusted_MissingDotBeforeAcrSuffix_ReturnsFalse() =>
        SecurityConfiguration.Default.IsRegistryTrusted("evilazurecr.io").Should().BeFalse();

    [TestMethod]
    public void IsRegistryTrusted_AcrSuffixAppendedAsSubdomain_ReturnsFalse() =>
        SecurityConfiguration.Default.IsRegistryTrusted("evil.azurecr.io.attacker.com").Should().BeFalse();

    [TestMethod]
    public void IsRegistryTrusted_AcrRootWithoutSubdomain_ReturnsFalse() =>
        SecurityConfiguration.Default.IsRegistryTrusted("azurecr.io").Should().BeFalse();

    [TestMethod]
    public void IsRegistryTrusted_SubdomainOfExactMcr_ReturnsFalse() =>
        SecurityConfiguration.Default.IsRegistryTrusted("foo.mcr.microsoft.com").Should().BeFalse();

    [TestMethod]
    public void IsRegistryTrusted_SubdomainOfExactGhcr_ReturnsFalse() =>
        SecurityConfiguration.Default.IsRegistryTrusted("foo.ghcr.io").Should().BeFalse();

    [TestMethod]
    public void IsRegistryTrusted_McrWithExtraSuffix_ReturnsFalse() =>
        SecurityConfiguration.Default.IsRegistryTrusted("mcr.microsoft.com.attacker.com").Should().BeFalse();

    [TestMethod]
    public void IsRegistryTrusted_TypoDomain_ReturnsFalse() =>
        SecurityConfiguration.Default.IsRegistryTrusted("gchr.io").Should().BeFalse();

    [TestMethod]
    public void IsRegistryTrusted_UnknownDomain_ReturnsFalse() =>
        SecurityConfiguration.Default.IsRegistryTrusted("evil.example.com").Should().BeFalse();

    // ── Group C: IsRegistryTrusted – normalization ───────────────────────────────────

    [TestMethod]
    public void IsRegistryTrusted_UppercaseInput_NormalizesAndReturnsTrue() =>
        SecurityConfiguration.Default.IsRegistryTrusted("CONTOSO.AZURECR.IO").Should().BeTrue();

    [TestMethod]
    public void IsRegistryTrusted_TrailingDotOnSubdomain_NormalizesAndReturnsTrue() =>
        SecurityConfiguration.Default.IsRegistryTrusted("contoso.azurecr.io.").Should().BeTrue();

    [TestMethod]
    public void IsRegistryTrusted_TrailingDotOnExactEntry_NormalizesAndReturnsTrue() =>
        SecurityConfiguration.Default.IsRegistryTrusted("ghcr.io.").Should().BeTrue();

    [TestMethod]
    public void IsRegistryTrusted_UrlFormHttps_ReturnsFalse() =>
        SecurityConfiguration.Default.IsRegistryTrusted("https://contoso.azurecr.io").Should().BeFalse();

    [TestMethod]
    public void IsRegistryTrusted_PathAppended_ReturnsFalse() =>
        SecurityConfiguration.Default.IsRegistryTrusted("contoso.azurecr.io/v2/").Should().BeFalse();

    [TestMethod]
    public void IsRegistryTrusted_UserinfoPresent_ReturnsFalse() =>
        SecurityConfiguration.Default.IsRegistryTrusted("user:pass@contoso.azurecr.io").Should().BeFalse();

    // ── Group D: IsRegistryTrusted – user TrustedRegistries ──────────────────────────

    [TestMethod]
    public void IsRegistryTrusted_UserExactEntry_ReturnsTrue()
    {
        var config = Bind("""{"trustedRegistries":["registry.example.com"]}""");
        config.IsRegistryTrusted("registry.example.com").Should().BeTrue();
    }

    [TestMethod]
    public void IsRegistryTrusted_UserExactEntry_SubdomainNotMatched()
    {
        var config = Bind("""{"trustedRegistries":["registry.example.com"]}""");
        config.IsRegistryTrusted("sub.registry.example.com").Should().BeFalse();
    }

    [TestMethod]
    public void IsRegistryTrusted_UserWildcardEntry_SingleSubdomain()
    {
        var config = Bind("""{"trustedRegistries":["*.example.com"]}""");
        config.IsRegistryTrusted("a.example.com").Should().BeTrue();
    }

    [TestMethod]
    public void IsRegistryTrusted_UserWildcardEntry_MultiLabelSubdomain()
    {
        var config = Bind("""{"trustedRegistries":["*.example.com"]}""");
        config.IsRegistryTrusted("a.b.example.com").Should().BeTrue();
    }

    [TestMethod]
    public void IsRegistryTrusted_UserWildcardEntry_BaseNotMatched()
    {
        var config = Bind("""{"trustedRegistries":["*.example.com"]}""");
        config.IsRegistryTrusted("example.com").Should().BeFalse();
    }

    [TestMethod]
    public void IsRegistryTrusted_UserWildcardEntry_BypassAttempt()
    {
        var config = Bind("""{"trustedRegistries":["*.example.com"]}""");
        config.IsRegistryTrusted("evilexample.com").Should().BeFalse();
    }

    // ── Group E: ValidateRegistryPattern – invalid pattern rejection ─────────────────

    [TestMethod]
    public void ValidateRegistryPattern_GlobalWildcard_ReturnsError() =>
        SecurityConfiguration.ValidateRegistryPattern("*").Should().NotBeNull();

    [TestMethod]
    public void ValidateRegistryPattern_TldWildcardCom_ReturnsError() =>
        SecurityConfiguration.ValidateRegistryPattern("*.com").Should().NotBeNull();

    [TestMethod]
    public void ValidateRegistryPattern_TldWildcardIo_ReturnsError() =>
        SecurityConfiguration.ValidateRegistryPattern("*.io").Should().NotBeNull();

    [TestMethod]
    public void ValidateRegistryPattern_MultipleWildcards_ReturnsError() =>
        SecurityConfiguration.ValidateRegistryPattern("*.*.example.com").Should().NotBeNull();

    [TestMethod]
    public void ValidateRegistryPattern_InfixWildcard_ReturnsError() =>
        SecurityConfiguration.ValidateRegistryPattern("ex*ample.com").Should().NotBeNull();

    [TestMethod]
    public void ValidateRegistryPattern_WildcardTld_ReturnsError() =>
        SecurityConfiguration.ValidateRegistryPattern("example.*").Should().NotBeNull();

    [TestMethod]
    public void ValidateRegistryPattern_EmptyString_ReturnsError() =>
        SecurityConfiguration.ValidateRegistryPattern("").Should().NotBeNull();

    [TestMethod]
    public void ValidateRegistryPattern_WhitespaceString_ReturnsError() =>
        SecurityConfiguration.ValidateRegistryPattern("   ").Should().NotBeNull();

    [TestMethod]
    public void ValidateRegistryPattern_UrlForm_ReturnsError() =>
        SecurityConfiguration.ValidateRegistryPattern("https://example.com").Should().NotBeNull();

    [TestMethod]
    public void ValidateRegistryPattern_LeadingDot_ReturnsError() =>
        SecurityConfiguration.ValidateRegistryPattern(".example.com").Should().NotBeNull();

    [TestMethod]
    public void ValidateRegistryPattern_EmptyLabel_ReturnsError() =>
        SecurityConfiguration.ValidateRegistryPattern("example..com").Should().NotBeNull();

    [TestMethod]
    public void ValidateRegistryPattern_PathInPattern_ReturnsError() =>
        SecurityConfiguration.ValidateRegistryPattern("example.com/repo").Should().NotBeNull();

    [DataTestMethod]
    [DataRow("ghcr.io")]
    [DataRow("registry.example.com")]
    [DataRow("*.azurecr.io")]
    [DataRow("*.example.com")]
    [DataRow("*.corp.example.com")]
    public void ValidateRegistryPattern_ValidPatterns_ReturnsNull(string pattern) =>
        SecurityConfiguration.ValidateRegistryPattern(pattern).Should().BeNull();

    // ── Group F: Bind ─────────────────────────────────────────────────────────

    [TestMethod]
    public void Bind_ValidEntries_PopulatesTrustedRegistries()
    {
        var config = Bind("""{"trustedRegistries":["registry.example.com","*.corp.com"]}""");

        config.TrustedRegistries.Should().BeEquivalentTo(["registry.example.com", "*.corp.com"]);
        config.InvalidRegistryPatterns.Should().BeEmpty();
        config.HasInvalidRegistryPatterns.Should().BeFalse();
    }

    [TestMethod]
    public void Bind_MixedValidAndInvalid_IsolatesInvalidPatterns()
    {
        var config = Bind("""{"trustedRegistries":["registry.example.com","*"]}""");

        config.TrustedRegistries.Should().BeEquivalentTo(["registry.example.com"]);
        config.InvalidRegistryPatterns.Should().BeEquivalentTo(["*"]);
        config.HasInvalidRegistryPatterns.Should().BeTrue();
    }

    [TestMethod]
    public void Bind_AllInvalid_EmptyTrustedRegistries()
    {
        var config = Bind("""{"trustedRegistries":["*","*.com"]}""");

        config.TrustedRegistries.Should().BeEmpty();
        config.InvalidRegistryPatterns.Should().BeEquivalentTo(["*", "*.com"]);
        config.HasInvalidRegistryPatterns.Should().BeTrue();
    }

    [TestMethod]
    public void Bind_MissingTrustedRegistriesKey_ReturnsDefault()
    {
        var config = Bind("{}");

        config.TrustedRegistries.Should().BeEmpty();
        config.InvalidRegistryPatterns.Should().BeEmpty();
        config.HasInvalidRegistryPatterns.Should().BeFalse();
    }

    [TestMethod]
    public void Bind_EmptyArray_ReturnsEmptyTrustedRegistries()
    {
        var config = Bind("""{"trustedRegistries":[]}""");

        config.TrustedRegistries.Should().BeEmpty();
        config.HasInvalidRegistryPatterns.Should().BeFalse();
    }

    [TestMethod]
    public void Bind_MissingSection_BuiltInsStillMatch()
    {
        // Even with no user config, built-in registries are always trusted
        SecurityConfiguration.Default.IsRegistryTrusted("contoso.azurecr.io").Should().BeTrue();
        SecurityConfiguration.Default.IsRegistryTrusted("mcr.microsoft.com").Should().BeTrue();
        SecurityConfiguration.Default.IsRegistryTrusted("evil.example.com").Should().BeFalse();
    }

    // ── Group G: IPv6 address support ────────────────────────────────────────

    [TestMethod]
    public void ValidateRegistryPattern_IPv6Bare_ReturnsNull() =>
        SecurityConfiguration.ValidateRegistryPattern("[::1]").Should().BeNull();

    [TestMethod]
    public void ValidateRegistryPattern_IPv6WithPort_ReturnsError() =>
        SecurityConfiguration.ValidateRegistryPattern("[::1]:5000").Should().NotBeNull();

    [TestMethod]
    public void ValidateRegistryPattern_IPv6Loopback_Trusted()
    {
        var config = Bind("""{"trustedRegistries":["[::1]"]}""");
        config.IsRegistryTrusted("[::1]").Should().BeTrue();
        config.IsRegistryTrusted("[::1]:5000").Should().BeTrue();
    }

    [TestMethod]
    public void IsRegistryTrusted_IPv6WithPort_StripsPortAndMatches()
    {
        var config = Bind("""{"trustedRegistries":["[::1]"]}""");
        config.IsRegistryTrusted("[::1]:5000").Should().BeTrue();
    }

    [TestMethod]
    public void IsRegistryTrusted_IPv6Bare_Matches()
    {
        var config = Bind("""{"trustedRegistries":["[::1]"]}""");
        config.IsRegistryTrusted("[::1]").Should().BeTrue();
    }

    [TestMethod]
    public void IsRegistryTrusted_IPv6DifferentAddress_DoesNotMatch()
    {
        var config = Bind("""{"trustedRegistries":["[::1]"]}""");
        config.IsRegistryTrusted("[::2]").Should().BeFalse();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static SecurityConfiguration Bind(string json)
    {
        var element = JsonDocument.Parse(json).RootElement;
        return SecurityConfiguration.Bind(element);
    }
}
