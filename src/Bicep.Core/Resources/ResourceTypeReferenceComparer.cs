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

        public bool Equals(ResourceTypeReference? x, ResourceTypeReference? y)
            => x is not null &&
            y is not null &&
            LanguageConstants.ResourceTypeComparer.Equals(x.Namespace, y.Namespace) &&
            x.Types.Length == y.Types.Length &&
            Enumerable.SequenceEqual(x.Types, y.Types, LanguageConstants.ResourceTypeComparer) &&
            LanguageConstants.ResourceTypeComparer.Equals(x.ApiVersion, y.ApiVersion);

        public int GetHashCode(ResourceTypeReference obj)
            => LanguageConstants.ResourceTypeComparer.GetHashCode(obj.Namespace) ^
            Enumerable.Select(obj.Types, x => LanguageConstants.ResourceTypeComparer.GetHashCode(x)).Aggregate((a, b) => a ^ b) ^
            LanguageConstants.ResourceTypeComparer.GetHashCode(obj.ApiVersion);
    }
}
