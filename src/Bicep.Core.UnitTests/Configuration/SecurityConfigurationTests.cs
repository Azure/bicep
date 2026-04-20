// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
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
        config.InvalidRegistryPatterns.Should().HaveCount(1);
        config.InvalidRegistryPatterns[0].Pattern.Should().Be("*");
        config.InvalidRegistryPatterns[0].Reason.Should().NotBeNullOrEmpty();
        config.HasInvalidRegistryPatterns.Should().BeTrue();
    }

    [TestMethod]
    public void Bind_AllInvalid_EmptyTrustedRegistries()
    {
        var config = Bind("""{"trustedRegistries":["*","*.com"]}""");

        config.TrustedRegistries.Should().BeEmpty();
        config.InvalidRegistryPatterns.Should().HaveCount(2);
        config.InvalidRegistryPatterns.Select(p => p.Pattern).Should().BeEquivalentTo(["*", "*.com"]);
        config.InvalidRegistryPatterns.All(p => !string.IsNullOrEmpty(p.Reason)).Should().BeTrue();
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

    // ── Group H: Validation reason messages ─────────────────────────────────

    [TestMethod]
    public void Bind_InvalidPatterns_CaptureSpecificReasons()
    {
        var config = Bind("""{"trustedRegistries":["*","*.com","https://example.com","ex*ample.com","example.:5000"]}""");

        config.InvalidRegistryPatterns.Should().HaveCount(5);

        // Each invalid pattern should have its specific validation reason
        config.InvalidRegistryPatterns[0].Pattern.Should().Be("*");
        config.InvalidRegistryPatterns[0].Reason.Should().Be(
            "Pattern '*' is not allowed because it trusts all registries. Use a specific hostname or '*.example.com' form.");

        config.InvalidRegistryPatterns[1].Pattern.Should().Be("*.com");
        config.InvalidRegistryPatterns[1].Reason.Should().Be(
            "Pattern '*.com' is too broad. Wildcards over a top-level domain suffix (e.g. '*.com', '*.io') are not permitted.");

        config.InvalidRegistryPatterns[2].Pattern.Should().Be("https://example.com");
        config.InvalidRegistryPatterns[2].Reason.Should().Be(
            "Pattern 'https://example.com' must be a bare hostname, not a URL. Remove the scheme (e.g. 'https://') and any credentials.");

        config.InvalidRegistryPatterns[3].Pattern.Should().Be("ex*ample.com");
        config.InvalidRegistryPatterns[3].Reason.Should().Be(
            "Pattern 'ex*ample.com' uses a wildcard in an unsupported position. The wildcard '*' is only allowed as the entire left-most label (e.g. '*.example.com').");

        config.InvalidRegistryPatterns[4].Pattern.Should().Be("example.:5000");
        config.InvalidRegistryPatterns[4].Reason.Should().Be(
            "Pattern 'example.:5000' must not contain a port. Omit the port number from the pattern.");
    }

    // ── Group I: RootConfiguration diagnostic emission (cap-at-5) ────────────

    [TestMethod]
    public void RootConfiguration_FewInvalidPatterns_EmitsDetailedBcp447ForEach()
    {
        // 3 invalid patterns (<=5) → 3 BCP447 warnings, none with overflow text
        var security = Bind("""{"trustedRegistries":["*","*.com","*.io"]}""");
        var diagnostics = BuildDiagnosticsFromSecurity(security);

        diagnostics.Should().HaveCount(3);
        diagnostics.Should().AllSatisfy(d =>
        {
            d.Code.Should().Be("BCP447");
            d.Level.Should().Be(DiagnosticLevel.Warning);
            d.Message.Should().Contain("is invalid and will be ignored");
            d.Message.Should().Contain("Reason:");
            d.Message.Should().Contain("https://aka.ms/bicep-registry-trust");
        });
        // No overflow text since count <= 5
        diagnostics.Should().NotContain(d => d.Message.Contains("additional invalid pattern"));

        // Verify each diagnostic embeds its specific pattern and reason
        diagnostics[0].Message.Should().Contain(
            "The trusted registry pattern \"*\" in \"security.trustedRegistries\" is invalid and will be ignored. " +
            "Reason: Pattern '*' is not allowed because it trusts all registries.");
        diagnostics[1].Message.Should().Contain(
            "The trusted registry pattern \"*.com\" in \"security.trustedRegistries\" is invalid and will be ignored. " +
            "Reason: Pattern '*.com' is too broad. Wildcards over a top-level domain suffix");
        diagnostics[2].Message.Should().Contain(
            "The trusted registry pattern \"*.io\" in \"security.trustedRegistries\" is invalid and will be ignored. " +
            "Reason: Pattern '*.io' is too broad. Wildcards over a top-level domain suffix");
    }

    [TestMethod]
    public void RootConfiguration_ExactlyFiveInvalidPatterns_EmitsDetailedBcp447ForEach_NoOverflow()
    {
        // 5 invalid patterns (==5) → 5 BCP447 warnings, none with overflow text
        var security = Bind("""{"trustedRegistries":["*","*.com","*.io","*.net","*.org"]}""");
        var diagnostics = BuildDiagnosticsFromSecurity(security);

        diagnostics.Should().HaveCount(5);
        diagnostics.Should().AllSatisfy(d =>
        {
            d.Code.Should().Be("BCP447");
            d.Level.Should().Be(DiagnosticLevel.Warning);
            d.Message.Should().Contain("is invalid and will be ignored");
            d.Message.Should().Contain("Reason:");
        });
        diagnostics.Should().NotContain(d => d.Message.Contains("additional invalid pattern"));
    }

    [TestMethod]
    public void RootConfiguration_MoreThanFiveInvalidPatterns_EmitsFiveBcp447_LastOneHasOverflowCount()
    {
        // 7 invalid patterns (>5) → 5 BCP447 (the last one includes "2 additional" overflow text)
        var security = Bind("""{"trustedRegistries":["*","*.com","*.io","*.net","*.org","*.xyz","*.info"]}""");
        var diagnostics = BuildDiagnosticsFromSecurity(security);

        diagnostics.Should().HaveCount(5);
        diagnostics.Should().AllSatisfy(d =>
        {
            d.Code.Should().Be("BCP447");
            d.Level.Should().Be(DiagnosticLevel.Warning);
            d.Message.Should().Contain("is invalid and will be ignored");
            d.Message.Should().Contain("Reason:");
            d.Message.Should().Contain("https://aka.ms/bicep-registry-trust");
        });

        // The 6th and 7th patterns (*.xyz, *.info) are NOT reported individually
        diagnostics.Should().NotContain(d => d.Message.Contains("*.xyz"));
        diagnostics.Should().NotContain(d => d.Message.Contains("*.info"));

        // First 4 do NOT have overflow text
        diagnostics.Take(4).Should().NotContain(d => d.Message.Contains("additional invalid pattern"));

        // The 5th (last) warning includes the overflow count
        var lastDiagnostic = diagnostics.Last();
        lastDiagnostic.Message.Should().Contain(
            "The trusted registry pattern \"*.org\" in \"security.trustedRegistries\" is invalid and will be ignored. " +
            "Reason: Pattern '*.org' is too broad. Wildcards over a top-level domain suffix");
        lastDiagnostic.Message.Should().Contain(
            "2 additional invalid pattern(s) were also found and will be ignored. Fix the above patterns to see details for the rest.");
    }

    [TestMethod]
    public void RootConfiguration_NoInvalidPatterns_EmitsNoDiagnostics()
    {
        var security = Bind("""{"trustedRegistries":["registry.example.com","*.corp.com"]}""");
        var diagnostics = BuildDiagnosticsFromSecurity(security);

        diagnostics.Should().BeEmpty();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static SecurityConfiguration Bind(string json)
    {
        var element = JsonDocument.Parse(json).RootElement;
        return SecurityConfiguration.Bind(element);
    }

    /// <summary>
    /// Builds the security-related diagnostics that RootConfiguration would emit,
    /// replicating its cap-at-5 logic without constructing a full RootConfiguration.
    /// </summary>
    private static IDiagnostic[] BuildDiagnosticsFromSecurity(SecurityConfiguration security)
    {
        const int maxDetailedWarnings = 5;
        var invalidPatterns = security.InvalidRegistryPatterns;
        var overflowCount = Math.Max(0, invalidPatterns.Length - maxDetailedWarnings);
        var patternsToReport = invalidPatterns.Take(maxDetailedWarnings).ToArray();
        var result = patternsToReport
            .Select((p, i) => (IDiagnostic)DiagnosticBuilder.ForDocumentStart().InvalidTrustedRegistryPattern(
                p.Pattern,
                p.Reason,
                i == patternsToReport.Length - 1 ? overflowCount : 0))
            .ToList();
        return [.. result];
    }
}
