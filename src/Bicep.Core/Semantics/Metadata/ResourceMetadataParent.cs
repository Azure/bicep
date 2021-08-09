// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics.Metadata
{
    public class ResourceMetadataParent
    {
        public ResourceMetadataParent(ResourceMetadata metadata, SyntaxBase? indexExpression, bool isNested)
        {
            // TODO: turn this into a record when the target framework supports it
            Metadata = metadata;
            IndexExpression = indexExpression;
            IsNested = isNested;
        }

        public ResourceMetadata Metadata { get; }

        public SyntaxBase? IndexExpression { get; }

        public bool IsNested { get; }
    }
}
