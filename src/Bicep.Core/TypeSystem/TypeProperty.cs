// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Core.TypeSystem
{
    public class TypeProperty
    {
        public TypeProperty(string name, TypeSymbol type, TypePropertyFlags flags = TypePropertyFlags.None)
        {
            this.Name = name;
            this.Type = type;
            this.Flags = flags;
        }

        public string Name { get; }

        public TypeSymbol Type { get; }

        public TypePropertyFlags Flags { get; }
    }
}
