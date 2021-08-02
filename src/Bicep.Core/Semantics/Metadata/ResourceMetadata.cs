// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Resources;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics.Metadata
{
    public class ResourceMetadata
    {
        public ResourceMetadata(
            ResourceType type,
            SyntaxBase nameSyntax,
            ResourceSymbol symbol,
            ResourceMetadataParent? parent,
            SyntaxBase? scopeSyntax,
            bool isExistingResource)
        {
            // TODO: turn this into a record when the target framework supports it
            Type = type;
            NameSyntax = nameSyntax;
            Symbol = symbol;
            Parent = parent;
            ScopeSyntax = scopeSyntax;
            IsExistingResource = isExistingResource;
        }

        public ResourceSymbol Symbol { get; }

        public ResourceTypeReference TypeReference => Type.TypeReference;

        public ResourceType Type { get; }

        public ResourceMetadataParent? Parent { get; }

        public SyntaxBase NameSyntax { get; }

        public SyntaxBase? ScopeSyntax { get; }

        public bool IsExistingResource { get; }
    }
}
