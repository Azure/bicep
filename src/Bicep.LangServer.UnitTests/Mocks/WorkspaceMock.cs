// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests.Mock;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using System.Threading;

namespace Bicep.LangServer.UnitTests.Mocks
{
    public class WorkspaceMock
    {
        public Mock<IWorkspaceLanguageServer> Mock;

        public bool DoesClientSupportWorkspaceFolders { get; private set; }

        public WorkspaceMock(bool enableClientCapabilities = true)
        {
            Mock = StrictMock.Of<IWorkspaceLanguageServer>();

            if (enableClientCapabilities)
            {
                // These defaults will cause ClientCapability to indicate that these calls are available, but will throw if they are called (unless you
                //   override with your own callbacks).  This makes sense if you want to verify that the code makes no unexpected calls.
                // Use enableClientCapabilities=false if you know your code makes these calls but only after checking against the client capabilities

                OnRequestWorkspaceFoldersThrow(enableClientCapability: true);
            }
        }

        public WorkspaceMock OnRequestWorkspaceFoldersThrow(bool enableClientCapability)
        {
            Mock
                .Setup(m => m.SendRequest<Container<WorkspaceFolder>?>(It.IsAny<WorkspaceFolderParams>(), It.IsAny<CancellationToken>()))
                .Callback((IRequest<Container<WorkspaceFolder>?> request, CancellationToken token) =>
                {
                    Assert.Fail($"{nameof(WorkspaceMock)}: Unexpected RequestWorkspaceFolders call");
                });
            this.DoesClientSupportWorkspaceFolders = enableClientCapability;

            return this;
        }

        public WorkspaceMock OnRequestWorkspaceFolders(Container<WorkspaceFolder> workspaceFolders)
        {
            Mock
                .Setup(m => m.SendRequest<Container<WorkspaceFolder>?>(It.IsAny<WorkspaceFolderParams>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => workspaceFolders);

            return this;
        }
    }
}
