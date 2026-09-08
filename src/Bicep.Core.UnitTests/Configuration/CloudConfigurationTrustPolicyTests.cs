// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Configuration;
using Bicep.Core.Json;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Configuration;

[TestClass]
public class CloudConfigurationTrustPolicyTests
{
    [TestMethod]
    [DataRow("AzureCloud")]
    [DataRow("AzureBleuCloud")]
    [DataRow("AzureChinaCloud")]
    [DataRow("AzureGermanyCloud")]
    [DataRow("AzureUSGovernment")]
    public void BuiltInProfilesAreTrusted(string profileName)
    {
        var cloud = CreateCloud(profileName, GetBuiltInProfile(profileName));

        new CloudConfigurationTrustPolicy().IsTrusted(cloud).Should().BeTrue();
    }

    [TestMethod]
    public void ModifiedBuiltInAuthorityIsNotTrusted()
    {
        var cloud = CreateCloud("AzureCloud", new("https://management.azure.com", "https://login.example.invalid"));

        new CloudConfigurationTrustPolicy().IsTrusted(cloud).Should().BeFalse();
    }

    [TestMethod]
    public void ModifiedBuiltInEndpointIsNotTrusted()
    {
        var cloud = CreateCloud("AzureCloud", new("https://management.example.invalid", "https://login.microsoftonline.com"));

        new CloudConfigurationTrustPolicy().IsTrusted(cloud).Should().BeFalse();
    }

    [TestMethod]
    public void CustomProfileRequiresAnExactExternalTrustPair()
    {
        var cloud = CreateCloud("Custom", new("https://management.example.invalid", "https://login.example.invalid"));
        var trustJson = """
            [{
                "resourceManagerEndpoint": "https://management.example.invalid",
                "activeDirectoryAuthority": "https://login.example.invalid"
            }]
            """;

        new CloudConfigurationTrustPolicy().IsTrusted(cloud).Should().BeFalse();
        CloudConfigurationTrustPolicy.FromEnvironmentValue(trustJson).IsTrusted(cloud).Should().BeTrue();
    }

    [TestMethod]
    public void AdfsAuthorityRequiresAnExactExternalTrustPair()
    {
        var cloud = CreateCloud("Custom", new("https://management.example.invalid", "https://login.example.invalid/adfs"));
        var trustJson = """
            [{
                "resourceManagerEndpoint": "https://management.example.invalid",
                "activeDirectoryAuthority": "https://login.example.invalid/adfs"
            }]
            """;

        new CloudConfigurationTrustPolicy().IsTrusted(cloud).Should().BeFalse();
        new CloudConfigurationTrustPolicy().IsAuthorityTrusted(cloud.ActiveDirectoryAuthorityUri).Should().BeFalse();
        CloudConfigurationTrustPolicy.FromEnvironmentValue(trustJson).IsTrusted(cloud).Should().BeTrue();
    }

    [TestMethod]
    public void AdfsAuthorityOnABuiltInHostIsNotTrusted()
    {
        // A custom "/adfs" path under a built-in authority host must not inherit built-in trust.
        var cloud = CreateCloud("Custom", new("https://management.azure.com", "https://login.microsoftonline.com/adfs"));

        var policy = new CloudConfigurationTrustPolicy();

        policy.IsTrusted(cloud).Should().BeFalse();
        policy.IsAuthorityTrusted(cloud.ActiveDirectoryAuthorityUri).Should().BeFalse();
        policy.Invoking(x => x.ThrowIfCloudIsUntrusted(cloud)).Should().Throw<InvalidOperationException>();
        policy.Invoking(x => x.ThrowIfAuthorityIsUntrusted(cloud.ActiveDirectoryAuthorityUri)).Should().Throw<InvalidOperationException>();
    }

    [TestMethod]
    [DataRow("https://different.example.invalid", "https://login.example.invalid")]
    [DataRow("https://management.example.invalid", "https://different.example.invalid")]
    public void ChangingEitherPairComponentDoesNotMatchExternalTrust(
        string resourceManagerEndpoint,
        string activeDirectoryAuthority)
    {
        var cloud = CreateCloud("Custom", new("https://management.example.invalid", "https://login.example.invalid"));
        var trusted = new CloudProfileTrustPair(
            resourceManagerEndpoint,
            activeDirectoryAuthority);

        new CloudConfigurationTrustPolicy([trusted]).IsTrusted(cloud).Should().BeFalse();
    }

    [TestMethod]
    [DataRow("*")]
    [DataRow("{\"resourceManagerEndpoint\":\"https://management.example.invalid\",\"activeDirectoryAuthority\":\"https://login.example.invalid\"}")]
    [DataRow("[{\"resourceManagerEndpoint\":\"https://management.example.invalid\"}]")]
    [DataRow("[{\"resourceManagerEndpoint\":\"https://management.example.invalid\",\"activeDirectoryAuthority\":\"https://login.example.invalid\",\"extra\":true}]")]
    [DataRow("[{\"resourceManagerEndpoint\":\"https://management.example.invalid:443.evil.invalid\",\"activeDirectoryAuthority\":\"https://login.example.invalid\"}]")]
    public void InvalidExternalTrustConfigurationFailsClosed(string value)
    {
        var policy = CloudConfigurationTrustPolicy.FromEnvironmentValue(value);

        policy.IsTrusted((CloudConfiguration)BicepConfiguration.BuiltIn.Cloud).Should().BeTrue();
        policy.IsTrusted(CreateCloud("Custom", new("https://management.example.invalid", "https://login.example.invalid"))).Should().BeFalse();
    }

    [TestMethod]
    public void MalformedEntriesDoNotInvalidateValidEntriesInTheSameArray()
    {
        var policy = CloudConfigurationTrustPolicy.FromEnvironmentValue("""
            [
                "not-an-object",
                { "resourceManagerEndpoint": "https://management.example.invalid" },
                {
                    "resourceManagerEndpoint": "https://management.valid.invalid",
                    "activeDirectoryAuthority": "https://login.valid.invalid"
                },
                {
                    "resourceManagerEndpoint": "https://management.other.invalid",
                    "activeDirectoryAuthority": "https://login.other.invalid",
                    "unexpected": "value"
                }
            ]
            """);

        policy.IsTrusted(CreateCloud("Custom", new("https://management.valid.invalid", "https://login.valid.invalid"))).Should().BeTrue();
        policy.IsTrusted(CreateCloud("Custom", new("https://management.other.invalid", "https://login.other.invalid"))).Should().BeFalse();
        policy.IsTrusted(CreateCloud("Custom", new("https://management.example.invalid", "https://login.example.invalid"))).Should().BeFalse();
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("{ not json")]
    [DataRow("\"https://management.example.invalid\"")]
    [DataRow("123")]
    public void MalformedOrNonArrayDocumentsGrantNoAdditionalTrust(string value)
    {
        var policy = CloudConfigurationTrustPolicy.FromEnvironmentValue(value);

        policy.IsTrusted((CloudConfiguration)BicepConfiguration.BuiltIn.Cloud).Should().BeTrue();
        policy.IsTrusted(CreateCloud("Custom", new("https://management.example.invalid", "https://login.example.invalid"))).Should().BeFalse();
    }

    [TestMethod]
    public void ExternalTrustNormalizesHostCaseButDoesNotMatchLookalikesOrNonDefaultPorts()
    {
        var policy = CloudConfigurationTrustPolicy.FromEnvironmentValue("""
            [{
                "resourceManagerEndpoint": "https://MANAGEMENT.example.invalid",
                "activeDirectoryAuthority": "https://LOGIN.example.invalid"
            }]
            """);

        policy.IsTrusted(CreateCloud("Custom", new("https://management.example.invalid", "https://login.example.invalid"))).Should().BeTrue();
        policy.IsTrusted(CreateCloud("Custom", new("https://management.example.invalid.evil.invalid", "https://login.example.invalid"))).Should().BeFalse();
        policy.IsTrusted(CreateCloud("Custom", new("https://management.example.invalid:8443", "https://login.example.invalid"))).Should().BeFalse();
    }

    [TestMethod]
    // userinfo smuggling: "https://management.azure.com@evil.invalid" actually targets evil.invalid.
    [DataRow("https://management.azure.com@evil.invalid", "https://login.microsoftonline.com")]
    [DataRow("https://management.azure.com", "https://login.microsoftonline.com@evil.invalid")]
    // trailing-dot (fully qualified) host form.
    [DataRow("https://management.azure.com.", "https://login.microsoftonline.com")]
    [DataRow("https://management.azure.com", "https://login.microsoftonline.com.")]
    // IDN/punycode homoglyph: the "a" below is a Cyrillic U+0430.
    [DataRow("https://m\u0430nagement.azure.com", "https://login.microsoftonline.com")]
    [DataRow("https://xn--mnagement-0yh.azure.com", "https://login.microsoftonline.com")]
    [DataRow("https://management.azure.com", "https://l\u043egin.microsoftonline.com")]
    // HTTP downgrade.
    [DataRow("http://management.azure.com", "https://login.microsoftonline.com")]
    [DataRow("https://management.azure.com", "http://login.microsoftonline.com")]
    public void UriNormalizationBypassAttemptsAreNotTrusted(string resourceManagerEndpoint, string activeDirectoryAuthority)
    {
        var cloud = CreateCloud("AzureCloud", new(resourceManagerEndpoint, activeDirectoryAuthority));

        var policy = new CloudConfigurationTrustPolicy();

        policy.IsTrusted(cloud).Should().BeFalse();
        policy.Invoking(x => x.ThrowIfCloudIsUntrusted(cloud)).Should().Throw<InvalidOperationException>();
    }

    [TestMethod]
    [DataRow("http://login.microsoftonline.com")]
    [DataRow("https://login.microsoftonline.com@evil.invalid")]
    [DataRow("https://login.microsoftonline.com.")]
    [DataRow("https://l\u043egin.microsoftonline.com")]
    public void UriNormalizationBypassAttemptsAreNotTrustedForAuthoritiesAlone(string activeDirectoryAuthority)
    {
        var policy = new CloudConfigurationTrustPolicy();

        policy.IsAuthorityTrusted(new Uri(activeDirectoryAuthority)).Should().BeFalse();
        policy.Invoking(x => x.ThrowIfAuthorityIsUntrusted(new Uri(activeDirectoryAuthority))).Should().Throw<InvalidOperationException>();
    }

    [TestMethod]
    public void HttpProfilesRequireAnExactExternalTrustPair()
    {
        var policy = CloudConfigurationTrustPolicy.FromEnvironmentValue("""
            [{
                "resourceManagerEndpoint": "http://management.example.invalid",
                "activeDirectoryAuthority": "http://login.example.invalid"
            }]
            """);

        var trustedCloud = CreateCloud("Custom", new("http://management.example.invalid", "http://login.example.invalid"));

        policy.IsTrusted(trustedCloud).Should().BeTrue();
        policy.IsAuthorityTrusted(trustedCloud.ActiveDirectoryAuthorityUri).Should().BeTrue();
        policy.IsTrusted(CreateCloud("Custom", new("https://management.example.invalid", "http://login.example.invalid"))).Should().BeFalse();
        new CloudConfigurationTrustPolicy().IsTrusted(trustedCloud).Should().BeFalse();
    }

    private static CloudProfile GetBuiltInProfile(string profileName) => profileName switch
    {
        "AzureCloud" => new("https://management.azure.com", "https://login.microsoftonline.com"),
        "AzureBleuCloud" => new("https://management.sovcloud-api.fr", "https://login.sovcloud-identity.fr"),
        "AzureChinaCloud" => new("https://management.chinacloudapi.cn", "https://login.chinacloudapi.cn"),
        "AzureGermanyCloud" => new("https://management.sovcloud-api.de", "https://login.sovcloud-identity.de"),
        "AzureUSGovernment" => new("https://management.usgovcloudapi.net", "https://login.microsoftonline.us"),
        _ => throw new ArgumentOutOfRangeException(nameof(profileName)),
    };

    private static CloudConfiguration CreateCloud(string profileName, CloudProfile profile)
    {
        var element = JsonElementFactory.CreateElement(new Cloud
        {
            CurrentProfileName = profileName,
            Profiles = new Dictionary<string, CloudProfile> { [profileName] = profile }.ToImmutableSortedDictionary(),
            CredentialPrecedence = [CredentialType.AzureCLI],
        });

        return CloudConfiguration.Bind(element);
    }
}
