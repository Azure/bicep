// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics;

namespace Bicep.Core.Intermediate
{
    public record ParameterAccessOperation(
        ParameterSymbol Symbol) : Operation
    {
        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitParameterAccessOperation(this);
    }
}
