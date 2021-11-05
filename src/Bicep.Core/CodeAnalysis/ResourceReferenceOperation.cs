// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics.Metadata;

namespace Bicep.Core.CodeAnalysis
{
    public class ResourceReferenceOperation : Operation
    {
        // reference(resourceId(...), 'apiVersion', 'full')
        // OR
        // reference(resourceId(...))
        public ResourceReferenceOperation(ResourceMetadata metadata, ResourceIdOperation resourceId, bool full)
        {
            Metadata = metadata;
            ResourceId = resourceId;
            Full = full;
        }

        public ResourceMetadata Metadata { get; }

        public ResourceIdOperation ResourceId { get; }

        public bool Full { get; }

        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitResourceReferenceOperation(this);
    }
}
