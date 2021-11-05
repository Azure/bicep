// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.CodeAnalysis
{
    public class ConstantValueOperation : Operation
    {
        public ConstantValueOperation(string value)
        {
            Value = value;
        }

        public ConstantValueOperation(int value)
        {
            Value = value;
        }

        public object Value { get; }

        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitConstantValueOperation(this);
    }
}
