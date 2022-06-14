// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;

namespace Bicep.Core.Intermediate
{
    public record ObjectOperation(
        ImmutableArray<ObjectPropertyOperation> Properties) : Operation
    {
        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitObjectOperation(this);
    }
}
