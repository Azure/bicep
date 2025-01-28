// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Semantics.Metadata
{
    // Represents a resource that is declared as a parameter in Bicep.
    public record ModuleOutputResourceMetadata(
        ResourceType Type,
        ModuleSymbol Module,
        string OutputName)
        : ResourceMetadata(Type, IsExistingResource: true)
    {
    }
}
