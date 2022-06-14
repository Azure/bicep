// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Intermediate
{
    public record VariableOperation(
        string Name,
        Operation Value) : Operation
    {
        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitVariableOperation(this);
    }
}
