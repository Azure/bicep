// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Local.Extension.Protocol;
using Moq;
using Protocol = Bicep.Local.Extension.Protocol;

namespace Bicep.Local.Deploy.IntegrationTests;

public static class MockExtensions
{
    public static void SetupCreateOrUpdate<THandler>(this Mock<THandler> handlerMock, Func<Protocol.ResourceSpecification, Protocol.LocalExtensionOperationResponse> responseFunc)
        where THandler : class, IGenericResourceHandler
        => handlerMock.Setup(x => x.CreateOrUpdate(It.IsAny<Protocol.ResourceSpecification>(), It.IsAny<CancellationToken>()))
            .Returns<Protocol.ResourceSpecification, CancellationToken>((req, _) =>
                Task.FromResult(responseFunc(req)));
}
