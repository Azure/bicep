// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics.Metadata;

namespace Bicep.Core.Intermediate
{
    public record SymbolicResourceReferenceOperation(
        DeclaredResourceMetadata Metadata,
        IndexReplacementContext? IndexContext,
        bool Full) : Operation
    {
        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitSymbolicResourceReferenceOperation(this);
    }
}
