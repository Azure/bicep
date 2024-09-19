// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.ResourceManager;
using Bicep.Core.Configuration;
using Bicep.Core.Registry.Auth;
using Bicep.Core.Tracing;

namespace Bicep.Deploy;

public class DeploymentManagerFactory : IDeploymentManagerFactory
{
    private readonly ITokenCredentialFactory tokenCredentialFactory;

    public DeploymentManagerFactory(ITokenCredentialFactory tokenCredentialFactory)
    {
        this.tokenCredentialFactory = tokenCredentialFactory;
    }

    public IDeploymentManager CreateDeploymentManager(RootConfiguration rootConfiguration)
    {
        var credential = tokenCredentialFactory
            .CreateChain(rootConfiguration.Cloud.CredentialPrecedence, rootConfiguration.Cloud.CredentialOptions, rootConfiguration.Cloud.ActiveDirectoryAuthorityUri);

        var options = new ArmClientOptions();
        options.Diagnostics.ApplySharedResourceManagerSettings();
        options.Environment = new ArmEnvironment(rootConfiguration.Cloud.ResourceManagerEndpointUri, rootConfiguration.Cloud.AuthenticationScope);

        return new DeploymentManager(new ArmClient(credential));
    }
}
