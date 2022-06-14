// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Intermediate
{
    public record ConstantValueOperation : Operation
    {
        public ConstantValueOperation(string value)
        {
            Value = value;
        }

        public ConstantValueOperation(long value)
        {
            Value = value;
        }

        public ConstantValueOperation(bool value)
        {
            Value = value;
        }

        public object Value { get; }

        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitConstantValueOperation(this);
    }
}
