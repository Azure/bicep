// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics.Metadata;

namespace Bicep.Core.CodeAnalysis
{
    public class ResourceTypeOperation : Operation
    {
        public ResourceTypeOperation(ResourceMetadata metadata)
        {
            Metadata = metadata;
        }

        public ResourceMetadata Metadata { get; }

        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitResourceTypeOperation(this);
    }
}
