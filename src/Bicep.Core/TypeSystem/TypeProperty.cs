// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem
{
    public class TypeProperty
    {
        public TypeProperty(string name, ITypeReference typeReference, TypePropertyFlags flags = TypePropertyFlags.None, string? description = null)
        {
            this.Name = name;
            this.TypeReference = typeReference;
            this.Flags = flags;
            this.Description = description;
        }

        public string Name { get; }

        public string? Description { get; }

        public ITypeReference TypeReference { get; }

        public TypePropertyFlags Flags { get; }
    }
}
