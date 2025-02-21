// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem.Types
{
    public record TypeProperty(
        ITypeReference TypeReference,
        TypePropertyFlags Flags = TypePropertyFlags.None,
        string? Description = null);

    public record NamedTypeProperty(
        string Name,
        ITypeReference TypeReference,
        TypePropertyFlags Flags = TypePropertyFlags.None,
        string? Description = null) : TypeProperty(TypeReference, Flags, Description);
}
