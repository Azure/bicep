// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem;

namespace Bicep.LanguageServer.Features.Custom.Visualization
{
    /// <summary>
    /// Resolves the scopes at which resource types can be deployed (declared without <c>existing</c>).
    /// </summary>
    internal interface IResourceWritableScopeResolver
    {
        /// <summary>
        /// Returns the writable scopes of each type. Types the resolver cannot find are omitted.
        /// </summary>
        ImmutableDictionary<ResourceTypeReference, ResourceScope> Resolve(IEnumerable<ResourceTypeReference> references);
    }
}
