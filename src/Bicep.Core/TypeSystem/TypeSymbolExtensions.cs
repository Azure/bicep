// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;

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

        public static bool ProviderNameEquals(this NamespaceType namespaceType, string providerName)
            => StringComparer.Ordinal.Equals(namespaceType.ProviderName, providerName);

        public static bool AliasNameEquals(this NamespaceType namespaceType, string aliasName)
            => LanguageConstants.IdentifierComparer.Equals(namespaceType.Name, aliasName);
    }
}
