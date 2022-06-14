// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Intermediate
{
    public record ObjectPropertyOperation(
        Operation Key,
        Operation Value) : Operation
    {
        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitObjectPropertyOperation(this);
    }
}
