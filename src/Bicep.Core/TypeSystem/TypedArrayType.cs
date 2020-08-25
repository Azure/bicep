// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Core.TypeSystem
{
    public class TypedArrayType : ArrayType
    {
        public TypedArrayType(TypeSymbol itemType)
            : base(itemType.Name + "[]")
        {
            this.ItemType = itemType;
        }

        public override TypeSymbol ItemType { get; }
    }
}
