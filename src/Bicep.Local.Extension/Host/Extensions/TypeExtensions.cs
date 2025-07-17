// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Local.Extension.Host.Handlers;

namespace Bicep.Local.Extension.Host.Extensions;

/// <summary>
/// Provides extension methods for working with Type objects, particularly for identifying and working with 
/// resource handler implementations.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Determines whether the specified type implements the non-generic IResourceHandler interface.
    /// Used to identify generic/untyped resource handlers that can process any resource type without strong typing.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>True if the type implements the non-generic IResourceHandler interface; otherwise, false.</returns>
    /// <remarks>
    /// This method identifies handlers that implement <see cref="IResourceHandler"/> directly (without generic parameters).
    /// Such handlers are typically used as fallback handlers that can process any resource type.
    /// </remarks>
    public static bool IsGenericTypedResourceHandler(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type, nameof(type));

        return type.GetInterfaces().Any(i => !i.IsGenericType && i == typeof(IResourceHandler));
    }

    /// <summary>
    /// Attempts to get the generic IResourceHandler&lt;T&gt; interface implemented by the specified type.
    /// Used to identify strongly-typed resource handlers that are constrained to handle specific resource types.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <param name="resourceHandlerInterface">When this method returns, contains the generic IResourceHandler&lt;T&gt;
    /// interface if found; otherwise, null.</param>
    /// <returns>True if the type implements a generic IResourceHandler&lt;T&gt; interface; otherwise, false.</returns>
    /// <remarks>
    /// This method identifies handlers that implement <see cref="IResourceHandler{T}"/> where T is a specific resource type.
    /// Such handlers provide strong typing and are used for type-safe resource operations.
    /// </remarks>
    public static bool TryGetTypedResourceHandlerInterface(this Type type, [NotNullWhen(true)] out Type? resourceHandlerInterface)
    {
        ArgumentNullException.ThrowIfNull(type, nameof(type));

        resourceHandlerInterface = type.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IResourceHandler<>));

        return resourceHandlerInterface is not null;
    }
}
