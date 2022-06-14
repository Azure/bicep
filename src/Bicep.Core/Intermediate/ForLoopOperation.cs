// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Intermediate
{
    public record ForLoopOperation(
        Operation Expression,
        Operation Body) : Operation
    {
        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitForLoopOperation(this);
    }
}
