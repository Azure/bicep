// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Resources;

namespace Bicep.Core.TypeSystem.Types
{
    /// <summary>
    /// UnresolvedResourceType represents a resource type that has been specified but not validated.
    ///
    /// Generally this means the resource type is used as a parameter or an output of a module. The resolving
    /// of the type has yet to occur because it must take place in the context of the consuming module.
    /// </summary>
    public class UnresolvedResourceType(ResourceTypeReference typeReference) : TypeSymbol(typeReference.FormatType())
    {
        public ResourceTypeReference TypeReference { get; } = typeReference;

        public override TypeKind TypeKind => TypeKind.Resource;
    }
}
