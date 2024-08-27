// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.TypeSystem.Types;

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

        public static bool IsIntegerOrIntegerLiteral(this TypeSymbol type) =>
             type is IntegerType or IntegerLiteralType;

        public static bool ExtensionNameEquals(this NamespaceType namespaceType, string extensionName)
            => StringComparer.Ordinal.Equals(namespaceType.ExtensionName, extensionName);

        public static bool AliasNameEquals(this NamespaceType namespaceType, string aliasName)
            => LanguageConstants.IdentifierComparer.Equals(namespaceType.Name, aliasName);
    }
}
