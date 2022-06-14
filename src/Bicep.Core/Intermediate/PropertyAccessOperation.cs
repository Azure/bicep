// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Intermediate
{
    public record PropertyAccessOperation(
        Operation Base,
        string PropertyName) : Operation
    {
        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitPropertyAccessOperation(this);
    }
}
