// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem
{
    public class TypeTypeProperty : TypeProperty
    {
        public TypeTypeProperty(string name, TypeType type, TypePropertyFlags flags = TypePropertyFlags.None, string? description = null) : base(name, type, flags, description)
        {
            TypeReference = type;
        }

        public new TypeType TypeReference { get; }
    }
}
