// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.CodeAnalysis
{
    public class ArrayAccessOperation : Operation
    {
        public ArrayAccessOperation(Operation @base, Operation access)
        {
            Base = @base;
            Access = access;
        }

        public Operation Base { get; }

        public Operation Access { get; }

        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitArrayAccessOperation(this);
    }
}
