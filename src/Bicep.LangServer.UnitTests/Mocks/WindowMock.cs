// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests.Mock;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using System;
using System.Threading;

namespace Bicep.LangServer.UnitTests.Mocks
{
    public class WindowMock
    {
        public Mock<IWindowLanguageServer> Mock;

        public bool DoesClientSupportShowDocumentRequest { get; private set; }

        public WindowMock(bool enableClientCapabilities = true)
        {
            Mock = StrictMock.Of<IWindowLanguageServer>();

            // Ignore all Log calls
            Mock.Setup(m => m.SendNotification(It.IsAny<LogMessageParams>()));

            if (enableClientCapabilities)
            {
                // These defaults will cause ClientCapability to indicate that these calls are available, but will throw if they are called (unless you
                //   override with your own callbacks).  This makes sense if you want to verify that the code makes no unexpected calls.
                // Use enableClientCapabilities=false if you know your code makes these calls but only after checking against the client capabilities

                OnShowMessageThrow();
                OnShowDocumentThrow(enableClientCapability: true);
            }
        }

        // No enableClientCapability parameter because the capability appears to always be assumed
        public WindowMock OnShowMessageThrow()
        {
            Mock
              .Setup(m => m.SendNotification(It.IsAny<ShowMessageParams>()))
              .Callback((IRequest request) =>
              {
                  var @params = (ShowMessageParams)request;
                  Assert.Fail($"{nameof(WindowMock)}: Unexpected ShowMessage call: {@params.Message} [{@params.Type}]");
              });

            return this;
        }

        // No enableClientCapability parameter because the capability appears to always be assumed
        public WindowMock OnShowMessage(Action<ShowMessageParams> callback)
        {
            Mock
                .Setup(m => m.SendNotification(It.IsAny<ShowMessageParams>()))
                .Callback((IRequest request) =>
                {
                    var @params = (ShowMessageParams)request;
                    callback(@params);
                });

            return this;
        }

        public WindowMock OnShowDocumentThrow(bool enableClientCapability)
        {
            Mock
                .Setup(m => m.SendRequest<ShowDocumentResult>(It.IsAny<ShowDocumentParams>(), It.IsAny<CancellationToken>()))
                .Callback((IRequest<ShowDocumentResult> request, CancellationToken token) =>
                {
                    var @params = (ShowDocumentParams)request;
                    Assert.Fail($"{nameof(WindowMock)}: Unexpected ShowDocument call: {@params.Uri.ToString()}");
                });
            this.DoesClientSupportShowDocumentRequest = enableClientCapability;

            return this;
        }

        public WindowMock OnShowDocument(Action<ShowDocumentParams>? callback = null, ShowDocumentResult? returnValue = null, bool enableClientCapability = true)
        {
            Mock
                .Setup(m => m.SendRequest<ShowDocumentResult>(It.IsAny<ShowDocumentParams>(), It.IsAny<CancellationToken>()))
                .Callback((IRequest<ShowDocumentResult> request, CancellationToken token) =>
                {
                    if (callback is not null)
                    {
                        var @params = (ShowDocumentParams)request;
                        callback(@params);
                    }
                })
                .ReturnsAsync(() => returnValue ?? new ShowDocumentResult() { Success = true });
            this.DoesClientSupportShowDocumentRequest = enableClientCapability;

            return this;
        }
    }
}
