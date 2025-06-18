// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem.Types
{
    public record TypeProperty(
        ITypeReference TypeReference,
        TypePropertyFlags Flags = TypePropertyFlags.None,
        string? Description = null)
    {
        public TType WithoutFlags<TType>(TypePropertyFlags flagsToRemove) where TType : TypeProperty
            => (TType)this with { Flags = Flags & ~flagsToRemove };

        public TypeProperty WithoutFlags(TypePropertyFlags flagsToRemove) => WithoutFlags<TypeProperty>(flagsToRemove);
    }

    public record NamedTypeProperty(
        string Name,
        ITypeReference TypeReference,
        TypePropertyFlags Flags = TypePropertyFlags.None,
        string? Description = null) : TypeProperty(TypeReference, Flags, Description)
    {
        public new NamedTypeProperty WithoutFlags(TypePropertyFlags flagsToRemove) => WithoutFlags<NamedTypeProperty>(flagsToRemove);
    }
}
