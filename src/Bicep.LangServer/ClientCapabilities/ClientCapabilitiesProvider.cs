// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

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
            return server.Workspace.ClientSettings.Capabilities is global::OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities.ClientCapabilities clientCapabilitites &&
                clientCapabilitites.Workspace is WorkspaceClientCapabilities workspaceClientCapabilities &&
                workspaceClientCapabilities.WorkspaceFolders.IsSupported;
        }

        public bool DoesClientSupportShowDocumentRequest()
        {
            return server.ClientSettings.Capabilities is global::OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities.ClientCapabilities clientCapabilities &&
                clientCapabilities.Window is WindowClientCapabilities windowClientCapabilities &&
                windowClientCapabilities.ShowDocument.IsSupported;
        }
    }
}
