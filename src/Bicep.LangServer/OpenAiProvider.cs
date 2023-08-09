// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Azure.ResourceManager;
using Bicep.Core.Tracing;
using Bicep.Core.Registry.Auth;
using Bicep.Core.Configuration;
using Azure.AI.OpenAI;
using Azure;

namespace Bicep.LanguageServer.Providers;

public class OpenAiProvider
{
    private readonly ITokenCredentialFactory credentialFactory;

    public OpenAiProvider(ITokenCredentialFactory credentialFactory)
    {
        this.credentialFactory = credentialFactory;
    }

    public OpenAIClient CreateClient(RootConfiguration configuration, Uri endpoint, string? openAiKey)
    {
        var options = new ArmClientOptions();
        options.Diagnostics.ApplySharedResourceManagerSettings();
        options.Environment = new ArmEnvironment(configuration.Cloud.ResourceManagerEndpointUri, configuration.Cloud.AuthenticationScope);

        if (openAiKey is {})
        {
            return new OpenAIClient(endpoint, new AzureKeyCredential(openAiKey));
        }

        var credential = this.credentialFactory.CreateChain(configuration.Cloud.CredentialPrecedence, configuration.Cloud.ActiveDirectoryAuthorityUri);
        return new OpenAIClient(endpoint, credential);
    }
}