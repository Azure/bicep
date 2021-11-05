// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Bicep.Core.CodeAnalysis
{
    public class FunctionCallOperation : Operation
    {
        public FunctionCallOperation(string name, ImmutableArray<Operation> parameters)
        {
            Name = name;
            Parameters = parameters;
        }

        public FunctionCallOperation(string name, params Operation[] parameters)
            : this(name, parameters.ToImmutableArray())
        {
        }

        public string Name { get; }

        public ImmutableArray<Operation> Parameters { get; }

        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitFunctionCallOperation(this);
    }
}
