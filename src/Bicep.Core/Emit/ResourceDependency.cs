// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Emit
{
    public class ResourceDependency
    {
        public ResourceDependency(DeclaredSymbol resource, SyntaxBase? indexExpression)
        {
            this.Resource = resource;
            this.IndexExpression = indexExpression;
        }

        public DeclaredSymbol Resource { get; }

        /// <summary>
        /// The optional index expression to address a single Target resource when the Target represents a collection of resources.
        /// </summary>
        public SyntaxBase? IndexExpression { get; }

        public override bool Equals(object obj)
        {
            if (obj is not ResourceDependency other)
            {
                return false;
            }

            return ReferenceEquals(this.Resource, other.Resource) && ReferenceEquals(this.IndexExpression, other.IndexExpression);
        }

        public override int GetHashCode() => HashCode.Combine(this.Resource, this.IndexExpression);

        public static readonly IEqualityComparer<ResourceDependency> EqualityComparer = new ResourceDependencyComparer();
        private class ResourceDependencyComparer : IEqualityComparer<ResourceDependency>
        {

            public bool Equals(ResourceDependency x, ResourceDependency y)
            {
                if (x.Resource == y.Resource && (x.IndexExpression as IntegerLiteralSyntax)?.Value == (y.IndexExpression as IntegerLiteralSyntax)?.Value)
                {
                    return true;
                }
                return false;
            }

            public int GetHashCode(ResourceDependency obj)
            {
                var hc = new HashCode();
                hc.Add(obj.Resource.GetHashCode());
                hc.Add((obj.IndexExpression as IntegerLiteralSyntax)?.Value);
                return hc.ToHashCode();
            }
        }
    }
}
