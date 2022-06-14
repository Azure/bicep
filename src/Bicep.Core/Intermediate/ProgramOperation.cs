// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;

namespace Bicep.Core.Intermediate
{
    public record ProgramOperation(
        ImmutableArray<ParameterOperation> Parameters,
        ImmutableArray<OutputOperation> Outputs,
        ImmutableArray<ImportOperation> Imports,
        ImmutableArray<VariableOperation> Variables) : Operation
    {
        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitProgramOperation(this);
    }
}
