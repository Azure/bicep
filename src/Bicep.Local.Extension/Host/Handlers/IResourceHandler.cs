// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Local.Extension.Host.Handlers;
public interface IResourceHandler
{
    Task<HandlerResponse> CreateOrUpdate(
       HandlerRequest request,
       CancellationToken cancellationToken);

    Task<HandlerResponse> Preview(
        HandlerRequest request,
        CancellationToken cancellationToken);

    Task<HandlerResponse> Delete(
        HandlerRequest request,
        CancellationToken cancellationToken);

    Task<HandlerResponse> Get(
        HandlerRequest request,
        CancellationToken cancellationToken);
}

public interface IResourceHandler<TResource>
    : IResourceHandler
    where TResource : class
{
    Task<HandlerResponse> CreateOrUpdate(
       HandlerRequest<TResource> request,
       CancellationToken cancellationToken);

    Task<HandlerResponse> Preview(
        HandlerRequest<TResource> request,
        CancellationToken cancellationToken);

    Task<HandlerResponse> IResourceHandler.CreateOrUpdate(HandlerRequest request, CancellationToken cancellationToken)
        => CreateOrUpdate(request as HandlerRequest<TResource> ?? throw new ArgumentNullException(nameof(request)), cancellationToken);

    Task<HandlerResponse> IResourceHandler.Preview(HandlerRequest request, CancellationToken cancellationToken)
        => Preview(request as HandlerRequest<TResource> ?? throw new ArgumentNullException(nameof(request)), cancellationToken);
}
