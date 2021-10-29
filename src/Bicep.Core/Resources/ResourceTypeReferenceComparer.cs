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

        public bool Equals(ResourceTypeReference? x, ResourceTypeReference? y)
        {
            if (x is null || y is null)
            {
                return x == y;
            }

            return Enumerable.SequenceEqual(x.TypeSegments, y.TypeSegments, StringComparer.OrdinalIgnoreCase) &&
                StringComparer.OrdinalIgnoreCase.Equals(x.ApiVersion, y.ApiVersion);
        }

        public int GetHashCode(ResourceTypeReference obj)
            => HashCode.Combine(
                Enumerable.Select(obj.TypeSegments, x => StringComparer.OrdinalIgnoreCase.GetHashCode(x)).Aggregate((a, b) => a ^ b),
                (obj.ApiVersion is null ? 0 : StringComparer.OrdinalIgnoreCase.GetHashCode(obj.ApiVersion)));
    }
}
