// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.TypeSystem;

namespace Bicep.Core.Analyzers.Linter.Common
{
    internal static class TypeExtensions
    {
        /// <summary>
        /// True if the given type symbol is a string type (and not "any")
        /// </summary>
        public static bool IsStrictlyAssignableToString(this TypeSymbol typeSymbol)
        {
            return typeSymbol is not AnyType
                && TypeValidator.AreTypesAssignable(typeSymbol, LanguageConstants.String);
        }
    }
}
