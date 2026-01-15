// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Local.Extension.Types.Attributes;
using Bicep.Local.Extension.Types.Models;

namespace Bicep.Local.Extension.Types;

/// <summary>
/// Defines a contract for discovering resource types for Bicep extensions.
/// </summary>
/// <remarks>
/// Implementations of <see cref="ITypeProvider"/> are responsible for returning applicable types to
/// be used in Bicep extensions, typically those annotated with <see cref="ResourceTypeAttribute"/>.
/// </remarks>
public interface ITypeProvider
{
    /// <summary>
    /// Retrieves all resource type definitions available to the extension.
    /// </summary>
    /// <param name="throwOnDuplicate">Indicates whether to throw an exception if duplicate resource types are found.</param>
    /// <returns>An enumerable collection of resource type definitions.</returns>
    IEnumerable<ResourceTypeDefinitionDetails> GetResourceTypes(bool throwOnDuplicate = true);

    /// <summary>
    /// Retrieves the fallback resource type definition, if one has been registered.
    /// </summary>
    /// <returns>The fallback resource type definition, or <see langword="null"/> if no fallback type is registered.</returns>
    ResourceTypeDefinitionDetails? GetFallbackType();
}
