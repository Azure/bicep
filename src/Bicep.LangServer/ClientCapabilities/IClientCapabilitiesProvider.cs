// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.LanguageServer.ClientCapabilities
{
    public interface IClientCapabilitiesProvider
    {
        bool DoesClientSupportWorkspaceFolders();
        bool DoesClientSupportShowDocumentRequest();
    }
}
