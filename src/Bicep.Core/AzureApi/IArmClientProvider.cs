// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Core;
using Azure.ResourceManager;
using Bicep.Core.Configuration;

namespace Bicep.Core.AzureApi;

public interface IArmClientProvider
{
    ArmClient CreateArmClient(TokenCredential credential, string? defaultSubscriptionId, ArmClientOptions options);

    ArmClient CreateArmClient(RootConfiguration configuration, string? defaultSubscriptionId);
}

