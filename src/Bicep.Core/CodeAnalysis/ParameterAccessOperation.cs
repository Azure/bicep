// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics;

namespace Bicep.Core.CodeAnalysis
{
    public class ParameterAccessOperation : Operation
    {
        public ParameterAccessOperation(ParameterSymbol symbol)
        {
            Symbol = symbol;
        }

        public ParameterSymbol Symbol { get; }

        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitParameterAccessOperation(this);
    }
}
