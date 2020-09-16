// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem
{
    /// <summary>
    /// Represents a built-in primitive type.
    /// </summary>
    public class PrimitiveType : TypeSymbol
    {
        public PrimitiveType(string name) : base(name)
        {
        }

        public override TypeKind TypeKind => TypeKind.Primitive;
    }
}
