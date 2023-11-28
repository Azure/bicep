// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem.Types
{
    public class TypeProperty
    {
        public TypeProperty(string name, ITypeReference typeReference, TypePropertyFlags flags = TypePropertyFlags.None, string? description = null)
        {
            Name = name;
            TypeReference = typeReference;
            Flags = flags;
            Description = description;
        }

        public string Name { get; }

        public string? Description { get; }

        public ITypeReference TypeReference { get; }

        public TypePropertyFlags Flags { get; }

        public TypeProperty With(TypePropertyFlags flags) => new(Name, TypeReference, flags, Description);
    }
}
