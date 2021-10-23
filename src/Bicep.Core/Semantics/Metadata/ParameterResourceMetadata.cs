// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Resources;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics.Metadata
{
    // Represents a resource that is declared as a parameter in Bicep.
    public record ParameterResourceMetadata(
        ResourceType Type,
        ParameterSymbol Symbol)
        : ResourceMetadata(Type, IsExistingResource: true)
    {
    }
}
