// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.TypeSystem;

namespace Bicep.Core.Analyzers.Linter.Common
{
    public static class TypeExtensions
    {
        /// <summary>
        /// True if the given type symbol is a string type (and not "any")
        /// </summary>
        public static bool IsString(this TypeSymbol typeSymbol)
        {
            return typeSymbol is not AnyType
                && TypeValidator.AreTypesAssignable(typeSymbol, LanguageConstants.String);
        }

        /// <summary>
        /// True if the given type symbol is an object type (and not "any")
        /// </summary>
        public static bool IsObject(this TypeSymbol typeSymbol)
        {
            return typeSymbol is not AnyType
                && TypeValidator.AreTypesAssignable(typeSymbol, LanguageConstants.Object);
        }
    }
}
