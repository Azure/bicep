// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;

namespace Bicep.Core.Intermediate
{
    public record ArrayOperation(
        ImmutableArray<Operation> Items) : Operation
    {
        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitArrayOperation(this);
    }
}
