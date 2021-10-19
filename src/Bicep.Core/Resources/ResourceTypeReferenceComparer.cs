// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Resources
{
    public class ResourceTypeReferenceComparer : IEqualityComparer<ResourceTypeReference>
    {
        private StringComparer TypeComparer = LanguageConstants.ResourceTypeComparer;

        public static IEqualityComparer<ResourceTypeReference> Instance { get; }
            = new ResourceTypeReferenceComparer();

        public bool Equals(ResourceTypeReference x, ResourceTypeReference y)
            => TypeComparer.Equals(x.Namespace, y.Namespace) &&
            x.Types.Length == y.Types.Length &&
            Enumerable.SequenceEqual(x.Types, y.Types, TypeComparer) &&
            TypeComparer.Equals(x.ApiVersion, y.ApiVersion);

        public int GetHashCode(ResourceTypeReference obj)
            => TypeComparer.GetHashCode(obj.Namespace) ^
            Enumerable.Select(obj.Types, x => TypeComparer.GetHashCode(x)).Aggregate((a, b) => a ^ b) ^
            TypeComparer.GetHashCode(obj.ApiVersion);
    }
}