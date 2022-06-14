// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics.Metadata;

namespace Bicep.Core.Intermediate
{
    public record ResourceNameOperation(
        ResourceMetadata Metadata,
        IndexReplacementContext? IndexContext,
        bool FullyQualified) : Operation
    {
        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitResourceNameOperation(this);
    }
}
