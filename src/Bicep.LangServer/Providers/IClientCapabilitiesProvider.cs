// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.LanguageServer.Providers
{
    public interface IClientCapabilitiesProvider
    {
        bool DoesClientSupportWorkspaceFolders();
        bool DoesClientSupportShowDocumentRequest();
    }
}
