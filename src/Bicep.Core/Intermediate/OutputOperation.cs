// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;

namespace Bicep.Core.Intermediate
{
    public record OutputOperation(
        string Name,
        string Type,
        Operation Value,
        ImmutableArray<ObjectPropertyOperation> AdditionalProperties) : Operation
    {
        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitOutputOperation(this);
    }
}
