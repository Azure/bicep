// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics;

namespace Bicep.Core.CodeAnalysis
{
    public class VariableAccessOperation : Operation
    {
        public VariableAccessOperation(VariableSymbol symbol)
        {
            Symbol = symbol;
        }

        public VariableSymbol Symbol { get; }

        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitVariableAccessOperation(this);
    }
}
