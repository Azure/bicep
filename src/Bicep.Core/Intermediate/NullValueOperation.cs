// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Intermediate
{
    public record NullValueOperation() : Operation
    {
        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitNullValueOperation(this);
    }
}
