// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Local.Extension.Host.Handlers;

/// <summary>
/// Defines a contract for handlers that process operations on resources.
/// Resource handlers are responsible for creating, updating, previewing, deleting, 
/// and retrieving resources in the Bicep local extension system.
/// </summary>
public interface IResourceHandler
{
    /// <summary>
    /// Creates a new resource or updates an existing resource based on the specified request.
    /// </summary>
    /// <param name="request">The request containing resource type, API version, extension settings, and resource JSON.</param>
    /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
    /// <returns>A response containing the result of the create or update operation, including status and properties.</returns>
    Task<HandlerResponse> CreateOrUpdate(
       HandlerRequest request,
       CancellationToken cancellationToken);

    /// <summary>
    /// Previews the changes that would occur from a create or update operation without actually making the changes.
    /// </summary>
    /// <param name="request">The request containing resource type, API version, extension settings, and resource JSON.</param>
    /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
    /// <returns>A response indicating what changes would occur if the operation were performed.</returns>
    Task<HandlerResponse> Preview(
        HandlerRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Deletes an existing resource.
    /// </summary>
    /// <param name="request">The request containing resource type, API version, extension settings, and resource JSON.</param>
    /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
    /// <returns>A response indicating the result of the delete operation.</returns>
    Task<HandlerResponse> Delete(
        HandlerRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves an existing resource.
    /// </summary>
    /// <param name="request">The request containing resource type, API version, extension settings, and resource JSON.</param>
    /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
    /// <returns>A response containing the retrieved resource's properties or an error if the resource doesn't exist.</returns>
    Task<HandlerResponse> Get(
        HandlerRequest request,
        CancellationToken cancellationToken);
}

/// <summary>
/// Defines a strongly-typed resource handler for processing operations on resources of a specific type.
/// This generic interface extends <see cref="IResourceHandler"/> to provide type safety for resource operations.
/// </summary>
/// <typeparam name="TResource">The strongly-typed resource model. Must be a reference type.</typeparam>
/// <remarks>
/// When implementing this interface:
/// - The type parameter TResource should represent your specific resource model
/// - You must implement the strongly-typed CreateOrUpdate and Preview methods
/// - The base interface implementations will automatically cast the request to your strongly-typed version
/// - You may need to implement Delete and Get from the base interface directly
/// </remarks>
public interface IResourceHandler<TResource>
    : IResourceHandler
    where TResource : class
{
    /// <summary>
    /// Creates a new resource or updates an existing resource based on the specified strongly-typed request.
    /// </summary>
    /// <param name="request">The strongly-typed request containing the resource instance and metadata.</param>
    /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
    /// <returns>A response containing the result of the create or update operation, including status and properties.</returns>
    Task<HandlerResponse> CreateOrUpdate(
       HandlerRequest<TResource> request,
       CancellationToken cancellationToken);

    /// <summary>
    /// Previews the changes that would occur from a create or update operation without actually making the changes.
    /// </summary>
    /// <param name="request">The strongly-typed request containing the resource instance and metadata.</param>
    /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
    /// <returns>A response indicating what changes would occur if the operation were performed.</returns>
    Task<HandlerResponse> Preview(
        HandlerRequest<TResource> request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Implementation of the non-generic <see cref="IResourceHandler.CreateOrUpdate"/> method that delegates to the strongly-typed version.
    /// </summary>
    /// <param name="request">The request to be cast to a strongly-typed request.</param>
    /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
    /// <returns>The response from the strongly-typed CreateOrUpdate method.</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null or cannot be cast to <see cref="HandlerRequest{TResource}"/>.</exception>
    Task<HandlerResponse> IResourceHandler.CreateOrUpdate(HandlerRequest request, CancellationToken cancellationToken)
        => CreateOrUpdate(request as HandlerRequest<TResource> ?? throw new ArgumentNullException(nameof(request)), cancellationToken);

    /// <summary>
    /// Implementation of the non-generic <see cref="IResourceHandler.Preview"/> method that delegates to the strongly-typed version.
    /// </summary>
    /// <param name="request">The request to be cast to a strongly-typed request.</param>
    /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
    /// <returns>The response from the strongly-typed Preview method.</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null or cannot be cast to <see cref="HandlerRequest{TResource}"/>.</exception>
    Task<HandlerResponse> IResourceHandler.Preview(HandlerRequest request, CancellationToken cancellationToken)
        => Preview(request as HandlerRequest<TResource> ?? throw new ArgumentNullException(nameof(request)), cancellationToken);
}
