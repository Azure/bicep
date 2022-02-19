// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Resources;

namespace Bicep.Core.TypeSystem
{
    /// <summary>
    /// UnboundResourceType represents a resource type that has been specified but not validated.
    ///
    /// Generally this means the resource type is used as a parameter or an output of a module. The binding
    /// of the type has yet to occur because it must take place in the context of the consuming module.
    /// </summary>
    public class UnboundResourceType : TypeSymbol
    {
        public UnboundResourceType(ResourceTypeReference typeReference)
            : base(typeReference.FormatType())
        {
            this.TypeReference = typeReference;
        }

        public ResourceTypeReference TypeReference { get; }

        public override TypeKind TypeKind => TypeKind.Resource;
    }
}
