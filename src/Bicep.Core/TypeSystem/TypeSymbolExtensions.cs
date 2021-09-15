// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem
{
    public static class TypeSymbolExtensions
    {
        public static TypeSymbol UnwrapArrayType(this TypeSymbol type) =>
            type switch
            {
                ArrayType arrayType => arrayType.Item.Type,
                _ => type
            };
    }
}
