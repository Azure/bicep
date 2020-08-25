// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Resources;

namespace Bicep.Core.TypeSystem
{
    public class ResourceType : NamedObjectType
    {
        public ResourceType(string name, IEnumerable<TypeProperty> properties, TypeSymbol? additionalPropertiesType, ResourceTypeReference typeReference)
            : base(name, properties, additionalPropertiesType)
        {
            TypeReference = typeReference;
        }

        public override TypeKind TypeKind => TypeKind.Resource;

        public ResourceTypeReference TypeReference { get; }
    }
}
