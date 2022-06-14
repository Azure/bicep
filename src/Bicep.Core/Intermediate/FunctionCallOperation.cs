// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;

namespace Bicep.Core.Intermediate
{
    public record FunctionCallOperation(
        string Name,
        ImmutableArray<Operation> Parameters) : Operation
    {
        public FunctionCallOperation(string name, params Operation[] parameters)
            : this(name, parameters.ToImmutableArray())
        {
        }

        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitFunctionCallOperation(this);
    }
}
