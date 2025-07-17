// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Local.Extension.Host.Handlers;

/// <summary>
/// Defines a contract for handlers that process operations on resources.
/// Resource handlers are responsible for creating, updating, previewing, deleting, 
/// and retrieving resources in the Bicep local extension system.
/// </summary>
/// <remarks>
/// Implement this interface when you need to handle any resource type generically.
/// For strongly-typed resource handling, implement <see cref="IResourceHandler{TResource}"/> instead.
/// </remarks>
public interface IResourceHandler
{
    /// <summary>
    /// Creates a new resource or updates an existing resource based on the specified request.
    /// </summary>
    /// <param name="request">The request containing resource type, properties, identifiers, configuration, and API version.</param>
    /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
    /// <returns>A response containing the result of the create or update operation, including status and updated properties.</returns>
    /// <remarks>
    /// This method should handle both create and update scenarios based on whether the resource already exists.
    /// Use the request.Identifiers to determine if this is a create or update operation.
    /// </remarks>
    Task<HandlerResponse> CreateOrUpdate(
       HandlerRequest request,
       CancellationToken cancellationToken);

    /// <summary>
    /// Previews the changes that would occur from a create or update operation without actually making the changes.
    /// </summary>
    /// <param name="request">The request containing resource type, properties, identifiers, configuration, and API version.</param>
    /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
    /// <returns>A response indicating what changes would occur if the operation were performed, typically with proposed properties.</returns>
    /// <remarks>
    /// This method should simulate the create or update operation and return what the resource would look like
    /// without actually persisting any changes. This is used for planning and validation purposes.
    /// </remarks>
    Task<HandlerResponse> Preview(
        HandlerRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Deletes an existing resource.
    /// </summary>
    /// <param name="request">The request containing resource type, identifiers, configuration, and API version needed to locate and delete the resource.</param>
    /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
    /// <returns>A response indicating the result of the delete operation.</returns>
    /// <remarks>
    /// Use the request.Identifiers to locate the specific resource instance to delete.
    /// The request.Properties may be empty for delete operations as only identifiers are typically needed.
    /// </remarks>
    Task<HandlerResponse> Delete(
        HandlerRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves an existing resource.
    /// </summary>
    /// <param name="request">The request containing resource type, identifiers, configuration, and API version needed to locate the resource.</param>
    /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
    /// <returns>A response containing the retrieved resource's current properties and identifiers, or an error if the resource doesn't exist.</returns>
    /// <remarks>
    /// Use the request.Identifiers to locate the specific resource instance to retrieve.
    /// The request.Properties may be empty for get operations as only identifiers are typically needed.
    /// </remarks>
    Task<HandlerResponse> Get(
        HandlerRequest request,
        CancellationToken cancellationToken);
}

/// <summary>
/// Defines a strongly-typed resource handler for processing operations on resources of a specific type.
/// This generic interface extends <see cref="IResourceHandler"/> to provide type safety for resource operations.
/// </summary>
/// <typeparam name="TResource">The strongly-typed resource model that represents the specific resource type. Must be a reference type.</typeparam>
/// <remarks>
/// When implementing this interface:
/// - The type parameter TResource should represent your specific resource model (e.g., StorageAccount, VirtualMachine)
/// - You must implement the strongly-typed CreateOrUpdate and Preview methods
/// - The base interface implementations are automatically provided and will cast requests to your strongly-typed version
/// - You still need to implement Delete and Get from the base interface directly since they typically work with identifiers only
/// </remarks>
/// <example>
/// <code>
/// public class StorageAccountHandler : IResourceHandler&lt;StorageAccount&gt;
/// {
///     public async Task&lt;HandlerResponse&gt; CreateOrUpdate(HandlerRequest&lt;StorageAccount&gt; request, CancellationToken cancellationToken)
///     {
///         var account = request.Resource;
///         // Implement storage account creation/update logic
///         return HandlerResponse.Success(request.Type, updatedProperties, identifiers, request.ApiVersion);
///     }
///     
///     // Implement other required methods...
/// }
/// </code>
/// </example>
public interface IResourceHandler<TResource> : IResourceHandler
    where TResource : class
{
    /// <summary>
    /// Creates a new resource or updates an existing resource based on the specified strongly-typed request.
    /// </summary>
    /// <param name="request">The strongly-typed request containing the deserialized resource instance along with metadata.</param>
    /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
    /// <returns>A response containing the result of the create or update operation, including status and updated properties.</returns>
    /// <remarks>
    /// This method provides strongly-typed access to the resource via request.Resource, while still having access
    /// to the raw JSON properties, identifiers, and configuration through the base request properties.
    /// </remarks>
    Task<HandlerResponse> CreateOrUpdate(
       HandlerRequest<TResource> request,
       CancellationToken cancellationToken);

    /// <summary>
    /// Previews the changes that would occur from a create or update operation without actually making the changes.
    /// </summary>
    /// <param name="request">The strongly-typed request containing the deserialized resource instance along with metadata.</param>
    /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
    /// <returns>A response indicating what changes would occur if the operation were performed, typically with proposed properties.</returns>
    /// <remarks>
    /// This method provides strongly-typed access to the resource via request.Resource for validation and planning purposes.
    /// </remarks>
    Task<HandlerResponse> Preview(
        HandlerRequest<TResource> request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Implementation of the non-generic <see cref="IResourceHandler.CreateOrUpdate"/> method that delegates to the strongly-typed version.
    /// </summary>
    /// <param name="request">The request to be cast to a strongly-typed request.</param>
    /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
    /// <returns>The response from the strongly-typed CreateOrUpdate method.</returns>
    /// <exception cref="InvalidCastException">Thrown when request cannot be cast to <see cref="HandlerRequest{TResource}"/>.</exception>
    Task<HandlerResponse> IResourceHandler.CreateOrUpdate(HandlerRequest request, CancellationToken cancellationToken)
        => CreateOrUpdate((HandlerRequest<TResource>)request, cancellationToken);

    /// <summary>
    /// Implementation of the non-generic <see cref="IResourceHandler.Preview"/> method that delegates to the strongly-typed version.
    /// </summary>
    /// <param name="request">The request to be cast to a strongly-typed request.</param>
    /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
    /// <returns>The response from the strongly-typed Preview method.</returns>
    /// <exception cref="InvalidCastException">Thrown when request cannot be cast to <see cref="HandlerRequest{TResource}"/>.</exception>
    Task<HandlerResponse> IResourceHandler.Preview(HandlerRequest request, CancellationToken cancellationToken)
        => Preview((HandlerRequest<TResource>)request, cancellationToken);
}
