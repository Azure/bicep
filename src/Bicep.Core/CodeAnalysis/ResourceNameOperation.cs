// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics.Metadata;

namespace Bicep.Core.CodeAnalysis
{
    public class ResourceNameOperation : Operation
    {
        public ResourceNameOperation(ResourceMetadata metadata, IndexReplacementContext? indexContext, bool fullyQualified)
        {
            Metadata = metadata;
            IndexContext = indexContext;
            FullyQualified = fullyQualified;
        }

        public ResourceMetadata Metadata { get; }

        public IndexReplacementContext? IndexContext { get; }

        public bool FullyQualified { get; }

        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitResourceNameOperation(this);
    }
}
