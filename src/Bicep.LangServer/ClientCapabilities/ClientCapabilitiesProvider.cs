// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using LspClientCapabilities = OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities.ClientCapabilities;

namespace Bicep.LanguageServer.ClientCapabilities
{
    public class ClientCapabilitiesProvider : IClientCapabilitiesProvider
    {
        private readonly ILanguageServerFacade server;

        public ClientCapabilitiesProvider(ILanguageServerFacade server)
        {
            this.server = server;
        }

        public bool DoesClientSupportWorkspaceFolders()
        {
            return server.Workspace.ClientSettings.Capabilities is LspClientCapabilities clientCapabilitites &&
                clientCapabilitites.Workspace is WorkspaceClientCapabilities workspaceClientCapabilities &&
                workspaceClientCapabilities.WorkspaceFolders.IsSupported;
        }

        public bool DoesClientSupportShowDocumentRequest()
        {
            return server.ClientSettings.Capabilities is LspClientCapabilities clientCapabilities &&
                clientCapabilities.Window is WindowClientCapabilities windowClientCapabilities &&
                windowClientCapabilities.ShowDocument.IsSupported;
        }
    }
}
