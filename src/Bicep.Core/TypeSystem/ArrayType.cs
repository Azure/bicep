// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Core.TypeSystem
{
    public class ArrayType : TypeSymbol
    {
        public ArrayType(string name) : base(name)
        {
        }

        public override TypeKind TypeKind => TypeKind.Primitive;

        public virtual TypeSymbol ItemType => LanguageConstants.Any;
    }
}
