// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Resources
{
    public class ResourceTypeReferenceComparer : IEqualityComparer<ResourceTypeReference>
    {
        public static IEqualityComparer<ResourceTypeReference> Instance { get; }
            = new ResourceTypeReferenceComparer();

        public bool Equals(ResourceTypeReference x, ResourceTypeReference y)
            => StringComparer.OrdinalIgnoreCase.Equals(x.Namespace, y.Namespace) &&
            x.Types.Length == y.Types.Length &&
            Enumerable.SequenceEqual(x.Types, y.Types, StringComparer.OrdinalIgnoreCase) &&
            StringComparer.OrdinalIgnoreCase.Equals(x.ApiVersion, y.ApiVersion);

        public int GetHashCode(ResourceTypeReference obj)
            => StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Namespace) ^
            Enumerable.Select(obj.Types, x => StringComparer.OrdinalIgnoreCase.GetHashCode(x)).Aggregate((a, b) => a ^ b) ^
            StringComparer.OrdinalIgnoreCase.GetHashCode(obj.ApiVersion);
    }
}