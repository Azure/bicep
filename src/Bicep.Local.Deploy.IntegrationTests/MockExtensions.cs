// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Local.Extension.Host;
using Bicep.Local.Extension.Host.Handlers;
using Moq;

namespace Bicep.Local.Deploy.IntegrationTests;

public static class MockExtensions
{
    public static void SetupCreateOrUpdate<TResource>(this Mock<IResourceHandler<TResource>> handlerMock, Func<HandlerRequest<TResource>, HandlerResponse> responseFunc)
        where TResource : class
    {
        handlerMock.Setup(req => req.CreateOrUpdate(It.IsAny<HandlerRequest<TResource>>(), It.IsAny<CancellationToken>()))
            .Returns<HandlerRequest<TResource>, CancellationToken>((req, token) =>
                Task.FromResult(responseFunc(req)));

        handlerMock.Setup(req => req.CreateOrUpdate(It.IsAny<HandlerRequest>(), It.IsAny<CancellationToken>()))
            .Returns<HandlerRequest, CancellationToken>((req, token) => handlerMock.Object.CreateOrUpdate((HandlerRequest<TResource>)req, token));
    }

    public static void SetupCreateOrUpdate(this Mock<IResourceHandler> handlerMock, Func<HandlerRequest, HandlerResponse> responseFunc)
        => handlerMock.Setup(req => req.CreateOrUpdate(It.IsAny<HandlerRequest>(), It.IsAny<CancellationToken>()))
            .Returns<HandlerRequest, CancellationToken>((req, token) =>
                Task.FromResult(responseFunc(req)));
}
