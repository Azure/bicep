// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Core;
using Azure.ResourceManager;
using Bicep.Core.Configuration;
using Bicep.Core.AzureApi;
using Bicep.Core.Tracing;

namespace Bicep.Core.AzureApi;

public class ArmClientProvider(
    ITokenCredentialFactory credentialFactory
) : IArmClientProvider
{
    public ArmClient CreateArmClient(TokenCredential credential, string? defaultSubscriptionId, ArmClientOptions options)
    {
        return new ArmClient(credential, defaultSubscriptionId, options);
    }

    public ArmClient CreateArmClient(RootConfiguration configuration, string? defaultSubscriptionId)
    {
        var options = new ArmClientOptions();
        options.Diagnostics.ApplySharedResourceManagerSettings();
        options.Environment = new ArmEnvironment(configuration.Cloud.ResourceManagerEndpointUri, configuration.Cloud.AuthenticationScope);

        var credential = credentialFactory.CreateChain(configuration.Cloud.CredentialPrecedence, configuration.Cloud.CredentialOptions, configuration.Cloud.ActiveDirectoryAuthorityUri);
        return CreateArmClient(credential, defaultSubscriptionId, options);
    }
}