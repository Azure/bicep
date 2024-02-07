// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem.Types
{
    public class TypeProperty(string name, ITypeReference typeReference, TypePropertyFlags flags = TypePropertyFlags.None, string? description = null)
    {
        public string Name { get; } = name;

        public string? Description { get; } = description;

        public ITypeReference TypeReference { get; } = typeReference;

        public TypePropertyFlags Flags { get; } = flags;

        public TypeProperty With(TypePropertyFlags flags) => new(Name, TypeReference, flags, Description);
    }
}
