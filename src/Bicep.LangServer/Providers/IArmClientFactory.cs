// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Core;
using Azure.ResourceManager;

namespace Bicep.LanguageServer.Providers
{
    public interface IArmClientProvider {
        public ArmClient createArmClient(TokenCredential credential, string? defaultSubscriptionId, ArmClientOptions options);
    }
}
