// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics.Metadata;

namespace Bicep.Core.CodeAnalysis
{
    // reference('symbol')
    // reference('symbol[0]')
    public class SymbolicResourceReferenceOperation : Operation
    {
        public SymbolicResourceReferenceOperation(ResourceMetadata metadata, IndexReplacementContext? indexContext, bool full)
        {
            Metadata = metadata;
            IndexContext = indexContext;
            Full = full;
        }

        public ResourceMetadata Metadata { get; }

        public IndexReplacementContext? IndexContext { get; }

        public bool Full { get; }

        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitSymbolicResourceReferenceOperation(this);
    }
}
