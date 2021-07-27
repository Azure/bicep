// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;

namespace Bicep.Core.Utils
{
    internal class ResourceDefinition
    {
        public string ResourceName { get; }
        public ResourceMetadata? ResourceScope { get; }
        public string FullyQualifiedResourceType { get; }
        public StringSyntax ResourceNamePropertyValue { get; }

        public ResourceDefinition(string resourceName, ResourceMetadata? resourceScope, string fullyQualifiedResourceType, StringSyntax resourceNamePropertyValue)
        {
            ResourceName = resourceName;
            ResourceScope = resourceScope;
            FullyQualifiedResourceType = fullyQualifiedResourceType;
            ResourceNamePropertyValue = resourceNamePropertyValue;
        }


        public static readonly IEqualityComparer<ResourceDefinition> EqualityComparer = new ResourceComparer();
        // comparers below are very simple now, however in future it might be used to do more exact comparsion on property value to include interpolations
        // also, we expect StringSyntax as values it can be other types as well (function calls, variable accesses, etc.)
        private class ResourceComparer : IEqualityComparer<ResourceDefinition>
        {

            public bool Equals(ResourceDefinition x, ResourceDefinition y)
            {

                if (!string.Equals(x.FullyQualifiedResourceType, y.FullyQualifiedResourceType, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                if (x.ResourceScope != y.ResourceScope)
                {
                    return false;
                }

                var xv = x.ResourceNamePropertyValue.TryGetLiteralValue();
                var yv = y.ResourceNamePropertyValue.TryGetLiteralValue();

                //if literal value is null, we assume resources are not equal, as this indicates that interpolated value is used
                //and as for now we're unable to determine if they will have equal values or not.
                return xv is not null && yv is not null && string.Equals(xv, yv, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(ResourceDefinition obj)
            {
                var hc = new HashCode();
                hc.Add(obj.FullyQualifiedResourceType, StringComparer.OrdinalIgnoreCase);
                hc.Add(obj.ResourceScope);
                hc.Add(obj.ResourceNamePropertyValue.TryGetLiteralValue(), StringComparer.OrdinalIgnoreCase);
                return hc.ToHashCode();
            }
        }
    }
}
