// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Bicep.Core.Resources
{
    public class ResourceTypeReferenceComparer : IEqualityComparer<ResourceTypeReference>
    {
        public static IEqualityComparer<ResourceTypeReference> Instance { get; }
            = new ResourceTypeReferenceComparer();

        public bool Equals(ResourceTypeReference x, ResourceTypeReference y)
            => Enumerable.SequenceEqual(x.TypeSegments, y.TypeSegments, StringComparer.OrdinalIgnoreCase) &&
                StringComparer.OrdinalIgnoreCase.Equals(x.Version, y.Version);

        public int GetHashCode(ResourceTypeReference obj)
            => Enumerable.Select(obj.TypeSegments, x => StringComparer.OrdinalIgnoreCase.GetHashCode(x)).Aggregate((a, b) => a ^ b) ^
                StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Version);
    }
}