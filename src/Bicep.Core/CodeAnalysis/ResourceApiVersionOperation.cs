// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics.Metadata;

namespace Bicep.Core.CodeAnalysis
{
    public class ResourceApiVersionOperation : Operation
    {
        public ResourceApiVersionOperation(ResourceMetadata metadata)
        {
            Metadata = metadata;
        }

        public ResourceMetadata Metadata { get; }

        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitResourceApiVersionOperation(this);
    }
}
