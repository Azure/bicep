// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.AzureApi;
using Bicep.Core.Configuration;
using Bicep.Core.Registry;
using Bicep.Core.UnitTests.Mock;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Configuration;

/// <summary>
/// Verifies that the cloud trust gate runs before any credential acquisition or client construction.
/// </summary>
[TestClass]
public class CloudTrustGateTests
{
    private static readonly CloudConfigurationTrustPolicy BuiltInOnlyTrustPolicy = new();

    [TestMethod]
    public void ArmClientProvider_WithUntrustedCloud_ThrowsBeforeAcquiringCredentials()
    {
        var credentialFactory = StrictMock.Of<ITokenCredentialFactory>();
        var provider = new ArmClientProvider(credentialFactory.Object, BuiltInOnlyTrustPolicy);

        FluentActions.Invoking(() => provider.CreateArmClient(CreateUntrustedConfiguration(), defaultSubscriptionId: null))
            .Should().Throw<InvalidOperationException>()
            .WithMessage($"*{CloudConfigurationTrustPolicy.TrustedCloudsEnvironmentVariable}*");

        credentialFactory.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void ContainerRegistryClientFactory_WithUntrustedCloud_ThrowsBeforeAcquiringCredentials()
    {
        var credentialFactory = StrictMock.Of<ITokenCredentialFactory>();
        var factory = new ContainerRegistryClientFactory(
            new RegistryConfiguration(PermitUntrustedRegistries: true),
            credentialFactory.Object,
            BuiltInOnlyTrustPolicy);
        var cloud = CreateUntrustedConfiguration().Cloud;
        var registryUri = new Uri("https://contoso.azurecr.io");

        FluentActions.Invoking(() => factory.CreateAuthenticatedBlobClient(cloud, registryUri, "test/repo"))
            .Should().Throw<InvalidOperationException>();
        FluentActions.Invoking(() => factory.CreateAnonymousBlobClient(cloud, registryUri, "test/repo"))
            .Should().Throw<InvalidOperationException>();
        FluentActions.Invoking(() => factory.CreateAuthenticatedContainerClient(cloud, registryUri))
            .Should().Throw<InvalidOperationException>();
        FluentActions.Invoking(() => factory.CreateAnonymousContainerClient(cloud, registryUri))
            .Should().Throw<InvalidOperationException>();

        credentialFactory.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void TokenCredentialFactory_WithAttackerControlledAdfsAuthority_ThrowsForEveryCredentialType()
    {
        var factory = new TokenCredentialFactory(BuiltInOnlyTrustPolicy);
        var authorityUri = new Uri("https://login.evil.attacker.com/adfs");

        foreach (var credentialType in Enum.GetValues<CredentialType>())
        {
            FluentActions.Invoking(() => factory.CreateSingle(credentialType, null, authorityUri))
                .Should().Throw<InvalidOperationException>()
                .WithMessage($"*{CloudConfigurationTrustPolicy.TrustedCloudsEnvironmentVariable}*");
        }

        FluentActions.Invoking(() => factory.CreateChain([CredentialType.Environment], null, authorityUri))
            .Should().Throw<InvalidOperationException>()
            .WithMessage($"*{CloudConfigurationTrustPolicy.TrustedCloudsEnvironmentVariable}*");
    }

    private static IBicepConfiguration CreateUntrustedConfiguration() => BicepTestConstants.CreateMockConfiguration(new()
    {
        ["cloud.currentProfile"] = "Custom",
        ["cloud.profiles.Custom.resourceManagerEndpoint"] = "https://management.evil.attacker.com",
        ["cloud.profiles.Custom.activeDirectoryAuthority"] = "https://login.evil.attacker.com/adfs",
    });
}
