// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.UnitTests.Mock;
using Bicep.LanguageServer.Providers;
using Moq;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace Bicep.LangServer.UnitTests.Mocks
{
    public class LanguageServerMock
    {
        private readonly ClientCapabilitiesProviderMock clientCapabilitiesProviderMock;

        public Mock<ILanguageServerFacade> Mock { get; private set; }
        public WindowMock WindowMock { get; private set; }
        public WorkspaceMock WorkspaceMock { get; private set; }

        public IClientCapabilitiesProvider ClientCapabilitiesProvider => clientCapabilitiesProviderMock;

        private class ClientCapabilitiesProviderMock : IClientCapabilitiesProvider
        {
            private readonly LanguageServerMock serverMock;

            public ClientCapabilitiesProviderMock(LanguageServerMock serverMock)
            {
                this.serverMock = serverMock;
            }

            bool IClientCapabilitiesProvider.DoesClientSupportShowDocumentRequest() => serverMock.WindowMock.DoesClientSupportShowDocumentRequest;

            bool IClientCapabilitiesProvider.DoesClientSupportWorkspaceFolders() => serverMock.WorkspaceMock.DoesClientSupportWorkspaceFolders;
        }


        public LanguageServerMock()
        {
            clientCapabilitiesProviderMock = new ClientCapabilitiesProviderMock(this);

            WindowMock = new WindowMock();
            WorkspaceMock = new WorkspaceMock();

            var server = StrictMock.Of<ILanguageServerFacade>();
            server
                .Setup(m => m.Window)
                .Returns(this.WindowMock.Mock.Object);
            server
                .Setup(m => m.Workspace)
                .Returns(WorkspaceMock.Mock.Object);

            Mock = server;
        }

        public LanguageServerMock WithWindowMock(WindowMock windowMock)
        {
            WindowMock = windowMock;
            return this;
        }
    }
}
