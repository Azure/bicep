// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics.Metadata;

namespace Bicep.Core.Intermediate
{
    public record ResourceApiVersionOperation(
        ResourceMetadata Metadata) : Operation
    {
        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitResourceApiVersionOperation(this);
    }
}
