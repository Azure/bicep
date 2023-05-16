// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Core;
using Azure.ResourceManager;

namespace Bicep.LanguageServer.Providers
{
    public class ArmClientProvider : IArmClientProvider
    {
        public ArmClient createArmClient(TokenCredential credential, string? defaultSubscriptionId, ArmClientOptions options)
        {
            return new ArmClient(credential, defaultSubscriptionId, options);
        }
    }
}
