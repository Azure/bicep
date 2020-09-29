// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Resources;

namespace Bicep.Core.TypeSystem
{
    public class ResourceType : TypeSymbol
    {
        public ResourceType(ResourceTypeReference typeReference, ITypeReference body, TypeSymbolValidationFlags validationFlags)
            : base(typeReference.FormatName())
        {
            TypeReference = typeReference;
            Body = body;
            ValidationFlags = validationFlags;
        }

        public override TypeKind TypeKind => TypeKind.Resource;

        public ResourceTypeReference TypeReference { get; }

        public ITypeReference Body { get; }

        public override TypeSymbolValidationFlags ValidationFlags { get; }
    }
}
