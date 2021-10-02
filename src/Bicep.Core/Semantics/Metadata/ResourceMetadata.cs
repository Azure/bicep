// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Resources;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics.Metadata
{
    public record ResourceMetadata(
        ResourceType Type,
        SyntaxBase NameSyntax,
        ResourceSymbol Symbol,
        ResourceMetadataParent? Parent,
        SyntaxBase? ScopeSyntax,
        bool IsExistingResource)
    {
        public ResourceTypeReference TypeReference => Type.TypeReference;
    }
}
