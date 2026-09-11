// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Configuration;
using Bicep.Core.Json;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Configuration;

[TestClass]
public class CloudConfigurationTrustPolicyRegistrationTests
{
    private const string CustomCloudTrustJson = """
        [{
            "resourceManagerEndpoint": "https://management.example.invalid",
            "activeDirectoryAuthority": "https://login.example.invalid"
        }]
        """;

    [TestMethod]
    public void TrustPolicyIsBuiltFromTheTrustedCloudsEnvironmentVariable()
    {
        var policy = BuildPolicy((CloudConfigurationTrustPolicy.TrustedCloudsEnvironmentVariable, CustomCloudTrustJson));

        policy.IsTrusted(CreateCustomCloud("https://management.example.invalid", "https://login.example.invalid")).Should().BeTrue();
        policy.IsTrusted(CreateCustomCloud("https://management.example.invalid", "https://login.other.invalid")).Should().BeFalse();
    }

    [TestMethod]
    [DataRow("BICEP_TRUSTED_CLOUD")]
    [DataRow("BICEP_TRUSTED_CLOUDS_")]
    [DataRow("bicep_trusted_clouds")]
    public void MisspelledEnvironmentVariableGrantsNoTrust(string variableName)
    {
        var policy = BuildPolicy((variableName, CustomCloudTrustJson));

        policy.IsTrusted(CreateCustomCloud("https://management.example.invalid", "https://login.example.invalid")).Should().BeFalse();
        policy.IsTrusted((CloudConfiguration)BicepConfiguration.BuiltIn.Cloud).Should().BeTrue();
    }

    [TestMethod]
    public void UnsetEnvironmentVariableGrantsNoAdditionalTrust()
    {
        var policy = BuildPolicy();

        policy.IsTrusted(CreateCustomCloud("https://management.example.invalid", "https://login.example.invalid")).Should().BeFalse();
        policy.IsTrusted((CloudConfiguration)BicepConfiguration.BuiltIn.Cloud).Should().BeTrue();
    }

    private static CloudConfigurationTrustPolicy BuildPolicy(params (string key, string? value)[] variables)
        => new ServiceBuilder()
            .WithRegistration(services => services.WithEnvironmentVariables(variables))
            .Build()
            .Construct<CloudConfigurationTrustPolicy>();

    private static CloudConfiguration CreateCustomCloud(string resourceManagerEndpoint, string activeDirectoryAuthority)
    {
        var element = JsonElementFactory.CreateElement(new Cloud
        {
            CurrentProfileName = "Custom",
            Profiles = new Dictionary<string, CloudProfile>
            {
                ["Custom"] = new(resourceManagerEndpoint, activeDirectoryAuthority),
            }.ToImmutableSortedDictionary(),
            CredentialPrecedence = [CredentialType.AzureCLI],
        });

        return CloudConfiguration.Bind(element);
    }
}
