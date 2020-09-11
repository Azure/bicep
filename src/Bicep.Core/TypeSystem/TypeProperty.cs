// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem
{
    public class TypeProperty
    {
        public TypeProperty(string name, ITypeReference typeReference, TypePropertyFlags flags = TypePropertyFlags.None)
        {
            this.Name = name;
            this.TypeReference = typeReference;
            this.Flags = flags;
        }

        public string Name { get; }

        public ITypeReference TypeReference { get; }

        public TypePropertyFlags Flags { get; }
    }
}
