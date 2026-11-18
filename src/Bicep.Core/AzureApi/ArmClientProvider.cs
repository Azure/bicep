// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Core;
using Azure.ResourceManager;
using Bicep.Core.AzureApi;
using Bicep.Core.Configuration;
using Bicep.Core.Tracing;

namespace Bicep.Core.AzureApi;

public class ArmClientProvider(
    ITokenCredentialFactory credentialFactory,
    CloudConfigurationTrustPolicy cloudTrustPolicy
) : IArmClientProvider
{
    public ArmClient CreateArmClient(TokenCredential credential, string? defaultSubscriptionId, ArmClientOptions options)
    {
        return new ArmClient(credential, defaultSubscriptionId, options);
    }

    public ArmClient CreateArmClient(IBicepCloudConfiguration cloud, string? defaultSubscriptionId)
    {
        cloudTrustPolicy.ThrowIfCloudIsUntrusted(cloud);

        var options = new ArmClientOptions();
        options.Diagnostics.ApplySharedResourceManagerSettings();
        options.Environment = new ArmEnvironment(cloud.ResourceManagerEndpointUri, cloud.AuthenticationScope);

        var credential = credentialFactory.CreateChain(cloud.CredentialPrecedence, cloud.CredentialOptions, cloud.ActiveDirectoryAuthorityUri);
        return CreateArmClient(credential, defaultSubscriptionId, options);
    }
}
