// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Utils
{
    internal class ResourceDefinition
    {
        public string ResourceName { get; }
        public ResourceMetadata? ResourceScope { get; }
        public string FullyQualifiedResourceType { get; }
        public SyntaxBase ResourceNamePropertyValue { get; }
        public TypeSymbol ResourceNamePropertyType { get; }

        public ResourceDefinition(
            string resourceName,
            ResourceMetadata? resourceScope,
            string fullyQualifiedResourceType,
            SyntaxBase resourceNamePropertyValue,
            TypeSymbol resourceNamePropertyType)
        {
            ResourceName = resourceName;
            ResourceScope = resourceScope;
            FullyQualifiedResourceType = fullyQualifiedResourceType;
            ResourceNamePropertyValue = resourceNamePropertyValue;
            ResourceNamePropertyType = resourceNamePropertyType;
        }

        public static readonly IEqualityComparer<ResourceDefinition> EqualityComparer = new ResourceComparer();

        // comparers below are very simple now, however in future it might be used to do more exact comparison on property value to include interpolations
        // also, we expect StringSyntax as values it can be other types as well (function calls, variable accesses, etc.)
        private class ResourceComparer : IEqualityComparer<ResourceDefinition>
        {

            public bool Equals(ResourceDefinition? x, ResourceDefinition? y)
            {
                if (x is null && y is null)
                {
                    return true;
                }

                if (x is null || y is null)
                {
                    return false;
                }

                if (!string.Equals(x.FullyQualifiedResourceType, y.FullyQualifiedResourceType, LanguageConstants.ResourceTypeComparison))
                {
                    return false;
                }

                if (x.ResourceScope != y.ResourceScope)
                {
                    return false;
                }

                var xv = (x.ResourceNamePropertyType as StringLiteralType)?.RawStringValue;
                var yv = (y.ResourceNamePropertyType as StringLiteralType)?.RawStringValue;

                //if literal value is null, we assume resources are not equal, as this indicates that interpolated value is used
                //and as for now we're unable to determine if they will have equal values or not.
                return xv is not null && yv is not null && string.Equals(xv, yv, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(ResourceDefinition obj)
            {
                var hc = new HashCode();
                hc.Add(obj.FullyQualifiedResourceType, StringComparer.OrdinalIgnoreCase);
                hc.Add(obj.ResourceScope);
                hc.Add(obj.ResourceNamePropertyType.Name, StringComparer.OrdinalIgnoreCase);
                return hc.ToHashCode();
            }
        }
    }
}
